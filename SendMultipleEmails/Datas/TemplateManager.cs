using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMultipleEmails.Datas
{
    public class TemplateManager : ManagerBase
    {
        public BindingList<FileInfo> TemplateFiles { get; private set; }

        public TemplateManager(AppConfig config) : base(config)
        {
            // 获取用户模板列表
            FileInfo[] userTemplates = new FileInfo[0];
            if(Directory.Exists(config.UserTemplateDir)) userTemplates = new DirectoryInfo(config.UserTemplateDir).GetFiles("*.html", SearchOption.AllDirectories);

            // 获取全局模板列表
            FileInfo[] globalTemplates = new DirectoryInfo(config.TemplateDir).GetFiles("*.html", SearchOption.AllDirectories);

            // 将全局模板过滤
            List<FileInfo> gInfos = globalTemplates.Where(g => userTemplates.Where(ut => ut.Name == g.Name).FirstOrDefault() == null).ToList();

            TemplateFiles = new BindingList<FileInfo>();
            userTemplates.ToList().ForEach(item => TemplateFiles.Add(item));
            gInfos.ForEach(item => TemplateFiles.Add(item));
        }

        /// <summary>
        /// 获取模板内容
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetTemplate(string path)
        {
            using (StreamReader stream = new StreamReader(path))
            {
                string content = stream.ReadToEnd();
                stream.Close();
                return content;
            }
        }


        /// <summary>
        /// 保存模板，如果是同名，就覆盖。不同名，就新增
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool Save(string path, string content)
        {
            using (FileStream file = File.Open(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Write(content);
                    writer.Close();

                    FileInfo fileInfo = new FileInfo(path);

                    // 找到同名文件并替换,// 如果没有，则直接添加
                    FileInfo existInfo = TemplateFiles.Where(item => item.Name == fileInfo.Name).FirstOrDefault();
                    if (existInfo != null) TemplateFiles.Remove(existInfo);
                    TemplateFiles.Add(fileInfo);
                };
                file.Close();
            };
            return true;
        }

        public override bool Save()
        {
            return true;
        }
    }
}
