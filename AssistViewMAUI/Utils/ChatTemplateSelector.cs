using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;

namespace ChartGenerater;

public class ChatTemplateSelector : ResponseItemTemplateSelector
{
    public DataTemplate DefaultTemplate { get; set; }

    public DataTemplate CartesianTemplate { get; set; }

    public DataTemplate CircularTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is CartesianAssistItem)
        {
            return CartesianTemplate;
        }
        else if (item is CircularAssistItem)
        {
            return CircularTemplate;
        }
        else
            return DefaultTemplate;
    }
}


public class StatisticsAssistItem : AssistItem
{
    private ObservableCollection<WeeklyStats> chartData;
    public ObservableCollection<WeeklyStats> ChartData
    {
        get { return chartData; }
        set
        {
            chartData = value;
            OnPropertyChanged(nameof(ChartData));
        }
    }
}

public class CartesianAssistItem : AssistItem
{
    private ChartConfig chart;

    public ChartConfig ChartConfig { get { return chart; } set { chart = value; OnPropertyChanged(nameof(ChartConfig)); } }
}

public class CircularAssistItem : AssistItem
{
    private ChartConfig chart;

    public ChartConfig ChartConfig { get { return chart; } set { chart = value; OnPropertyChanged(nameof(ChartConfig)); } }
}
// Data model for the chart
public class WeeklyStats
{
    public double Week { get; set; }
    public double Views { get; set; }
}

