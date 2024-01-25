using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Customer.Application.Cart;

public class CartMetrics : ICartMetrics
{
    private readonly ActivitySource _activitySource;
    private readonly UpDownCounter<int> _cartCounter;
    private readonly UpDownCounter<int> _productsCounter;

    public ActivitySource ActivitySource => _activitySource;

    public CartMetrics(IMeterFactory meterFactory,
        IConfiguration configuration)
    {
        // Meter
        var meterName = configuration.GetValue<string>("Metrics:ApiMetrics:Meter:Name")
            ?? throw new ArgumentNullException(nameof(configuration));
        var meterVersion = configuration.GetValue<string>("Metrics:ApiMetrics:Meter:Version")
            ?? throw new ArgumentNullException(nameof(configuration));

        var meter = meterFactory.Create(name: meterName,
            version: meterVersion);

        // Counters
        _cartCounter = meter.CreateUpDownCounter<int>($"{(meterName.ToLower())}.carts");
        _productsCounter = meter.CreateUpDownCounter<int>($"{(meterName.ToLower())}.products_sold");

        // ActivitySource
        var activitySourceName = configuration.GetValue<string>("Metrics:ApiMetrics:ActivitySource:Name")
            ?? throw new ArgumentNullException(nameof(configuration));
        var activitySourceVersion = configuration.GetValue<string>("Metrics:ApiMetrics:ActivitySource:Version")
            ?? throw new ArgumentNullException(nameof(configuration));

        _activitySource = new ActivitySource(name: activitySourceName,
            version: activitySourceVersion);
    }

    public void CartUpdate(int delta)
    {
        this._cartCounter.Add(delta);
    }

    public void ProductUpdate(int delta, string productName)
    {
        this._productsCounter.Add(delta,
            new KeyValuePair<string, object?>("product", productName));
    }
}
