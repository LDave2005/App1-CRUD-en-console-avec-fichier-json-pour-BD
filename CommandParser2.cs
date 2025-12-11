using App1.Commands;
using App1.Commands.ClientCommand;
using App1.Commands.ProductCommand;
using App1.Commands.UserCommand;
using App1.Core;


namespace App1
{
    public class CommandParser2
    {
        private readonly Dictionary<string, ICommand2> commands = new();
        public CommandParser2()
        {
            // Ajouter la commande d'aide avec toutes les commandes disponibles
            commands["user"] = new UserCommand2();
            commands["client"] = new ClientCommand();
            commands["product"] = new ProductCommand();
            commands["help"] = new HelpCommand(commands);
        }
        public void ProcessInput(string input)
        {
            var tokens = input.Split(' ');

            if (tokens[0] == "help")
            {
                var args2 = new Dictionary<string, string>();

                if (tokens.Length >= 2)
                {
                    args2["cmd"] = tokens[1];
                }
                if (tokens.Length >= 3)
                {
                    args2["sub"] = tokens[2];
                }
                commands["help"].Execute(args2);
                return;
            }
            if (!commands.TryGetValue(tokens[0], out var command))
            {
                Console.WriteLine("Unknown command");
                return;
            }

            if (tokens.Length == 1)
            {
                command.Execute(new());
                return;
            }

            if (!command.SousCommands.TryGetValue(tokens[1], out var sub))
            {
                Console.WriteLine("Unknown subcommand");
                return;
            }

            var args = ParseArgs(tokens.Skip(2).ToArray());
            sub.Execute(args);
        }

        private Dictionary<string, string> ParseArgs(string[] parts)
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < parts.Length - 1; i++)
            {
                if (parts[i].StartsWith("-"))
                    dict[parts[i].TrimStart('-')] = parts[i + 1];
            }
            return dict;
        }
    }
}
