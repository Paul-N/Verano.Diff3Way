using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way
{
    public class Merge
    {
        public int[,] LCSLength(string[] X, string[] Y)
        {
            var m = X.Length; var n = Y.Length;
            var C = new int[m + 1, n + 1];
            for (int i = 0; i < m; i++)
                for (int j = 0; j < n; j++)
                    if (X[i] == Y[j])
                        C[i + 1, j + 1] = C[i, j] + 1;
                    else
                        C[i + 1, j + 1] = Math.Max(C[i + 1, j], C[i, j + 1]);
            return C;
        }

        //public IEnumerable<Tuple<string, FileType?>> BacktrackSmart(int[,] C, string[] O, string[] A, int i, int j)
        //{
        //    if (i == 0 || j == 0)
        //        return Enumerable.Empty<Tuple<string, FileType?>>();
        //    else if (O[i - 1] == A[j - 1])
        //    {
        //        var lstToRet = BacktrackSmart(C, O, A, i - 1, j - 1).ToList();
        //        lstToRet.Add(new Tuple<string, FileType?>(O[i - 1], null));
        //        return lstToRet;
        //    }
        //    else
        //        if (C[i, j - 1] > C[i - 1, j])
        //        {
        //            var lstToRet = BacktrackSmart(C, O, A, i, j - 1).ToList();
        //            lstToRet.Add(new Tuple<string, FileType?>(null, FileType.New));
        //            return lstToRet;
        //        }
        //        else
        //        {
        //            var lstToRet = BacktrackSmart(C, O, A, i - 1, j).ToList();
        //            lstToRet.Add(new Tuple<string, FileType?>(null, FileType.Orig));
        //            return lstToRet;
        //        }
        //}

        public IEnumerable<string> BacktrackEasy(int[,] C, string[] O, string[] A, int i, int j)
        {
            if (i == 0 || j == 0)
                return Enumerable.Empty<string>();
            else if (O[i - 1] == A[j - 1])
            {
                var lstToRet = BacktrackEasy(C, O, A, i - 1, j - 1).ToList();
                lstToRet.Add(O[i - 1]);
                return lstToRet;
            }
            else
                if (C[i, j - 1] > C[i - 1, j])
                {
                    var lstToRet = BacktrackEasy(C, O, A, i, j - 1).ToList();
                    //lstToRet.Add(new Tuple<string, FileType?>(null, FileType.New));
                    return lstToRet;
                }
                else
                {
                    var lstToRet = BacktrackEasy(C, O, A, i - 1, j).ToList();
                    //lstToRet.Add(new Tuple<string, FileType?>(null, FileType.Orig));
                    return lstToRet;
                }
        }

        public IEnumerable<Tuple<string, string>> BacktrackNormalize(IEnumerable<string> backtrack, string[] O, string[] X)
        {
            var res = new List<Tuple<string, string>>();

            for (int i = 0, j = 0, k = 0; i < backtrack.Count() || j < O.Length || k < X.Length; )
            {
                if (i < backtrack.Count())
                {
                    if (backtrack.ElementAt(i) == O[j] && backtrack.ElementAt(i) == X[k])
                    {
                        res.Add(new Tuple<string, string>(O[j], X[k]));
                        i++; j++; k++;
                    }
                    else if (backtrack.ElementAt(i) == O[j] || backtrack.ElementAt(i) == X[k])
                    {
                        if (backtrack.ElementAt(i) == O[j])
                        {
                            res.Add(new Tuple<string, string>(null, X[k])); k++;
                        }
                        else
                        {
                            res.Add(new Tuple<string, string>(O[j], null)); j++;
                        }
                    }
                    else
                    {
                        res.Add(new Tuple<string, string>(O[j], X[k]));
                        j++; k++;
                    }
                }
                else
                {
                    if (j < O.Count() && k < X.Count())
                    {
                        res.Add(new Tuple<string, string>(O[j], X[k]));
                        j++; k++;
                    }
                    else if (j < O.Count())
                    {
                        res.Add(new Tuple<string, string>(O[j], null));
                        j++;
                    }
                    else
                    {
                        res.Add(new Tuple<string, string>(null, X[k]));
                        k++;
                    }
                }
            }

            return res;
        }

        //public IEnumerable<Tuple<string, string>> GetMaximumMatches(string[] X, string[] Y, IEnumerable<Tuple<string, FileType?>> backtracks)
        //{
        //    var result = new List<Tuple<string, string>>();

        //    var maxLen = backtracks.Count();
        //    int j = 0, k = 0;

        //    for (int i = 0; i < maxLen; i++)
        //    {
        //        if (backtracks.ElementAt(i).Item1 != null)
        //        {
        //            result.Add(new Tuple<string, string>(backtracks.ElementAt(i).Item1, backtracks.ElementAt(i).Item1));
        //            j++; k++;
        //        }
        //        else if (backtracks.ElementAt(i).Item2 == FileType.Orig)
        //        {
        //            result.Add(new Tuple<string, string>(X[k], null));
        //            k++;
        //        }
        //        else if (backtracks.ElementAt(i).Item2 == FileType.New)
        //        {
        //            result.Add(new Tuple<string, string>(null, Y[j]));
        //            j++;
        //        }
        //    }

        //    return result;
        //}

        public IEnumerable<Chunk> DiffParse(IEnumerable<Tuple<string, string>> OA, IEnumerable<Tuple<string, string>> OB)
        {
            var res = new List<Chunk>();

            var chunkWasStarted = false;
            var hunk_a = false;
            var hunk_b = false;

            for (int i = 0, j = 0; i < OA.Count() || j < OB.Count(); )
            {
                if (i == OA.Count())
                {
                    if (hunk_b)
                        res.Last().AddToB(OB.ElementAt(j).Item2);
                    else
                    {
                        res.Add(new Chunk(null, null, OB.ElementAt(j).Item2));
                        hunk_b = true;
                    }

                    j++;
                }
                else if (j == OB.Count())
                {
                    if (hunk_a)
                        res.Last().AddToA(OA.ElementAt(i).Item2);
                    else
                    {
                        res.Add(new Chunk(OA.ElementAt(i).Item2, null, null));
                        hunk_a = true;
                    }

                    i++;
                }
                else if (OA.ElementAt(i).Item1 == OA.ElementAt(i).Item2 && OB.ElementAt(j).Item1 == OB.ElementAt(j).Item2 && OA.ElementAt(i).Item1 == OB.ElementAt(j).Item1)
                {
                    if (!chunkWasStarted)
                    {

                        res.Add(new Chunk(OA.ElementAt(i).Item2, OA.ElementAt(i).Item1, OB.ElementAt(j).Item1));

                        chunkWasStarted = true;
                        hunk_a = false;
                        hunk_b = false;
                    }
                    else
                    {
                        res.Last().AddToEnd(OA.ElementAt(i).Item1, OA.ElementAt(i).Item1, OA.ElementAt(i).Item1);
                        chunkWasStarted = true;
                        hunk_a = false;
                        hunk_b = false;
                    }
                    i++; j++;
                }
                else if (OA.ElementAt(i).Item1 == OB.ElementAt(j).Item1) // AO OB: ? (x || false) (x || false) ? => ...
                {
                    if (OA.ElementAt(i).Item1 == null) // AO OB: x false false y => AOB: x false y
                    {
                        if (hunk_a || hunk_b)
                        {
                            res.Last().AddToB(OB.ElementAt(j).Item2);
                            res.Last().AddToA(OA.ElementAt(i).Item2);
                        }
                        else
                        {
                            res.Add(new Chunk(OA.ElementAt(i).Item2, null, OB.ElementAt(j).Item2));

                            hunk_a = true; hunk_b = true;
                        }
                    }
                    else if (OA.ElementAt(i).Item2 == null || OB.ElementAt(j).Item2 == null)// AO OB: false x x x => AOB: false x x
                    {
                        var isSect_A = OA.ElementAt(i).Item1 == OA.ElementAt(i).Item2;

                        if (hunk_a || hunk_b)
                        {
                            res.Last().AddToO(OA.ElementAt(i).Item1);

                            if (isSect_A)
                                res.Last().AddToA(OA.ElementAt(i).Item2);
                            else
                                res.Last().AddToB(OB.ElementAt(j).Item2);
                        }
                        else
                            res.Add(new Chunk(isSect_A ? OA.ElementAt(i).Item2 : null, OA.ElementAt(i).Item1, isSect_A ? null : OB.ElementAt(j).Item2));

                        if (isSect_A) hunk_a = true; else hunk_b = true;
                    }
                    else // AO OB: xx xy => AOB: xxy
                    {

                        if (hunk_a || hunk_b)
                        {
                            res.Last().AddToEnd(OA.ElementAt(i).Item2, OA.ElementAt(i).Item1, OB.ElementAt(j).Item2);
                        }
                        else
                            res.Add(new Chunk(OA.ElementAt(i).Item2, OA.ElementAt(i).Item1, OB.ElementAt(j).Item2));

                        hunk_a = true; hunk_b = true;
                    }

                    chunkWasStarted = false; i++; j++;
                }
                else //if (OA.ElementAt(i).Item1 == null || OB.ElementAt(j).Item1 == null)
                {
                    if (OB.ElementAt(j).Item1 == OB.ElementAt(j).Item2)
                    {
                        if (hunk_a)
                            res.Last().AddToA(OA.ElementAt(i).Item2);
                        else
                        {
                            res.Add(new Chunk(OA.ElementAt(i).Item2, null, null));
                            hunk_a = true;
                        }
                        chunkWasStarted = false; i++;
                    }
                    else if (OA.ElementAt(i).Item1 == OA.ElementAt(i).Item2)
                    {
                        if (hunk_b)
                            res.Last().AddToB(OB.ElementAt(j).Item2);
                        else
                        {
                            res.Add(new Chunk(null, null, OB.ElementAt(j).Item2));
                            hunk_b = true;
                        }
                        chunkWasStarted = false; j++;
                    }
                    else if (OB.ElementAt(j).Item1 == null)
                    {
                        if (hunk_a || hunk_b)
                        {
                            res.Last().AddToEnd(OA.ElementAt(i).Item2, OA.ElementAt(i).Item1, OB.ElementAt(j).Item2);
                        }
                        else
                        {
                            res.Add(new Chunk(OA.ElementAt(i).Item2, OA.ElementAt(i).Item1, OB.ElementAt(j).Item2));
                        }
                        i++; j++; hunk_a = true; hunk_b = true; chunkWasStarted = false;
                    }
                    else if (OA.ElementAt(i).Item1 == null)
                    {
                        if (hunk_a || hunk_b)
                        {
                            res.Last().AddToEnd(OA.ElementAt(i).Item2, OB.ElementAt(j).Item1, OB.ElementAt(j).Item2);
                        }
                        else
                        {
                            res.Add(new Chunk(OA.ElementAt(i).Item2, OB.ElementAt(j).Item1, OB.ElementAt(j).Item2));
                        }
                        i++; j++; hunk_a = true; hunk_b = true; chunkWasStarted = false;
                    }
                    //else { throw new Exception(); }
                }
            }

            return res;
        }

        private void AddConflictChunk(string aName, string oName, string bName, Chunk chunk, List<string> listToAdd)
        {
            listToAdd.Add("<<<<<<< " + aName);
            if (chunk.ASeq != null && chunk.ASeq.Any(x => x != null)) listToAdd.AddRange(chunk.ASeq.Where(c => c != null));
            listToAdd.Add("||||||| " + oName);
            listToAdd.AddRange(chunk.OSeq.Where(c => c != null));
            listToAdd.Add("=======");
            if (chunk.BSeq != null && chunk.BSeq.Any(x => x != null)) listToAdd.AddRange(chunk.BSeq.Where(c => c != null));
            listToAdd.Add(">>>>>>> " + bName);
        }

        public void SmoothChunks(IList<Chunk> parsedDiff)
        {
            if (parsedDiff == null)
                return;

            var len = parsedDiff.Count();

            for (int i = 0; i < len; i++)
            {
                if (parsedDiff[i].ChunkType == ChunkType.Chunk && i > 0 && i < len - 1)
                {
                    //if (parsedDiff[i - 1].ChunkType == ChunkType.HunkConflict && parsedDiff[i + 1].ChunkType == ChunkType.HunkConflict && parsedDiff[i].OSeq.Count() <= 3
                    //    && !parsedDiff[i].OSeq.Any(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)))
                    if (parsedDiff[i - 1].HasSameConflictTypeWith(parsedDiff[i + 1]) && parsedDiff[i].OSeq.Count() <= 3
                        && !parsedDiff[i].OSeq.Any(s => !string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s)))
                    {
                        parsedDiff[i - 1].CombineWith(parsedDiff[i], parsedDiff[i + 1]);
                        parsedDiff.RemoveAt(i);
                        parsedDiff.RemoveAt(i); // not i + 1 'cause it's (i + 1)-th now is just i-th
                        len -= 2;
                    }
                }
            }
        }

        public IEnumerable<string> MergeChunks(IList<Chunk> parsedDiff, string aName, string oName, string bName)
        {
            var res = new List<string>();

            foreach (var chunk in parsedDiff)
            {
                if (/*chunk.OSeq.SequenceEqual(chunk.ASeq) && chunk.OSeq.SequenceEqual(chunk.BSeq)*/chunk.ASeq.SequenceEqual(chunk.BSeq))
                {
                    res.AddRange(chunk.ASeq);
                }
                //else if (chunk.ASeq.SequenceEqual(chunk.BSeq))
                //{
                //    res.AddRange(chunk.ASeq);
                //}
                else
                    if (!chunk.ASeq.Any())
                    {
                        if (!chunk.OSeq.Any())
                            res.AddRange(chunk.BSeq);
                        else
                        {
                            if (!chunk.BSeq.SequenceEqual(chunk.OSeq))
                                AddConflictChunk(aName, oName, bName, chunk, res);
                        }
                    }
                    else if (!chunk.BSeq.Any())
                    {
                        if (!chunk.OSeq.Any())
                            res.AddRange(chunk.ASeq);
                        else
                        {
                            if (!chunk.ASeq.SequenceEqual(chunk.OSeq))
                                AddConflictChunk(aName, oName, bName, chunk, res);
                        }
                    }
                    else if ((chunk.OSeq == null || !chunk.OSeq.Any()) && chunk.ASeq.SequenceEqual(chunk.BSeq))
                        res.AddRange(chunk.ASeq);
                    else if (chunk.ASeq.SequenceEqual(chunk.OSeq))
                        res.AddRange(chunk.BSeq);
                    else if (chunk.BSeq.SequenceEqual(chunk.OSeq))
                        res.AddRange(chunk.ASeq);
                    else
                        AddConflictChunk(aName, oName, bName, chunk, res);
            }

            return res;
        }

        public string[] Merge3Way(string[] a, string[] o, string[] b, string aFileName, string oFileName, string bFileName)
        {
            var strsMatrOA = this.LCSLength(o, a);
            var strsMatrOB = this.LCSLength(o, b);
            var oa = this.BacktrackNormalize(this.BacktrackEasy(strsMatrOA, o, a, o.Length, a.Length), o, a);
            var ob = this.BacktrackNormalize(this.BacktrackEasy(strsMatrOB, o, b, o.Length, b.Length), o, b);

            var diffs = this.DiffParse(oa, ob).ToList();
            this.SmoothChunks(diffs);

            return this.MergeChunks(diffs, aFileName, oFileName, bFileName).ToArray();
        }

        public string[] Merge3Way(string aFileName, string oFileName, string bFileName)
        {
            if (aFileName == null)
                throw new ArgumentNullException("aFileName");
            else if (oFileName == null)
                throw new ArgumentNullException("oFileName");
            else if (bFileName == null)
                throw new ArgumentNullException("bFileName");
            if (!File.Exists(aFileName))
                throw new ArgumentException(aFileName + " doesn't exists");
            else if (!File.Exists(oFileName))
                throw new ArgumentException(oFileName + " doesn't exists");
            else if (!File.Exists(bFileName))
                throw new ArgumentException(bFileName + " doesn't exists");

            var a = File.ReadAllLines(aFileName);
            var o = File.ReadAllLines(oFileName);
            var b = File.ReadAllLines(bFileName);

            return this.Merge3Way(a, o, b, aFileName, oFileName, bFileName);
        }
    }

    public enum ChunkType { /*Undef, */Chunk, /*Hunk, */HunkConflict, HunkNoConflictInA, HunkNoConflictInB }
}
