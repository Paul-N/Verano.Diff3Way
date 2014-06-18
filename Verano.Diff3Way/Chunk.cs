using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way
{
    public class Chunk
    {
        private List<string> _aSeq;

        /// <summary>
        /// Sequence of A file (first modified file)
        /// </summary>
        public IReadOnlyCollection<string> ASeq { get { return _aSeq.AsReadOnly(); } }

        private List<string> _oSeq;

        /// <summary>
        /// Sequence of O file (orginal file)
        /// </summary>
        public IReadOnlyCollection<string> OSeq { get { return _oSeq.AsReadOnly(); } }

        private List<string> _bSeq;

        /// <summary>
        /// Sequence of B file (second modified file)
        /// </summary>
        public IReadOnlyCollection<string> BSeq { get { return _bSeq.AsReadOnly(); } }
        
        public ChunkType ChunkType
        {
            get; private set;
        }

        public Chunk()
        {
            _aSeq = new List<string>();
            _bSeq = new List<string>();
            _oSeq = new List<string>();
            ChunkType = default(ChunkType);
        }

        public Chunk(List<string> aSeqs, List<string> oSeqs, List<string> bSeqs)
        {
            _aSeq = aSeqs ?? new List<string>();
            _bSeq = bSeqs ?? new List<string>();
            _oSeq = oSeqs ?? new List<string>();
            this.ChunkType = DefineChunkType(_aSeq, _oSeq, _bSeq);
        }

        public Chunk(string a1stElem, string o1stElem, string b1stElem)
        {
            _aSeq = a1stElem == null ? new List<string>() : new List<string> { a1stElem };
            _bSeq = b1stElem == null ? new List<string>() : new List<string> { b1stElem };
            _oSeq = o1stElem == null ? new List<string>() : new List<string> { o1stElem };
            this.ChunkType = DefineChunkType(_aSeq, _oSeq, _bSeq);
        }

        public bool HasSameConflictTypeWith(Chunk chunk)
        {
            if (this.ChunkType == ChunkType.Chunk || chunk == null || chunk.ChunkType == ChunkType.Chunk)
                return false;
            else
                return this.ChunkType == chunk.ChunkType;
        }

        private ChunkType DefineChunkType<T>(T a, T o, T b) where T : class
        {
            
            ChunkEx.IsStringsOrStringSequences(a, o, b);
            if (a.IsEqualTo(b))
            {
                if (a.IsEqualTo(o))
                    return ChunkType.Chunk;
                else
                    return ChunkType.HunkNoConflictInA;
            }
            else
            {
                if (a.IsEqualTo(o))
                    return ChunkType.HunkNoConflictInB;
                else
                    if (b.IsEqualTo(o))
                        return ChunkType.HunkNoConflictInA;
                    else
                        return ChunkType.HunkConflict;
            }
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as Chunk;
            if (toCompare == null) return false;
            if (ASeq.SequenceEqual(toCompare.ASeq))
                if (BSeq.SequenceEqual(toCompare.BSeq))
                    if (OSeq.SequenceEqual(toCompare.OSeq))
                        return true;
                    else return false;
                else return false;
            else return false;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 1;
                if (ASeq != null)
                    foreach (var s in ASeq) hash = hash * 17 + s.GetHashCode();

                if (OSeq != null)
                    foreach (var s in OSeq) hash = hash * 23 + s.GetHashCode();

                if (BSeq != null)
                    foreach (var s in BSeq) hash = hash * 29 + s.GetHashCode();
                return hash;
            }
        }

        public void CombineWith(Chunk chunkToSwallow, Chunk nextHunk)
        {
            if (_aSeq == null || _oSeq == null || _bSeq == null || chunkToSwallow._aSeq == null || chunkToSwallow._oSeq == null || chunkToSwallow._bSeq == null || nextHunk._aSeq == null || nextHunk._oSeq == null || nextHunk._bSeq == null)
                throw new InvalidOperationException("One of the Chunk arguments has null sequence");

            _aSeq.AddRange(chunkToSwallow._aSeq); _aSeq.AddRange(nextHunk._aSeq);
            _bSeq.AddRange(chunkToSwallow._bSeq); _bSeq.AddRange(nextHunk._bSeq);

            this.ChunkType = chunkToSwallow.ChunkType.GetCommonWith(nextHunk.ChunkType).GetCommonWith(this.ChunkType);
        }

        public void AddToA(string str)
        {
            if (ChunkType == ChunkType.Chunk)
                throw new InvalidOperationException("You can't add element in the only one sequence of chunk type of Chunk. It's only possible in hunk type of Chunk. Use AddToEnd() method instead");

            if (_aSeq == null)
                _aSeq = new List<string> { str };
            else
                _aSeq.Add(str);
        }

        public void AddToO(string str)
        {
            if (ChunkType == ChunkType.Chunk)
                throw new InvalidOperationException("You can't add element in the only one sequence of chunk type of Chunk. It's only possible in hunk type of Chunk. Use AddToEnd() method instead");

            if (_oSeq == null)
                throw new InvalidOperationException("OSequence is null");
            else
                _oSeq.Add(str);
        }

        public void AddToB(string str)
        {
            if (ChunkType == ChunkType.Chunk)
                throw new Exception("You can't add element in the only one sequence of chunk type of Chunk. It's only possible in hunk type of Chunk. Use AddToEnd() method instead");

            if (_bSeq == null)
                _bSeq = new List<string> { str };
            else
                _bSeq.Add(str);
        }

        public void AddToEnd(string aStr, string oStr, string bStr)
        {
            if (_aSeq == null)
                throw new InvalidOperationException("ASequence is null");

            if (_oSeq == null)
                throw new InvalidOperationException("OSequence is null");

            if (_bSeq == null)
                throw new InvalidOperationException("BSequence is null");

            _aSeq.Add(aStr);
            _oSeq.Add(oStr);
            _bSeq.Add(bStr);

            var chunkTypeNew = DefineChunkType(aStr, oStr, bStr);

            if (this.ChunkType == ChunkType.Chunk && chunkTypeNew != ChunkType.Chunk
                || this.ChunkType != ChunkType.Chunk && chunkTypeNew == ChunkType.Chunk)
                throw new Exception("Can't mix Chunks and Hunks");

            this.ChunkType = this.ChunkType.GetCommonWith(chunkTypeNew);

        }
    }
}
