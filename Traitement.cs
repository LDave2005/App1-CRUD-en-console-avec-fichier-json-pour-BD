using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Modeles;

namespace App1
{
    public class Traitement
    {
        public Traitement()
        {
        }
        public static int CalculID(Donnees donnees)
        {
            int max = 0;
            foreach (var u in donnees.users)
            {
                if (u.id > max) max = u.id;
            }
            int nextId = max + 1;
            return nextId;
        }

        public static bool RechercherEmail(string? dEmail, string? cEmail)
        {
            if(!string.IsNullOrEmpty(dEmail) && dEmail.Equals(cEmail, StringComparison.OrdinalIgnoreCase)){
                return true;
            }
            return false;
        }

        public static User? RechercheUser(Donnees donnees, string cEmail)
        {
            User? cible = null;
            foreach (var u in donnees.users)
            {
                if (RechercherEmail(u.email,cEmail))
                {
                    cible = u;
                    break;
                }
            }
            return cible;
        }

        // Lecture avec détection Ctrl+R (retourne null si reset demandé)
        public string? LireAvecReset(string invite, out bool reset)
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
        public string? LireMotDePasseAvecReset(string invite = "Mot de passe: ")
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

    }
}
