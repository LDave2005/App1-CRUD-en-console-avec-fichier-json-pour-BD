using App1.Core;
using App1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commands.UserCommand
{
    public class UserCreateSubCommand : ISousCommand
    {
        string ISousCommand.Name => "create";
        public List<Parametre> parametres { get; } = new List<Parametre>
        {
            new Parametre("u","Nom d'utilisateur"),
            new Parametre("e", "Email"),
            new Parametre("p", "Mot de passe "),
            new Parametre("a", "Droit (si administrateur)"),
            new Parametre("r", "Role a assigner (si administrateur)"),
        };

        public void Execute(Dictionary<string, string> args)
        {
            Traitement op = new Traitement();
            UserServices services = new UserServices();

            string? username;
            if (args.ContainsKey("u"))
            {
                username = args["u"];
            }
            else
            {
                username = op.LireAvecReset("Nom d'utilisateur : ", out bool reset); if (reset) return;
            }
            string? email;
            if (args.ContainsKey("e"))
            {
                email = args["e"];
            }
            else
            {
                email = op.LireAvecReset("Email : ", out bool reset); if (reset) return;
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

            if (!args.ContainsKey("a"))
            {
                services.CreerUtilisateur(username, email, password, "guest");
                Console.WriteLine("Utilisateur cree avec le droit 'guest'");
            }
            else
            {
                string? adminUserName;
                if (args.ContainsKey("a"))
                {
                    if (args["a"] != null)
                    {
                        adminUserName = args["a"];
                    }
                    else
                    {
                        adminUserName = op.LireAvecReset("Admin Email : ", out bool reset); if (reset) return;
                    }
                }
                else
                {
                    adminUserName = null;
                }

                if (adminUserName != null)
                {
                    string? adminUserPassword = op.LireMotDePasseAvecReset("Admin Mot de Passe : ");
                    if (adminUserPassword == null) return;
                    string? role;
                    if (args.ContainsKey("r"))
                    {
                        role = args["r"];
                    }
                    else
                    {
                        role = op.LireAvecReset("Role a assigner (user par defaut) : ", out bool reset); if (reset) return;
                    }
                    var auth = services.Authentifier(adminUserName, adminUserPassword);
                    if (auth != null)
                    {
                        services.CreerUtilisateurAuth(adminUserName, adminUserPassword, username, email, password, role);
                        op.Afficher($"Utilisateur cree avec le role {role}");
                    }
                }
            }
            
        }
    }
}
