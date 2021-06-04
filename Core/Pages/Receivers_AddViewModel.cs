using GalensSDK.Enumerable;
using Panuon.UI.Silver;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SendMultipleEmails.Pages
{
    class Receivers_AddViewModel:ScreenChild
    {
        private List<Group> _groups;

        public string Title { get; set; }
        public bool IsNew { get; set; }

        public Receivers_AddViewModel(Store store) : base(store) 
        {
            Receiver = new Receiver();
            ReadGroups();

            IsNew = true;
            Title = "添加收件箱";
        }

        public Receivers_AddViewModel(Store store, DataRowView row) : base(store)
        {
            // 读取数据进行展示
            Receiver = row.Row.ConvertToModel<Receiver>();
            IsNew = false;
            Title = "编辑收件箱";

            ReadGroups();
        }

        private void ReadGroups()
        {
            // 从数据库中读取所有的组
            List<Group> groups = Store.GetUserDatabase<IGroup>().GetAllGroups().ToList();
            groups.ForEach(g => g.GenerateFullName(groups));

            Groups = groups.ConvertAll(g => g.FullName);
            _groups = groups;
        }

        public Receiver Receiver { get; set; }

        public IList<string> Groups { get; set; }

        public void Confirm()
        {
            if (!Receiver.Validate(null)) return;

            // 判断收件人是否重复
            Receiver existPerson = Store.GetUserDatabase<IReceiverDb>().FindOneReceiverByEmail(Receiver.Email);

            if (existPerson!=null && IsNew)
            {
                Store.ShowInfo("当前的收件箱已经存在，请勿重复添加", "邮箱重复");
                return;
            }

            if (!IsNew && existPerson != null && existPerson.Id != Receiver.Id)
            {
                Store.ShowInfo("当前的收件箱已经存在，请勿重复添加", "邮箱重复");
                return;
            }

            // 判断数据库中的组是否存在，如果不存在，要新建组
            Group group = _groups.Find(g => g.FullName == Receiver.GroupFullName.Trim('/'));
            if(group == null)
            {
                // 建立新的组
                int goupId = Group.GetGroupIdByFullName(_groups, Store, Receiver.GroupFullName);
                group = _groups.Find(g => g.FullName == Receiver.GroupFullName.Trim('/'));
            }
            Receiver.GroupId = group.Id;

            // 添加到数据库
            if (IsNew) Store.GetUserDatabase<IReceiverDb>().InsertReceiver(existPerson);
            else
            {
                bool result = Store.GetUserDatabase<IReceiverDb>().UpdateReceiver(Receiver);
            }           

            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }
    }
}
