using App1;
using App1.Commandes;

public class jsonCrud
{
    public static void Main(string[] args)
    {
        // Créer et enregistrer les commandes au démarrage (réutilisable)
        Dictionary<string, App1.Interfaces.ICommand> commands = new Dictionary<string, App1.Interfaces.ICommand>(StringComparer.OrdinalIgnoreCase)
        {
            {"connect", new ConnectCommand() },
            {"create", new CreateUserCommand() },
            {"user", new UserCommand() }
            // ajouter d'autres commandes ici : { "create", new CreateCommand() }, ...
        };

        CommandParser parser = new CommandParser();
        var parser2 = new CommandParser2();


        while (true)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("dave> "); Console.ForegroundColor= ConsoleColor.White;
            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                continue;

            if (input == "exit") break;
            parser2.ProcessInput(input);

        }
    }
}