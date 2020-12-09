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
            Templates = store.TemplateManager.TemplateFiles;
            if (Templates.Count > 0)
            {
                string path = store.PersonalDataManager.PersonalData.usedTemplatePath;
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
                        MessageBoxResult result = MessageBoxX.Show(null,"模板未保存，是否保存？", "文件更改", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            Store.TemplateManager.Save(_selectedItem.FullName, Content);
                        }
                    }
                    else
                    {
                        // 另存
                        MessageBoxResult result = MessageBoxX.Show(null,"模板未保存，是否另存为？", "文件更改", MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            SaveAs();
                            return;
                        }
                    }
                }

                // 读取新的模板数据
                string newContent = Store.TemplateManager.GetTemplate(value.FullName);
                SetContent(newContent);

                base.SetAndNotify(ref _selectedItem, value);

                // 保存当前选择的模板到数据中心
                Store.PersonalDataManager.PersonalData.usedTemplatePath = value.FullName;

                IsAllowEdit = false;

                // 如果路径不是个人用户的，不允许删除,也不能保存
                if (value.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir)) CanDelete = true;
                else CanDelete = false;

                CanSave = false;
            }
        }

        private void SetContent(string content)
        {
            _originContent = content;
            Content = content;
        }
        public string Content { get; set; } = string.Empty;

        public bool IsAllowEdit { get; set; } = false;

        public bool CanEdit { get; set; } = true;
        public void Edit()
        {
            IsAllowEdit = true;

            // 如果不属于用户模板，没有保存
            if (SelectedItem.FullName.Contains(Store.ConfigManager.AppConfig.UserTemplateDir)) CanSave = true;
            else CanSave = false;
        }

        public bool CanSave { get; set; }
        public void Save()
        {
            Store.TemplateManager.Save(SelectedItem.FullName, Content);
            IsAllowEdit = false;
        }

        public bool CanSaveAs { get; set; } = true;
        public void SaveAs()
        {
            SaveAsInputViewModel save = new SaveAsInputViewModel(Store)
            {
                TemplateName = this.SelectedItem.Name.Replace(this.SelectedItem.Extension, "_copy")
            };
            bool result = (bool)Store.WindowManager.ShowDialog(save);
            if (!result) return;

            string newPath = Store.ConfigManager.AppConfig.TemplateDir + "\\" + Store.TemplateName + ".html";

            Store.TemplateManager.Save(newPath, Content);
            IsAllowEdit = false;

            // 发给 content
            _originContent = Content;
            // 选中当前
            _selectedItem = new FileInfo(newPath);
            Store.PersonalDataManager.PersonalData.usedTemplatePath = newPath;

            base.NotifyOfPropertyChange("SelectedItem");
        }

        public bool CanDelete { get; set; }
        public void Delete()
        {

        }
    }
}
