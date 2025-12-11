using App1.Services;
using App1.View;
using System;
using System.Text.RegularExpressions;

namespace App1
{
    public class CommandParser
    {
        public UserServices service = new UserServices();
        public Traitement op = new Traitement();
        public ClientServices client = new ClientServices();
        public ClientView clientVue = new ClientView();
        public UserView userVue = new UserView();
        public void ConnectParse(string input)
        {
            string pattern = @"^dave\sconnect\s-u\s(?<email>\S+)\s-p\s(?<password>\S+)\s*$";

            Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                string email = match.Groups["email"].Value;
                string password = match.Groups["password"].Value;

                // Appel de ta fonction existante

                var user = service.Authentifier(email, password);
            }
            else
            {
                Console.WriteLine("Format invalide. Exemple : dave connect -u John -p 1234");
            }
        }

        public void CreateParse(string input)
        {
            string registerPattern = @"^dave\screate\s-u\s(?<username>.+?)\s-p\s(?<password>\S+)\s-m\s(?<email>\S+)$";

            var matchRegister = Regex.Match(input, registerPattern, RegexOptions.IgnoreCase);
            if (!matchRegister.Success)
            {
                Console.WriteLine("Commande invalide !");
                return;
            }
            string username = matchRegister.Groups["username"].Value;
            string password = matchRegister.Groups["password"].Value;
            string email = matchRegister.Groups["email"].Value;

            //var result = service.CreerUtilisateur(username, email, password);
            if (service.CreerUtilisateur(username, email, password, "guest"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Compte créé avec le rôle 'guest'");
                Console.WriteLine("Un administrateur doit valider votre compte avant que vous puissiez vous connecter.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
                Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");

            var action = op.AttendreEntreeOuReset();
            if (action == Traitement.PauseAction.Reset)
            {
                Environment.Exit(0); // Retour au menu principal
            }
        }

        public void HelpParse(string input)
        {
            string helpPattern = @"^dave\shelp$";

            var matchHelp = Regex.Match(input, helpPattern, RegexOptions.IgnoreCase);

            if (matchHelp.Success)
            {
                Console.WriteLine("Commandes disponibles :");
                Console.WriteLine("1. dave connect -u <email> -p <password> : Pour se connecter.");
                Console.WriteLine("2. dave create -u <username> -p <password> -m <email> : Pour s'inscrire.");
                Console.WriteLine("3. dave quit : Pour fermer l'application.");
                Console.WriteLine();
            }
        }

        public void ExitParse(string input)
        {
            string exitPattern = @"^dave\squit$";

            var matchExit = Regex.Match(input, exitPattern, RegexOptions.IgnoreCase);

            if (matchExit.Success)
            {
                Console.WriteLine("Fermeture de l'application. Au revoir !");
                Environment.Exit(0);
            }
        }

        public void AfficherClientsParse(string input)
        {
            string afficherPattern = @"^dave\sclient\slist$";
            var matchAfficher = Regex.Match(input, afficherPattern, RegexOptions.IgnoreCase);
            if (matchAfficher.Success)
            {
                clientVue.AfficherClients();
            }
        }

        public void CreateClientParse(string input)
        {
            string clientPattern = @"^dave\sclient\screate\s-n\s(?<clientname>.+?)\s-ph\s(?<phone>\d{9})?$";

            var matchClient = Regex.Match(input, clientPattern, RegexOptions.IgnoreCase);

            if (matchClient.Success)
            {
                string clientname = matchClient.Groups["clientname"].Value;
                int phone = Convert.ToInt32(matchClient.Groups["phone"].Value);
                client.AjouterClient(new Modeles.Client { nom = clientname, numeroTel = phone });
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Client ajouté.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.WriteLine("Commande invalide pour créer un client.");
                return;
            }
            var action = op.AttendreEntreeOuReset();
            if (action == Traitement.PauseAction.Reset)
            {
                Environment.Exit(0); // Retour au menu principal
            }
        }

        public void ModifyClientParse(string input)
        {
            string modifyPattern = @"^dave\sclient\smodify\s-id\s(?<id>\d+)\s-n\s(?<clientname>.+?)\s-ph\s(?<phone>\d{9})?$";
            var matchModify = Regex.Match(input, modifyPattern, RegexOptions.IgnoreCase);
            if (matchModify.Success)
            {
                int id = Convert.ToInt32(matchModify.Groups["id"].Value);
                string clientname = matchModify.Groups["clientname"].Value;
                int phone = Convert.ToInt32(matchModify.Groups["phone"].Value);
                var clientModifie = new Modeles.Client { nom = clientname, numeroTel = phone };
                bool success = client.ModifierClient(id, clientModifie);
                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Client modifié avec succès.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Erreur lors de la modification du client (ID peut-être inexistant).");
                }
            }
            else
            {
                Console.WriteLine("Commande invalide pour modifier un client.");
                return;
            }
        }

        public void DeleteClientParse(string input)
        {
            string deletePattern = @"^dave\sclient\sdelete\s-id\s(?<id>\d+)$";
            var matchDelete = Regex.Match(input, deletePattern, RegexOptions.IgnoreCase);
            if (matchDelete.Success)
            {
                int id = Convert.ToInt32(matchDelete.Groups["id"].Value);
                bool success = client.Supprimer(id);
                if (success)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Client supprimé avec succès.");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression du client (ID peut-être inexistant).");
                }
            }
            else
            {
                Console.WriteLine("Commande invalide pour supprimer un client.");
                return;
            }
        }

        public void ExitClient()
        {

        }

        public void AfficherUSerParse(string input)
        {
            string afficherPattern = @"^dave\suser\slist$";
            var matchAfficher = Regex.Match(input, afficherPattern, RegexOptions.IgnoreCase);
            if (matchAfficher.Success)
            {
                userVue.AfficherUtilisateursTable();
            }
        }

        public void CreateUserParse(string input)
        {
            string createPattern = @"^dave\suser\screate\s-u\s(?<username>.+?)\s-p\s(?<password>\S+)\s-m\s(?<email>\S+)\s-r\s(?<role>\S+)$";
            var matchCreate = Regex.Match(input, createPattern, RegexOptions.IgnoreCase);
            if (matchCreate.Success)
            {
                string username = matchCreate.Groups["username"].Value;
                string password = matchCreate.Groups["password"].Value;
                string email = matchCreate.Groups["email"].Value;
                string role = matchCreate.Groups["role"].Value;
                bool result = service.CreerUtilisateur(username, email, password, role);
                if (result)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Utilisateur créé avec le rôle '{role}'");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine("Erreur création (email déjà utilisé ou données invalides).");
                }
            }
            else
            {
                Console.WriteLine("Commande invalide pour créer un utilisateur.");
                return;
            }
        }
    }
}
