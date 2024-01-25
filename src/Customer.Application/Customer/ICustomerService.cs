namespace Customer.Application.Customer;

public interface ICustomerService
{
    Task<Customer> GetCustomerById(int id);

    Task<IList<Customer>> GetCustomers();
}
