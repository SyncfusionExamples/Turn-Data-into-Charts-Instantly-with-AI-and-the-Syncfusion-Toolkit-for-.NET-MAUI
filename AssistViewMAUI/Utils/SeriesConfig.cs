using System;
using System.Collections.ObjectModel;

namespace ChartGenerater;

public class SeriesConfig
{
    public SeriesType Type 
    {
        get; 
        set;
    }

    public string XPath
    {
        get;
        set;
    }

    public ObservableCollection<DataModel> DataSource 
    {
        get; 
        set;
    }

    public bool Tooltip
    {
        get;
        set;
    }
}

public class DataModel
{
    public string xvalue
    {
        get; 
        set;
    }

    public double yvalue
    {
        get; 
        set;
    }

    public DateTime? date
    {
        get; 
        set;
    }

    public double? xval
    {
        get; 
        set;
    }
}