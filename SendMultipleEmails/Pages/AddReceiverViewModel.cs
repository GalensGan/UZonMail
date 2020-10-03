using Panuon.UI.Silver;
using SendMultipleEmails.Datas;

namespace SendMultipleEmails.Pages
{
    class AddReceiverViewModel:ScreenChild
    {
        public AddReceiverViewModel(Store store) : base(store) 
        {
            Receiver = new Person();
        }

        public Person Receiver { get; set; }

        public void AddReceiver()
        {
            if (!Receiver.Validate(null)) return;

            if (!Store.PersonalDataManager.AddReceiver(Receiver, true))
            {
                MessageBoxX.Show("添加的收件人已经存在，请不要重复添加", "温馨提示");
                return;
            }

            this.RequestClose(true);
        }

        public void Quite()
        {
            this.RequestClose(false);
        }
    }
}
