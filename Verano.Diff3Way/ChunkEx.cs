using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way
{
    internal static class ChunkEx
    {
        /// <summary>
        /// Comparing two arguments, both of string or IEnumerable<string>.
        /// </summary>
        /// <param name="a">First argument (string or IEnumerable<string>)</param>
        /// <param name="b">Secind argument (string or IEnumerable<string>)</param>
        /// <returns>Result of Either string.Compare() or IEnumerable<T>.SequenceEqual(IEnumerable<T> second)</returns>
        public static bool IsEqualTo(this object a, object b)
        {
            IsStringsOrStringSequences(a, b);

            if (a is string && b is string)
                return string.Compare(a as string, b as string) == 0;
            else
            {
                if (a == null && b == null)
                    return true;
                else if ((a == null && b != null) || (a != null && b == null))
                    return false;
                else
                    return (a as IEnumerable<string>).SequenceEqual(b as IEnumerable<string>);
            }

        }

        public static ChunkType GetCommonWith(this ChunkType t1, ChunkType t2)
        {
            if (t1 == ChunkType.Chunk)
            {
                if (t2 == ChunkType.Chunk)
                    return ChunkType.Chunk;
                return ChunkType.HunkConflict;
            }
            else if (t2 == ChunkType.Chunk)
            {
                if (t1 == ChunkType.Chunk)
                    return ChunkType.Chunk;
                return ChunkType.HunkConflict;
            }
            else
            {
                if (t1 == ChunkType.HunkConflict || t2 == ChunkType.HunkConflict)
                    return ChunkType.HunkConflict;
                else
                {
                    if (t1 == ChunkType.HunkNoConflictInA && t2 == ChunkType.HunkNoConflictInA)
                        return ChunkType.HunkNoConflictInA;
                    else if (t1 == ChunkType.HunkNoConflictInB && t2 == ChunkType.HunkNoConflictInB)
                        return ChunkType.HunkNoConflictInA;
                    else
                        return ChunkType.HunkConflict;
                }
            }
        }

        /// <summary>
        /// Check that all arguments have either string or IEnumerable<string> type.
        /// </summary>
        /// <param name="p">Arguments to check</param>
        public static void IsStringsOrStringSequences(params object[] p)
        {
            if (p.Length == 0) throw new ArgumentException("No params");

            var exceptionMsgTempl = "Expected type string or IEnumerable<string> but {0} type found";

            if (!(p[0] is string || p[0] is IEnumerable<string>))
                throw new ArgumentException(string.Format(exceptionMsgTempl, p[0].GetType().FullName));

            var isStrings = p[0] is string;

            foreach (var param in p)
            {
                if ((param is string) && isStrings)
                {
                    continue;
                }
                else if (param is IEnumerable<string> && !isStrings)
                {
                    continue;
                }
                else throw new ArgumentException(string.Format(exceptionMsgTempl, param.GetType().FullName));
            }
        }
    }
}
