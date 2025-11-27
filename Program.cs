using App1;
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
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class jsonCrud
{
    

    private static string chemin = "E:\\Cours de Developpement C#\\CRUD\\Console\\App1\\data.json";

    //Lire les donnees
    public static Donnees Lire()
    {
        if (!File.Exists(chemin))
        {
            var initial = new Donnees();
            //Creer un compte admin par defaut 
            initial.users.Add(new User
            {
                id = 1,
                nom = "Administrateur",
                email = "admin@local",
                motDePasseHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                statut = "actif",
                role = "admin",
                createdAt = System.DateTime.UtcNow
            });
            //Ecrire le fichier initial et le retourner
            Ecrire(initial);
            return initial;
        }

        //Lire et deserialiser le fichier existant
        try
        {
            var json = File.ReadAllText(chemin);
            var donnees = JsonConvert.DeserializeObject<Donnees>(json);
            return donnees ?? new Donnees();
        }
        catch
        {
            //En cas d'erreur
            return new Donnees();
        }

    }

    //Ecrire les donnees
    public static void Ecrire(Donnees donnees)
    {
        var json = JsonConvert.SerializeObject(donnees, Formatting.Indented);
        File.WriteAllText(chemin, json);
    }

    //Creation d'un utilisateur dans data.json
    public static bool CreerUtilisateur(string nom, string email, string mdp, string role = "user")
    {
        if (string.IsNullOrWhiteSpace(nom) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(mdp))
            return false;

        //Validation de l'email
        try
        {
            var _ = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            return false;
        }

        var donnees = Lire();
        //Verifier unicite de l'email 
        foreach(var u in donnees.users)
        {
            if (!string.IsNullOrEmpty(u.email) && u.email.Equals(email, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        //Calculer l'id
        int nextId = 1;
        if (donnees.users.Count > 0)
        {
            int max = 0;
            foreach(var u in donnees.users)
            {
                if (u.id > max) max = u.id;
            }
            nextId = max + 1;
        }

        //Hacher le mot de passe 
        string hash = BCrypt.Net.BCrypt.HashPassword(mdp);
        var nouvel = new User
        {
            id = nextId,
            nom = nom.Trim(),
            email = email.Trim(),
            motDePasseHash = hash,
            statut = string.Equals(role?.Trim(),"guest",StringComparison.OrdinalIgnoreCase) ? "pending" : "actif",
            role = string.IsNullOrWhiteSpace(role) ? "user" : role.Trim(),
            createdAt = System.DateTime.UtcNow
        };

        donnees.users.Add(nouvel);
        Ecrire(donnees);
        return true;
    }

    //Authentifier : retourne l'utilisateur si OK (et actif)
    public static User? Authentifier (string email, string mdp)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(mdp))
            return null;

        var donnees = Lire();
        User? trouve = null;
        foreach(var u in donnees.users)
        {
            if (!string.IsNullOrEmpty(u.email) && u.email.Equals(email, StringComparison.OrdinalIgnoreCase))
            {
                trouve = u;
                break;
            }
        }
        if (trouve == null) return null;
        if (!string.Equals(trouve.statut, "actif", StringComparison.OrdinalIgnoreCase)) return null;

        if (string.Equals(trouve.role, "guest", StringComparison.OrdinalIgnoreCase)) return null;

        //Verification du mot de passe
        if (string.IsNullOrEmpty(trouve.motDePasseHash)) return null;
        bool ok = BCrypt.Net.BCrypt.Verify(mdp, trouve.motDePasseHash);
        return ok ? trouve : null;

    }



    public static bool ReinitialiserMdp(string dEmail, string dMdp, string cEmail, string nouveauMdp)
    {
        if (string.IsNullOrWhiteSpace(dEmail) || string.IsNullOrWhiteSpace(dMdp)
            || string.IsNullOrWhiteSpace(cEmail) || string.IsNullOrWhiteSpace(nouveauMdp))
            return false;

        //Authentifier la personne qui demande l'operation
        var demande = Authentifier(dEmail, dMdp);
        if (demande == null) return false;

        //Verifier le role admin
        if(!string.Equals(demande.email, cEmail, StringComparison.OrdinalIgnoreCase)
            && !string.Equals(demande.role, "admin", StringComparison.OrdinalIgnoreCase))
        {
            return false;//Pas les droits
        }

        var donnees = Lire();

        //Trouver l'utilisateur cibler
        User? cible = null;
        foreach(var u in donnees.users)
        {
            if (!string.IsNullOrEmpty(u.email) && u.email.Equals(cEmail, StringComparison.OrdinalIgnoreCase))
            {
                cible = u;
                break;
            }
        }
        if (cible == null) return false;

        //Hacher le nouveau mot de passe
        cible.motDePasseHash = BCrypt.Net.BCrypt.HashPassword(nouveauMdp);
        Ecrire(donnees);
        return true;
    }

    public static bool DesactiverUtilisateur(string dEmail, string dMdp, string cEmail)
    {
        if (string.IsNullOrWhiteSpace(dEmail) || string.IsNullOrWhiteSpace(dMdp) || string.IsNullOrWhiteSpace(cEmail))
            return false;

        // Authentifier la personne qui demande l'opération
        var demandeur = Authentifier(dEmail, dMdp);
        if (demandeur == null) return false;

        // Empêcher la désactivation de soi-même (sécurité basique)
        if (string.Equals(dEmail.Trim(), cEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            return false;

        //Verification du role admin
        if(!string.Equals(demandeur.role, "admin", StringComparison.OrdinalIgnoreCase))
            return false; // Pas les droits

        var donnees = Lire();

        User? cible = null;
        foreach (var u in donnees.users)
        {
            if (!string.IsNullOrEmpty(u.email) && string.Equals(u.email.Trim(), cEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                cible = u;
                break;
            }
        }
        if (cible == null) return false;

        // Marquer inactif et sauvegarder
        cible.statut = "inactif";
        Ecrire(donnees);
        return true;
    }

    public static List<Client> LireClients()
    {
        var donnees = Lire();
        //return donnees?.clients ?? new List<Client>();
        if (donnees == null) return new List<Client>();
        if (donnees.clients == null) return new List<Client>();
        return donnees.clients;
    }

    //Create
    public static void AjouterClient(Client client)
    {
        if (client == null) return;
        var donnees = Lire();
        if (donnees.clients == null) donnees.clients = new List<Client>();

        int max = 0;
        foreach(var c in donnees.clients)
        {
            if (c.id > max) max = c.id;
        }
        client.id = max + 1;
        donnees.clients.Add(client);
        Ecrire(donnees);
    }

    //Update
    public static bool ModifierClient(int id, Client clientModifie)
    {
        if (clientModifie == null) return false;
        var donnees = Lire();
        if (donnees.clients == null) return false;

        foreach(var c in donnees.clients)
        {
            if(c.id == id)
            {
                c.nom = clientModifie.nom;
                c.numeroTel = clientModifie.numeroTel;
                Ecrire(donnees);
                return true;
            }
        }
        return false;
    }

    //Supprimer un client

    //Delete
    public static bool Supprimer(int id)
    {
        var donnees = Lire();
        if (donnees.clients == null) return false;

        Client? cible = null;
        foreach (var c in donnees.clients)
        {
            if(c.id == id)
            {
                cible = c;
                break;
            }
        }
        if (cible == null) return false;

        donnees.clients.Remove(cible);
        Ecrire(donnees);
        return true;
    }

    //Affichage de clients
    public static void AfficherClients()
    {
        var donnees = Lire();
        var clients =(donnees == null || donnees.clients == null) ? new List<Client>() : donnees.clients;

        //Calcul des largeur de champs
        int idW = 2;
        int nomW = 3;
        int numeroTelW = 9;

        foreach (var c in clients)
        {
            if(c.id.ToString().Length > idW) idW = c.id.ToString().Length;
            if((c.nom ?? "").Length > nomW) nomW = (c.nom ?? "").Length;
            if(c.numeroTel.ToString().Length > numeroTelW) numeroTelW = c.numeroTel.ToString().Length;
        }

        string sep = "+" + new string('-',idW+2) + "+" + new string('-',nomW+2) + "+" + new string('-', numeroTelW+2) + "+";

        Console.WriteLine(sep);
        Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Telephone".PadRight(numeroTelW)} |");
        Console.WriteLine(sep);

        foreach (var c in clients)
        {
            string idS = c.id.ToString().PadRight(idW);
            string nom = (c.nom ?? "").PadRight(nomW);
            string nroTel = c.numeroTel.ToString().PadRight(numeroTelW);

            Console.WriteLine($"| {idS} | {nom} | {nroTel} |");
            Console.WriteLine(sep);
        }
        


    }

    //Affiche les utilisateurs avec formatage en tableau
    public static void AfficherUtilisateursTable()
    {
        var donnees = Lire();
        var users = (donnees == null || donnees.users == null) ? new List<User>() : donnees.users;

        // Calcul des largeurs de colonnes sans LINQ
        int idW = 2;
        int nomW = 3;
        int emailW = 5;
        int statutW = 6;
        int dateW = 19; // "yyyy-MM-dd HH:mm:ss"
        int roleW = 4;

        foreach (var u in users)
        {
            var idLen = u.id.ToString().Length;
            if (idLen > idW) idW = idLen;
            var nlen = (u.nom ?? "").Length;
            if (nlen > nomW) nomW = nlen;
            var elen = (u.email ?? "").Length;
            if (elen > emailW) emailW = elen;
            var slen = (u.statut ?? "").Length;
            if (slen > statutW) statutW = slen;
            var roleLen = (u.role ?? "").Length;
            if (roleLen > roleW) roleW = roleLen;
        }

        string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" +
                     new string('-', emailW + 2) + "+" + new string('-', statutW + 2) + "+" +
                     new string('-', dateW + 2) + "+" + new string('-', roleW + 2) + "+";

        Console.WriteLine(sep);
        Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Email".PadRight(emailW)} | {"Statut".PadRight(statutW)} | {"Créé le".PadRight(dateW)} | {"Role".PadRight(roleW)} |");
        Console.WriteLine(sep);

        foreach (var u in users)
        {
            string idS = u.id.ToString().PadRight(idW);
            string nom = (u.nom ?? "").PadRight(nomW);
            string email = (u.email ?? "").PadRight(emailW);
            string statut = (u.statut ?? "").PadRight(statutW);
            string date = (u.createdAt == default ? "" : u.createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")).PadRight(dateW);
            string role = (u.role ?? "").PadRight(roleW);

            Console.WriteLine($"| {idS} | {nom} | {email} | {statut} | {date} | {role} |");
        }

        Console.WriteLine(sep);
    }

    public static void ColorText()
    {
        Console.Write("Souhaitez vous reefectuer l'operation ou retourner au menu "); Console.ForegroundColor = ConsoleColor.White;
        Console.Write("("); Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("S:Sortir "); Console.ForegroundColor = ConsoleColor.White;
        Console.Write("/ "); Console.ForegroundColor = ConsoleColor.Green;
        Console.Write("Sinon:Continuer"); Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(")");
    }

    //Lecture masquee du mot de passe
    // Lecture avec détection Ctrl+R (retourne null si reset demandé)
    private static string? LireAvecReset(string invite, out bool reset)
    {
        Console.Write(invite);
        var sb = new System.Text.StringBuilder();
        reset = false;
        while (true)
        {
            var key = Console.ReadKey(true);

            // Ctrl+R -> reset
            if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.R)
            {
                Console.WriteLine();
                reset = true;
                return null;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return sb.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }

            // ignorer touches non caractères
            if (!char.IsControl(key.KeyChar))
            {
                sb.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    // Lecture masquée du mot de passe avec reset (retourne null si reset)
    private static string? LireMotDePasseAvecReset(string invite = "Mot de passe: ")
    {
        Console.Write(invite);
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);

            // Ctrl+R -> reset
            if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.R)
            {
                Console.WriteLine();
                return null;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return sb.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (sb.Length > 0)
                {
                    sb.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                sb.Append(key.KeyChar);
                Console.Write("*");
            }
        }
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
        u != null && string.Equals(u.role, "admin", StringComparison.OrdinalIgnoreCase);

    //Authentifie et retourne l'utilisateur
    private static User? AuthentificateRequester(string email, string mdp) =>
        Authentifier(email,mdp);

    //------Exigence d'authentification pour gestion clients------
    public static bool AjouterClientAuth(string requestEmail, string requestMdp, Client client)
    {
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return false;
        AjouterClient(client);
        return true;
    }

    public static bool ModifierClientAuth(string requestEmail, string requestMdp, int id, Client clientModifie)
    {
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return false;
        return ModifierClient(id, clientModifie);
    }

    //Necessite d'etre un admin pour supprimer un client
    public static bool SupprimerClientAuth(string requestEmail, string requestMdp, int id)
    {
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return false;
        if (!EstAdmin(req)) return false;
        return Supprimer(id);
    }

    //Necessite une authentification pour lire les clients
    public static List<Client> LireClientsAuth(string requestEmail, string requestMdp)
    {
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return new List<Client>();
        return LireClients();
    }

    //------Exigence d'administration pour gerer les utilisateurs------
    public static bool CreerUtilisateurAuth(string requestEmail, string requestMdp, string nom, string email, string mdp, string role = "user")
    {
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return false;
        if (!EstAdmin(req)) return false;
        return CreerUtilisateur(nom, email, mdp, role);
    }

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
        var req = AuthentificateRequester(requestEmail, requestMdp);
        if (req == null) return false;
        if (!EstAdmin(req)) return false;
        AfficherUtilisateursTable();
        return true;
    }

    public static bool AssignerRoleAdmin(string adminEmail, string adminMdp, string cibleEmail, string nouveauRole)
    {
        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminMdp)
            || string.IsNullOrWhiteSpace(cibleEmail) || string.IsNullOrWhiteSpace(nouveauRole))
            return false;

        var admin = Authentifier(adminEmail, adminMdp);
        if (admin == null || !EstAdmin(admin)) return false;

        var donnees = Lire();
        foreach (var u in donnees.users)
        {
            if (!string.IsNullOrEmpty(u.email) && u.email.Equals(cibleEmail, StringComparison.OrdinalIgnoreCase))
            {
                u.role = nouveauRole.Trim();
                // Optionnel : activer le compte lors de la validation
                u.statut = "actif";
                Ecrire(donnees);
                return true;
            }
        }
        return false;
    }

    public static void AfficherProduitsTable()
    {
        var produits = LireProduits();

        int idW = 2;
        int nomW = 3;
        int descW = 11;
        int prixW = 5;
        int stockW = 5;

        foreach (var p in produits)
        {
            if (p.id.ToString().Length > idW) idW = p.id.ToString().Length;
            if ((p.nom ?? "").Length > nomW) nomW = (p.nom ?? "").Length;
            if ((p.description ?? "").Length > descW) descW = (p.description ?? "").Length;
            if (p.prix.ToString("F2").Length > prixW) prixW = p.prix.ToString("F2").Length;
            if (p.stock.ToString().Length > stockW) stockW = p.stock.ToString().Length;
        }

        string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" +
                     new string('-', descW + 2) + "+" + new string('-', prixW + 2) + "+" +
                     new string('-', stockW + 2) + "+";

        Console.WriteLine(sep);
        Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Description".PadRight(descW)} | {"Prix".PadRight(prixW)} | {"Stock".PadRight(stockW)} |");
        Console.WriteLine(sep);

        foreach (var p in produits)
        {
            string idS = p.id.ToString().PadRight(idW);
            string nom = (p.nom ?? "").PadRight(nomW);
            string desc = (p.description ?? "").PadRight(descW);
            string prix = p.prix.ToString("F2").PadRight(prixW);
            string stock = p.stock.ToString().PadRight(stockW);

            Console.WriteLine($"| {idS} | {nom} | {desc} | {prix} | {stock} |");
        }
    }

    //---------------- Gestions des produits (Sans verification) ----------------

    public static List<Product> LireProduits()
    {
        var donnees = Lire();
        if (donnees == null || donnees.products == null) return new List<Product>();
        return donnees.products;
    }

    public static void AjouterProduit(Product p)
    {
        if(p == null) return;
        var donnees = Lire();
        if(donnees.products == null) donnees.products = new List<Product>();
        int max = 0;
        foreach(var pr in donnees.products)
        {
            if (pr.id > max) max = pr.id;
        }
        p.id = max + 1;
        donnees.products.Add(p);
        Ecrire(donnees);
    }

    public static bool ModifierProduit(int id, Product pModifie)
    {
        if (pModifie == null) return false;
        var donnees = Lire();
        if (donnees.products == null) return false;
        foreach(var pr in donnees.products)
        {
            if(pr.id == id)
            {
                pr.nom = pModifie.nom;
                pr.prix = pModifie.prix;
                Ecrire(donnees);
                return true;
            }
        }
        return false;
    }

    public static bool SupprimerProduit(int id)
    {
        var donnees = Lire();
        if (donnees.products == null) return false;
        Product? cible = null;
        foreach (var pr in donnees.products)
        {
            if(pr.id == id)
            {
                cible = pr;
                break;
            }
        }
        if (cible == null) return false;
        donnees.products.Remove(cible);
        Ecrire(donnees);
        return true;
    }

    //---------------- Gestions des produits (Avec verification) ----------------
    public static bool AjouterProduitAdmin(string requesterEmail, string requesterMdp, Product p)
    {
        var req = AuthentificateRequester(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        AjouterProduit(p);
        return true;
    }

    public static bool ModifierProduitAdmin(string requesterEmail, string requesterMdp, int id, Product p)
    {
        var req = AuthentificateRequester(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        return ModifierProduit(id, p);
    }

    public static bool SupprimerProduitAdmin(string requesterEmail, string requesterMdp, int id)
    {
        var req = AuthentificateRequester(requesterEmail, requesterMdp);
        if (req == null || !EstAdmin(req)) return false;
        return SupprimerProduit(id);
    }

    // Affichage produits réservé aux admins via wrapper
    public static bool AfficherProduitsTableAuth(string requesterEmail, string requesterMdp)
    {
        var req = AuthentificateRequester(requesterEmail, requesterMdp);
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
                        while (session)
                        {
                            Console.Clear();
                            Console.WriteLine($"Connecté: {user.nom} ({user.email}) - Statut: {user.statut} - Rôle: {user.role}");
                            Console.WriteLine("1) Gérer les clients");
                            Console.WriteLine("2) Afficher les utilisateurs");
                            // Seuls les admins voient l'option "Créer un utilisateur" centrale (les autres peuvent se créer eux-mêmes via écran principal)
                            if (string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("3) Créer un utilisateur (admin)");
                                Console.WriteLine("4) Réinitialiser mot de passe (admin peut changer tous les mots de passe)");
                                Console.WriteLine("5) Désactiver un utilisateur");
                                Console.WriteLine("6) Valider un invite");
                                Console.WriteLine("7) Gérer les produits (admin)");
                                Console.WriteLine("8) Déconnexion");
                            }
                            else
                            {
                                Console.WriteLine("3) Réinitialiser mon mot de passe");
                                Console.WriteLine("4) Déconnexion");
                            }

                            Console.Write("Choix: ");
                            var opt = Console.ReadLine();

                            // Gestion selon role
                            if (string.Equals(user.role, "admin", StringComparison.OrdinalIgnoreCase))
                            {
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients identique)
                                        goto case_clients;
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
                                            bool ok = AssignerRoleAdmin(user.email, adminPwd, cibleEmail, roleAttribue);
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
                                            bool inProducts = true;
                                            while (inProducts)
                                            {
                                                Console.Clear();
                                                Console.WriteLine("=== Gestion des produits (admin) ===");
                                                Console.WriteLine("1) Afficher tous les produits");
                                                Console.WriteLine("2) Ajouter un produit");
                                                Console.WriteLine("3) Modifier un produit");
                                                Console.WriteLine("4) Supprimer un produit");
                                                Console.WriteLine("5) Retour");
                                                Console.Write("Choix: ");
                                                var pc = Console.ReadLine();

                                                switch (pc)
                                                {
                                                    case "1":
                                                        AfficherProduitsTable();
                                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                                        Console.ReadLine();
                                                        break;
                                                    case "2":
                                                        var pnom = LireAvecReset("Nom du Produit: ", out reset);
                                                        if (reset) break; // retour au menu principal
                                                        var pdesc = LireAvecReset("Description du produit: ", out reset);
                                                        if (reset) break; // retour au menu principal
                                                        //Console.Write("Prix: ");
                                                        if (!decimal.TryParse(LireAvecReset("Prix: ", out reset), out decimal pprix)) pprix = 0m;
                                                        if (reset) break;
                                                        //Console.Write("Stock: ");
                                                        if (!int.TryParse(LireAvecReset("Stock: ", out reset), out int pstock)) pstock = 0;
                                                        if (reset) break;
                                                        // ici on suppose l'admin déjà authentifié : passer user.email/user.mot de passe si vous appelez wrapper
                                                        AjouterProduit(new Product { nom = pnom, description = pdesc, prix = pprix, stock = pstock });
                                                        Console.WriteLine("Produit ajouté.");
                                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                                        Console.ReadLine();
                                                        break;
                                                    case "3":
                                                        Console.Write("ID produit: ");
                                                        if (!int.TryParse(Console.ReadLine(), out int mid)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                                                        Console.Write("Nouveau nom: "); var mnom = Console.ReadLine() ?? "";
                                                        Console.Write("Nouvelle description: "); var mdesc = Console.ReadLine() ?? "";
                                                        Console.Write("Nouveau prix: ");
                                                        if (!decimal.TryParse(Console.ReadLine(), out decimal mprix)) mprix = 0m;
                                                        Console.Write("Nouveau stock: ");
                                                        if (!int.TryParse(Console.ReadLine(), out int mstock)) mstock = 0;
                                                        if (ModifierProduit(mid, new Product { nom = mnom, description = mdesc, prix = mprix, stock = mstock }))
                                                            Console.WriteLine("Produit modifié.");
                                                        else
                                                            Console.WriteLine("Produit non trouvé.");
                                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                                        Console.ReadLine();
                                                        break;
                                                    case "4":
                                                        Console.Write("ID produit à supprimer: ");
                                                        if (!int.TryParse(Console.ReadLine(), out int did)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                                                        if (SupprimerProduit(did))
                                                            Console.WriteLine("Produit supprimé.");
                                                        else
                                                            Console.WriteLine("Produit non trouvé.");
                                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                                        Console.ReadLine();
                                                        break;
                                                    case "5":
                                                        inProducts = false;
                                                        break;
                                                    default:
                                                        Console.WriteLine("Option non valide");
                                                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                                        Console.ReadLine();
                                                        break;
                                                }
                                            }
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
                                // utilisateur non-admin
                                switch (opt)
                                {
                                    case "1":
                                        // (sous-menu clients)
                                        goto case_clients;
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

                        // label pour réutiliser le même sous-menu clients dans les deux branches
                        case_clients:
                            {
                                bool inClients = true;
                                while (inClients)
                                {
                                    Console.Clear();
                                    Console.WriteLine("=== Gestion des clients ===");
                                    Console.WriteLine("1) Afficher tous les clients");
                                    Console.WriteLine("2) Ajouter un client");
                                    Console.WriteLine("3) Modifier un client");
                                    Console.WriteLine("4) Supprimer un client");
                                    Console.WriteLine("5) Retour");
                                    Console.Write("Choix: ");
                                    var c = Console.ReadLine();

                                    switch (c)
                                    {
                                        case "1":
                                            AfficherClients();
                                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                            Console.ReadLine();
                                            break;
                                        case "2":
                                            Console.Write("Nom: "); var nomc = Console.ReadLine() ?? "";
                                            Console.Write("Numéro de téléphone: ");
                                            if (!int.TryParse(Console.ReadLine(), out int tel)) tel = 0;
                                            AjouterClient(new Client { nom = nomc, numeroTel = tel });
                                            Console.WriteLine("Client ajouté.");
                                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                            Console.ReadLine();
                                            break;
                                        case "3":
                                            Console.Write("ID du client: ");
                                            if (!int.TryParse(Console.ReadLine(), out int id)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                                            Console.Write("Nouveau nom: "); var nNom = Console.ReadLine() ?? "";
                                            Console.Write("Nouveau téléphone: ");
                                            if (!int.TryParse(Console.ReadLine(), out int nTel)) nTel = 0;
                                            if (ModifierClient(id, new Client { nom = nNom, numeroTel = nTel }))
                                                Console.WriteLine("Client modifié.");
                                            else
                                                Console.WriteLine("Client non trouvé.");
                                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                            Console.ReadLine();
                                            break;
                                        case "4":
                                            Console.Write("ID du client à supprimer: ");
                                            if (!int.TryParse(Console.ReadLine(), out int idDel)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                                            if (Supprimer(idDel))
                                                Console.WriteLine("Client supprimé.");
                                            else
                                                Console.WriteLine("Client non trouvé.");
                                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                            Console.ReadLine();
                                            break;
                                        case "5":
                                            inClients = false;
                                            break;
                                        default:
                                            Console.WriteLine("Option non valide");
                                            Console.WriteLine("Appuyez sur Entrée pour continuer...");
                                            Console.ReadLine();
                                            break;
                                    }
                                }
                                break;
                            }
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



    /*public static void CreerUtilisateur(string nom, string email, string mdp)
    {
        string sql = "INSERT INTO utilisateurs (nom, email, mot_de_passe) VALUES (@nom, @email, @mdp)";

        var conn = DBConnection.GetConnection();
        var cmd = new MySqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@nom", nom);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.Parameters.AddWithValue("@mdp", mdp);

        conn.Open();
        cmd.ExecuteNonQuery();
        Console.WriteLine("Utilisateur Creee");
    }

    public static bool Authentifier(string email, string mdp)
    {
        string sql = "SELECT mot_de_passe FROM utilisateurs WHERE email = @email AND statut = 'actif'";

        using(var conn = DBConnection.GetConnection())
        {
            var cmd = new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@email", email);

            conn.Open();
            var resultat = cmd.ExecuteScalar();

            if (resultat != null && resultat != DBNull.Value)
            {
                string? motDePasseHache = resultat.ToString();
                // Utiliser BCrypt.Net.BCrypt.Verify pour comparer le mot de passe fourni avec le hash stocké
                return BCrypt.Net.BCrypt.Verify(mdp, motDePasseHache);
            }
            return false;
        }
    }

    public static void AfficherUtilisateur()
    {
        string sql = "SELECT id, nom, email, statut FROM utilisateurs";
        
        using(var conn = DBConnection.GetConnection())
        {
            var cmd = new MySqlCommand(sql, conn);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]} Nom: {reader["nom"]} Email: {reader["email"]}");
                    
                }
            }
        }
    }*/


}