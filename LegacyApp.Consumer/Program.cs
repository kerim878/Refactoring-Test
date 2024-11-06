namespace LegacyApp.Consumer;

internal class Program
{
    private static void Main(string[] args)
    {
        AddUser(args);
    }

    public static void AddUser(string[] args)
    {
        // DO NOT CHANGE THIS FILE AT ALL
        var clientRepository = new ClientRepository();
        var userCreditServiceClient = new UserCreditServiceClient();
        var userDataAccess = new UserDataAccess();
        var userService = new UserService(clientRepository, userCreditServiceClient, userDataAccess);
        var addResult = userService.AddUser("John", 
            "Doe", 
            "John.doe@gmail.com", 
            new DateTime(1993, 1, 1), 
            4);
        
        Console.WriteLine("Adding John Doe was " + (addResult ? "successful" : "unsuccessful"));
    }
}