using Intuit_Entrevista.DTO;

namespace Intuit_Entrevista.Services.Abstractions
{
    public interface ICustomerService
    {
        Task<List<CustomerDTO>> GetCustomersAsync();
    }
}
