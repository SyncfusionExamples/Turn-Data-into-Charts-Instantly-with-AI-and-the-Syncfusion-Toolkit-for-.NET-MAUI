using AssistViewMAUI.Helper;
using ChartGenerater;
using Newtonsoft.Json;
using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.DataForm;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.Popup;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace AssistViewMAUI
{
    public class ChatGPTViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        private SfPopup OptionsPopup;
        private ChatHistoryModel currentChat;
        private AzureAIService azureAIService;
        private string inputText;
        private string headerText;
        private bool isHeaderVisible = true;
        private ObservableCollection<IAssistItem> messages;
        private ObservableCollection<Option> optionsContextMenu;
        private ObservableCollection<Option> headerPrompts;
        private ObservableCollection<ImageSource> imageSourceCollection;
        private bool isPresented;
        private bool isTemporaryChatEnabled;
        private bool isNewChatEnabled;
        internal AssistItem CurrentAssistItem;
        private string sendIconText;
        private double sendIconWidth;
        private SfPopup RequestPopup;
        private SfPopup ResponsePopup;
        private SfPopup BadResponsePopup;
        private SfPopup RenamePopup;
        private SfPopup ExpandedEditorPopup;
        private ObservableCollection<ChatHistoryModel> chatHistories;
        private ChatHistoryModel currentChatHistory;
        private bool isRenameEnabled;
        internal bool isResponseStreaming;
        internal double autoSuggestionListHeight;
        private bool canStopResponse;
        private bool autoSuggestionPopupIsOpen;
        private bool isScrollToBottom;
        private XmlFileCreator chatHistoryHandler;
        private double webViewWidthRequest = 400;
        private double webViewHeightRequest = 300;
        private readonly ImagePickerHelper _imagePickerHelper;
        private int imageNo;
        private bool isSendIconEnabled;
        #endregion

        #region Public APIs

        public bool IsSendIconEnabled
        {
            get
            {
                return isSendIconEnabled;
            }
            set
            {
                isSendIconEnabled = value;
                RaisePropertyChanged(nameof(IsSendIconEnabled));
            }
        }

        public ObservableCollection<IAssistItem> Messages
        {
            get
            {
                return this.messages;
            }

            set
            {
                this.messages = value;
                RaisePropertyChanged(nameof(Messages));
            }
        }

        public ObservableCollection<ChatHistoryModel> ChatHistories
        {
            get
            {
                return this.chatHistories;
            }
            set
            {
                this.chatHistories = value;
                RaisePropertyChanged(nameof(ChatHistories));
            }
        }

        public ChatHistoryModel CurrentChatHistory
        {
            get
            {
                return this.currentChatHistory;
            }
            set
            {
                this.currentChatHistory = value;
                RaisePropertyChanged(nameof(CurrentChatHistory));
            }
        }
        public string HeaderText
        {
            get
            {
                return this.headerText;
            }
            set
            {
                this.headerText = value;
                RaisePropertyChanged(nameof(HeaderText));
            }
        }

        public string InputText
        {
            get { return inputText; }
            set
            {
                inputText = value;
                UpdateSendIcon(inputText);
                RaisePropertyChanged(nameof(InputText));
                UpdateFilter();
                AutoSuggestionPopupIsOpen = !string.IsNullOrEmpty(InputText) && this.Messages.Count == 0;
            }
        }

        public double SendIconWidth
        {
            get { return sendIconWidth; }
            set
            {
                sendIconWidth = value;
                RaisePropertyChanged(nameof(SendIconWidth));
            }
        }

        public bool IsHeaderVisible
        {
            get
            {
                return isHeaderVisible;
            }
            set
            {
                isHeaderVisible = value;
                if (!value)
                {
                    this.IsNewChatEnabled = true;
                }
                else
                {
                    this.IsNewChatEnabled = false;
                }
                RaisePropertyChanged(nameof(IsHeaderVisible));
            }
        }

        public ObservableCollection<ImageSource> ImageSourceCollection
        {
            get
            {
                return imageSourceCollection;
            }
            set
            {
                imageSourceCollection = value;
                RaisePropertyChanged(nameof(ImageSourceCollection));
            }
        }

        public bool IsPresented
        {
            get
            {
                return isPresented;
            }
            set
            {
                isPresented = value;
                RaisePropertyChanged(nameof(IsPresented));
            }
        }

        public bool HasImageUploaded
        {
            get { return this.ImageSourceCollection.Count > 0; }
        }

        public string SendIconText
        {
            get
            {
                return sendIconText;
            }
            set
            {
                sendIconText = value;
                RaisePropertyChanged(nameof(SendIconText));
            }
        }

        public bool IsTemporaryChatEnabled
        {
            get
            {
                return isTemporaryChatEnabled;
            }
            set
            {
                isTemporaryChatEnabled = value;
                RaisePropertyChanged(nameof(IsTemporaryChatEnabled));
            }
        }

        public bool IsGoodResponse { get; set; }
        public string GoodResponsePopupText { get; set; } = "Thank you for your feedback!";
        public bool IsRenameEnabled
        {
            get { return isRenameEnabled; }
            set
            {
                isRenameEnabled = value;
                RaisePropertyChanged(nameof(IsRenameEnabled));
            }
        }

        public bool AutoSuggestionPopupIsOpen
        {
            get { return autoSuggestionPopupIsOpen; }
            set
            {
                autoSuggestionPopupIsOpen = value;
                RaisePropertyChanged(nameof(AutoSuggestionPopupIsOpen));
            }
        }
        public bool IsScrollToBottom
        {
            get { return isScrollToBottom; }
            set
            {
                isScrollToBottom = value;
                RaisePropertyChanged(nameof(IsScrollToBottom));
            }
        }

        public double WebViewWidthRequest
        {
            get { return webViewWidthRequest; }
            set
            {
                webViewWidthRequest = value;
                RaisePropertyChanged(nameof(WebViewWidthRequest));
            }
        }

        public double WebViewHeightRequest
        {
            get { return webViewHeightRequest; }
            set
            {
                webViewHeightRequest = value;
                RaisePropertyChanged(nameof(WebViewHeightRequest));
            }
        }

        public bool IsNewChatEnabled
        {
            get
            {
                return isNewChatEnabled;
            }
            set
            {
                isNewChatEnabled = value;
                RaisePropertyChanged(nameof(IsNewChatEnabled));
            }
        }

        public ObservableCollection<string> AutoSuggestions { get; set; }
        public ObservableCollection<string> AutoSuggestionsCollection { get; set; }
        internal ObservableCollection<ChatHistoryModel> ArchieveCollection { get; set; }

        #endregion

        #region Chip items source
        public ObservableCollection<Option> EditorOptions { get; set; }
        public ObservableCollection<Option> HeaderPrompts
        {
            get { return headerPrompts; }
            set
            {
                headerPrompts = value;
                RaisePropertyChanged(nameof(HeaderPrompts));
            }
        }

        // Context menu options
        public ObservableCollection<Option> ResponseContextMenu { get; set; }
        public ObservableCollection<Option> RequestContextMenu { get; set; }
        public ObservableCollection<Option> OptionsContextMenu
        {
            get
            {
                return optionsContextMenu;
            }
            set
            {
                optionsContextMenu = value;
                RaisePropertyChanged(nameof(OptionsContextMenu));
            }
        }

        public bool CanStopResponse
        {
            get
            {
                return canStopResponse;
            }
            set
            {
                canStopResponse = value;
                RaisePropertyChanged(nameof(CanStopResponse));
            }
        }
        public ObservableCollection<Option> AttachmentContextMenu { get; set; }

        #endregion

        #region Commands
        public Command<object> SendButtonCommand { get; set; }
        public ICommand HeaderPromptCommand { get; set; }
        public ICommand NewChatTappedCommand { get; set; }
        public Command<object> AttachmentContextMenuCommand { get; set; }
        public Command<object> EditorOptionsComamnd { get; set; }
        public Command<object> LongPressedCommand { get; set; }
        public Command<object> ResponseContextMenuCommand { get; set; }
        public Command<object> RequestContextMenuCommand { get; set; }
        public Command<object> OptionsContextMenuCommand { get; set; }
        public Command<object> OptionsContextMenuTapCommand { get; set; }
        public Command<object> SideBarCommand { get; set; }
        public Command<object> VoiceInputCommand { get; set; }
        public ICommand SubmitFeedbackCommand { get; set; }
        public Command<object> ChatHistoryItemTappedCommand { get; set; }
        public Command<object> ChatHistoryLongPressCommand { get; set; }
        public Command<object> AcceptRenameCommand { get; set; }
        public Command<object> DeclineCommand { get; set; }
        public Command<object> SearchCommand { get; set; }
        public Command<object> ClearTextCommand { get; set; }
        public Command<object> EditorExpandCollapseCommand { get; set; }
        public ICommand TemporaryChatCommand { get; set; }
        public ICommand AutoSuggestionItemTappedCommand { get; set; }
        public Command<object> ScrollToBottomCommand { get; set; }
        public Command<object> CancelImageSelected { get; set; }

        #endregion

        #region Constructor

        public ChatGPTViewModel()
        {
            azureAIService = new AzureAIService();
            _imagePickerHelper = new ImagePickerHelper();
            PopulateChipOptions();
            PopulateHeaderPrompts();
            PopulateHeaderText();
            PopulateRequestContextMenu();
            PopulateResponseContextMenu();
            PopulateOptionsContextMenu();
            PopulateAttachmentContextMenu();
            InitializeCommands();
            InitializeCollection();
        }

        #endregion

        #region Private Methods

        private void UpdateFilter()
        {
            if (string.IsNullOrEmpty(InputText))
            {
                autoSuggestionListHeight = 0;
                // Reset filter
                AutoSuggestions = new ObservableCollection<string>();
            }
            else
            {
                // Apply filter
                var filteredItems = AutoSuggestionsCollection
                .Where(item => item.Split(' ').Contains(InputText, StringComparer.OrdinalIgnoreCase) || item.StartsWith(InputText))
                .ToList();
                AutoSuggestions = new ObservableCollection<string>(filteredItems);
                autoSuggestionListHeight = AutoSuggestions.Count > 0 ? AutoSuggestions.Count * 40 : 0;
            }

            RaisePropertyChanged(nameof(AutoSuggestions));
        }

        private void UpdateSendIcon(string input)
        {
            if (string.IsNullOrWhiteSpace(input) && !this.HasImageUploaded)
            {
                //SendIconText = "\ue7E8" + " Voice"; // TODO:Need to add voice input support
                SendIconText = "\ue710";
                SendIconWidth = 40;
                if (!this.isResponseStreaming)
                {
                    this.IsSendIconEnabled = false;
                }

            }
            else if (!isResponseStreaming)
            {
                SendIconText = "\ue710";
                SendIconWidth = 40;
                this.IsSendIconEnabled = true;
            }
        }

        private void InitializeCollection()
        {
            messages = new ObservableCollection<IAssistItem>();
            chatHistories = new ObservableCollection<ChatHistoryModel>();
            ArchieveCollection = new ObservableCollection<ChatHistoryModel>();
            //SendIconText = "\ue7E8" + " Voice"; // TODO:Need to add voice input support
            SendIconText = "\ue710";
            SendIconWidth = 40;
            AutoSuggestionsCollection = new ObservableCollection<string>();
            AutoSuggestionsCollection.Add("Characteristics of different dog breeds");
            AutoSuggestionsCollection.Add("Characteristics of strong friendship in story");
            AutoSuggestionsCollection.Add("Characteristics that make a story compelling");
            AutoSuggestionsCollection.Add("Characteristics  of popular music genres");
            AutoSuggestionsCollection.Add("Brainstorm ideas");
            AutoSuggestionsCollection.Add("Brainstorm tips for creative projects");
            chatHistoryHandler = new XmlFileCreator();
            this.ImageSourceCollection = new ObservableCollection<ImageSource>();
        }
        private void InitializeCommands()
        {
            SendButtonCommand = new Command<object>(ExecuteSendButtonCommand);
            HeaderPromptCommand = new Command(ExecuteHeaderSuggestionsCommand);
            AttachmentContextMenuCommand = new Command<object>(OnAttachmentContextMenuTapCommand);
            EditorOptionsComamnd = new Command<object>(ExecuteEditorOptionsCommand);
            LongPressedCommand = new Command<object>(ExecuteItemLongPressedCommand);
            ResponseContextMenuCommand = new Command<object>(OnResponseContextMenuTapCommand);
            RequestContextMenuCommand = new Command<object>(OnRequestContextMenuTapCommand);
            NewChatTappedCommand = new Command(ExecuteNewChatTappedCommand);
            OptionsContextMenuCommand = new Command<object>(ExecuteOptionsContextMenuCommand);
            OptionsContextMenuTapCommand = new Command<object>(ExecuteOptionsContextMenuTapCommand);
            SideBarCommand = new Command<object>(ExecuteSideBarCommand);
            VoiceInputCommand = new Command<object>(ExecuteVoiceInputCommand);
            SubmitFeedbackCommand = new Command(ExecuteSubmitFeedbackCommand);
            ChatHistoryItemTappedCommand = new Command<object>(ExecuteChatHistoryItemTappedCommand);
            ChatHistoryLongPressCommand = new Command<object>(ExecuteChatHistoryLongPressCommand);
            AcceptRenameCommand = new Command<object>(ExecuteAcceptCommand);
            DeclineCommand = new Command<object>(ExecuteDeclineCommand);
            SearchCommand = new Command<object>(ExecuteSearchCommand);
            ClearTextCommand = new Command<object>(ExecuteClearTextCommand);
            EditorExpandCollapseCommand = new Command<object>(ExecuteEditorExpandCollapseCommand);
            TemporaryChatCommand = new Command(ExecuteTemporaryChatCommand);
            AutoSuggestionItemTappedCommand = new Command<object>(ExecuteAutoSuggestionItemTappedCommand);
            ScrollToBottomCommand = new Command<object>(ExecuteScrollToBottomCommand);
            CancelImageSelected = new Command<object>(ExecuteCancelImageSelectedCommand);
        }
        public void ExecuteCancelImageSelectedCommand(object obj)
        {
            this.ImageSourceCollection.Remove(obj as ImageSource);
            RaisePropertyChanged(nameof(this.HasImageUploaded));
            this.UpdateSendIcon(this.InputText);
        }
        private void ExecuteScrollToBottomCommand(object obj)
        {
            var assistView = obj as SfAIAssistView;
            var propertyInfo = assistView.GetType().GetField("AssistViewChat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var chat = propertyInfo?.GetValue(assistView) as AssistViewChat;
            this.IsScrollToBottom = false;
            chat.ScrollToMessage(this.Messages[this.Messages.Count - 1]);
        }
        private void ExecuteAutoSuggestionItemTappedCommand(object obj)
        {
            var data = obj as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            this.InputText = data.DataItem.ToString();
            this.ExecuteSendButtonCommand(null);
        }
        private void ExecuteChatHistoryLongPressCommand(object obj)
        {
            var args = obj as Syncfusion.Maui.ListView.ItemLongPressEventArgs;
            this.OptionsPopup = App.Current.Resources["OptionsContextMenuPopup"] as SfPopup;
            this.OptionsPopup.BindingContext = this;
            this.PopulateOptionsContextMenu(true);
            if (args.DataItem is ChatHistoryModel data)
            {
                this.currentChat = this.CurrentChatHistory;
                this.CurrentChatHistory = data;
                this.CurrentChatHistory.Title = data.Title; // To-Do: title should auto update.
            }
            this.OptionsPopup.Show(args.Position.X, args.Position.Y);
        }

        private void ExecuteTemporaryChatCommand()
        {
            var temporaryChatPopup = App.Current.Resources["TemporaryChatPopup"] as SfPopup;
            temporaryChatPopup.BindingContext = this;
            if (!temporaryChatPopup.IsOpen)
            {
                temporaryChatPopup.Show(true);
            }
            else
            {
                temporaryChatPopup.Dismiss();
            }
        }
        private void ExecuteEditorExpandCollapseCommand(object obj)
        {
            var editor = obj as CustomEditor;
            this.ExpandedEditorPopup = App.Current.Resources["EditorExpandPopup"] as SfPopup;
            this.ExpandedEditorPopup.BindingContext = this;
            if (!this.ExpandedEditorPopup.IsOpen)
            {
                this.ExpandedEditorPopup.Show(true);
                editor.Focus();
            }
            else
            {
                this.ExpandedEditorPopup.Dismiss();
            }
        }
        private void ExecuteSearchCommand(object obj)
        {
            var editor = obj as SfEntry;
            if (!editor.IsEnabled)
            {
                editor.IsEnabled = true;
                editor.Focus();
            }
            else if (editor!.IsFocused)
            {
                editor.Unfocus();
            }
            else
            {
                editor.Focus();
            }
        }
        private void ExecuteClearTextCommand(object obj)
        {
            var editor = obj as SfEntry;
            editor.Text = string.Empty;
            editor.Focus();
        }
        private void PopulateChipOptions()
        {
            EditorOptions = new ObservableCollection<Option>();
            EditorOptions.Add(new Option() { Name = "Attachment", Icon = "\ue754" });
            //EditorOptions.Add(new Option() { Name = "MicroPhone", Icon = "\ue76a" }); // To-Do : Need to add 
            //EditorOptions.Add(new Option() { Name = "Web", Icon = "\ue7bb" }); // To-Do : Need to add 
        }

        private void PopulateHeaderPrompts()
        {
            HeaderPrompts = new ObservableCollection<Option>();
            HeaderPrompts.Add(new Option() { Name = "Investment portfolio allocation", Icon = "\uE751" });
            HeaderPrompts.Add(new Option() { Name = "Year-over-year profit margin area chart", Icon = "\ue774" });
            HeaderPrompts.Add(new Option() { Name = "More", Icon = "\ue733" });
        }

        private void PopulateHeaderText()
        {
            HeaderText = "How can i help you?";
        }

        private void PopulateResponseContextMenu()
        {
            ResponseContextMenu = new ObservableCollection<Option>();
            ResponseContextMenu.Add(new Option() { Name = "Copy", Icon = "\ue737" });
            ResponseContextMenu.Add(new Option() { Name = "SelectText", Icon = "\ue7b1" });
            ResponseContextMenu.Add(new Option() { Name = "Good Response", Icon = "\ue772" });
            ResponseContextMenu.Add(new Option() { Name = "Bad Response", Icon = "\ue773" });
            ResponseContextMenu.Add(new Option() { Name = "Read aloud", Icon = "\ue7e8" });
            ResponseContextMenu.Add(new Option() { Name = "Search the web", Icon = "\ue7e7" });
            ResponseContextMenu.Add(new Option() { Name = "Regenerate Response", Icon = "\ue72b" });
            ResponseContextMenu.Add(new Option() { Name = "Change model", Icon = "\ue7e1" });
        }

        private void PopulateRequestContextMenu()
        {
            RequestContextMenu = new ObservableCollection<Option>();
            RequestContextMenu.Add(new Option() { Name = "Copy", Icon = "\ue737" });
            RequestContextMenu.Add(new Option() { Name = "SelectText", Icon = "\ue7b1" });
            RequestContextMenu.Add(new Option() { Name = "Edit Message", Icon = "\ue73d" });
        }

        private void PopulateOptionsContextMenu(bool isChatHistoryItemPressed = false)
        {
            OptionsContextMenu = new ObservableCollection<Option>();
            if (isChatHistoryItemPressed)
            {
                if (this.OptionsPopup != null)
                {
                    this.OptionsPopup.WidthRequest = 130;
                }
                OptionsContextMenu.Add(new Option() { Name = "Rename", Icon = "\ue73d" });
                OptionsContextMenu.Add(new Option() { Name = "Archive", Icon = "\ue777" });
                OptionsContextMenu.Add(new Option() { Name = "Delete", Icon = "\ue73c" });
            }
            else if (Messages != null && Messages.Count > 0 && !IsTemporaryChatEnabled)
            {
                if (this.OptionsPopup != null)
                {
                    this.OptionsPopup.WidthRequest = 190;
                }
                OptionsContextMenu.Add(new Option() { Name = "View Details", Icon = "\ue719" });
                //OptionsContextMenu.Add(new Option() { Name = "Share", Icon = "\ue770" });
                OptionsContextMenu.Add(new Option() { Name = "Rename", Icon = "\ue73d", CanShowSeparator = true });
                OptionsContextMenu.Add(new Option() { Name = "Archive", Icon = "\ue777" });
                OptionsContextMenu.Add(new Option() { Name = "Delete", Icon = "\ue73c" });
                //OptionsContextMenu.Add(new Option() { Name = "Move to project", Icon = "\ue72e" });
                OptionsContextMenu.Add(new Option() { Name = "Temporary Chat", Icon = "\ue7e9", CanShowSeparator = true });
            }
            else
            {
                if (this.OptionsPopup != null)
                {
                    this.OptionsPopup.WidthRequest = 190;
                }
                OptionsContextMenu.Add(new Option() { Name = "View Details", Icon = "\ue719" });
                OptionsContextMenu.Add(new Option() { Name = "Temporary Chat", Icon = "\ue7e9", CanShowSeparator = true });
            }
        }

        private void PopulateAttachmentContextMenu()
        {
            AttachmentContextMenu = new ObservableCollection<Option>();
            AttachmentContextMenu.Add(new Option() { Name = "Upload Image", Icon = "\ue70d" });
            AttachmentContextMenu.Add(new Option() { Name = "Take Photo", Icon = "\ue76d" });
            //AttachmentContextMenu.Add(new Option() { Name = "Upload File", Icon = "\ue72e" }); // To-Do : Need to add 
        }

        private void ExecuteAcceptCommand(object obj)
        {
            (obj as SfDataForm)?.Commit();
            chatHistoryHandler.UpdateChatHistory(this.CurrentChatHistory.Title, this.CurrentChatHistory);
            if (this.RenamePopup != null)
            {
                this.RenamePopup.Dismiss();
            }
            if (this.currentChat != null)
            {
                this.CurrentChatHistory = this.currentChat;
                this.CurrentChatHistory.Title = this.currentChat.Title;
                this.currentChat = null;
            }
            IsRenameEnabled = false;
        }

        private void ExecuteDeclineCommand(object obj)
        {
            if (this.RenamePopup != null)
            {
                this.RenamePopup.Dismiss();
                if (this.CurrentChatHistory == null)
                    return;
                this.CurrentChatHistory.Title = ((obj as SfDataForm).DataObject as ChatHistoryModel).Title; // To-Do: title should auto update.
            }
            IsRenameEnabled = false;
        }

        private void ExecuteVoiceInputCommand(object obj)
        {
            // To do
        }

        private void ExecuteDeleteCommand()
        {
            if (CurrentChatHistory != null)
            {
                chatHistoryHandler.DeleteChatHistory(CurrentChatHistory);
                CurrentChatHistory = null;
                this.Messages.Clear();
                ExecuteNewChatTappedCommand(null);
                ChatHistories = chatHistoryHandler.LoadFromXml();
            }
        }

        private void ExecuteArchivedCommand()
        {
            if (CurrentChatHistory != null)
            {
                this.ArchieveCollection.Add(CurrentChatHistory);
                chatHistoryHandler.DeleteChatHistory(CurrentChatHistory);
                ChatHistories = chatHistoryHandler.LoadFromXml();
                CurrentChatHistory = null;
                this.Messages.Clear();
                ExecuteNewChatTappedCommand(null);
            }
        }
        private void ExecuteSendButtonCommand(object obj)
        {
            var text = this.InputText;
            if (this.isResponseStreaming)
            {
                this.CanStopResponse = true;
            }
            else
            {
                this.CanStopResponse = false;
            }
            if (string.IsNullOrWhiteSpace(text) && !HasImageUploaded)
            {
                return;
            }

            if (this.ExpandedEditorPopup != null && this.ExpandedEditorPopup.IsOpen)
            {
                this.ExpandedEditorPopup.Dismiss();
            }

            if (!HasImageUploaded)
            {
                var requestItem = new AssistItem()
                {
                    Text = text,
                    IsRequested = true
                };
                this.SendRequest(requestItem, obj);
            }
            else
            {
                foreach (var imageSource in this.ImageSourceCollection)
                {
                    var requestItem = new AssistImageItem()
                    {
                        Source = imageSource,
                        IsRequested = true,
                        Text = text
                    };
                    this.SendRequest(requestItem, obj);
                }
                this.ImageSourceCollection.Clear();
            }
        }
        private async void SendRequest(AssistItem requestItem, object obj)
        {
            if (this.isResponseStreaming)
            {
                return;
            }
            this.isResponseStreaming = true;
            this.Messages.Add(requestItem);
            if (OptionsContextMenu.Count <= 2)
                PopulateOptionsContextMenu();
            this.InputText = string.Empty;
            await this.GetResult(requestItem, obj).ConfigureAwait(true);
        }
        private async void ExecuteRetryCommand()
        {
            await this.GetResult(CurrentAssistItem.RequestItem, null).ConfigureAwait(true);
        }

        private void ExecuteHeaderSuggestionsCommand(object obj)
        {
            var chipText = obj as string;

            if (chipText == "More")
            {
                HeaderPrompts.RemoveAt(HeaderPrompts.Count - 1);
                HeaderPrompts.Add(new Option() { Name = "Expenses breakdown on Q1", Icon = "\uE7E2" });
                HeaderPrompts.Add(new Option() { Name = "Productivity comparison", Icon = "\uE761" });
            }
            else
            {
                this.InputText = chipText;
            }

            if (chipText != "More")
            {
                this.IsHeaderVisible = false;
            }
        }

        private async void ExecuteNewChatTappedCommand(object obj)
        {
            this.CanStopResponse = true;
            this.IsScrollToBottom = false;
            await Task.Delay(100);
            this.IsTemporaryChatEnabled = false;
            this.InputText = string.Empty;
            CurrentChatHistory = null;
            this.Messages.Clear();
            this.IsNewChatEnabled = false;
            this.IsHeaderVisible = true;
            this.ImageSourceCollection.Clear();
            PopulateOptionsContextMenu();
            PopulateHeaderPrompts();
        }

        private void AddToChatHistoryCollection()
        {
            if (!isTemporaryChatEnabled && this.Messages != null && this.Messages.Count > 0)
            {
                if (CurrentChatHistory == null)
                {
                    var chatHistory = new ChatHistoryModel() { Messages = new ObservableCollection<IAssistItem>(this.Messages), ConversationCreatedDate = System.DateTime.Now, Message = string.Empty };
                    if (this.Messages[0] is AssistImageItem && string.IsNullOrWhiteSpace(this.Messages[0].Text))
                    {
                        chatHistory.Title = "Image"; // TODO :Need to updated title based on image
                    }
                    else
                    {
                        chatHistory.Title = this.Messages[0].Text;
                    }
                    // ChatHistories.Add(chatHistory);
                    this.CurrentChatHistory = chatHistory;
                    chatHistoryHandler.AddChatHistory(chatHistory);
                }
                else
                {
                    CurrentChatHistory.Messages = new ObservableCollection<IAssistItem>(this.Messages);
                    chatHistoryHandler.UpdateChatHistory(CurrentChatHistory);
                }
                ChatHistories = chatHistoryHandler.LoadFromXml();
            }
        }

        private void ExecuteOptionsContextMenuCommand(object obj)
        {
            OptionsPopup = App.Current.Resources["OptionsContextMenuPopup"] as SfPopup;
            OptionsPopup.BindingContext = this;
            OptionsPopup.AutoSizeMode = PopupAutoSizeMode.Height;
            OptionsPopup.WidthRequest = 190;
            this.PopulateOptionsContextMenu();
            OptionsPopup.ShowRelativeToView(obj as SfChip, PopupRelativePosition.AlignBottomLeft);
        }

        private void ExecuteOptionsContextMenuTapCommand(object eventArgs)
        {
            var eventData = eventArgs as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            if (eventData == null)
                return;

            OptionsPopup.Dismiss();
            var action = (eventData.DataItem as Option).Name;
            switch (action)
            {
                case "View Details":
                    var viewDetailsPopup = App.Current.Resources["ViewDetailsPopup"] as SfPopup;
                    viewDetailsPopup.Show();
                    break;
                case "Share":
                    // Need to check the behvaior. 
                    break;
                case "Rename":
                    this.RenamePopup = App.Current.Resources["RenamePopup"] as SfPopup;
                    this.RenamePopup.BindingContext = this;
                    this.RenamePopup.Show();
                    break;
                case "Archive":
                    ExecuteArchivedCommand();
                    break;
                case "Delete":
                    ExecuteDeleteCommand();
                    break;
                case "Move to project":
                    // Need to check the detials.
                    break;
                case "Temporary Chat":
                    IsTemporaryChatEnabled = !IsTemporaryChatEnabled;
                    break;
            }
        }

        private void ExecuteSideBarCommand(object obj)
        {
            IsPresented = !IsPresented;
            ChatHistories = chatHistoryHandler.LoadFromXml();
        }

        private void OnAttachmentContextMenuTapCommand(object eventArgs)
        {
            var eventData = eventArgs as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            if (eventData == null)
                return;

            var action = (eventData.DataItem as Option).Name;
            switch (action)
            {
                case "Upload Image":
                    ExecuteUploadImage(eventData.DataItem);
                    break;
                case "Take Photo":
                    ExecuteTakePhoto(eventData.DataItem);
                    break;
                case "Upload File":
                    ExecuteUploadFile(eventData.DataItem);
                    break;
            }
        }

        private void OnRequestContextMenuTapCommand(object eventArgs)
        {
            var eventData = eventArgs as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            if (eventData == null)
                return;

            RequestPopup.Dismiss();

            var action = (eventData.DataItem as Option).Name;
            switch (action)
            {
                case "Copy":
                    string text = CurrentAssistItem.Text;
                    text = Regex.Replace(text, "<.*?>|&nbsp;", string.Empty);
                    Clipboard.SetTextAsync(text);
                    break;
                case "SelectText":
                    // Todo
                    break;
                case "Edit Message":
                    this.InputText = (CurrentAssistItem as AssistItem).Text;
                    break;
            }
        }

        private void OnResponseContextMenuTapCommand(object eventArgs)
        {
            var eventData = eventArgs as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            if (eventData == null)
                return;

            ResponsePopup.Dismiss();

            var action = (eventData.DataItem as Option).Name;
            switch (action)
            {
                case "Copy":
                    string text = CurrentAssistItem.Text;
                    text = Regex.Replace(text, "<.*?>|&nbsp;", string.Empty);
                    Clipboard.SetTextAsync(text);
                    break;
                case "SelectText":
                    // Todo
                    break;
                case "Good Response":
                case "Bad Response":
                    ExecuteResponseFeedbackCommand(action);
                    break;
                case "Read aloud":
                    break;
                case "Search the web":
                    break;
                case "Regenerate Response":
                    ExecuteRetryCommand();
                    break;
            }
        }

        private async void ExecuteResponseFeedbackCommand(string action)
        {
            // Todo - need to show toast for good
            // Need to show popup to get feedback for bad
            if (action.Equals("Good Response"))
            {
                IsGoodResponse = true;
                RaisePropertyChanged(nameof(IsGoodResponse));
                await Task.Delay(3000);
                IsGoodResponse = false;
                RaisePropertyChanged(nameof(IsGoodResponse));
            }
            else
            {
                BadResponsePopup = App.Current.Resources["BadResponsePopup"] as SfPopup;
                BadResponsePopup.Show();
            }
        }

        private async void ExecuteSubmitFeedbackCommand()
        {
            BadResponsePopup.Dismiss();
            IsGoodResponse = true;
            RaisePropertyChanged(nameof(IsGoodResponse));
            await Task.Delay(3000);
            IsGoodResponse = false;
            RaisePropertyChanged(nameof(IsGoodResponse));
        }

        private async void ExecuteChatHistoryItemTappedCommand(object eventArgs)
        {
            this.IsScrollToBottom = false;
            var eventData = eventArgs as Syncfusion.Maui.ListView.ItemTappedEventArgs;
            if (eventData == null)
                return;

            CurrentChatHistory = eventData.DataItem as ChatHistoryModel;
            if (CurrentChatHistory == null)
                return;
            CurrentChatHistory.Title = (eventData.DataItem as ChatHistoryModel).Title; // To-Do: title should auto update.
            if (CurrentChatHistory == null)
                return;
            IsHeaderVisible = false;
            this.InputText = string.Empty;
            this.Messages = new ObservableCollection<IAssistItem>(CurrentChatHistory.Messages);
            ExecuteSideBarCommand(null);
            PopulateOptionsContextMenu();

        }

        private void ExecuteEditorOptionsCommand(object obj)
        {
            var chipText = obj as Option;

            if (chipText.Name == "Attachment")
            {
                chipText.IsOpen = true;
            }
            else if (chipText.Name == "MicroPhone")
            {
                ExecuteVoiceInputCommand(chipText);
            }
        }

        private void ExecuteItemLongPressedCommand(object obj)
        {
            var ItemLongPressedArgs = obj as ItemLongPressedEventArgs;
            CurrentAssistItem = ItemLongPressedArgs.Item as AssistItem;

            if (CurrentAssistItem.IsRequested)
            {
                RequestPopup = App.Current.Resources["RequestContextMenuPopup"] as SfPopup;
                RequestPopup.BindingContext = this;
                RequestPopup.Show(ItemLongPressedArgs.Position.X, ItemLongPressedArgs.Position.Y);
            }
            else
            {
                ResponsePopup = App.Current.Resources["ResponseContextMenuPopup"] as SfPopup;
                ResponsePopup.BindingContext = this;
                ResponsePopup.Show(ItemLongPressedArgs.Position.X, ItemLongPressedArgs.Position.Y);
            }
        }
        private async void ExecuteUploadImage(object obj)
        {
            var chipText = obj as Option;

            EditorOptions[0].IsOpen = false;
            ShowImagePicker();
        }

        private void ExecuteTakePhoto(object obj)
        {
            var chipText = obj as Option;

            EditorOptions[0].IsOpen = false;

            TakePhotoAsync();
        }

        private void ExecuteUploadFile(object obj)
        {
            var chipText = obj as Option;

            EditorOptions[0].IsOpen = false;

        }

        private async void ShowImagePicker()
        {
#if ANDROID || WINDOWS || (IOS && !MACCATALYST)
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "image" + ++imageNo + ".jpg");

            // Get and store the image from gallery
            await _imagePickerHelper.SaveImageAsync(filePath);

            var imageSource = ImageSource.FromFile(filePath);

            this.ImageSourceCollection.Add(imageSource);

            RaisePropertyChanged(nameof(this.HasImageUploaded));

            this.UpdateSendIcon(this.InputText);
            //#elif MACCATALYST

            //            // TODO: The file picker is not supported in Mac Catalyst. So, we have used the custom file picker.
            //            // https://github.com/dotnet/maui/issues/11088
            //            var result = await NativeHelper.ImagePickAsync();
            //            if (string.IsNullOrEmpty(result))
            //            {
            //                return;
            //            }

            //            var imageSource = ImageSource.FromFile(result);            
            //#else
            await Task.Delay(1);
#endif
        }

        private async Task TakePhotoAsync()
        {
            try
            {
                // Check if the MediaPicker is supported
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    // Capture a photo
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        // Get the file path of the photo
                        string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);

                        // Save the photo to the app's local storage
                        using var stream = await photo.OpenReadAsync();
                        using var fileStream = File.OpenWrite(localFilePath);
                        await stream.CopyToAsync(fileStream);

                        var imageSource = ImageSource.FromFile(localFilePath);
                        this.ImageSourceCollection.Add(imageSource);
                        RaisePropertyChanged(nameof(this.HasImageUploaded));
                        this.UpdateSendIcon(this.InputText);
                        Console.WriteLine($"Photo saved to: {localFilePath}");
                    }
                }
                else
                {
                    Console.WriteLine("Camera capture is not supported on this device.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        internal async Task GetResult(object inputQuery, object assistView)
        {
            var htmlConverter = new HtmlConverter();
            this.IsHeaderVisible = false;
            this.SendIconText = "\ue7EB";
            this.SendIconWidth = 40;
            AssistItem responseItem = new AssistItem() { Text = "⚫", ShowAssistItemFooter = false, IsRequested = false };
            this.messages.Add(responseItem);
            this.AddToChatHistoryCollection();
            AssistItem request = (AssistItem)inputQuery;
            if (request != null)
            {
                var userAIPrompt_chart = this.GetChartUserPrompt(request.Text);

                var aiResponseTask1 = Task.Run(async () =>
                {
                    if (!isTemporaryChatEnabled)
                    {
                        string responseText = userAIPrompt_chart;
                        string imagedescription = string.Empty;
                        var source = (request as AssistImageItem)?.Source;
                        if (source != null)
                        {
                            imagedescription += await azureAIService!.AnalyzeImageAzureAsync(source, request.Text);
                            responseText += imagedescription + request.Text;
                        }
                        else
                            responseText += request.Text;

                        CurrentChatHistory.Message += imagedescription + request.Text;
                        var response = await azureAIService!.GetResultsFromAI(responseText, request.Text, userAIPrompt_chart).ConfigureAwait(false);
                        return response;
                    }
                    else
                    {
                        return await azureAIService!.GetResultsFromAI(request.Text, request.Text, userAIPrompt_chart).ConfigureAwait(false);
                    }
                });

                bool isAnimating = true;
                _ = Task.Run(async () =>
                {
                    while (isAnimating)
                    {
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            responseItem.Text = "⚫";
                            await Task.Delay(300);
                            responseItem.Text = "";
                            await Task.Delay(300);
                        });
                    }
                });

                var response1 = await aiResponseTask1;
                response1 = response1.Replace("\n", "<br>");
                response1 = htmlConverter.ConvertToHTML(response1);
                //Json to chart
                var jsonString = ExtractJsonContent(response1);
                var chartConfig = DeserializeJson(jsonString);
                if (chartConfig != null)
                {
                    IAssistItem assistItem = null;
                    switch (chartConfig.ChartType)
                    {
                        case ChartTypeEnum.Cartesian:
                            assistItem = new CartesianAssistItem()
                            {
                                ChartConfig = chartConfig,
                            };
                            break;
                        case ChartTypeEnum.Circular:
                            assistItem = new CircularAssistItem()
                            {
                                ChartConfig = chartConfig,
                            };
                            break;
                    }

                    if (assistItem != null)
                        this.messages.Insert(messages.Count - 1, assistItem);
                }

                var userAIPrompt = this.GetUserAIPrompt(request.Text, jsonString);
                var aiResponseTask = Task.Run(async () =>
                {
                    if (!isTemporaryChatEnabled)
                    {
                        if (string.IsNullOrEmpty(currentChatHistory.Message))
                            CurrentChatHistory.Message += userAIPrompt;
                        CurrentChatHistory.Message += request.Text;
                        var response = await azureAIService!.GetResultsFromAI(CurrentChatHistory.Message, request.Text, userAIPrompt).ConfigureAwait(false);
                        return response;
                    }
                    else
                    {
                        return await azureAIService!.GetResultsFromAI(request.Text, request.Text, userAIPrompt).ConfigureAwait(false);
                    }
                });
                var response = await aiResponseTask;
                isAnimating = false;
                if (!this.isTemporaryChatEnabled)
                {
                    CurrentChatHistory.Message += response;
                }
                response = response.Replace("\n", "<br>");
                response = htmlConverter.ConvertToHTML(response);

                responseItem.RequestItem = inputQuery;
                this.IsNewChatEnabled = true;
                var headerContent = ExtractHeadContent(response);
                var withoutHeaderContent = RemoveHeadContent(response, headerContent);

                string[] words = withoutHeaderContent.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                AssistViewChat chat = null;
                if (assistView != null)
                {
                    var propertyInfo = assistView.GetType().GetField("AssistViewChat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    chat = propertyInfo?.GetValue(assistView) as AssistViewChat;
                }
                string displayText = headerContent;
                foreach (string word in words)
                {
                    if (this.CanStopResponse) break;

                    await Task.Delay(100); // Simulate typing delay
                    displayText += word + " ";
                    responseItem.Text = displayText + "⚫"; // Dot stays at the end
                    chat?.ScrollToMessage(responseItem);
                }

                // Remove the dot at the end of the response
                responseItem.Text = displayText.Trim();
                this.AddToChatHistoryCollection();
                this.isResponseStreaming = false;
                //this.SendIconText = "\ue7E8" + " Voice"; // TODO:Need to add voice input support
                SendIconText = "\ue710";
                this.UpdateSendIcon(this.InputText);
                this.SendIconWidth = 40;

            }
        }
        public static string RemoveHeadContent(string htmlString, string headContent)
        {
            if (string.IsNullOrWhiteSpace(htmlString) || string.IsNullOrWhiteSpace(headContent))
                return htmlString;

            // Remove the head content completely
            return htmlString.Replace(headContent, string.Empty);
        }


        private string ExtractJsonContent(string htmlString)
        {
            // Regex pattern to extract content within <code> tags
            var jsonPattern = @"<code>(.*?)</code>";
            var regex = new Regex(jsonPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var match = regex.Match(htmlString);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return string.Empty; // Return empty if no JSON is found
        }

        private string ExtractHeadContent(string htmlString)
        {
            if (string.IsNullOrWhiteSpace(htmlString))
                return string.Empty;

            var headRegex = new Regex("<head[^>]*>(.*?)</head>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var match = headRegex.Match(htmlString);

            if (match.Success)
            {
                return match.Value;
            }

            return string.Empty;
        }

        private ChartConfig DeserializeJson(string jsonString)
        {
            try
            {
                var chartConfig = JsonConvert.DeserializeObject<ChartConfig>(jsonString);
                return chartConfig;
            }
            catch (JsonException ex)
            {
                // Handle JSON deserialization error
                Console.WriteLine($"Error deserializing JSON: {ex.Message}");
                return null;
            }
        }
        private string GetUserAIPrompt(string userPrompt, string jsonString)
        {
            string userQuery = $"Given a user query and chart data: {userPrompt + jsonString}" +
                   "\nPlease adhere to the following conditions:" +
                   "\n1. Provide a clear title for the topic in bold." +
                   "\n2. Offer a simplified answer consisting of 4 key points, formatted with numbers." +
                   "\n3. Ensure the response is a plain string." +
                   "\n4. If the text is in markdown format, convert it to HTML." +
                   "\n5. Provide the response based on chat histroy." +
                   "\n6. Eliminate any asterisks (**) and any quotation marks present in the string." +
                   "\n7. Provide nested list as as mentioned {userPrompt}";

            return userQuery;
        }


        private string GetChartUserPrompt(string userPrompt)
        {
            string userQuery = @"
As an AI service, your task is to convert user inputs describing chart specifications into JSON formatted strings. Each user input will describe a specific chart type and its configurations, including axes titles, legend visibility, series configurations, etc. You will structure the output in JSON format accordingly.

Example user input: ""Sales by region column chart.""
Expected JSON output:
{
  ""chartType"": ""cartesian"",
  ""title"": ""Revenue by Region"",
  ""showLegend"": true,
""sideBySidePlacement"": ""true/false"",
  ""xAxis"":[
{
    ""type"": ""category"",
    ""title"": ""Region""
  }
],
  ""yAxis"": [
{
    ""title"": ""Revenue"",
    ""type"": ""numerical"",
    ""min"": 0
  }
],
  ""series"": [
    {
      ""type"": ""column"",
      ""xpath"": ""region"",
      ""dataSource"": [
        { ""xvalue"": ""North America"", ""yvalue"": 120000 },
        { ""xvalue"": ""Europe"", ""yvalue"": 90000 },
        { ""xvalue"": ""Asia"", ""yvalue"": 70000 },
        { ""xvalue"": ""South America"", ""yvalue"": 45000 },
        { ""xvalue"": ""Australia"", ""yvalue"": 30000 }
      ],
      ""tooltip"": true
    }
  ]
}

When generating the JSON output, take into account the following:

1. **Chart Type**: Determine the type of chart based on keywords in the user query. and it should be only circular or cartesian
2. **Chart Title**: Craft an appropriate title using key elements of the query.
3. **Axis Information**: Define the x-axis and y-axis with relevant titles and types. Use categories for discrete data and numerical for continuous data.
4. **Series Configuration**: Include details about the series type and data points as mentioned in the query. it supports only  Line, Column, Spline, Area, Pie, Doughnut, RadialBar.
5. **Data Source**: Provide a sample data source for the series, it should only name as ""dataSource"" and include ""xvalue"" and ""yvalue"".
6. **Show Legend**: Default as `true` unless specified otherwise.
7.  **SideBySidePlacement**: Default to 'false' and return bool value based on multiple column series placement and column placed side by side being true, column back to back being false, or Bottom/Top being false, or one column being positive and another being negative values.
     

Generate appropriate configurations according to these guidelines, and return the result as a JSON formatted string for any query shared with you." +

  $"User Request: {userPrompt}";
            return userQuery;
        }

        #endregion

        #region Property Changed

        public event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        #endregion
    }
}
