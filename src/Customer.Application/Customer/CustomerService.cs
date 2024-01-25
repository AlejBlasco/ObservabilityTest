namespace Customer.Application.Customer;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CustomerService : ICustomerService
{
    private readonly string[] _names = ["Silvinus", "Liviu", "Judith", "Vegard", "Ariel"];
    
    protected IList<Customer> customers = new List<Customer>();

    public async Task<Customer> GetCustomerById(int id)
    {
        var selectedCustomer = customers.FirstOrDefault(x => x.Id == id);
        if (selectedCustomer == null)
        {
            selectedCustomer = new Customer
            {
                Id = id,
                Name = _names[new Random().Next(0, 4)],
            };
            customers.Add(selectedCustomer);
        }

        return await Task.FromResult(selectedCustomer);
    }

    public async Task<IList<Customer>> GetCustomers()
    {
        return await Task.FromResult(customers);
    }

}
