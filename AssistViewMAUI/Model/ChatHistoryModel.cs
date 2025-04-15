using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AssistViewMAUI
{
    public class ChatHistoryModel : INotifyPropertyChanged
    {
        private DateTime conversationCreatedDate;
        private ObservableCollection<IAssistItem> messages;
        private string title;

        [Display(AutoGenerateField = false)]
        public DateTime ConversationCreatedDate
        {
            get { return conversationCreatedDate; }
            set
            {
                conversationCreatedDate = value;
                RaisePropertyChanged(nameof(ConversationCreatedDate));
            }
        }

        [Display(AutoGenerateField = false)]
        public ObservableCollection<IAssistItem> Messages
        {
            get { return messages; }
            set
            {
                messages = value;
                RaisePropertyChanged(nameof(Messages));
            }
        }

        [Display(AutoGenerateField = false)]
        public string Message { get;set; }

        [Display(Name = "New name")]
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
