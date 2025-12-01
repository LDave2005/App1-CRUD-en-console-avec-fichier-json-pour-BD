using System.CommandLine;
using System.CommandLine.Parsing;
using App1;
using App1.Services;
using App1.View;

public class jsonCrud
{
    public static void Main(string[] args)
    {
       
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Gestion utilisateurs / clients ===");
            Console.WriteLine("1) Se connecter");
            Console.WriteLine("2) Créer un utilisateur");
            Console.WriteLine("3) Quitter");
            Console.Write("Choix: ");
            var choix = Console.ReadLine();
            UserServices service = new UserServices();
            Traitement op = new Traitement();

            switch (choix)
            {
                case "1":
                    {

                        bool reset;
                        var email = op.LireAvecReset("Email: ", out reset);
                        if (reset) break; // retour au menu principal

                        var mdp = op.LireMotDePasseAvecReset("Mot de passe: ");
                        if (mdp == null) break; // reset -> retour au menu principal

                        var user = service.Authentifier(email ?? "", mdp);
                        if (user == null)
                        {
                            Console.WriteLine("Échec d'authentification (ou compte inactif ou en attente de validation).");
                            var action = op.AttendreEntreeOuReset("Appuyez sur Entrée pour réessayer ou Ctrl+R pour revenir au menu principal...");
                            if (action == Traitement.PauseAction.Reset)
                                break; // revient au menu principal
                            else
                                continue; // retenter l'authentification
                        }

                        // Menu après authentification
                        bool session = true;
                        UserView userView = new UserView();
                        while (session)
                        {
                            userView.MenuUser(user);
                            ProductView prView = new ProductView();
                            var opt = Console.ReadLine();

                            // Gestion selon role
                            if (string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                ClientView clientView = new ClientView();
                                UserView usrView = new UserView();
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients identique)
                                        clientView.MenuClient(); break;
                                    case "2":
                                        Console.Clear();
                                        usrView.AfficherUtilisateursTable();
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "3":
                                        var nom = op.LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        var mail = op.LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        Console.Write("Mot de passe: "); var pwd = op.LireMotDePasseAvecReset();
                                        Console.Write("Rôle (admin/user): "); var role = Console.ReadLine() ?? "user";
                                        if (service.CreerUtilisateur(nom, mail, pwd, role))
                                            Console.WriteLine("Utilisateur créé.");
                                        else
                                            Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "4":
                                        var cible = op.LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        Console.Write("Votre mot de passe (pour confirmer): "); var conf = op.LireMotDePasseAvecReset();
                                        Console.Write("Nouveau mot de passe pour la cible: "); var nouv = op.LireMotDePasseAvecReset();
                                        if (service.ReinitialiserMdp(user.email, conf, cible, nouv))
                                            Console.WriteLine("Mot de passe réinitialisé.");
                                        else
                                            Console.WriteLine("Échec de la réinitialisation (droits insuffisants ou cible introuvable).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "5":
                                        var cible2 = op.LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal
                                        Console.Write("Votre mot de passe (pour confirmer): "); var conf2 = op.LireMotDePasseAvecReset();
                                        if (service.DesactiverUtilisateur(user.email, conf2, cible2))
                                            Console.WriteLine("Si l'utilisateur existait, il a été désactivé.");
                                        else
                                            Console.WriteLine("Échec (droits insuffisants ou cible introuvable).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "6":
                                        {
                                            // Reconfirmer le mot de passe admin avant action
                                            UserView table = new UserView();
                                            table.AfficherUtilisateursTable();
                                            var adminPwd = op.LireMotDePasseAvecReset("Confirmez votre mot de passe admin pour continuer: ");
                                            if (adminPwd == null) break; // reset -> retour au menu principal

                                            // Lire l'email du guest à valider
                                            var cibleEmail = op.LireAvecReset("Email du guest à valider: ", out reset);
                                            if (reset || string.IsNullOrWhiteSpace(cibleEmail))
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
                                    case "8":
                                        session = false;
                                        break;
                                    case "7":
                                        {
                                            prView.MenuProduct();
                                            break;
                                        }
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
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients)
                                        clientView.MenuClient(); break;
                                        ;
                                    case "2":
                                        Console.Clear();
                                        usrView.AfficherUtilisateursTable();
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "3":
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
                                    case "4":
                                        prView.MenuProduct();
                                        break;
                                    case "5":
                                        session = false;
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

                    break;
                case "2":
                    {

                        bool reset;
                        var nom = op.LireAvecReset("Nom: ", out reset);
                        if (reset) break;

                        var mail = op.LireAvecReset("Email: ", out reset);
                        if (reset) break;

                        Console.Write("Mot de passe: "); var pwd = op.LireMotDePasseAvecReset();
                        if (service.CreerUtilisateur(nom, mail, pwd, "guest"))
                        {
                            Console.WriteLine("Compte créé avec le rôle 'guest'");
                            Console.WriteLine("Un administrateur doit valider votre compte avant que vous puissiez vous connecter.");
                        }
                        else
                            Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");

                        var action = op.AttendreEntreeOuReset();
                        if (action == Traitement.PauseAction.Reset)
                        {
                            break; // Retour au menu principal
                        }
                        break;
                    }
                case "3":
                    return;
                default:
                    Console.WriteLine("Option non valide");
                    Console.WriteLine("Appuyez sur Entrée pour continuer...");
                    Console.ReadLine();
                    break;
            }
        }
    }
}