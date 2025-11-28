using App1;
using App1.DAL;
using App1.Modeles;
using App1.Services;
using App1.View;
using BCrypt.Net;
using MySql.Data.MySqlClient;
using Mysqlx.Resultset;
using Mysqlx.Session;
using Newtonsoft;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class jsonCrud
{
   
    
    //Authentifier : retourne l'utilisateur si OK (et actif)
    public static User? Authentifier (string email, string mdp)
    {
        UserServices services = new UserServices();
        return services.Authentifier(email,mdp);

    }

    public static bool CreerUtilisateur(string? nom, string? email, string? mdp, string? role = "user")
    {
        UserServices services = new UserServices();
        return services.CreerUtilisateur(nom, email, mdp, role);
    }


    public static bool ReinitialiserMdp(string? dEmail, string? dMdp, string? cEmail, string? nouveauMdp)
    {
        UserServices services = new UserServices();
        return services.ReinitialiserMdp(dEmail, dMdp, cEmail, nouveauMdp);
       
    }


    public static bool DesactiverUtilisateur(string? dEmail, string? dMdp, string? cEmail)
    {
        UserServices services = new UserServices();
        return DesactiverUtilisateur(dEmail, dMdp,cEmail);
    }

    public static List<Client> LireClients()
    {
        ClientServices client = new ClientServices();
        return client.LireClients();
    }

    //Create
    public static void AjouterClient(Client client)
    {
        ClientServices cli = new ClientServices();
        cli.AjouterClient(client);
    }

    //Update
    public static bool ModifierClient(int id, Client clientModifie)
    {
        ClientServices client = new ClientServices();
        return client.ModifierClient(id, clientModifie);
    }

    //Supprimer un client

    //Delete
    public static bool Supprimer(int id)
    {
        ClientServices client = new ClientServices();
        return client.Supprimer(id);
    }

    //Affichage de clients
    public static void AfficherClients()
    {
        ClientView view = new ClientView();
        view.AfficherClients();
    }

    //Affiche les utilisateurs avec formatage en tableau
    public static void AfficherUtilisateursTable()
    {
        UserView view = new UserView();
        view.AfficherUtilisateursTable();
    }

    /*public static void ColorText()
    {
        Console.Write("Souhaitez vous reefectuer l'operation ou retourner au menu "); Console.ForegroundColor = ConsoleColor.White;
        Console.Write("("); Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("S:Sortir "); Console.ForegroundColor = ConsoleColor.White;
        Console.Write("/ "); Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Sinon:Continuer"); Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(")");
    }*/

    //Lecture masquee du mot de passe
    // Lecture avec détection Ctrl+R (retourne null si reset demandé)
    public static string? LireAvecReset(string invite, out bool reset)
    {
        Traitement op = new Traitement();
        return op.LireAvecReset(invite, out reset);
    }

    // Lecture masquée du mot de passe avec reset (retourne null si reset)
    public static string? LireMotDePasseAvecReset(string invite = "Mot de passe: ")
    {
        Traitement op = new Traitement();
        return op.LireMotDePasseAvecReset(invite);
    }

    private enum PauseAction { Continuer, Reset}

    //Retourne PauseAction.Reset si Ctrl+R, sinon continuer
    private static PauseAction AttendreEntreeOuReset(string message = "Appuyez sur Entrée pour continuer ou ctrl+R pour revenir au menu...")
    {
        Console.WriteLine(message);
        while (true)
        {
            var key = Console.ReadKey(true);
            if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.R)
                return PauseAction.Reset;
            if (key.Key == ConsoleKey.Enter)
                return PauseAction.Continuer;
            // Ignorer les autres touches
        }
    }

    //---------------- Gestions des verifications ----------------

    //Verifie si l'utilisateur est admin
    private static bool EstAdmin(User? u) =>
        EstAdmin(u); 

    //Authentifie et retourne l'utilisateur
    /*private static User? AuthentificateRequester(string email, string mdp) =>
        Authentifier(email,mdp);*/

    //------Exigence d'authentification pour gestion clients------
    /*public static bool AjouterClientAuth(string requestEmail, string requestMdp, Client client)
    {
        var req = Authentifier(requestEmail, requestMdp);
        if (req == null) return false;
        AjouterClient(client);
        return true;
    }

    public static bool ModifierClientAuth(string requestEmail, string requestMdp, int id, Client clientModifie)
    {
        var req = Authentifier(requestEmail, requestMdp);
        if (req == null) return false;
        return ModifierClient(id, clientModifie);
    }

    //Necessite d'etre un admin pour supprimer un client
    public static bool SupprimerClientAuth(string requestEmail, string requestMdp, int id)
    {
        var req = Authentifier(requestEmail, requestMdp);
        if (req == null) return false;
        if (!EstAdmin(req)) return false;
        return Supprimer(id);
    }

    //Necessite une authentification pour lire les clients
    public static List<Client> LireClientsAuth(string requestEmail, string requestMdp)
    {
        var req = Authentifier(requestEmail, requestMdp);
        if (req == null) return new List<Client>();
        return LireClients();
    }

    //------Exigence d'administration pour gerer les utilisateurs------
    

    public static bool DesactiverUtilisateurAuth(string requestEmail, string requestMdp, string cibleEmail)
    {
        //Verifiais deja la condition
        return DesactiverUtilisateur(requestEmail, requestMdp, cibleEmail);
    }

    public static bool reinitialiserMdpAuth(string requestEmail, string requestMdp, string cibleEmail, string nouveauMdp)
    {
        //Verifiais deja la condition
        return ReinitialiserMdp(requestEmail, requestMdp, cibleEmail, nouveauMdp);
    }

    public static bool AfficherUtilisateursTabbleAuth(string requestEmail, string requestMdp)
    {
        var req = Authentifier(requestEmail, requestMdp);
        if (req == null) return false;
        if (!EstAdmin(req)) return false;
        AfficherUtilisateursTable();
        return true;
    }*/

    public static bool AssignerRoleAdmin(string adminEmail, string adminMdp, string cibleEmail, string nouveauRole)
    {
        UserServices user = new UserServices();
        return user.AssignerRoleAdmin(adminEmail, adminMdp, cibleEmail, nouveauRole);
    }

    public static void AfficherProduitsTable()
    {
        ProductView product = new ProductView();
        product.AfficherProduitsTable();
    }

    //---------------- Gestions des produits (Sans verification) ----------------

    public static List<Product> LireProduits()
    {
        ProductServices product = new ProductServices();
        return product.LireProduits();
    }

    public static void AjouterProduit(Product p)
    {
        ProductServices product = new ProductServices();
        product.AjouterProduit(p);
    }

    public static bool ModifierProduit(int id, Product pModifie)
    {
        ProductServices product = new ProductServices();
        return product.ModifierProduit(id, pModifie);
    }

    public static bool SupprimerProduit(int id)
    {
        ProductServices product = new ProductServices();
        return product.SupprimerProduit(id);
    }

    //---------------- Gestions des produits (Avec verification) ----------------
    public static bool AjouterProduitAdmin(string requesterEmail, string requesterMdp, Product p)
    {
        var req = Authentifier(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        AjouterProduit(p);
        return true;
    }

    public static bool ModifierProduitAdmin(string requesterEmail, string requesterMdp, int id, Product p)
    {
        var req = Authentifier(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        return ModifierProduit(id, p);
    }

    public static bool SupprimerProduitAdmin(string requesterEmail, string requesterMdp, int id)
    {
        var req = Authentifier(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        return SupprimerProduit(id);
    }

    // Affichage produits réservé aux admins via wrapper
    public static bool AfficherProduitsTableAuth(string requesterEmail, string requesterMdp)
    {
        var req = Authentifier(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        AfficherProduitsTable(); // si vous avez déjà une méthode d'affichage
        return true;
    }


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

            switch (choix)
            {
                case "1":
                    {
                        bool reset;
                        var email = LireAvecReset("Email: ", out reset);
                        if (reset) break; // retour au menu principal

                        var mdp = LireMotDePasseAvecReset("Mot de passe: ");
                        if (mdp == null) break; // reset -> retour au menu principal

                        var user = Authentifier(email ?? "", mdp);
                        if (user == null)
                        {
                            Console.WriteLine("Échec d'authentification (ou compte inactif ou en attente de validation).");
                            var action = AttendreEntreeOuReset("Appuyez sur Entrée pour réessayer ou Ctrl+R pour revenir au menu principal...");
                            if (action == PauseAction.Reset)
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
                            var opt = Console.ReadLine();

                            // Gestion selon role
                            if (string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                ClientView cli = new ClientView();
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients identique)
                                        cli.MenuClient(); break;
                                    case "2":
                                        Console.Clear();
                                        AfficherUtilisateursTable();
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "3":
                                        var nom = LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        var mail = LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        Console.Write("Mot de passe: "); var pwd = LireMotDePasseAvecReset();
                                        Console.Write("Rôle (admin/user): "); var role = Console.ReadLine() ?? "user";
                                        if (CreerUtilisateur(nom, mail, pwd, role))
                                            Console.WriteLine("Utilisateur créé.");
                                        else
                                            Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "4":
                                        var cible = LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal

                                        Console.Write("Votre mot de passe (pour confirmer): "); var conf = LireMotDePasseAvecReset();
                                        Console.Write("Nouveau mot de passe pour la cible: "); var nouv = LireMotDePasseAvecReset();
                                        if (ReinitialiserMdp(user.email, conf, cible, nouv))
                                            Console.WriteLine("Mot de passe réinitialisé.");
                                        else
                                            Console.WriteLine("Échec de la réinitialisation (droits insuffisants ou cible introuvable).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "5":
                                        var cible2 = LireAvecReset("Email: ", out reset);
                                        if (reset) break; // retour au menu principal
                                        Console.Write("Votre mot de passe (pour confirmer): "); var conf2 = LireMotDePasseAvecReset();
                                        if (DesactiverUtilisateur(user.email, conf2, cible2))
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
                                            var adminPwd = LireMotDePasseAvecReset("Confirmez votre mot de passe admin pour continuer: ");
                                            if (adminPwd == null) break; // reset -> retour au menu principal

                                            // Lire l'email du guest à valider
                                            var cibleEmail = LireAvecReset("Email du guest à valider: ", out reset);
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
                                            bool ok = AssignerRoleAdmin(user.email ?? "", adminPwd, cibleEmail, roleAttribue);
                                            if (ok)
                                                Console.WriteLine($"Le rôle '{roleAttribue}' a été attribué à {cibleEmail}.");
                                            else
                                                Console.WriteLine("Échec : vérifiez vos droits, mot de passe ou email cible.");

                                            var pause = AttendreEntreeOuReset();
                                            if (pause == PauseAction.Reset) { session = false; } // ou ajustez pour revenir au menu principal
                                            break;
                                        }
                                    case "8":
                                        session = false;
                                        break;
                                    case "7":
                                        {
                                            ProductView productView = new ProductView();
                                            productView.MenuProduct();
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
                                ClientView cli = new ClientView();
                                // utilisateur non-admin
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients)
                                        cli.MenuClient(); break;
                                        ;
                                    case "2":
                                        Console.Clear();
                                        AfficherUtilisateursTable();
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "3":
                                        // Réinitialiser son propre mot de passe
                                        Console.Write("Ancien mot de passe: "); var ancien = LireMotDePasseAvecReset();
                                        Console.Write("Nouveau mot de passe: "); var nouv = LireMotDePasseAvecReset();
                                        if (ReinitialiserMdp(user.email, ancien, user.email, nouv))
                                            Console.WriteLine("Mot de passe changé.");
                                        else
                                            Console.WriteLine("Échec (ancien mot de passe invalide).");
                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                        Console.ReadLine();
                                        break;
                                    case "4":
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
                        var nom = LireAvecReset("Nom: ", out reset);
                        if (reset) break;

                        var mail = LireAvecReset("Email: ", out reset);
                        if (reset) break;

                        Console.Write("Mot de passe: "); var pwd = LireMotDePasseAvecReset();
                    if (CreerUtilisateur(nom, mail, pwd, "guest"))
                    {
                         Console.WriteLine("Compte créé avec le rôle 'guest'"); 
                         Console.WriteLine("Un administrateur doit valider votre compte avant que vous puissiez vous connecter.");
                    }
                    else
                        Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");
                    
                    var action = AttendreEntreeOuReset();
                    if (action == PauseAction.Reset)
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