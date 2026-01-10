using Intuit_Entrevista.Domain;

namespace Intuit_Entrevista.Data.Abstractions
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
    }
}
