using App1;
using Newtonsoft;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

public class jsonCrud
{
    private static string chemin = "E:\\Cours de Developpement C#\\CRUD\\Console\\App1\\client.json";

    //Lire les donnees
    public static List<Client> Lire()
    {
        if (!File.Exists(chemin)) return new List<Client>();
        var json = File.ReadAllText(chemin);
        return JsonConvert.DeserializeObject<List<Client>>(json);
    }

    //Ecrire les donnees
    public static void Ecrire(List<Client> client)
    {
        var json = JsonConvert.SerializeObject(client, Formatting.Indented);
        File.WriteAllText(chemin, json);
    }

    //Create
    public static void Ajouter(Client client)
    {
        var clients = Lire();
        if (clients.Count > 0)
        {
            client.id = clients.Max(c => c.id) + 1;
        }
        else
        {
            client.id = 1;
        }
        clients.Add(client);
        Ecrire(clients);
    }

    //Update
    public static void Modifier(int id, Client clientModifie)
    {
        var clients = Lire();
        var client = clients.Find(c => c.id == id);
        if (client != null)
        {
            client.nom = clientModifie.nom;
            client.numeroTel = clientModifie.numeroTel;
            Ecrire(clients);
        }
    }

    //Delete
    public static void Supprimer(int id)
    {
        var clients = Lire();
        var client = clients.Find(client => client.id == id);
        if(client != null) clients.Remove(client); Ecrire(clients);
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

    public static void Main(string[] args)
    {
        do
        {
            Console.WriteLine("----------------Gestion des Clients----------------");
            Console.WriteLine("1.Afficher tous les clients\n" +
                "2.Ajouter un client\n" +
                "3.Modifier un client\n" +
                "4.Supprimer un client\n" +
                "5.Quitter");
            int choix = Convert.ToInt32(Console.ReadLine());
            string? sortie = "";
            switch(choix)
            {
                case 1:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.Clear();
                    do
                    {
                        var clients = jsonCrud.Lire();
                        foreach (var c in clients)
                        {
                            Console.WriteLine($"ID : {c.id}     Nom : {c.nom}     Numero de telephone : {c.numeroTel}");
                        }
                        //Console.WriteLine("Souhaitez vous reefectuer l'operation ou retourner au menu (S:sortir /sinon:continuer)");
                        ColorText();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        sortie = Console.ReadLine();
                    } while (sortie != "S");
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Clear();
                    do
                    {
                        Console.Write("Entrez le nom du client : "); Console.ForegroundColor = ConsoleColor.Yellow;
                        string nomC = Console.ReadLine(); Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("Entre le numero de telephone du client : "); Console.ForegroundColor = ConsoleColor.Yellow;
                        int nroC = Convert.ToInt32(Console.ReadLine()); Console.ForegroundColor = ConsoleColor.DarkGray;
                        jsonCrud.Ajouter(new Client { nom = nomC, numeroTel = nroC });
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client ajoutes");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        //Console.WriteLine("Souhaitez vous reefectuer l'operation ou retourner au menu (S:sortir /sinon:continuer)");
                        ColorText();
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        sortie = Console.ReadLine();
                    } while(sortie != "S");
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.Clear();
                    do
                    {
                        Console.WriteLine("Entrez l'identifiant du client que vous souhaitez modifier");
                        Console.ForegroundColor = ConsoleColor.White;
                        int idC = Convert.ToInt32(Console.ReadLine());
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Entrer le nouveau nom : "); Console.ForegroundColor = ConsoleColor.White;
                        string nomM = Console.ReadLine(); Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Entrer le nouveau numero de telephone : "); Console.ForegroundColor = ConsoleColor.White;
                        int nroM = Convert.ToInt32(Console.ReadLine()); Console.ForegroundColor = ConsoleColor.DarkYellow;
                        jsonCrud.Modifier(idC, new Client { nom = nomM, numeroTel = nroM });
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Client modifie");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        //Console.WriteLine("Souhaitez vous reefectuer l'operation ou retourner au menu (S:sortir /sinon:continuer)");
                        ColorText();
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        sortie = Console.ReadLine();
                    } while(sortie != "S");
                    break;
                case 4:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    do
                    {
                        Console.WriteLine("Entrez l'identifiant du client a supprimer");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        int idS = Convert.ToInt32(Console.ReadLine());
                        Console.ForegroundColor = ConsoleColor.Green;
                        jsonCrud.Supprimer(idS);
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("Client supprime");
                        Console.ForegroundColor = ConsoleColor.Green;
                        //Console.WriteLine("Souhaitez vous reefectuer l'operation ou retourner au menu (S:sortir /sinon:continuer)");
                        ColorText(); Console.ForegroundColor = ConsoleColor.Green;
                        sortie = Console.ReadLine();
                    } while(sortie != "S");
                    break;
                case 5:
                    return;
                default:
                    Console.WriteLine("Option non valide");
                    break;
            }
        } while (true);
    }

}