using Syncfusion.Maui.Core;

namespace AssistViewMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnChipClicked(object sender, EventArgs e)
        {
            var viewmodel = this.BindingContext as ChatGPTViewModel;
            var chip = (sender as SfChip);
            var layout = chip.Children[0] as HorizontalStackLayout;

            Option option;
            if (layout != null)
            {
                option = layout.BindingContext as Option;
            }
            else
            {
                var label = chip.Children[0] as Grid;
                option = label.BindingContext as Option;
            }

            if (string.IsNullOrEmpty(option.Name) || !option.IsEnabled)
                return;

            switch (option.Name)
            {
                case "Attachment":
                    viewmodel.EditorOptionsComamnd.Execute(option);
                    break;
                case "MicroPhone":
                    viewmodel.EditorOptionsComamnd.Execute(option);
                    break;
                default:
                    viewmodel.HeaderPromptCommand.Execute(option.Name);
                    break;
            }
        }
    }

}
