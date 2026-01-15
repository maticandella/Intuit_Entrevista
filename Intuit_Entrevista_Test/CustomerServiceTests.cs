using AutoMapper;
using Intuit_Entrevista.Data.Abstractions;
using Intuit_Entrevista.Domain;
using Intuit_Entrevista.DTO;
using Intuit_Entrevista.Services;
using NUnit.Framework;
using Moq;
using FluentAssertions;

namespace Intuit_Entrevista.Tests
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<ICustomerRepository> _mockRepo;
        private CustomerService _service;

        [SetUp]
        public void SetUp()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepo = new Mock<ICustomerRepository>();
            _service = new CustomerService(_mockMapper.Object, _mockRepo.Object);
        }

        #region GetCustomersAsync Tests

        [Test]
        public async Task GetCustomersAsync_ReturnsListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com"),
                CreateSampleCustomer(2, "María", "González", "27-23456789-0", "maria@test.com")
            };
            var customerDtos = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new CustomerDTO { Id = 2, Nombre = "María", Apellido = "González" }
            };

            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(customers)).Returns(customerDtos);

            // Act
            var result = await _service.GetCustomersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Nombre.Should().Be("Juan");
            result[1].Nombre.Should().Be("María");

            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<List<CustomerDTO>>(customers), Times.Once);
        }

        [Test]
        public async Task GetCustomersAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<Customer>();
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(emptyList)).Returns(new List<CustomerDTO>());

            // Act
            var result = await _service.GetCustomersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ExistingId_ReturnsCustomer()
        {
            // Arrange
            var customer = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customerDto = new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDTO>(customer)).Returns(customerDto);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Nombre.Should().Be("Juan");
            result.Apellido.Should().Be("Pérez");
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer)null);
            _mockMapper.Setup(m => m.Map<CustomerDTO>(null)).Returns((CustomerDTO)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public async Task GetByIdAsync_DifferentIds_CallsRepositoryWithCorrectId(int id)
        {
            // Arrange
            var customer = CreateSampleCustomer(id, "Test", "User", "20-12345678-9", "test@test.com");
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDTO>(customer)).Returns(new CustomerDTO { Id = id });

            // Act
            await _service.GetByIdAsync(id);

            // Assert
            _mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
        }

        #endregion

        #region Create Tests

        [Test]
        public async Task Create_ValidCustomer_ReturnsNewId()
        {
            // Arrange
            var dto = CreateSampleCreateDTO("Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customer = CreateSampleCustomer(0, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var createdCustomer = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockMapper.Setup(m => m.Map<Customer>(dto)).Returns(customer);
            _mockRepo.Setup(r => r.Create(It.IsAny<Customer>())).ReturnsAsync(createdCustomer);

            // Act
            var result = await _service.Create(dto);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(1);

            _mockMapper.Verify(m => m.Map<Customer>(dto), Times.Once);
            _mockRepo.Verify(r => r.Create(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task Create_RepositoryReturnsNull_ReturnsNull()
        {
            // Arrange
            var dto = CreateSampleCreateDTO("Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customer = CreateSampleCustomer(0, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockMapper.Setup(m => m.Map<Customer>(dto)).Returns(customer);
            _mockRepo.Setup(r => r.Create(It.IsAny<Customer>())).ReturnsAsync((Customer)null);

            // Act
            var result = await _service.Create(dto);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task Create_MapsCorrectly_CallsMapperWithDTO()
        {
            // Arrange
            var dto = CreateSampleCreateDTO("Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customer = CreateSampleCustomer(0, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var createdCustomer = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockMapper.Setup(m => m.Map<Customer>(dto)).Returns(customer);
            _mockRepo.Setup(r => r.Create(It.IsAny<Customer>())).ReturnsAsync(createdCustomer);

            // Act
            await _service.Create(dto);

            // Assert
            _mockMapper.Verify(m => m.Map<Customer>(dto), Times.Once);
        }

        #endregion

        #region Update Tests

        [Test]
        public async Task Update_ValidCustomer_ReturnsRowsAffected()
        {
            // Arrange
            var dto = CreateSampleUpdateDTO(1, "Juan Actualizado", "Pérez", "20-12345678-9", "juan@test.com");
            var customerInDb = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customerInDb);
            _mockRepo.Setup(r => r.Update(It.IsAny<Customer>())).ReturnsAsync(1);

            // Act
            var result = await _service.Update(1, dto);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().Be(1);

            _mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepo.Verify(r => r.Update(It.IsAny<Customer>()), Times.Once);
        }

        [Test]
        public async Task Update_NonExistingCustomer_ReturnsValidationFailure()
        {
            var dto = CreateSampleUpdateDTO(999, "Test", "User", "20-12345678-9", "test@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Customer)null);

            var result = await _service.Update(999, dto);

            result.IsT1.Should().BeTrue();
            var errors = result.AsT1;
            errors.Should().NotBeEmpty();

            errors.Should().Contain(e =>
                e.ErrorMessage.Contains("no existe", StringComparison.OrdinalIgnoreCase) ||
                e.ErrorMessage.Contains("especificado", StringComparison.OrdinalIgnoreCase)
            );
        }

        [Test]
        public async Task Update_UpdatesAllFields_CorrectlyMapsFields()
        {
            var dto = CreateSampleUpdateDTO(1, "NuevoNombre", "NuevoApellido", "20-99999999-9", "nuevo@test.com");
            var customerInDb = CreateSampleCustomer(1, "Viejo", "Nombre", "20-12345678-9", "viejo@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customerInDb);
            _mockRepo.Setup(r => r.Update(It.IsAny<Customer>())).ReturnsAsync(1);

            var result = await _service.Update(1, dto);

            result.IsT0.Should().BeTrue();

            _mockRepo.Verify(r => r.Update(It.Is<Customer>(c =>
                c.Nombre == "NuevoNombre" &&
                c.Apellido == "NuevoApellido" &&
                c.CUIT == "20-99999999-9" &&
                c.Email == "nuevo@test.com" &&
                c.FechaModificacion != null
            )), Times.Once);
        }

        [Test]
        public async Task Update_SetsFechaModificacion_ToCurrentDateTime()
        {
            // Arrange
            var dto = CreateSampleUpdateDTO(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customerInDb = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var beforeUpdate = DateTime.Now;

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customerInDb);
            _mockRepo.Setup(r => r.Update(It.IsAny<Customer>())).ReturnsAsync(1);

            // Act
            await _service.Update(1, dto);
            var afterUpdate = DateTime.Now;

            // Assert
            _mockRepo.Verify(r => r.Update(It.Is<Customer>(c =>
                c.FechaModificacion >= beforeUpdate &&
                c.FechaModificacion <= afterUpdate
            )), Times.Once);
        }

        [Test]
        public async Task Update_RepositoryReturnsZero_ReturnsZero()
        {
            // Arrange
            var dto = CreateSampleUpdateDTO(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");
            var customerInDb = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customerInDb);
            _mockRepo.Setup(r => r.Update(It.IsAny<Customer>())).ReturnsAsync(0);

            // Act
            var result = await _service.Update(1, dto);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().Be(0);
        }

        #endregion

        #region Delete Tests

        [Test]
        public async Task Delete_ExistingCustomer_ReturnsRowsAffected()
        {
            // Arrange
            var customer = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockRepo.Setup(r => r.Delete(customer)).ReturnsAsync(1);

            // Act
            var result = await _service.Delete(1);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().Be(1);

            _mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockRepo.Verify(r => r.Delete(customer), Times.Once);
        }

        [Test]
        public async Task Delete_RepositoryReturnsZero_ReturnsZero()
        {
            // Arrange
            var customer = CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);
            _mockRepo.Setup(r => r.Delete(customer)).ReturnsAsync(0);

            // Act
            var result = await _service.Delete(1);

            // Assert
            result.IsT0.Should().BeTrue();
            result.AsT0.Should().Be(0);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(100)]
        public async Task Delete_DifferentIds_CallsRepositoryWithCorrectCustomer(int id)
        {
            // Arrange
            var customer = CreateSampleCustomer(id, "Test", "User", "20-12345678-9", "test@test.com");

            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(customer);
            _mockRepo.Setup(r => r.Delete(customer)).ReturnsAsync(1);

            // Act
            await _service.Delete(id);

            // Assert
            _mockRepo.Verify(r => r.Delete(It.Is<Customer>(c => c.Id == id)), Times.Once);
        }

        #endregion

        #region Search Tests

        [Test]
        public async Task Search_WithTerm_ReturnsMatchingCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com")
            };
            var customerDtos = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" }
            };

            _mockRepo.Setup(r => r.SearchByNameWithSpAsync("juan")).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(customers)).Returns(customerDtos);

            // Act
            var result = await _service.Search("juan");

            // Assert
            result.Should().HaveCount(1);
            result[0].Nombre.Should().Be("Juan");

            _mockRepo.Verify(r => r.SearchByNameWithSpAsync("juan"), Times.Once);
        }

        [Test]
        public async Task Search_EmptyTerm_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                CreateSampleCustomer(1, "Juan", "Pérez", "20-12345678-9", "juan@test.com"),
                CreateSampleCustomer(2, "María", "González", "27-23456789-0", "maria@test.com")
            };
            var customerDtos = new List<CustomerDTO>
            {
                new CustomerDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
                new CustomerDTO { Id = 2, Nombre = "María", Apellido = "González" }
            };

            _mockRepo.Setup(r => r.SearchByNameWithSpAsync("")).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(customers)).Returns(customerDtos);

            // Act
            var result = await _service.Search("");

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public async Task Search_NoMatches_ReturnsEmptyList()
        {
            // Arrange
            var emptyList = new List<Customer>();
            _mockRepo.Setup(r => r.SearchByNameWithSpAsync("xyz")).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(emptyList)).Returns(new List<CustomerDTO>());

            // Act
            var result = await _service.Search("xyz");

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        [TestCase("juan")]
        [TestCase("maría")]
        [TestCase("pérez")]
        [TestCase("")]
        public async Task Search_DifferentTerms_CallsRepositoryWithCorrectTerm(string term)
        {
            // Arrange
            _mockRepo.Setup(r => r.SearchByNameWithSpAsync(term)).ReturnsAsync(new List<Customer>());
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(It.IsAny<List<Customer>>())).Returns(new List<CustomerDTO>());

            // Act
            await _service.Search(term);

            // Assert
            _mockRepo.Verify(r => r.SearchByNameWithSpAsync(term), Times.Once);
        }

        #endregion

        #region Helper Methods

        private Customer CreateSampleCustomer(int id, string nombre, string apellido, string cuit, string email)
        {
            return new Customer
            {
                Id = id,
                Nombre = nombre,
                Apellido = apellido,
                RazonSocial = $"{nombre} {apellido} S.A.",
                CUIT = cuit,
                Email = email,
                FechaNacimiento = new DateTime(1990, 1, 1),
                TelefonoCelular = "1199999999",
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };
        }

        private CustomerCreateDTO CreateSampleCreateDTO(string nombre, string apellido, string cuit, string email)
        {
            return new CustomerCreateDTO
            {
                Nombre = nombre,
                Apellido = apellido,
                RazonSocial = $"{nombre} {apellido} S.A.",
                CUIT = cuit,
                Email = email,
                FechaNacimiento = new DateTime(1990, 1, 1),
                TelefonoCelular = "1199999999"
            };
        }

        private CustomerUpdateDTO CreateSampleUpdateDTO(int id, string nombre, string apellido, string cuit, string email)
        {
            return new CustomerUpdateDTO
            {
                Nombre = nombre,
                Apellido = apellido,
                RazonSocial = $"{nombre} {apellido} S.A.",
                CUIT = cuit,
                Email = email,
                FechaNacimiento = new DateTime(1990, 1, 1),
                TelefonoCelular = "1199999999"
            };
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            _mockMapper = null;
            _mockRepo = null;
            _service = null;
        }
    }
}
