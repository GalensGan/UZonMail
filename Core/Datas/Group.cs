using LiteDB;
using SendMultipleEmails.Database;
using SendMultipleEmails.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class Group
    {
        [BsonId]
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [BsonIgnore]
        public string FullName { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Group group) return group.Id == Id;

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void GenerateFullName(List<Group> groups)
        {
            List<Group> result = FindGroup(groups, Id);
            result.Reverse();
            List<string> names = result.ConvertAll(item => item.Name);
            FullName = string.Join("/", names.ToArray());
        }

        private List<Group> FindGroup(List<Group> groups, int groupId)
        {
            List<Group> result = new List<Group>();
            if (groupId < 1) return result;

            Group group = groups.Find(item => item.Id == groupId);
            if (group != null)
            {
                result.Add(group);
                // 查找父组
                result.AddRange(FindGroup(groups, group.ParentId));
            }
            return result;
        }

        /// <summary>
        /// 传入的groups必须是所有的group且已经计算过fullName
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="store"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static int GetGroupIdByFullName(List<Group> groups,Store store,string fullName)
        {
            Group group = groups.Find(g => g.FullName == fullName);
            if (group == null)
            {
                // 添加Group
                CreateGroup(groups, store, fullName.Trim('/').Split('/'));

                // 再获取组
                group = groups.Find(g => g.FullName == fullName);
            }

            return group.Id ;
        }

        private static void CreateGroup(List<Group> groups,Store store,string[] names)
        {
            // 从上向下创建
            Group parentGroup = new Group();
            for(int i = 0; i < names.Length; i++)
            {
                string[] namesTemp = names.Take(i + 1).ToArray();
                string fullName = string.Join("/", namesTemp);
                Group groupTemp = groups.Find(item => item.FullName == fullName);
                if (groupTemp == null)
                {
                    // 创建新的组
                    Group newGroup = new Group()
                    {
                        ParentId = parentGroup.Id,
                        Name = namesTemp.Last(),
                        FullName = fullName,
                    };
                    int newGroupId = store.GetUserDatabase<IGroup>().InsertGroup(newGroup);
                    newGroup.Id = newGroupId;
                    parentGroup = newGroup;

                    // 添加到列表里面
                    groups.Add(newGroup);
                }
                else
                {
                    parentGroup = groupTemp;
                }
            }
        }
    }
}
