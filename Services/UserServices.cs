using App1.DAL;
using App1.Modeles;


namespace App1.Services
{
    public class UserServices
    {
        public DataStore DataStore { get; set; }

        public UserServices( )
        {
                DataStore = new DataStore();
        }

        public User? Authentifier(string email, string mdp)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(mdp))
                return null;

            var donnees = DataStore.Lire();
            User? trouve = Traitement.RechercheUser(donnees, email);
            /*foreach(var u in donnees.users)
            {
                if (!string.IsNullOrEmpty(u.email) && u.email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    trouve = u;
                    break;
                }
            }*/
            if (trouve == null) return null;
            if (!string.Equals(trouve.statut, "actif", StringComparison.OrdinalIgnoreCase)) return null;

            if (string.Equals(trouve.role, "guest", StringComparison.OrdinalIgnoreCase)) return null;

            //Verification du mot de passe
            if (string.IsNullOrEmpty(trouve.motDePasseHash)) return null;
            bool ok = BCrypt.Net.BCrypt.Verify(mdp, trouve.motDePasseHash);
            return ok ? trouve : null;

        }

        public bool EstAdmin(User? u) =>
        u != null && string.Equals(u.role, "admin", StringComparison.OrdinalIgnoreCase);

        public  bool ReinitialiserMdp(string? dEmail, string? dMdp, string? cEmail, string? nouveauMdp)
        {
            if (string.IsNullOrWhiteSpace(dEmail) || string.IsNullOrWhiteSpace(dMdp)
                || string.IsNullOrWhiteSpace(cEmail) || string.IsNullOrWhiteSpace(nouveauMdp))
                return false;

            //Authentifier la personne qui demande l'operation
            var demande = Authentifier(dEmail, dMdp);
            if (demande == null) return false;

            //Verifier le role admin
            if (!string.Equals(demande.email, cEmail, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(demande.role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return false;//Pas les droits
            }

            var donnees = DataStore.Lire();

            //Trouver l'utilisateur cibler
            User? cible = Traitement.RechercheUser(donnees, cEmail);
            /*foreach(var u in donnees.users)
            {
                if (!string.IsNullOrEmpty(u.email) && u.email.Equals(cEmail, StringComparison.OrdinalIgnoreCase))
                {
                    cible = u;
                    break;
                }
            }*/
            if (cible == null) return false;

            //Hacher le nouveau mot de passe
            cible.motDePasseHash = BCrypt.Net.BCrypt.HashPassword(nouveauMdp);
            DataStore.Ecrire(donnees);
            return true;
        }

        //Creation d'un utilisateur dans data.json
        public bool CreerUtilisateur(string? nom, string? email, string? mdp, string? role = "user")
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

            var donnees = DataStore.Lire();
            //Verifier unicite de l'email 
            foreach (var u in donnees.users)
            {
                if (Traitement.RechercherEmail(u.email, email)) //Si l'email existe deja
                    return false;
            }

            //Calculer l'id
            int nextId = 1;
            if (donnees.users.Count > 0)
            {
                /*int max = 0;
                foreach(var u in donnees.users)
                {
                    if (u.id > max) max = u.id;
                }
                nextId = max + 1;*/
                nextId = Traitement.CalculIDUser(donnees);
            }

            //Hacher le mot de passe 
            string hash = BCrypt.Net.BCrypt.HashPassword(mdp);
            var nouvel = new User
            {
                id = nextId,
                nom = nom.Trim(),
                email = email.Trim(),
                motDePasseHash = hash,
                statut = string.Equals(role?.Trim(), "guest", StringComparison.OrdinalIgnoreCase) ? "pending" : "actif",
                role = string.IsNullOrWhiteSpace(role) ? "user" : role.Trim(),
                createdAt = System.DateTime.UtcNow
            };

            donnees.users.Add(nouvel);
            DataStore.Ecrire(donnees);
            return true;
        }

        //Creation d'un utilsateur (En prenant la restriction de l'admin)
        public bool CreerUtilisateurAuth(string requestEmail, string requestMdp, string? nom, string? email, string? mdp, string? role = "user")
        {
            UserServices service = new UserServices();
            var req = Authentifier(requestEmail, requestMdp);
            if (req == null) return false;
            if (!EstAdmin(req)) return false;
            return service.CreerUtilisateur(nom, email, mdp, role);
        }

        public bool DesactiverUtilisateur(string? dEmail, string? dMdp, string? cEmail)
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
            if (!string.Equals(demandeur.role, "admin", StringComparison.OrdinalIgnoreCase))
                return false; // Pas les droits

            var donnees = DataStore.Lire();

            User? cible = Traitement.RechercheUser(donnees, cEmail);
            /*foreach (var u in donnees.users)
            {
                if (!string.IsNullOrEmpty(u.email) && string.Equals(u.email.Trim(), cEmail.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    cible = u;
                    break;
                }
            }*/
            if (cible == null) return false;

            // Marquer inactif et sauvegarder
            cible.statut = "inactif";
            DataStore.Ecrire(donnees);
            return true;
        }

        public bool AssignerRoleAdmin(string? adminEmail, string? adminMdp, string? cibleEmail, string? nouveauRole)
        {
            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminMdp)
                || string.IsNullOrWhiteSpace(cibleEmail) || string.IsNullOrWhiteSpace(nouveauRole))
                return false;

            var admin = Authentifier(adminEmail, adminMdp);
            if (admin == null || !EstAdmin(admin)) return false;

            var donnees = DataStore.Lire();
            foreach (var u in donnees.users)
            {
                if (Traitement.RechercherEmail(u.email, cibleEmail))
                {
                    u.role = nouveauRole.Trim();
                    // Optionnel : activer le compte lors de la validation
                    u.statut = "actif";
                    DataStore.Ecrire(donnees);
                    return true;
                }
            }
            return false;
        }
    }
}
