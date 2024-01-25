using Customer.Application.Customer;
using Customer.Application.Item;
using Microsoft.Extensions.Logging;

namespace Customer.Application.Cart;

public class Cart
{
    public Customer.Customer Customer { get; set; } = new Customer.Customer();
    public IList<CartLine> Lines { get; set; } = new List<CartLine>();
}

public class CartLine
{
    public Item.Item Item { get; set; } = new Item.Item();
    public int Quantity { get; set; } = 0;
}

public class CartService : ICartService
{
    private readonly ICustomerService _customerService;
    private readonly IItemService _itemService;
    private readonly ICartMetrics _metrics;

    private readonly ILogger<ICartService> _logger;

    protected IList<Cart> carts { get; set; } = new List<Cart>();

    public CartService(ICustomerService customerService, IItemService itemService, ICartMetrics metrics, ILogger<ICartService> logger)
    {
        _customerService = customerService
            ?? throw new ArgumentNullException(nameof(customerService));

        _itemService = itemService
            ?? throw new ArgumentNullException(nameof(itemService));

        _metrics = metrics;

        _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Cart> CreateCart(int customerId)
    {
        using var activity = _metrics.ActivitySource.StartActivity("CreateCartActivity");
        _logger.LogInformation("CreateCart called with customerId={customerId}", customerId);
        _metrics.CartUpdate(1);

        var selectedCart = new Cart()
        {
            Customer = await _customerService.GetCustomerById(customerId),
            Lines = new List<CartLine>()
        };
        carts.Add(selectedCart);

        return await Task.FromResult(selectedCart);
    }

    public async Task DeleteCart(int customerId)
    {
        using var activity = _metrics.ActivitySource.StartActivity("DeleteCartActivity");
        _logger.LogInformation("DeleteCart called with customerId={customerId}", customerId);
        _metrics.CartUpdate(-1);

        var selectedCart = await GetCartByCustomerId(customerId);
        carts.Remove(selectedCart);
    }

    public async Task<Cart> GetCartByCustomerId(int customerId)
    {
        using var activity = _metrics.ActivitySource.StartActivity("GetCartByCustomerIdActivity");
        _logger.LogInformation("GetCartByCustomerId called with customerId={customerId}", customerId);

        var selectedCart = carts.FirstOrDefault(x => x.Customer.Id == customerId);
        if (selectedCart == null)
            selectedCart = await CreateCart(customerId);

        return await Task.FromResult(selectedCart);
    }

    public async Task AddItem(int customerId, int itemId, int quantity)
    {
        using var activity = _metrics.ActivitySource.StartActivity("AddItemActivity");
        _logger.LogInformation("AddItem called with customerId={customerId}, itemId={itemId}, quantity={quantity}", customerId, itemId, quantity);

        var selectedCart = await GetCartByCustomerId(customerId);
        var selectedItem = await _itemService.GetItemById(itemId);

        _metrics.ProductUpdate(quantity, selectedItem.Name);

        carts.Remove(selectedCart);

        if (selectedCart.Lines.Any(x => x.Item.Id == itemId))
        {
            var actualLine = selectedCart.Lines.First(x => x.Item.Id == itemId);
            var actualQty = actualLine.Quantity;
            quantity += actualQty;
            selectedCart.Lines.Remove(actualLine);
        }
        selectedCart.Lines.Add(new CartLine
        {
            Item = selectedItem,
            Quantity = quantity
        });

        carts.Add(selectedCart);
    }
}
