using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Item;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class ItemService : IItemService
{
    private readonly string[] _names = ["Torreznos", "Cabecilla", "Mollejas", "Criadillas", "Carrilleras"];

    protected IList<Item> items = new List<Item>();

    public async Task<Item> GetItemById(int id)
    {
        var selectedItem = items.FirstOrDefault(x => x.Id == id);
        if (selectedItem == null)
        {
            selectedItem = new Item
            {
                Id = id,
                Name = _names[new Random().Next(0, 4)],
            };
            items.Add(selectedItem);
        }

        return await Task.FromResult(selectedItem);
    }
}
