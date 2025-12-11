using App1.Interfaces;
using App1.Services;
using App1.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Commandes
{
    public class UserCommand : ICommand
    {
        public string Name => "user";

        public void Execute(string[] args)
        {
            UserServices service = new UserServices();
            Traitement op = new Traitement();
            UserView userView = new UserView();

            var cmd = args[0].ToLower();
            switch (cmd)
            {
                case "list":
                    userView.AfficherUtilisateursTable();
                    break;
                case "create":
                    string? nom = null;
                    string? email = null;
                    string? password = null;
                    string? role = "guest";
                    string? adminEmail = null;
                    string? adminMdp = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        switch (args[i])
                        {
                            case "-n":
                                nom = args.Length > i + 1 ? args[i + 1] : null;
                                i++;
                                break;
                            case "-e":
                                email = args.Length > i + 1 ? args[i + 1] : null;
                                i++;
                                break;
                            case "-p":
                                password = args.Length > i + 1 ? args[i + 1] : null;
                                i++;
                                break;
                            case "-r":
                                role = args.Length > i + 1 ? args[i + 1] : null;
                                i++;
                                break;
                            case "-a":
                                if (i + 2 < args.Length) { adminEmail = args[i + 1]; adminMdp = args[i + 2]; }
                                break;
                        }
                    }

                    if (string.IsNullOrEmpty(nom))
                    {
                        nom = op.LireAvecReset("Nom: ", out bool reset2);
                        if (reset2) return;
                    }
                    if (string.IsNullOrEmpty(email))
                    {
                        email = op.LireAvecReset("Email: ", out bool reset2);
                        if (reset2) return;
                    }
                    if (string.IsNullOrEmpty(password))
                    {
                        Console.Write("Mot de passe: "); password = op.LireMotDePasseAvecReset();
                        if (password == null) return;

                    }

                    bool ok;
                    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminMdp))
                    {
                        ok = service.CreerUtilisateurAuth(adminEmail, adminMdp, nom, email, password);
                    }
                    else
                    {
                        ok = service.CreerUtilisateur(nom, email, password, "guest");
                    }

                    //Console.ForegroundColor = ok ? ConsoleColor.Yellow : ConsoleColor.Red;
                    //Console.WriteLine(ok ? "Utilisateur créé." : "Échec création (email déjà utilisé ou données invalides / droits manquants).");
                    //Console.ForegroundColor = ConsoleColor.White;
                    if (ok)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Utilisateur Cree.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Echec creation (email deja utilise ou donnees invalides / droits manquants).");
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    Console.WriteLine("Commande invalide");
                    Console.WriteLine("Exemples : user list    |    user create -u <nom> -m <email> -p <pwd> [-r <role>] [--auth adminEmail adminPwd]");
                    break;
            }
        }
    }
}
