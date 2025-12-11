using App1.Core;

namespace App1.Commands
{
    public class HelpCommand : ICommand2
    {
        public string Name => "help";
        public Dictionary<string, ISousCommand> SousCommands { get; } = new Dictionary<string, ISousCommand>();
        private readonly Dictionary<string, ICommand2> allCommand;
        public HelpCommand(Dictionary<string, ICommand2> _allCommand)
        {
            allCommand = _allCommand;
        }
        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            //Affichage des Commandes disponibles
            if (args.Count == 0)
            {
                Console.WriteLine("Commandes disponibles :");
                foreach (var command in allCommand.Values)
                    op.Afficher($"- {command.Name}");
                op.Afficher("- clear : Nettoyer la console");
                op.Afficher("- exit : Pour fermer l'application");
                Console.WriteLine("\nEntrer help <commande> pour voir les sous commandes");
                return;
            }

            //help <commande>
            string commandName = args["cmd"];
            if (!allCommand.ContainsKey(commandName))
            {
                Console.WriteLine($"La commande '{commandName}' n'existe pas.");
                return;
            }

            var cmd = allCommand[commandName];
            if (args.Count == 1)
            {
                Console.WriteLine($"\nSous commandes de la commande '{commandName}':");
                foreach (var sous in cmd.SousCommands.Values)
                    op.Afficher($"- {sous.Name}");
                Console.WriteLine($"\nEntrer help {commandName} <sous Commande> pour plus de details");
                return;
            }

            //help <commande> <sous Commande>
            string sousCmd = args["sub"];
            if (!cmd.SousCommands.ContainsKey(sousCmd))
            {
                Console.WriteLine($"La sous commande '{sousCmd}' n'existe pas pour la commande '{commandName}'.");
                return;
            }
            var subCommand = cmd.SousCommands[sousCmd];

            Console.WriteLine($"\nDetails de  '{commandName} {sousCmd}':");
            if (!subCommand.parametres.Any())
            {
                op.Afficher("Aucun parametre.");
                return;
            }
            Console.WriteLine("Parametres :");
            foreach (var param in subCommand.parametres)
                op.Afficher($"- {param.Key} : {param.Value}");
            
        }
    }
}
