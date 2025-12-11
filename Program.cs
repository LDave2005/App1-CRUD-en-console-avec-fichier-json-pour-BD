using App1;
using App1.Commandes;
using App1.Interfaces;
using App1.Modeles;
using App1.Services;
using App1.View;
using System.Text.RegularExpressions;
using System.Windows.Input;


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

            if (string.IsNullOrEmpty(input)){
                continue;
            }

            if (input == "exit") break;
            parser2.ProcessInput(input);

            /*string[] cmdArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (cmdArgs.Length == 0) continue;
            string command = cmdArgs[0].ToLower();

            // Dispatcher vers une commande si enregistrée
            if (commands.TryGetValue(command, out var cmd))
            {
                // Passer uniquement les arguments après le nom de la commande
                var argsToPass = cmdArgs.Skip(1).ToArray();
                cmd.Execute(argsToPass);
                continue;
            }*/
            
            //Commande de connexion en utilisant une expression reguliere =>

            /*var cmdMatch = Regex.Match(input ?? "", @"^dave\s(?<cmd>\w+)", RegexOptions.IgnoreCase);
            if (!cmdMatch.Success)
            {
                Console.WriteLine("❌ Commande invalide.");
                continue;
            }
            string mainCommand = cmdMatch.Groups["cmd"].Value;*/

           // ConnectCommand cmdMere = new ConnectCommand();
            //List<string> entree = new List<string>();

            //UserServices service = new UserServices();
            //Traitement op = new Traitement();
            

            //bool? reset;


            /*while(ConsoleKey.Enter != Console.ReadKey(true).Key)
            {
                entree.Add(Console.ReadLine() ?? string.Empty);
            }

            while(entree[0] != cmdMere.nomC)
            {
                Console.WriteLine("Entrer une commande valide!");
                entree[0] = Console.ReadLine() ?? string.Empty; // Correction également ici pour éviter l'assignation de null
            }*/


            switch (input)
            {
                case "connect":
                    {

                        //cmdMere.Command(cmdArgs);
                        
                        /*string pattern = @"^dave\sconnect\s-u\s(?<email>\S+)\s-p\s(?<password>\S+)\s*$";

                        Match match = Regex.Match(input ?? "", pattern, RegexOptions.IgnoreCase);

                        if (!match.Success)
                        {
                            Console.WriteLine("Acces non autorise, Format invalide. Exemple : dave connect -u John -p 1234");
                        }

                        string email = match.Groups["email"].Value;
                        string password = match.Groups["password"].Value;

                        // Appel de ta fonction existante

                        var user = service.Authentifier(email, password);
                        if (user == null)
                        {
                            Console.WriteLine("Échec d'authentification (ou compte inactif ou en attente de validation).");
                            var action = op.AttendreEntreeOuReset("Appuyez sur Entrée pour réessayer ou Ctrl+R pour revenir au menu principal...");
                            if (action == Traitement.PauseAction.Reset)
                                break; // revient au menu principal
                            else
                                continue; // retenter l'authentification
                        }*/

                        /*bool reset;
                        var email = op.LireAvecReset("Email: ", out reset);
                        if (reset) break; // retour au menu principal

                        var mdp = op.LireMotDePasseAvecReset("Mot de passe: ");
                        if (mdp == null) break; // reset -> retour au menu principal

                        var user = service.Authentifier(email ?? "", mdp);*/


                        // Menu après authentification

                        /*Console.ForegroundColor = ConsoleColor.Yellow;
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
                                        switch(scmd.ToLower())
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
                                                break;//
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
                                        Console.ReadLine();//
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
                                            if (reset1|| string.IsNullOrWhiteSpace(cibleEmail))
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

                        }*/
                    }

                    break;
                case "create":
                    {

                        /*bool reset2;
                        var nom = op.LireAvecReset("Nom: ", out reset2);
                        if (reset2) break;

                        var mail = op.LireAvecReset("Email: ", out reset2);
                        if (reset2) break;

                        Console.Write("Mot de passe: "); var pwd = op.LireMotDePasseAvecReset();

                        if (service.CreerUtilisateur(nom, mail, pwd, "guest"))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Compte créé avec le rôle 'guest'");
                            Console.WriteLine("Un administrateur doit valider votre compte avant que vous puissiez vous connecter.");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                            Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");

                        var action = op.AttendreEntreeOuReset();
                        if (action == Traitement.PauseAction.Reset)
                        {
                            break; // Retour au menu principal
                        }*/
                        //parser.CreateParse(input ?? "");
                        break;
                    }
                case "help":
                    //parser.HelpParse(input ?? "");
                    break;
                case "quit":
                    //return;
                   // parser.ExitParse(input ?? "");
                    break;
                default:
                    /*Console.WriteLine("Option non valide");
                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                    Console.ReadLine();*/
                    break;
            }

        }
    }
}