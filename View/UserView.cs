using App1.DAL;
using App1.Modeles;
using Org.BouncyCastle.Crypto;


namespace App1.View
{
    public class UserView
    {
        public DataStore DataStore { get; set; }
        
        public UserView()
        {
            DataStore = new DataStore();
        }

        public void AfficherUtilisateursTable()
        {
            Traitement op = new Traitement();
            var donnees = DataStore.Lire();
            var users = (donnees == null || donnees.users == null) ? new List<Modeles.User>() : donnees.users;

            // Calcul des largeurs de colonnes sans LINQ
            int idW = 2;
            int nomW = 3;
            int emailW = 5;
            int statutW = 6;
            int dateW = 19; // "yyyy-MM-dd HH:mm:ss"
            int roleW = 4;

            int maxIdWidth = 5;
            int maxNomWidth = 20;
            int maxEmailWidth = 30;
            int maxStatutWidth = 15;
            int maxRoleWidth = 10;

            foreach (var u in users)
            {
                idW = Math.Min(Math.Max(idW, u.id.ToString().Length), maxIdWidth);
                nomW = Math.Min(Math.Max(nomW, (u.nom ?? "").Length), maxNomWidth);
                emailW = Math.Min(Math.Max(emailW, (u.email ?? "").Length), maxEmailWidth);
                statutW = Math.Min(Math.Max(statutW, (u.statut ?? "").Length), maxStatutWidth);
                roleW = Math.Min(Math.Max(roleW, (u.role ?? "").Length), maxRoleWidth);
            }

            string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" +
                         new string('-', emailW + 2) + "+" + new string('-', statutW + 2) + "+" +
                         new string('-', dateW + 2) + "+" + new string('-', roleW + 2) + "+";

            Console.WriteLine(sep);
            Console.WriteLine($"| {"ID".PadRight(idW)} | {"Nom".PadRight(nomW)} | {"Email".PadRight(emailW)} | {"Statut".PadRight(statutW)} | {"Créé le".PadRight(dateW)} | {"Role".PadRight(roleW)} |");
            Console.WriteLine(sep);

            foreach (var u in users)
            {
                // On coupe chaque cellule selon la largeur de sa colonne
                var idLines = op.Wrap(u.id.ToString(), idW);
                var nomLines = op.Wrap(u.nom ?? "", nomW);
                var emailLines = op.Wrap(u.email ?? "", emailW);
                var statutLines = op.Wrap(u.statut ?? "", statutW);
                var dateLines = op.Wrap((u.createdAt == default ? "" : u.createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")), dateW);
                var roleLines = op.Wrap(u.role ?? "", roleW);

                // Lignes maximales pour cet enregistrement
                int max = new List<int> {
                    idLines.Count, nomLines.Count, emailLines.Count,
                    statutLines.Count, dateLines.Count, roleLines.Count
                }.Max();

                //Affichage des lignes
                for(int i = 0; i < max; i++)
                {
                    string id = i < idLines.Count ? idLines[i].PadRight(idW) : new string(' ', idW);
                    string nom = i < nomLines.Count ? nomLines[i].PadRight(nomW) : new string(' ', nomW);
                    string email = i < emailLines.Count ? emailLines[i].PadRight(emailW) : new string(' ', emailW);
                    string statut = i < statutLines.Count ? statutLines[i].PadRight(statutW) : new string(' ', statutW);
                    string date = i < dateLines.Count ? dateLines[i].PadRight(dateW) : new string(' ', dateW);
                    string role = i < roleLines.Count ? roleLines[i].PadRight(roleW) : new string(' ', roleW);

                    Console.WriteLine($"| {id} | {nom} | {email} | {statut} | {date} | {role} |");
                }
                Console.WriteLine(sep);
            }
        }

        public void MenuUser(User user)
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
                Console.WriteLine("4) Gérer les produits");
                Console.WriteLine("5) Déconnexion");
            }

            Console.Write("Choix: ");
        }

    }
}
