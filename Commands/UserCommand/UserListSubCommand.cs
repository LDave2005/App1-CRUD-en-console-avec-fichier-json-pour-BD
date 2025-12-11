using App1.Core;
using App1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.UserCommand
{
    public class UserListSubCommand : ISousCommand
    {
        public string Name => "list";
        public List<Parametre> Parametres { get; } = new();

        public List<Parametre> parametres => throw new NotImplementedException();

        public void Execute(Dictionary<string, string> args)
        {
            //Console.WriteLine("Listing all users...");
            UserView user = new UserView();
            Console.ForegroundColor = ConsoleColor.Yellow;
            user.AfficherUtilisateursTable();
            Console.ForegroundColor= ConsoleColor.White;
        }
    }
}
