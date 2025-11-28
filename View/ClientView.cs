using App1.Services;
using App1.DAL;
using App1.Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1;

namespace App1.View
{
    public class ClientView
    {
        public DataStore DataStore { get; set; }

        public ClientView()
        {
            DataStore = new DataStore();
        }

        public void AfficherClients()
        {
            var donnees = DataStore.Lire();
            var clients = (donnees == null || donnees.clients == null) ? new List<Client>() : donnees.clients;

            //Calcul des largeur de champs
            int idW = 2;
            int nomW = 3;
            int numeroTelW = 9;

            foreach (var c in clients)
            {
                if (c.id.ToString().Length > idW) idW = c.id.ToString().Length;
                if ((c.nom ?? "").Length > nomW) nomW = (c.nom ?? "").Length;
                if (c.numeroTel.ToString().Length > numeroTelW) numeroTelW = c.numeroTel.ToString().Length;
            }

            string sep = "+" + new string('-', idW + 2) + "+" + new string('-', nomW + 2) + "+" + new string('-', numeroTelW + 2) + "+";

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

        
        public void MenuClient()
        {
            bool inClients = true;
            while (inClients)
            {
                ClientServices cli = new ClientServices();
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
                        cli.AjouterClient(new Client { nom = nomc, numeroTel = tel });
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
                        if (cli.ModifierClient(id, new Client { nom = nNom, numeroTel = nTel }))
                            Console.WriteLine("Client modifié.");
                        else
                            Console.WriteLine("Client non trouvé.");
                        Console.WriteLine("Appuyez sur Entrée pour continuer...");
                        Console.ReadLine();
                        break;
                    case "4":
                        Console.Write("ID du client à supprimer: ");
                        if (!int.TryParse(Console.ReadLine(), out int idDel)) { Console.WriteLine("ID invalide"); Console.ReadLine(); break; }
                        if (cli.Supprimer(idDel))
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
        }

    }
}
