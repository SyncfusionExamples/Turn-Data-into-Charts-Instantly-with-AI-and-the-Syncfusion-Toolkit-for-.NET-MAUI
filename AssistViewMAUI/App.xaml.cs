using Syncfusion.Maui.DataForm;

namespace AssistViewMAUI
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MDAxQDMyMzgyZTMwMmUzMGFMV081MVlpVnJ5aHJyQzUzMFFBOUJwZ1B0YWkxNC80WnA3SHdvbmdqNzA9");
            InitializeComponent();

            // MainPage = new FlyoutSideBarPage();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new FlyoutSideBarPage());
        }
        private void dataForm_ValidateProperty(object sender, Syncfusion.Maui.DataForm.DataFormValidatePropertyEventArgs e)
        {
            var dataForm = sender as SfDataForm;
            var viewModel = dataForm.BindingContext as ChatGPTViewModel;
            var currentTitle = (dataForm.DataObject as ChatHistoryModel).Title;
            if (currentTitle != null && !e.NewValue.Equals(currentTitle))
            {
                viewModel.IsRenameEnabled = true;
            }
            else
            {
                viewModel.IsRenameEnabled = false;
            }
        }
    }
}
