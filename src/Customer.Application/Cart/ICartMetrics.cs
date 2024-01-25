using System.Diagnostics;

namespace Customer.Application.Cart;

public interface ICartMetrics
{
    ActivitySource ActivitySource { get; }

    void CartUpdate(int delta);

    void ProductUpdate(int delta, string productName);
}
