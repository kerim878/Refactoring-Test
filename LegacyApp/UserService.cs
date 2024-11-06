using System.Text.RegularExpressions;

namespace LegacyApp;

public class UserService
{
    public bool AddUser(string firstName, string surname, string email, DateTime dateOfBirth, int clientId)
    {
        if (!IsValidName(firstName, surname) || !IsValidEmail(email) || !IsValidAge(dateOfBirth))
        {
            return false;
        }

        var client = GetClientById(clientId);
        var user = CreateUser(firstName, surname, email, dateOfBirth, client);

        if (!IsCreditLimitSufficient(user, client))
        {
            return false;
        }

        UserDataAccess.AddUser(user);
        return true;
    }

    private static bool IsValidName(string firstName, string surname)
    {
        return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(surname);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return false;
        }

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    private static bool IsValidAge(DateTime dateOfBirth)
    {
        var now = DateTime.Now;
        var age = now.Year - dateOfBirth.Year;
        if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day))
        {
            age--;
        }

        return age >= 21;
    }

    private static Client GetClientById(int clientId)
    {
        var clientRepository = new ClientRepository();
        return clientRepository.GetById(clientId);
    }

    private static User CreateUser(string firstName, string surname, string email, DateTime dateOfBirth, Client client)
    {
        return new User
        {
            Client = client,
            DateOfBirth = dateOfBirth,
            EmailAddress = email,
            Firstname = firstName,
            Surname = surname
        };
    }

    private static bool IsCreditLimitSufficient(User user, Client client)
    {
        if (client.Name == "VeryImportantClient")
        {
            user.HasCreditLimit = false;
            return true;
        }

        user.HasCreditLimit = true;
        var creditLimit = GetCreditLimit(user);

        if (client.Name == "ImportantClient")
        {
            creditLimit *= 2;
        }

        user.CreditLimit = creditLimit;
        return user.CreditLimit >= 500;
    }

    private static int GetCreditLimit(User user)
    {
        using var userCreditService = new UserCreditServiceClient();
        return userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);
    }
}