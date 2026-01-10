using AutoMapper;
using Intuit_Entrevista.Data.Abstractions;
using Intuit_Entrevista.DTO;
using Intuit_Entrevista.Services.Abstractions;

namespace Intuit_Entrevista.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(IMapper mapper, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<List<CustomerDTO>> GetCustomersAsync()
        {
            //TODO controlar errores
            var result = await _customerRepository.GetAllAsync();
            var customers = _mapper.Map<List<CustomerDTO>>(result);
            return customers;
        }
    }
}
