using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way.Tests.RI_Compare
{
    /// <summary>
    /// Test our Merge (it's SUT - System Under Test) class comparing to classic diff3.exe util from KDiff package from KDE (it will be the RI - reference implementation).
    /// Test cases source: http://www.guiffy.com/SureMergeWP.html
    /// </summary>
    class Program
    {
        private static string _ritxt = ".ri.txt";
        private static string _suttxt = ".sut.txt";
        
        /// <summary>
        /// Console app that run KDiff3's diff3.exe and our diff3.exe over small set of predefined samples.
        /// Note: path to RI diff3.exe must be set in App.config
        /// </summary>
        /// <param name="args">1st arg - directory with samples (SureMergeWP), 2nd - path to diff3.exe to test</param>
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2 || !Directory.Exists(args[0]) || !File.Exists(args[1]))
                throw new Exception("Smth wrong with args");

            var diff3Path = /*Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + */ConfigurationManager.AppSettings["Diff3_RI_path"];
            
            if(!File.Exists(diff3Path))
                throw new Exception("Dir to diff3 RI doesn't exists");

            var diff3_SUT = args[1];

            Console.WriteLine("Starting testing...");

            foreach(var dir in Directory.GetDirectories(args[0]))
            {
                var files = Directory.GetFiles(dir);
                if (files != null && files.Any())
                {
                    var parents = files.Where(f => f.EndsWith(".parent"));

                    if (parents.Any())
                    {

                        foreach (var parent in parents)
                        {
                            var fn = parent.Substring(0, parent.Length - ".parent".Length);
                            var fst = fn + ".1st";
                            var snd = fn + ".2nd";
                            if (File.Exists(fst) || File.Exists(snd))
                                TestSeqs(RunDiff3(diff3Path, diff3_SUT, parent, fst, snd), fn);
                        }
                    }
                    else if (files.Any(f => f.EndsWith(".html")))
                    {
                        var parent = files.First(f => f.EndsWith("ReleaseNotes_9_4.html"));
                        var file_a = files.First(f => f.EndsWith("ReleaseNotes_9_4_1.html"));
                        var file_b = files.First(f => f.EndsWith("ReleaseNotes_9_4_2.html"));
                        if (File.Exists(parent) && File.Exists(file_a) && File.Exists(file_b))
                            TestSeqs(RunDiff3(diff3Path, diff3_SUT, parent, file_a, file_b), "ReleaseNotes_9_4.html");
                    }
                    else if (files.Any(f => f.EndsWith(".h")))
                    {
                        var parent = files.First(f => f.EndsWith("Bug_ReporterApp_Parent.h"));
                        var file_a = files.First(f => f.EndsWith("Bug_ReporterApp_BranchA.h"));
                        var file_b = files.First(f => f.EndsWith("Bug_ReporterApp_BranchB.h"));
                        if (File.Exists(parent) && File.Exists(file_a) && File.Exists(file_b))
                            TestSeqs(RunDiff3(diff3Path, diff3_SUT, parent, file_a, file_b), "Bug_ReporterApp_Parent.h");
                    }
                }
            }

            Console.WriteLine("Finishing testing, press any key...");

            Console.ReadKey();
        }

        public static void TestSeqs(Tuple<IEnumerable<string>, IEnumerable<string>> seqs, string name)
        {
            Console.WriteLine("Entering test for " + name);

            if (seqs == null || seqs.Item1 == null || seqs.Item2 == null)
                Console.WriteLine(name + " no result");

            var diff3count = seqs.Item1.Count();
            var diffSUTcount = seqs.Item2.Count();

            if (diff3count > diffSUTcount)
            {
                Console.WriteLine("SUT seq is shorter than RI seq");
                DumpSeqs(seqs, name);
                return;
            }

            if (diff3count < diffSUTcount)
            {
                Console.WriteLine("SUT seq is longer than RI seq");
                DumpSeqs(seqs, name);
                return;
            }

            for (int i = 0; i < (diff3count > diffSUTcount ? diff3count : diffSUTcount); i++)
            {
                if (seqs.Item1.ElementAt(i) != seqs.Item2.ElementAt(i))
                {
                    Console.WriteLine(string.Format("Err: Strs not equals. RI: [{0}] SUT: [{1}]", seqs.Item1.ElementAt(i), seqs.Item2.ElementAt(i)));
                    DumpSeqs(seqs, name);
                    return;
                }
            }

            DeleteDumps(name);
            Console.WriteLine("Test passed OK");

        }
        
        public static void DumpSeqs(Tuple<IEnumerable<string>, IEnumerable<string>> seqs, string name)
        {
            File.WriteAllLines(name + _ritxt, seqs.Item1 ?? new string[] { "NO FILE" });
            File.WriteAllLines(name + _suttxt, seqs.Item2 ?? new string[] { "NO FILE" });
        }

        public static void DeleteDumps(string name)
        {
            File.Delete(name + _ritxt);
            File.Delete(name + _suttxt);
        }

        private static Tuple<IEnumerable<string>, IEnumerable<string>> RunDiff3(string diff3Path, string diff3_SUT, string parent, string fst, string snd)
        {
            var paramsTempl_diff3 = "{0} {1} {2} -m";
            var paramsTempl_SUT = "{0} {1} {2}";

            var diff3_out = RunDiff_Proc(diff3Path, paramsTempl_diff3, parent, fst, snd);

            var diff3SUT_out = RunDiff_Proc(diff3_SUT, paramsTempl_SUT, parent, fst, snd);

            return new Tuple<IEnumerable<string>, IEnumerable<string>>(diff3_out, diff3SUT_out);
            
        }

        private static IEnumerable<string> RunDiff_Proc(string pathToExe, string paramsTempl, string parent, string fst, string snd)
        {
            var diff3Proc = new Process();
            diff3Proc.StartInfo.FileName = pathToExe;
            diff3Proc.StartInfo.UseShellExecute = false;
            diff3Proc.StartInfo.RedirectStandardOutput = true;
            diff3Proc.StartInfo.Arguments = string.Format(paramsTempl, fst, parent, snd);
            var out_diff3 = new List<string>();
            diff3Proc.OutputDataReceived += (s, ea) =>
            {
                out_diff3.Add(ea.Data);
            };
            diff3Proc.StartInfo.RedirectStandardInput = true;
            diff3Proc.Start();
            diff3Proc.BeginOutputReadLine();
            diff3Proc.WaitForExit();
            diff3Proc.Close();

            return out_diff3;
        }
    }
}
