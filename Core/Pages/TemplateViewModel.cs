using Panuon.UI.Silver;
using SendMultipleEmails.Datas;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SendMultipleEmails.Pages
{
    class TemplateViewModel : ScreenChild
    {
        public TemplateViewModel(Store store) : base(store)
        {
            Templates = store.TemplateManager.GetTemplateFiles();
            if (Templates.Count > 0)
            {
                string path = store.ConfigManager.PersonalConfig.LastTemplatePath;
                // 如果有上一次的，显示上一次
                FileInfo showItem = null;

                // 显示查找到的模板
                showItem = Templates.Where(item => item.FullName == path).FirstOrDefault();
                if (showItem == null) showItem = Templates[0];
                SelectedItem = showItem;
            }
        }

        public BindingList<FileInfo> Templates { get; set; }

        private string _originContent = string.Empty;

        private FileInfo _selectedItem;
        public FileInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_originContent != Content)
                {
                    bool isUserTemplate = _selectedItem.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir);
                    // 如果是自己的模板，就保存，如果是全局模板，就另存为
                    if (isUserTemplate)
                    {
                        MessageBoxResult result = Store.ShowWarning("模板未保存，是否保存？", "文件更改");
                        if (result == MessageBoxResult.OK)
                        {
                            Store.TemplateManager.Save(_selectedItem.FullName, Content);
                        }
                    }
                    else
                    {
                        // 另存
                        MessageBoxResult result = Store.ShowWarning("模板未保存，是否另存为？", "文件更改");
                        if (result == MessageBoxResult.OK)
                        {
                            SaveAs();
                            return;
                        }
                    }
                }

                // 读取新的模板数据
                string newContent = Store.TemplateManager.GetTemplate(value.FullName);
                SetContent(newContent);

                // 保存当前选择的模板到数据中心
                Store.ConfigManager.PersonalConfig.LastTemplatePath = value.FullName;
                Store.ConfigManager.SavePersonalConfig();

                // 如果路径不是个人用户的，不允许删除,也不能保存
                if (value.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir)) CanDelete = true;
                else CanDelete = false;

                CanSave = false;

                base.SetAndNotify(ref _selectedItem, value);
            }
        }

        private void SetContent(string content)
        {
            _originContent = content;
            Content = content;
            bool isEqual = _originContent == content;
        }

        private string _content = string.Empty;
        public string Content
        {
            get => _content;
            set
            {
                // 如果新值和原始值不一样
                if (SelectedItem == null)
                {
                    base.SetAndNotify(ref _content, value);
                    return;
                };

                bool isUserTemplate = SelectedItem.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir);
                if (_originContent != value && isUserTemplate) this.CanSave = true;
                base.SetAndNotify(ref _content, value);
            }
        }

        public bool CanEdit { get; set; } = true;
        public void Edit()
        {
            // 如果不属于用户模板，没有保存
            if (SelectedItem.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir)) CanSave = true;
            else CanSave = false;
        }

        public bool CanSave { get; set; }
        public void Save()
        {
            Store.TemplateManager.Save(SelectedItem.FullName, Content);
            _originContent = Content;
            Store.ShowInfo("保存成功", "操作结果");
            // 提示
            CanSave = false;
        }

        public bool CanSaveAs { get; set; } = true;
        public void SaveAs()
        {
            // 判断是自己的还是全局的
            SaveAsInputViewModel save = new SaveAsInputViewModel(Store, Content)
            {
                TemplateName = this.SelectedItem.Name.Replace(this.SelectedItem.Extension, "_copy")
            };
            save.InvokeEvent += Save_InvokeEvent;
            Store.ShowDialogWithMask(save);
        }

        private void Save_InvokeEvent(GalensSDK.StyletEx.InvokeParameter obj)
        {
            // 保存
            string newPath = obj.Arg as string;
            // 发给 content
            _originContent = Content;

            // 选中当前
            FileInfo newFileInfo = new FileInfo(newPath);

            Templates.Add(newFileInfo);
            SelectedItem = newFileInfo;
        }

        public bool CanDelete { get; set; }
        public void Delete()
        {
            // 删除提示
            MessageBoxResult result = Store.ShowWarning("你即将删除该模板，是否继续？", "删除模板");
            if (result != MessageBoxResult.OK) return;

            // 删除当前数据
            int index = Templates.ToList().FindIndex(item => item.FullName == this.SelectedItem.FullName);
            if (index < 0) return;
            int nextIndex = index > 0 ? index - 1 : 1;

            // 删除原始数据
            SelectedItem.Delete();

            this.SelectedItem = Templates[nextIndex];
            this.Templates.RemoveAt(index);
        }

        public void ViewInExplorer()
        {
            // 在浏览器中查看
            System.Diagnostics.Process.Start("file:///"+SelectedItem.FullName);
        }
    }
}
