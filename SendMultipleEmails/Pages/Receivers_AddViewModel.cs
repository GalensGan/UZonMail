using Panuon.UI.Silver;
using SendMultipleEmails.Database;
using SendMultipleEmails.Datas;

namespace SendMultipleEmails.Pages
{
    class Receivers_AddViewModel:ScreenChild
    {
        public Receivers_AddViewModel(Store store) : base(store) 
        {
            Receiver = new Person();
        }

        public Person Receiver { get; set; }

        public void AddReceiver()
        {
            if (!Receiver.Validate(null)) return;

            // 判断收件人是否重复
            Receiver existPerson = Store.GetUserDatabase<IReceiverDb>().FindOneReceiverByName(Receiver.UserId);

            if (existPerson!=null)
            {
                MessageBoxX.Show("添加的收件人已经存在，请不要重复添加", "温馨提示");
                return;
            }

            // 添加到数据库
            Store.GetUserDatabase<IReceiverDb>().InsertReceiver(existPerson);

            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }
    }
}
