using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verano.Diff3Way.Tests
{
    [TestFixture]
    public class MergeTests
    {
        private string[] _X = "XMJYAUZ".ToCharArray().Select(c => c.ToString()).ToArray();
        private string[] _Y = "MZJAWXU".ToCharArray().Select(c => c.ToString()).ToArray();

        private readonly int[,] _correctXYMatrix = new int[8, 8] 
            {
                {0, 0, 0, 0, 0, 0, 0, 0 },
                {0, 0, 0, 0, 0, 0, 1, 1 },
                {0, 1, 1, 1, 1, 1, 1, 1 },
                {0, 1, 1, 2, 2, 2, 2, 2 },
                {0, 1, 1, 2, 2, 2, 2, 2 },
                {0, 1, 1, 2, 3, 3, 3, 3 },
                {0, 1, 1, 2, 3, 3, 3, 4 },
                {0, 1, 2, 2, 3, 3, 3, 4 },
            };

        string[] A = (new int[] { 1, 4, 5, 2, 3, 6 }).Select(x => x.ToString()).ToArray();
        string[] O = (new int[] { 1, 2, 3, 4, 5, 6 }).Select(x => x.ToString()).ToArray();
        string[] B = (new int[] { 1, 2, 4, 5, 3, 6 }).Select(x => x.ToString()).ToArray();

        private readonly int[,] _correctAOMatrix = new int[7, 7] 
            {
                {0, 0, 0, 0, 0, 0, 0 },
                {0, 1, 1, 1, 1, 1, 1 },
                {0, 1, 1, 1, 2, 2, 2 },
                {0, 1, 1, 1, 2, 3, 3 },
                {0, 1, 2, 2, 2, 3, 3 },
                {0, 1, 2, 3, 3, 3, 3 },
                {0, 1, 2, 3, 3, 3, 4 }
            };

        private List<Tuple<String, String>> _backtracksAO = new List<Tuple<String, String>>
            { 
                new Tuple<String, String>(1.ToString(), 1.ToString()),
                new Tuple<String, String>(null, 4.ToString()),
                new Tuple<String, String>(null, 5.ToString()),
                new Tuple<String, String>(2.ToString(), 2.ToString()),
                new Tuple<String, String>(3.ToString(), 3.ToString()),
                new Tuple<String, String>(4.ToString(), null),
                new Tuple<String, String>(5.ToString(), null),
                new Tuple<String, String>(6.ToString(), 6.ToString())

            };

        private List<Tuple<String, String>> _maxMatchesAO = new List<Tuple<String, String>>
        {
            new Tuple<string, string>("1", "1"),
            new Tuple<string, string>(null, "4"),
            new Tuple<string, string>(null, "5"),
            new Tuple<string, string>("2", "2"),
            new Tuple<string, string>("3", "3"),
            new Tuple<string, string>("4", null),
            new Tuple<string, string>("5", null),
            new Tuple<string, string>("6", "6"),
        };

        private IEnumerable<Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>> _parsedRes = new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>[] 
        {
            new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(new string[] {"1"}, new string[] {"1"}, new string[] {"1"}),
            new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(new string[] {"4", "5"}, new string[] {}, new string[] {}),
            new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(new string[] {"2"}, new string[] {"2"}, new string[] {"2"}),
            new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(new string[] {"3"}, new string[] {"3", "4", "5"}, new string[] {"4", "5", "3"}),
            new Tuple<IEnumerable<string>, IEnumerable<string>, IEnumerable<string>>(new string[] {"6"}, new string[] {"6"}, new string[] {"6"}),
        };

        private IEnumerable<Chunk> _propagatedRes2 = new Chunk[] 
        {
            new Chunk(new List<string> {"1"}, new List<string> {"1"}, new List<string> {"1"}),
            new Chunk(new List<string> {"4", "5"}, new List<string> {"4", "5"}, new List<string> {"4", "5"}),
            new Chunk(new List<string> {"2"}, new List<string> {"2"}, new List<string> {"2"}),
            new Chunk(new List<string> {"3"}, new List<string> {"3", "4", "5"}, new List<string> {"4", "5", "3"}),
            new Chunk(new List<string> {"6"}, new List<string> {"6"}, new List<string> {"6"}),
        };

        private string[] _diff3normOfWarmUp = new string[] 
        {
            "1",
            "4",
            "5",
            "2",
            "<<<<<<< A",
            "3",
            "||||||| O",
            "3",
            "4",
            "5",
            "=======",
            "4",
            "5",
            "3",
            ">>>>>>> B",
            "6",
        };

        
        string[] A555 = (new int[] { 1,2,4,6,8}).Select(x => x.ToString()).ToArray();
        string[] O555 = (new int[] { 1,2,3,4,5,5,5,6,7,8}).Select(x => x.ToString()).ToArray();
        string[] B555 = (new int[] { 1, 4, 5, 5, 5, 6, 2, 3, 4, 8 }).Select(x => x.ToString()).ToArray();

        [Test]
        public void TestLCS()
        {
            // Arrange

            var merge = new Merge();

            // Act
            var C = merge.LCSLength(_X, _Y);

            // Assert
            Assert.That(C, Is.EqualTo(_correctXYMatrix));
        }

        [Test]
        public void TestLCS2()
        {
            // Arrange

            var merge = new Merge();

            // Act
            var C = merge.LCSLength(A, O);

            // Assert
            Assert.That(C, Is.EqualTo(_correctAOMatrix));
        }

        [Test]
        public void TestBacktrack()
        {
            // Arrange
            var merge = new Merge();

            // Act
            var OtoAcommonChunks = merge.BacktrackNormalize(merge.BacktrackEasy(_correctAOMatrix, O, A, O.Length, A.Length), O, A);

            // Assert

            Assert.That(OtoAcommonChunks, Is.EqualTo(_backtracksAO));
        }

        [Test]
        public void TestDiffParse()
        {
            // Arrange
            var merge = new Merge();

            // Act

            var strsMatrOB = merge.LCSLength(O, B);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, O, B, O.Length, B.Length), O, B);

            var res = merge.DiffParse(_maxMatchesAO, ob);

            // Assert
            if (res == null || res.Count() != _parsedRes.Count())
                Assert.Fail("Result is null or empty");
            for (int i = 0; i < res.Count(); i++)
            {
                Assert.That(res.ElementAt(i).ASeq.SequenceEqual(_parsedRes.ElementAt(i).Item1));
                Assert.That(res.ElementAt(i).OSeq.SequenceEqual(_parsedRes.ElementAt(i).Item2));
                Assert.That(res.ElementAt(i).BSeq.SequenceEqual(_parsedRes.ElementAt(i).Item3));
            }
        }

        [Test]
        public void TestDiffParse2()
        {
            // Arrange
            var merge = new Merge();

            // Act

            var strsMatrOB = merge.LCSLength(O, B);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, O, B, O.Length, B.Length), O, B);

            var res = merge.DiffParse(_backtracksAO, ob);

            // Assert
            if (res == null || res.Count() != _parsedRes.Count())
                Assert.Fail("Result is null or empty");
            for (int i = 0; i < res.Count(); i++)
            {
                Assert.That(res.ElementAt(i).ASeq.SequenceEqual(_parsedRes.ElementAt(i).Item1));
                Assert.That(res.ElementAt(i).OSeq.SequenceEqual(_parsedRes.ElementAt(i).Item2));
                Assert.That(res.ElementAt(i).BSeq.SequenceEqual(_parsedRes.ElementAt(i).Item3));
            }
        }


        [Test]
        public void TestGetDiff()
        {
            // Arrange
            var merge = new Merge();

            IList<Chunk> parsedRes = new List<Chunk>
            {
                new Chunk(new List<string> {"1"}, new List<string> {"1"}, new List<string> {"1"}),
                new Chunk(new List<string> {"4", "5"}, new List<string> {}, new List<string> {}),
                new Chunk(new List<string> {"2"}, new List<string> {"2"}, new List<string> {"2"}),
                new Chunk(new List<string> {"3"}, new List<string> {"3", "4", "5"}, new List<string> {"4", "5", "3"}),
                new Chunk(new List<string> {"6"}, new List<string> {"6"}, new List<string> {"6"}),
            };

            var resDiff = new string[]
            {
                "1",
                "4",
                "5",
                "2",
                "<<<<<<< A",
                "3",
                "||||||| O",
                "3",
                "4",
                "5",
                "=======",
                "4",
                "5",
                "3",
                ">>>>>>> B",
                "6",
            };

            // Act
            var res = merge.MergeChunks(parsedRes, "A", "O", "B");

            // Assert
            if (parsedRes == null || parsedRes.Count() != _propagatedRes2.Count<Chunk>())
                Assert.Fail("Result is null or empty");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestGetDiff555()
        {
            // Arrange
            var merge = new Merge();


            var resDiff = new string[]
            {
                "1",
                "<<<<<<< A",
                "2",
                "||||||| O",
                "2",
                "3",
                "=======",
                ">>>>>>> B",
                "4",
                "6",
                "<<<<<<< A",
                "||||||| O",
                "7",
                "=======",
                "2",
                "3",
                "4",
                ">>>>>>> B",
                "8",
            };

            // Act

            var strsMatrOB = merge.LCSLength(O555, B555);
            var strsMatrOA = merge.LCSLength(O555, A555);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, O555, B555, O555.Length, B555.Length), O555, B555);
            var oa = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOA, O555, A555, O555.Length, A555.Length), O555, A555);

            var res = merge.MergeChunks(merge.DiffParse(oa, ob).ToList(), "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_DifferentStringsAddedAtTheSamePlaces()
        {
            // Arrange
            var merge = new Merge();


            var o = new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
            };

            var a = new string[]
            {
                "1",
                "2",
                "3",
                "11",
                "4",
                "5",
                "6",
                "7",
            };

            var b = new string[]
            {
                "1",
                "2",
                "3",
                "12",
                "4",
                "5",
                "6",
                "7",
            };

            var resDiff = new string[]
            {
                "1",
                "2",
                "3",
                "<<<<<<< A",
                "11",
                "||||||| O",
                "=======",
                "12",
                ">>>>>>> B",
                "4",
                "5",
                "6",
                "7",
            };

            // Act

            var strsMatrOB = merge.LCSLength(o, b);
            var strsMatrOA = merge.LCSLength(o, b);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, o, b, o.Length, b.Length), o, b);
            var oa = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOA, o, a, o.Length, a.Length), o, a);

            var res = merge.MergeChunks(merge.DiffParse(oa, ob).ToList(), "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_DifferentLengths()
        {
            // Arrange
            var merge = new Merge();


            var o = new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
            };

            var a = new string[]
            {
                "1",
                "2",
                "3",
                "12",
                "4",
                "5",
                "6",
                "7",
            };

            var b = new string[]
            {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
            };

            var resDiff = new string[]
            {
                "1",
                "2",
                "3",
                "12",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11"
            };

            // Act

            var strsMatrOB = merge.LCSLength(o, b);
            var strsMatrOA = merge.LCSLength(o, b);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, o, b, o.Length, b.Length), o, b);
            var oa = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOA, o, a, o.Length, a.Length), o, a);

            var res = merge.MergeChunks(merge.DiffParse(oa, ob).ToList(), "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_ConinsSimpl()
        {
            // Arrange
            var merge = new Merge();


            var o = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
            };

            var a = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
                "13",
                "21",
                "22",
                "14",
                "15",
                "16",
            };

            var b = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
                "13",
                "31",
                "15",
                "16",
            };

            var resDiff = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
                "13",
                "<<<<<<< A",
                "21",
                "22",
                "14",
                "||||||| O",
                "14",
                "=======",
                "31",
                ">>>>>>> B",
                "15",
                "16"
            };

            // Act

            var strsMatrOB = merge.LCSLength(o, b);
            var strsMatrOA = merge.LCSLength(o, b);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, o, b, o.Length, b.Length), o, b);
            var oa = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOA, o, a, o.Length, a.Length), o, a);

            var res = merge.MergeChunks(merge.DiffParse(oa, ob).ToList(), "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_ConinsSimpl_FromPreparedChunks()
        {
            // Arrange
            var merge = new Merge();

            var aob = new List<Chunk> 
            {
                new Chunk(
                    new List<string>
                    {
                        "1",
                        "2",
                        "3",
                        "10",
                        "11",
                        "12",
                        "13",
                    },
                    new List<string>
                    {
                        "1",
                        "2",
                        "3",
                        "10",
                        "11",
                        "12",
                        "13",
                    },
                    new List<string>
                    {
                        "1",
                        "2",
                        "3",
                        "10",
                        "11",
                        "12",
                        "13",
                    }
                ),
                new Chunk
                    (
                    new List<string>
                    {
                        "21",
                        "22",
                        "14",
                    },
                    new List<string>
                    {
                        "14",
                    },
                    new List<string>
                    {
                        "31",
                    }
                    ),
                new Chunk
                    (
                    new List<string>
                    {
                        "15",
                        "16",
                    },
                    new List<string>
                    {
                        "15",
                        "16",
                    },
                    new List<string>
                    {
                        "15",
                        "16",
                    }
                    )
            };

            var resDiff = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
                "13",
                "<<<<<<< A",
                "21",
                "22",
                "14",
                "||||||| O",
                "14",
                "=======",
                "31",
                ">>>>>>> B",
                "15",
                "16"
            };

            // Act

            var res = merge.MergeChunks(aob, "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_ConinsDifferendEnds()
        {
            // Arrange
            var merge = new Merge();


            var o = new string[]
            {
                "1",
                "2",
                "3",
            };

            var a = new string[]
            {
                "1",
                "2",
                "3",
                "10",
                "11",
                "12",
            };

            var b = new string[]
            {
                "1",
                "2",
                "3",
                "20",
                "21",
                "22",
                "23",
                "24",
                "25",
                "26",
            };

            var resDiff = new string[]
            {
                "1",
                "2",
                "3",
                "<<<<<<< A",
                "10",
                "11",
                "12",
                "||||||| O",
                "=======",
                "20",
                "21",
                "22",
                "23",
                "24",
                "25",
                "26",
                ">>>>>>> B"
            };

            // Act

            var strsMatrOB = merge.LCSLength(o, b);
            var strsMatrOA = merge.LCSLength(o, a);
            var ob = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOB, o, b, o.Length, b.Length), o, b);
            var oa = merge.BacktrackNormalize(merge.BacktrackEasy(strsMatrOA, o, a, o.Length, a.Length), o, a);

            var res = merge.MergeChunks(merge.DiffParse(oa, ob).ToList(), "A", "O", "B");


            // Assert
            if (res == null || res.Count() != resDiff.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < resDiff.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(resDiff[i]));
            }
        }

        [Test]
        public void TestMerge_WarmUpFull()
        {
            // Arrange
            var merge = new Merge();

            // Act

            var res = merge.Merge3Way(A, O, B, "A", "O", "B");

            // Assert

            if (res == null || res.Count() != _diff3normOfWarmUp.Count())
                Assert.Fail("Result is null or empty or row count is wrong");
            for (int i = 0; i < _diff3normOfWarmUp.Count(); i++)
            {
                Assert.That(res.ElementAt(i), Is.EqualTo(_diff3normOfWarmUp[i]));
            }
        }
    }
}
