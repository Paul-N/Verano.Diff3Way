#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way
{
    public static class DebugTimeHelpers
    {
        public static IEnumerable<string> ToStringList(this int[,] m)
        {
            if (m == null)
                yield return null;
            else
            {
                var sb = new StringBuilder();

                for (int i = 0; i < m.GetLength(0); i++)
                {
                    sb.Clear();
                    for (int j = 0; j < m.GetLength(1); j++)
                    {
                        sb.Append(m[i, j]);
                        sb.Append(", ");
                    }
                    yield return sb.ToString();
                }
            }
        }

        public static IEnumerable<string> ToStringList(this IEnumerable<Tuple<string, string>> tuples)
        {
            if (tuples == null)
                yield return null;
            else
                foreach (var t in tuples)
                    yield return string.Format("[{0}][{1}]", t.Item1 ?? "null", t.Item2 ?? "null");
        }

        public static IEnumerable<string> ToStringList(this IEnumerable<Chunk> chunks)
        {
            if (chunks == null)
                yield return null;
            else
                foreach (var chunk in chunks)
                {
                    switch (chunk.ChunkType)
                    {
                        case ChunkType.Chunk:
                            yield return "Chunk";
                            break;
                        case ChunkType.HunkConflict:
                            yield return "HunkConflict";
                            break;
                        case ChunkType.HunkNoConflictInA:
                            yield return "HunkNoConflictInA";
                            break;
                        case ChunkType.HunkNoConflictInB:
                            yield return "HunkNoConflictInB";
                            break;
                        default:
                            yield return "-";
                            break;
                    }
                    if (chunk.ChunkType == ChunkType.Chunk)
                    {
                        foreach (var o in chunk.OSeq)
                            yield return "AOB: " + o;
                    }
                    else if (chunk.ChunkType == ChunkType.HunkNoConflictInA)
                    {
                        foreach (var a in chunk.ASeq)
                            yield return "A: " + a;
                    }
                    else if (chunk.ChunkType == ChunkType.HunkNoConflictInB)
                    {
                        foreach (var b in chunk.BSeq)
                            yield return "B: " + b;
                    }
                    else
                    {
                        foreach (var a in chunk.ASeq)
                            yield return "A: " + a;
                        foreach (var a in chunk.OSeq)
                            yield return "O: " + a;
                        foreach (var a in chunk.BSeq)
                            yield return "B: " + a;
                    }
                }
        }

        public static IEnumerable<string> ToChunkTypesList(this IEnumerable<Chunk> chunks)
        {
            if (chunks == null)
                yield return null;
            else
                foreach (var chunk in chunks)
                {
                    switch (chunk.ChunkType)
                    {
                        case ChunkType.Chunk:
                            yield return "Chunk";
                            break;
                        case ChunkType.HunkNoConflictInA:
                            yield return "HunkNoConflictInA";
                            break;
                        case ChunkType.HunkNoConflictInB:
                            yield return "HunkNoConflictInB";
                            break;
                        case ChunkType.HunkConflict:
                            yield return "HunkConflict";
                            break;
                        default:
                            yield return "-";
                            break;
                    }
                }
        }
    }
}
#endif