using App1.Modeles;

namespace App1
{
    public class Traitement
    {
        public enum PauseAction { Continuer, Reset }
        public Traitement() { }

        public PauseAction AttendreEntreeOuReset(string message = "Appuyez sur Entrée pour continuer ou ctrl+R pour revenir au menu...")
        {
            Console.WriteLine(message);
            while (true)
            {
                var key = Console.ReadKey();
                if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.R) return PauseAction.Reset;
                if (key.Key == ConsoleKey.Enter) return PauseAction.Continuer;
                //Ignorer les autres touches
            }
        }

        public static int CalculIDUser(Donnees donnees)
        {
            int max = 0;
            foreach (var u in donnees.users)
            {
                if (u.id > max) max = u.id;
            }
            int nextId = max + 1;
            return nextId;
        }

        public static int CalculIDProduct(Donnees donnees)
        {
            int max = 0;
            foreach (var p in donnees.products)
            {
                if (p.id > max) max = p.id;
            }
            int nextId = max + 1;
            return nextId;
        }
        public static int CalculIDClient(Donnees donnees)
        {
            int max = 0;
            foreach (var u in donnees.clients)
            {
                if (u.id > max) max = u.id;
            }
            int nextId = max + 1;
            return nextId;
        }

        public static bool RechercherEmail(string? dEmail, string? cEmail)
        {
            if (!string.IsNullOrEmpty(dEmail) && dEmail.Equals(cEmail, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public static User? RechercheUser(Donnees donnees, string cEmail)
        {
            User? cible = null;
            foreach (var u in donnees.users)
            {
                if (RechercherEmail(u.email, cEmail))
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

        public void Afficher(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public List<string> Wrap(string text, int t)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text))
            {
                lines.Add("");
                return lines;
            }
            for(int i =0 ; i < text.Length; i += t)
            {
                int len = Math.Min(t, text.Length - i);
                lines.Add(text.Substring(i, len));
            }
            return lines;
        }

    }
}
