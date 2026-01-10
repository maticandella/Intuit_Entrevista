using Intuit_Entrevista.Data.Abstractions;
using Intuit_Entrevista.Domain;
using Microsoft.EntityFrameworkCore;

namespace Intuit_Entrevista.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly Context _context;
        private readonly DbSet<Customer> _customers;
        public CustomerRepository(Context context)
        {
            _context = context;
            _customers = _context.Set<Customer>();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _customers.ToListAsync();
        }
    }
}
