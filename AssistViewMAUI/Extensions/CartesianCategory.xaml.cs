using Syncfusion.Maui.Toolkit.Charts;
using System.Collections.ObjectModel;

namespace ChartGenerater;

public partial class CartesianCategory : SfCartesianChart
{
    public CartesianCategory()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ObservableCollection<SeriesConfig>), typeof(CartesianCategory), null, BindingMode.Default, null, OnPropertyChanged);
    public ObservableCollection<SeriesConfig> Source
    {
        get => (ObservableCollection<SeriesConfig>)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (Equals(oldValue, newValue))
        {
            return;
        }

        if (bindable is CartesianCategory chart)
        {
            chart.GenerateSeries(newValue as ObservableCollection<SeriesConfig>);
        }
    }

    private void GenerateSeries(ObservableCollection<SeriesConfig> configs)
    {
        if (configs != null)
        {
            this.Series.Clear();
            foreach (var config in configs)
            {
                CreateSeriesFromTemplate(config);
            }

            var paletteBrush = GetPaletteBrushes();

            if (Series.Count == 1)
            {
                if (Series[0] is ColumnSeries series)
                {
                    series.PaletteBrushes = paletteBrush;
                }
            }
            else
            {
                this.PaletteBrushes = paletteBrush;
            }
        }
    }

    private Brush[] GetPaletteBrushes()
    {
        var random = new Random();
        switch (random.Next(1, 6))
        {
            case 1:
                return Resources["Pallet1"] as Brush[];
            case 2:
                return Resources["Pallet2"] as Brush[];
            case 3:
                return Resources["Pallet3"] as Brush[];
            case 4:
                return Resources["Pallet4"] as Brush[];
            case 5:
                return Resources["Pallet5"] as Brush[];
            default:
                return Resources["Pallet6"] as Brush[];
        }
    }

    private void CreateSeriesFromTemplate(SeriesConfig seriesConfig)
    {
        var templateSelector = (SeriesTemplateSelector)Resources["seriesTemplateSelector"];
        var template = templateSelector.SelectTemplate(seriesConfig, null);

        if (template != null)
        {
            ChartSeries series = (ChartSeries)template.CreateContent();

            if (series != null)
            {
                series.BindingContext = seriesConfig;
                this.Series.Add(series);
            }
        }
    }
}