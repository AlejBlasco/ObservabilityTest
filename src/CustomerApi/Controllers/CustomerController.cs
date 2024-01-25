using Customer.Application.Customer;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers;

public class CustomerController : ApiControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        : base(logger)
    {
        _customerService = customerService
            ?? throw new ArgumentNullException(nameof(customerService));
    }


    [HttpGet("GetCustomers")]
    public async Task<IList<Customer.Application.Customer.Customer>> Get()
    {
        return await _customerService.GetCustomers();
    }

    [HttpGet("GetCustomers/{customerId}")]
    public async Task<Customer.Application.Customer.Customer> Get(int customerId)
    {
        return await _customerService.GetCustomerById(customerId);
    }
}
