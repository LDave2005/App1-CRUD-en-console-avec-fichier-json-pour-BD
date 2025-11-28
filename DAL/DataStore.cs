using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Modeles;

namespace App1.DAL
{
    public class DataStore
    {
        private static string chemin = "E:\\Cours de Developpement C#\\CRUD\\Console\\App1\\data.json";

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

        public static void Ecrire(Donnees donnees)
        {
            var json = JsonConvert.SerializeObject(donnees, Formatting.Indented);
            File.WriteAllText(chemin, json);
        }
    }
}
