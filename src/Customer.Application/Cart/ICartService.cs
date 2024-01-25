namespace Customer.Application.Cart;

public interface ICartService
{
    Task<Cart> GetCartByCustomerId(int customerId);

    Task AddItem(int customerId, int itemId, int quantity);

    Task DeleteCart(int customerId);
}
