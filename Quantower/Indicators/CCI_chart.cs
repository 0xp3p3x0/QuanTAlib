using System.Diagnostics;
using System.Drawing;
using TradingPlatform.BusinessLayer;
namespace QuanTAlib;

public class CCI_chart : Indicator
{
    #region Parameters

    [InputParameter("Smoothing period", 0, 1, 999, 1, 1)]
    private readonly int Period = 10;

    #endregion Parameters

    private TBars bars;

    ///////
    private CCI_Series indicator;
    ///////

    public CCI_chart()
    {
        this.SeparateWindow = true;
        this.Name = "CCI - Commodity Channel Index";
        this.Description = "CCI description";
        this.AddLineSeries("CCI", Color.RoyalBlue, 3, LineStyle.Solid);
    }

    protected override void OnInit()
    {
        this.bars = new();
				this.indicator = new(source: bars, period: this.Period, useNaN: false);
    }

	protected override void OnUpdate(UpdateArgs args)
	{
        bool update = (args.Reason != UpdateReason.NewBar && args.Reason != UpdateReason.HistoricalBar);
        this.bars.Add(this.Time(), this.GetPrice(PriceType.Open), this.GetPrice(PriceType.High), this.GetPrice(PriceType.Low),
                      this.GetPrice(PriceType.Close), this.GetPrice(PriceType.Volume), update);
        double result = this.indicator[this.indicator.Count - 1].v;
        this.SetValue(result);
    }
}
