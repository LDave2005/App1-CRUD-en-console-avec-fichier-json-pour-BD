using App1.DAL;
using App1.Modeles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public class ClientServices
    {
        public DataStore DataStore { get; set; }
        public ClientServices()
        {
            DataStore = new DataStore(); 
        }
        public List<Client> LireClients()
        {
            var donnees = DataStore.Lire();
            //return donnees?.clients ?? new List<Client>();
            if (donnees == null) return new List<Client>();
            if (donnees.clients == null) return new List<Client>();
            return donnees.clients;
        }

        public void AjouterClient(Client client)
        {
            if (client == null) return;
            var donnees = DataStore.Lire();
            if (donnees.clients == null) donnees.clients = new List<Client>();

            /*int max = 0;
            foreach(var c in donnees.clients)
            {
                if (c.id > max) max = c.id;
            }
            client.id = max + 1;*/
            client.id = Traitement.CalculID(donnees);
            donnees.clients.Add(client);
            DataStore.Ecrire(donnees);
        }

        public bool ModifierClient(int id, Client clientModifie)
        {
            if (clientModifie == null) return false;
            var donnees = DataStore.Lire();
            if (donnees.clients == null) return false;

            foreach (var c in donnees.clients)
            {
                if (c.id == id)
                {
                    c.nom = clientModifie.nom;
                    c.numeroTel = clientModifie.numeroTel;
                    DataStore.Ecrire(donnees);
                    return true;
                }
            }
            return false;
        }

        public bool Supprimer(int id)
        {
            var donnees = DataStore.Lire();
            if (donnees.clients == null) return false;

            Client? cible = null;
            foreach (var c in donnees.clients)
            {
                if (c.id == id)
                {
                    cible = c;
                    break;
                }
            }
            if (cible == null) return false;

            donnees.clients.Remove(cible);
            DataStore.Ecrire(donnees);
            return true;
        }

    }
}
