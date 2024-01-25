using Customer.Application.Cart;
using Customer.Application.Customer;
using Customer.Application.Item;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddSingleton<ICartMetrics, CartMetrics>();

        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IItemService, ItemService>();
        services.AddSingleton<ICartService, CartService>();

        return services;
    }
}