using Customer.Application.Cart;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers;

public class CartController : ApiControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService, ILogger<CartController> logger)
        : base(logger)
    {
        _cartService = cartService
            ?? throw new ArgumentException(nameof(cartService));
    }

    [HttpGet("GetCart")]
    public async Task<Cart> Get(int customerId)
    {
        return await _cartService.GetCartByCustomerId(customerId);
    }

    [HttpPut("AddItemAsync")]
    public async Task AddItem(int customerId, int itemId, int quantity)
    {
        await _cartService.AddItem(customerId, itemId, quantity);
    }

    [HttpPost("Delete")]
    public async Task Delete(int customerId)
    {
        await _cartService.DeleteCart(customerId);
    }
}
