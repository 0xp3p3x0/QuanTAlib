using System.Drawing;
using TradingPlatform.BusinessLayer;

namespace QuanTAlib;

public class CviIndicator : Indicator, IWatchlistIndicator
{
    [InputParameter("Periods", sortIndex: 1, 1, 2000, 1, 0)]
    public int Periods { get; set; } = 20;

    [InputParameter("Show cold values", sortIndex: 21)]
    public bool ShowColdValues { get; set; } = true;

    private Cvi? cvi;
    protected LineSeries? CviSeries;
    public int MinHistoryDepths => Math.Max(5, Periods * 2);
    int IWatchlistIndicator.MinHistoryDepths => MinHistoryDepths;

    public CviIndicator()
    {
        Name = "CVI - Chaikin's Volatility";
        Description = "Measures the volatility of a financial instrument by comparing the spread between the high and low prices.";
        SeparateWindow = true;

        CviSeries = new($"CVI {Periods}", Color.Blue, 2, LineStyle.Solid);
        AddLineSeries(CviSeries);
    }

    protected override void OnInit()
    {
        cvi = new Cvi(Periods);
        base.OnInit();
    }

    protected override void OnUpdate(UpdateArgs args)
    {
        TBar input = IndicatorExtensions.GetInputBar(this, args);
        TValue result = cvi!.Calc(input);

        CviSeries!.SetValue(result.Value);
        CviSeries!.SetMarker(0, Color.Transparent); //OnPaintChart draws the line, hidden here
    }

#pragma warning disable CA1416 // Validate platform compatibility

    public override string ShortName => $"CVI ({Periods})";

    public override void OnPaintChart(PaintChartEventArgs args)
    {
        base.OnPaintChart(args);
        this.PaintHLine(args, 0.05, new Pen(Color.DarkRed, width: 2));
        this.PaintSmoothCurve(args, CviSeries!, cvi!.WarmupPeriod, showColdValues: ShowColdValues, tension: 0.2);
    }
}
