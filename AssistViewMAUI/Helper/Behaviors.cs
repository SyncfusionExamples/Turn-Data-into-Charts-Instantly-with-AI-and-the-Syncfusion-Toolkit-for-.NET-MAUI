using Syncfusion.Maui.AIAssistView;
using Syncfusion.Maui.Core;
using Syncfusion.Maui.DataSource;

namespace AssistViewMAUI
{
    class EditorTextChangedBehavior : Behavior<CustomEditor>
    {
        protected override void OnAttachedTo(CustomEditor bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnEditorTextChanged;
        }

        protected override void OnDetachingFrom(CustomEditor bindable)
        {
            bindable.TextChanged -= OnEditorTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnEditorTextChanged(object? sender, TextChangedEventArgs e)
        {
            var viewModel = (sender as CustomEditor)!.BindingContext as ChatGPTViewModel;
            if (viewModel != null && viewModel.Messages.Count == 0 && string.IsNullOrEmpty(e.NewTextValue))
            {
                viewModel.IsHeaderVisible = true;
            }
        }
    }
    class SfAssistViewHeaderBehavior : Behavior<ContentPage>
    {
        #region Fields
        internal ChatGPTViewModel? viewModel;
        private Grid? mainGrid;
        int headerHeight = DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst ? 255 : 430;
        private int editorHeight = 56;
        const int minPadding = 24;
        #endregion

        #region Overrides

        protected override void OnAttachedTo(ContentPage bindable)
        {
            mainGrid = bindable.FindByName<Grid>("mainGrid");
            viewModel = bindable.BindingContext as ChatGPTViewModel;
            bindable.Loaded += this.OnMainPageLoaded;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            bindable.Loaded -= this.OnMainPageLoaded;
            viewModel = null;
            base.OnDetachingFrom(bindable);
        }
        #endregion

        #region CallBacks
        private void OnMainPageLoaded(object? sender, EventArgs e)
        {
#if WINDOWS
            var window = Application.Current?.MainPage?.Handler?.PlatformView as Microsoft.Maui.Platform.RootNavigationView;
            if (window != null)
            {
                // Hide the hamburger menu (PaneToggleButton).
                window.IsPaneToggleButtonVisible = false;
            }
#endif
        }
        #endregion

    }

    public class SfListViewGroupingBehavior : Behavior<ContentPage>
    {
        #region Fields

        private Syncfusion.Maui.ListView.SfListView ListView;
        private SfEntry searchBar = null;

        #endregion

        #region Overrides
        protected override void OnAttachedTo(ContentPage bindable)
        {
            ListView = ListView = bindable.FindByName<Syncfusion.Maui.ListView.SfListView>("listView");
            this.ListView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "ConversationDate",
                KeySelector = (obj) =>
                {
                    var groupName = ((ChatHistoryModel)obj).ConversationCreatedDate;
                    return GetKey(groupName);
                },
                Comparer = new CustomGroupComparer(),
            });
            searchBar = bindable.FindByName<SfEntry>("filterText");
            searchBar.TextChanged += SearchBar_TextChanged;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(ContentPage bindable)
        {
            ListView = null;
            searchBar = null;
            searchBar.TextChanged -= SearchBar_TextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SfEntry);
            if (ListView.DataSource != null)
            {
                ListView.DataSource.Filter = FilterContacts;
                ListView.DataSource.RefreshFilter();
            }
            ListView.RefreshView();
        }
        private bool FilterContacts(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var contactInfo = obj as ChatHistoryModel;
            return (contactInfo.Title.ToLower().Contains(searchBar.Text.ToLower()) || (contactInfo.Title.ToString()).ToLower().Contains(searchBar.Text.ToLower()));
        }


        #endregion

        /// <summary>
        /// Helper method to check whether particular date is in last week or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in last week.</returns>
        private bool IsLastWeek(DateTime groupName)
        {
            var groupWeekSunDay = groupName.AddDays(-(int)groupName.DayOfWeek).Day;
            var lastSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).Day - 7;

            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return lastSunday == groupWeekSunDay && (groupMonth == currentMonth || groupMonth == currentMonth - 1) && isCurrentYear;
        }

        /// <summary>
        /// Helper method to check whether particular date is in last month or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in last month.</returns>
        private bool IsLastMonth(DateTime groupName)
        {
            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.AddMonths(-1).Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return groupMonth == currentMonth && isCurrentYear;
        }
        /// <summary>
        /// Helper method to get the key value for the GroupHeader name based on Data.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns specific group name.</returns>
        private GroupName GetKey(DateTime groupName)
        {
            int compare = groupName.Date.CompareTo(DateTime.Now.Date);

            if (compare == 0)
            {
                return GroupName.Today;
            }
            else if (groupName.Date.CompareTo(DateTime.Now.AddDays(-1).Date) == 0)
            {
                return GroupName.Yesterday;
            }
            else if (IsLastWeek(groupName))
            {
                return GroupName.Previous7Days;
            }
            else if (IsLastMonth(groupName))
            {
                return GroupName.Previous30Days;
            }
            else
            {
                return GroupName.Older;
            }
        }

    }

    public class AssistViewBehavior : Behavior<SfAIAssistView>
    {
        private AssistViewChat? assistViewChat;
        private SfAIAssistView? assistView;
        private ChatGPTViewModel? viewModel;
        private ContentView? headerView;
        public Border? EditorView { get; set; }
        public Grid? HeaderView { get; set; }

        protected override void OnAttachedTo(SfAIAssistView bindable)
        {
            base.OnAttachedTo(bindable);
            this.assistView = bindable;
            this.assistView.PropertyChanged += AssistView_PropertyChanged;
            var propertyInfo = this.assistView.GetType().GetField("AssistViewChat", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            this.assistViewChat = propertyInfo?.GetValue(this.assistView) as AssistViewChat;
            if (this.assistViewChat != null)
            {
                this.assistViewChat.Scrolled += AssistViewChat_Scrolled;
                this.assistViewChat.Loaded += AssistViewChat_Loaded;
            }
            if (this.EditorView != null)
            {
                this.EditorView.PropertyChanged += EditorView_PropertyChanged;
            }
            if (this.HeaderView != null && this.HeaderView.Parent != null)
            {
                this.headerView = this.HeaderView.Parent as ContentView;
                this.headerView!.PropertyChanged += HeaderView_PropertyChanged;
            }
        }

        private void HeaderView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsVisible" && (sender as View).IsVisible)
            {
                this.SetHeaderMargin();
            }
        }

        private void EditorView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Height")
            {
                this.SetHeaderMargin();
            }
        }

        private void AssistView_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Height")
            {
                this.SetHeaderMargin();
            }
        }

        private void SetHeaderMargin()
        {
            if (this.viewModel != null && this.EditorView != null && this.HeaderView != null)
            {
                var chatHeight = this.assistView!.Height - this.EditorView.Height - viewModel.autoSuggestionListHeight - 20;// editor padding
                HeaderView.HeightRequest = chatHeight;
            }
        }

        private void AssistViewChat_Loaded(object? sender, EventArgs e)
        {
            this.viewModel = this.assistView!.BindingContext as ChatGPTViewModel;
            if (this.viewModel != null)
            {
                this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }
            this.viewModel!.WebViewWidthRequest = this.assistViewChat!.Width - (this.assistViewChat.Width * 0.2);
            this.SetHeaderMargin();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AutoSuggestions" && viewModel.autoSuggestionListHeight > 0)
                this.SetHeaderMargin();
        }

        private void AssistViewChat_Scrolled(object? sender, Syncfusion.Maui.Chat.ChatScrolledEventArgs e)
        {
            if (!this.viewModel.isResponseStreaming)
            {
                viewModel.IsScrollToBottom = !e.IsBottomReached;
            }
        }

        protected override void OnDetachingFrom(SfAIAssistView bindable)
        {
            base.OnDetachingFrom(bindable);
            this.assistViewChat.Scrolled -= AssistViewChat_Scrolled;
            this.assistViewChat.Loaded -= AssistViewChat_Loaded;
            this.assistView.PropertyChanged -= AssistView_PropertyChanged;
            this.EditorView.PropertyChanged -= EditorView_PropertyChanged;
            this.viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            this.headerView.PropertyChanged -= HeaderView_PropertyChanged;
            this.headerView = null;
            this.assistViewChat = null;
            this.assistView = null;
        }
    }
    public enum GroupName
    {
        Today = 0,
        Yesterday,
        Previous7Days,
        Previous30Days,
        ThisMonth,
        LastMonth,
        Older
    }

    public class ChipBehavior : Behavior<SfChip>
    {
        protected override void OnAttachedTo(SfChip bindable)
        {
            base.OnAttachedTo(bindable);
            var propertyInfo = bindable.GetType().GetProperty("HorizontalPadding", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var padding = propertyInfo?.GetValue(bindable);
            propertyInfo?.SetValue(bindable, 0);
        }
    }
}