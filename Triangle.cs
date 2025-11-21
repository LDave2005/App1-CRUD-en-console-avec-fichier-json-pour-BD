using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Triangle
    {
        public static void Main(string[] args)
        {   Console.ForegroundColor = ConsoleColor.Green; 
            for(int i = 0; i <= 300; i++)
            {
                var etoiles = new string('*', i);
                Console.WriteLine(etoiles);
                //var s = string.Concat(Enumerable.Repeat("*", i));
                //Console.WriteLine(s);
            }
            Console.WriteLine(new string(' ',50));
            int n = 300;
            for(int i = 1; i <= n; i++)
            {
                Console.Write(new string(' ',n - i));
                Console.WriteLine(new string('*', 2 * i - 1));
            }
        }
    }
}
