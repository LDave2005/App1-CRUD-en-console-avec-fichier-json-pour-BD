using App1.Interfaces;
using App1.Services;
using App1.View;
using System.Text.RegularExpressions;

namespace App1.Commandes
{
    public class ConnectCommand : ICommand
    {
        public string Name => "connect";
        public ConnectCommand() { }

        public void Execute(string[] args)
        {
            UserServices service = new UserServices();
            Traitement op = new Traitement();
            CommandParser parser = new CommandParser();

            string? username = null;
            string? password = null;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-u":
                        if (i + 1 < args.Length)
                        {
                            username = args[i + 1];
                            i++;
                        }
                        break;
                    case "-p":
                        if (i + 1 < args.Length)
                        {
                            password = args[i + 1];
                            i++;
                        }
                        break;
                }
            }
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Email : ");
                username = Console.ReadLine();
            }

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Mot de passe : ");
                password = Console.ReadLine();
            }

            var user = service.Authentifier(username ?? "", password ?? "");
            if (user == null)
            {
                Console.WriteLine("Échec d'authentification (ou compte inactif ou en attente de validation).");
                var action = op.AttendreEntreeOuReset("Appuyez sur Entrée pour réessayer ou Ctrl+R pour revenir au menu principal...");
                if (action == Traitement.PauseAction.Reset)
                    return; // revient au menu principal

            }

            // Menu après authentification
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Acces Autorise");
            Console.ForegroundColor = ConsoleColor.White;
            bool session = true;
            UserView userView = new UserView();
            while (session)
            {
                Console.Write("> ");
                //userView.MenuUser(user);
                ProductView prView = new ProductView();
                string? input2 = Console.ReadLine();
                string secondpattern = @"^dave\s+(?<cmd>\w+)(?:\s+(?<scmd>\w+))?(?:\s+(?<args>.*))?$";
                var cmdMatch2 = Regex.Match(input2 ?? "", secondpattern, RegexOptions.IgnoreCase);
                if (!cmdMatch2.Success)
                {
                    Console.WriteLine("❌ Commande invalide.");
                    return;
                }
                string cmd = cmdMatch2.Groups["cmd"].Value;
                // Gestion selon role
                if (string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase))
                {
                    ClientView clientView = new ClientView();
                    UserView usrView = new UserView();
                    switch (cmd.ToLower())
                    {
                        case "client":
                            // (sous-menu clients identique)
                            //clientView.MenuClient();
                            string scmd = cmdMatch2.Groups["scmd"].Value;
                            switch (scmd.ToLower())
                            {
                                case "list":
                                    parser.AfficherClientsParse(input2 ?? "");
                                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                    Console.ReadLine();
                                    break;
                                case "create":
                                    parser.CreateClientParse(input2 ?? "");
                                    break;
                                case "modify":
                                    parser.ModifyClientParse(input2 ?? "");
                                    break;
                                case "delete":
                                    parser.DeleteClientParse(input2 ?? "");
                                    break;
                                /*case "exit":
                                                                                    // Retour au menu principal
                                    break;*/
                                default:
                                    Console.WriteLine("Option non valide");
                                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                    Console.ReadLine();
                                    break;
                            }
                            break;
                        case "user":
                            /*Console.Clear();
                            usrView.AfficherUtilisateursTable();
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();*/
                            scmd = cmdMatch2.Groups["scmd"].Value;
                            switch (scmd.ToLower())
                            {
                                case "list":
                                    parser.AfficherUSerParse(input2 ?? "");
                                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                    Console.ReadLine();
                                    break;
                                case "create":
                                    parser.CreateUserParse(input2 ?? "");
                                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                    Console.ReadLine();
                                    break;
                                case "reinitialize":
                                    break;
                            }
                            break;
                        case "d-create-user":
                            bool reset1 = true;
                            var nom = op.LireAvecReset("Email: ", out reset1);
                            if (reset1) break; // retour au menu principal

                            var mail = op.LireAvecReset("Email: ", out reset1);
                            if (reset1) break; // retour au menu principal

                            Console.Write("Mot de passe: "); var pwd = op.LireMotDePasseAvecReset();
                            Console.Write("Rôle (admin/user): "); var role = Console.ReadLine() ?? "user";
                            if (service.CreerUtilisateur(nom, mail, pwd, role))
                                Console.WriteLine("Utilisateur créé.");
                            else
                                Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                        case "d-reinitialize-mdp":
                            var cible = op.LireAvecReset("Email: ", out reset1);
                            if (reset1) break; // retour au menu principal

                            Console.Write("Votre mot de passe (pour confirmer): "); var conf = op.LireMotDePasseAvecReset();
                            Console.Write("Nouveau mot de passe pour la cible: "); var nouv = op.LireMotDePasseAvecReset();
                            if (service.ReinitialiserMdp(user.email, conf, cible, nouv))
                                Console.WriteLine("Mot de passe réinitialisé.");
                            else
                                Console.WriteLine("Échec de la réinitialisation (droits insuffisants ou cible introuvable).");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                        case "d-deactivation":
                            var cible2 = op.LireAvecReset("Email: ", out reset1);
                            if (reset1) break; // retour au menu principal
                            Console.Write("Votre mot de passe (pour confirmer): "); var conf2 = op.LireMotDePasseAvecReset();
                            if (service.DesactiverUtilisateur(user.email, conf2, cible2))
                                Console.WriteLine("Si l'utilisateur existait, il a été désactivé.");
                            else
                                Console.WriteLine("Échec (droits insuffisants ou cible introuvable).");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                        case "d-validation":
                            {
                                // Reconfirmer le mot de passe admin avant action
                                UserView table = new UserView();
                                table.AfficherUtilisateursTable();
                                var adminPwd = op.LireMotDePasseAvecReset("Confirmez votre mot de passe admin pour continuer: ");
                                if (adminPwd == null) break; // reset -> retour au menu principal

                                // Lire l'email du guest à valider
                                var cibleEmail = op.LireAvecReset("Email du guest à valider: ", out reset1);
                                if (reset1 || string.IsNullOrWhiteSpace(cibleEmail))
                                {
                                    Console.WriteLine("Action annulée.");
                                    break;
                                }

                                // Choix du rôle à attribuer (par défaut "user")
                                Console.Write("Rôle à attribuer (user/admin) [user]: ");
                                var roleAttribue = Console.ReadLine();
                                if (string.IsNullOrWhiteSpace(roleAttribue)) roleAttribue = "user";

                                // Appel de la méthode d'assignation
                                bool ok = service.AssignerRoleAdmin(user.email ?? "", adminPwd, cibleEmail, roleAttribue);
                                if (ok)
                                    Console.WriteLine($"Le rôle '{roleAttribue}' a été attribué à {cibleEmail}.");
                                else
                                    Console.WriteLine("Échec : vérifiez vos droits, mot de passe ou email cible.");

                                var pause = op.AttendreEntreeOuReset();
                                if (pause == Traitement.PauseAction.Reset) { session = false; } // ou ajustez pour revenir au menu principal
                                break;
                            }
                        case "d-deconnect":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Deconnexion reussie");
                            Console.ForegroundColor = ConsoleColor.White;
                            session = false;
                            break;
                        case "d-gestion-produit":
                            {
                                prView.MenuProduct();
                                break;
                            }
                        case "d-help":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Voici les commandes a maitriser pour aisement naviguer dans cette section\n" +
                                "d-manage-client : Pour acceder a la section de management des client\n" +
                                "d-user-table : Permet d'afficher la table des utilsateurs de l'application\n" +
                                "d-create-user : Permet de creer un utilisateur (reserve seulement aux admin)\n" +
                                "d-reinitialize-mdp : Permet de reinitialiser les mots de passe de n'importe quel utilisateur\n" +
                                "d-deactivation : Permet de desactiver un utilisateur\n" +
                                "d-validation : Permet d'attribuer des droits d'utilisateur a un invite\n" +
                                "d-gestion-produit : Permet d'acceder a la section de management des produits\n" +
                                "d-deconnect : Se deconnecter\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        default:
                            Console.WriteLine("Option non valide");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    ClientView clientView = new ClientView();
                    UserView usrView = new UserView();
                    // utilisateur non-admin
                    switch (cmd.ToLower())
                    {
                        case "d-manage-client":
                            // (sous-menu clients)
                            clientView.MenuClient(); break;
                            ;
                        case "d-user-table":
                            Console.Clear();
                            usrView.AfficherUtilisateursTable();
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                        case "d-reinitialize-mdp":
                            // Réinitialiser son propre mot de passe
                            Console.Write("Ancien mot de passe: "); var ancien = op.LireMotDePasseAvecReset();
                            Console.Write("Nouveau mot de passe: "); var nouv = op.LireMotDePasseAvecReset();
                            if (service.ReinitialiserMdp(user.email, ancien, user.email, nouv))
                                Console.WriteLine("Mot de passe changé.");
                            else
                                Console.WriteLine("Échec (ancien mot de passe invalide).");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                        case "d-gestion-produit":
                            prView.MenuProduct();
                            break;
                        case "d-deconnect":
                            session = false;
                            break;
                        case "d-help":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Voici les commandes a maitriser pour aisement naviguer dans cette section\n" +
                                "d-manage-client : Pour acceder a la section de management des client\n" +
                                "d-user-table : Permet d'afficher la table des utilsateurs de l'application\n" +
                                "d-reinitialize-mdp : Permet de reinitialiser les mots de passe de n'importe quel utilisateur\n" +
                                "d-gestion-produit : Permet d'acceder a la section de management des produits\n" +
                                "d-deconnect : Se deconnecter\n");
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                        default:
                            Console.WriteLine("Option non valide");
                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                            Console.ReadLine();
                            break;
                    }
                }

                continue;

            }
        }
    }
}
