using App1.DAL;
using App1.Modeles;


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
            var donnees = DataStore.Lire();
            var users = (donnees == null || donnees.users == null) ? new List<Modeles.User>() : donnees.users;

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
