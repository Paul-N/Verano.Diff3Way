using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Count() < 3 || args.Where(a => a == "--help").Any())
                PrintHelp();
            else
            {
                if (!File.Exists(args[0]))
                    Console.WriteLine("MYFILE doesn't exist");
                else if(!File.Exists(args[1]))
                    Console.WriteLine("ORIGINAL file doesn't exist");
                else if (!File.Exists(args[2]))
                    Console.WriteLine("YOURFILE doesn't exist");
                else
                {
                    var merge = new Merge();
                    var merged = merge.Merge3Way(args[0], args[1], args[2]);

                    for (int i = 0; i < merged.Count() - 1; i++ )
                        Console.WriteLine(merged[i]);
                    Console.WriteLine(merged[merged.Count() - 1]);
                }
            }
            //Console.ReadLine();
        }

        private static void PrintHelp()
        {
            Console.Write(@"Usage: diff3.exe MYFILE ORIGINALFILE YOURFILE");
            Console.Write(@"--help - show this message");
        }
    }
}
