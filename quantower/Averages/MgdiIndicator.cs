using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace QuanTAlib;

public class MgdiIndicator : Indicator, IWatchlistIndicator
{
    [InputParameter("Periods", sortIndex: 1, 1, 1000, 1, 0)]
    public int Periods { get; set; } = 14;

    [InputParameter("K-Factor", sortIndex: 2, 0.1, 2, 0.1, 1)]
    public double KFactor { get; set; } = 0.6;

    [InputParameter("Data source", sortIndex: 3, variants: [
        "Open", SourceType.Open,
        "High", SourceType.High,
        "Low", SourceType.Low,
        "Close", SourceType.Close,
        "HL/2 (Median)", SourceType.HL2,
        "OC/2 (Midpoint)", SourceType.OC2,
        "OHL/3 (Mean)", SourceType.OHL3,
        "HLC/3 (Typical)", SourceType.HLC3,
        "OHLC/4 (Average)", SourceType.OHLC4,
        "HLCC/4 (Weighted)", SourceType.HLCC4
    ])]
    public SourceType Source { get; set; } = SourceType.Close;

    [InputParameter("Show cold values", sortIndex: 21)]
    public bool ShowColdValues { get; set; } = true;

    private Mgdi? ma;
    protected LineSeries? Series;
    protected string? SourceName;
    public int MinHistoryDepths => Periods;
    int IWatchlistIndicator.MinHistoryDepths => MinHistoryDepths;

    public override string ShortName => $"MGDI {Periods}:{KFactor}:{SourceName}";

    public MgdiIndicator()
    {
        OnBackGround = true;
        SeparateWindow = false;
        SourceName = Source.ToString();
        Name = "MGDI - McGinley Dynamic Indicator";
        Description = "McGinley Dynamic Indicator";
        Series = new(name: $"MGDI {Periods}", color: Color.Yellow, width: 2, style: LineStyle.Solid);
        AddLineSeries(Series);
    }

    protected override void OnInit()
    {
        ma = new Mgdi(Periods, KFactor);
        SourceName = Source.ToString();
        base.OnInit();
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        TValue input = this.GetInputValue(args, Source);
        TValue result = ma!.Calc(input);

        Series!.SetValue(result.Value);
        Series!.SetMarker(0, Color.Transparent); //OnPaintChart draws the line, hidden here
    }

    public override void OnPaintChart(PaintChartEventArgs args)
    {
        base.OnPaintChart(args);
        this.PaintSmoothCurve(args, Series!, ma!.WarmupPeriod, showColdValues: ShowColdValues, tension: 0.2);
        this.DrawText(args, Description);
    }
}
