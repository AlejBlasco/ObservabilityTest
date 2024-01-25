namespace Customer.Application.Item
{
    public interface IItemService
    {
        Task<Item> GetItemById(int id);
    }
}
