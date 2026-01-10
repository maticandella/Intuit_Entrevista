using Intuit_Entrevista.DTO;
using Intuit_Entrevista.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Intuit_Entrevista.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<CustomerDTO>>> Get()
        {
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers);
        }
    }
}