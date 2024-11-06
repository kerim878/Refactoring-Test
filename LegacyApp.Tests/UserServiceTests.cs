using Bogus;
using NSubstitute;

namespace LegacyApp.Tests;

[TestFixture]
public class UserServiceTests
{
    [SetUp]
    public void SetUp()
    {
        _faker = new Faker();
        _clientRepository = Substitute.For<IClientRepository>();
        _userCreditService = Substitute.For<IUserCreditService>();
        _userDataAccess = Substitute.For<IUserDataAccess>();
        _userService = new UserService(_clientRepository, _userCreditService, _userDataAccess);
    }

    private UserService _userService;
    private IClientRepository _clientRepository;
    private IUserCreditService _userCreditService;
    private IUserDataAccess _userDataAccess;
    private Faker _faker;

    [Test]
    public void AddUser_ShouldReturnFalse_WhenNameIsInvalid()
    {
        // Arrange
        var firstName = string.Empty;
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddUser_ShouldReturnFalse_WhenEmailIsInvalid()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = "invalid-email"; // Intentionally invalid
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddUser_ShouldReturnFalse_WhenAgeIsInvalid()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-20); // Under 21
        var clientId = _faker.Random.Int();

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddUser_ShouldReturnFalse_WhenCreditLimitIsInsufficient()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();
        var client = new Client { Name = "RegularClient" };

        _clientRepository.GetById(clientId).Returns(client);
        _userCreditService.GetCreditLimit(firstName, surname, dateOfBirth).Returns(400);

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void AddUser_ShouldReturnTrue_WhenAllConditionsAreMet()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();
        var client = new Client { Name = "RegularClient" };

        _clientRepository.GetById(clientId).Returns(client);
        _userCreditService.GetCreditLimit(firstName, surname, dateOfBirth).Returns(600);

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void AddUser_ShouldReturnTrue_WhenClientIsVeryImportant()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();
        var client = new Client { Name = "VeryImportantClient" };

        _clientRepository.GetById(clientId).Returns(client);

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void AddUser_ShouldReturnTrue_WhenClientIsImportantAndCreditLimitIsDoubled()
    {
        // Arrange
        var firstName = _faker.Name.FirstName();
        var surname = _faker.Name.LastName();
        var email = _faker.Internet.Email();
        var dateOfBirth = DateTime.Now.AddYears(-30);
        var clientId = _faker.Random.Int();
        var client = new Client { Name = "ImportantClient" };

        _clientRepository.GetById(clientId).Returns(client);
        _userCreditService.GetCreditLimit(firstName, surname, dateOfBirth).Returns(300);

        // Act
        var result = _userService.AddUser(firstName, surname, email, dateOfBirth, clientId);

        // Assert
        Assert.That(result, Is.True);
    }
}