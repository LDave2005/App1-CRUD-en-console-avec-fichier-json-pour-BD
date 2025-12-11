using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.UserCommand
{
    public class UserConnectCommand : ISousCommand
    {
        public string Name => "connect";
        public List<Parametre> Parametres { get; } = new List<Parametre>
        {
            new Parametre("e"),
            new Parametre("p"),
        };
        public List<Parametre> parametres => throw new NotImplementedException();
        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            UserServices services = new UserServices();

            string? email;
            if (args.ContainsKey("e"))
            {
                email = args["e"];
            }
            else
            {
                email = op.LireAvecReset("Votre Email : ", out bool reset); if(reset) return;
            }
            string? password;
            if (args.ContainsKey("p"))
            {
                password = args["p"];
            }
            else
            {
                password = op.LireMotDePasseAvecReset();
                if (password == null) return;
            }

            var user = services.Authentifier(email??" ", password);
            if (user == null)
            {
                op.Afficher("Connexion Echoue : Information Errone");
                Console.ReadLine();
            }
            else
            {
                op.Afficher("Connexion Reussir");
            }
        }
    }
}
