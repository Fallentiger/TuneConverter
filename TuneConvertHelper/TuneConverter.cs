using System;
using System.Collections.Generic;
using System.Text;

namespace TuneConvertHelper
{

    public class TuneConverter
    {
        public TuneConverter(bool markOnLeft = true, bool combineBracket = false)
        {
            c5Index = 48;
            MarkOnLeft = markOnLeft;
            CombineBracket = combineBracket;
            InitNodeList();
            InitTuneIndexDict();
        }
        private string sharpMark = "#";
        private string flatMark = "b♭";

        private readonly int c5Index;

        private string addOct = "[)【）";
        private string reduceOct = "](】（";


        private bool markOnLeft;
        public bool MarkOnLeft
        {
            get { return markOnLeft; }
            set
            {
                if (markOnLeft != value)
                {
                    markOnLeft = value;
                    InitNodeList();
                }
            }
        }

        //private bool combineBracket;
        public bool CombineBracket { get; set; }

        public List<string> majorTuneList = new List<string> {
            "G",
            "A♭/G#",
            "A",
            "B♭/A#",
            "B",
            "C",
            "C#/D♭",
            "D",
            "D#/E♭",
            "E",
            "F",
            "F#/G♭"
        };

        private Dictionary<char, int> tuneIndex;
        private void InitTuneIndexDict()
        {
            tuneIndex = new Dictionary<char, int>
            {
                { '1', 0 },
                { '2', 2 },
                { '3', 4 },
                { '4', 5 },
                { '5', 7 },
                { '6', 9 },
                { '7', 11 }
            };
        }

        public string StartConvert(string srcMusic, int baseOffset)
        {
            if (string.IsNullOrEmpty(srcMusic)) return null;

            int baseIndex = c5Index + baseOffset;
            string dstMusic = "";
            int sharpOffset = 0;
            int octOffset = 0;
            //string crtNode = "";
            srcMusic += " ";
            int rowCount = 1;
            int nodeCount = 1;

            //MusicTextReader srcMsc = new MusicTextReader(srcMusic);
            int inComment = 0;
            //string line = null;
            try
            {
                for (int idx = 0; idx < srcMusic.Length-1; idx++)
                {
                    string dm = srcMusic.Substring(idx,2);

                    if (dm[0] == '\n')
                    {
                        rowCount++;
                        nodeCount = 1;
                    }

                    if (inComment > 0)
                    {
                        if (dm[0] == '\n' && inComment == 1)
                        {
                            inComment = 0;
                            dstMusic += dm[0];
                        }
                        else if (dm == "*/" && inComment == 2)
                        {
                            inComment = 0;
                            dstMusic += dm;
                            idx++;
                        }
                        else
                        {
                            dstMusic += dm[0];
                        }
                    }
                    else
                    {
                        if (dm[0] == '/')
                        {
                            if (dm == "//")
                            {
                                dstMusic += dm;
                                inComment = 1;
                                idx++;
                            }
                            else if (dm == "/*")
                            {
                                dstMusic += dm;
                                inComment = 2;
                                idx++;
                            }
                            else
                            {
                                dstMusic += dm[0];
                            }
                        }
                        else if (addOct.Contains(dm[0].ToString()))
                        {
                            octOffset += 12;
                        }
                        else if (reduceOct.Contains(dm[0].ToString()))
                        {
                            octOffset -= 12;
                        }
                        else
                        {
                            if (MarkOnLeft)
                            {
                                if (sharpMark.Contains(dm[0].ToString()))
                                {
                                    sharpOffset = 1;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[1]] + sharpOffset + octOffset];
                                    sharpOffset = 0;
                                    nodeCount++;
                                    idx++;
                                }
                                else if (flatMark.Contains(dm[0].ToString()))
                                {
                                    sharpOffset = -1;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[1]] + sharpOffset + octOffset];
                                    sharpOffset = 0;
                                    nodeCount++;
                                    idx++;
                                }
                                else if (tuneIndex.ContainsKey(dm[0]))
                                {
                                    sharpOffset = 0;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[0]] + sharpOffset + octOffset];
                                    nodeCount++;
                                }
                                else
                                {
                                    dstMusic += dm[0];
                                }
                            }
                            else
                            {
                                if (sharpMark.Contains(dm[1].ToString()))
                                {
                                    sharpOffset = 1;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[0]] + sharpOffset + octOffset];
                                    sharpOffset = 0;
                                    nodeCount++;
                                    idx++;
                                }
                                else if (flatMark.Contains(dm[1].ToString()))
                                {
                                    sharpOffset = -1;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[0]] + sharpOffset + octOffset];
                                    sharpOffset = 0;
                                    nodeCount++;
                                    idx++;
                                }
                                else if (tuneIndex.ContainsKey(dm[0]))
                                {
                                    sharpOffset = 0;
                                    dstMusic += nodesList[baseIndex + tuneIndex[dm[0]] + sharpOffset + octOffset];
                                    nodeCount++;
                                }
                                else
                                {
                                    dstMusic += dm[0];
                                }
                            }
                        }
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                throw new ConverterException(string.Format("升降号音符不匹配!\n请确认音符方向\n行:{0},音符:{1}", rowCount, nodeCount));
                //return null;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ConverterException("超出音域!");
                //return null;
            }
            if (CombineBracket)
            {
                dstMusic = dstMusic.Replace(") (", " ").Replace(")(", "").Replace("] [", " ").Replace("][", "");
            }
            return dstMusic.Trim();
        }


        private List<string> nodesList;
        private void InitNodeList()
        {
            nodesList = new List<string>();
            List<string> singleOct = new List<string>();
            for (int i = 1; i < 8; i++)
            {
                foreach (string j in new string[] { "", sharpMark[0].ToString() })
                {
                    if (!((i == 3 || i == 7) && j == sharpMark))
                    {
                        if (MarkOnLeft)
                        {
                            singleOct.Add(j + i.ToString());
                        }
                        else
                        {
                            singleOct.Add(i.ToString() + j);
                        }
                    }
                }
            }
            string mark1;
            string mark2;
            int octDiv;
            for (int i = 1; i < 10; i++)
            {
                mark1 = "(";
                mark2 = ")";
                octDiv = i - 5;
                if (octDiv > 0)
                {
                    mark1 = "[";
                    mark2 = "]";
                }
                foreach (string node in singleOct)
                {
                    nodesList.Add(GetRepeatStr(mark1, Math.Abs(octDiv)) + node + GetRepeatStr(mark2, Math.Abs(octDiv)));
                }
            }
        }

        private string GetRepeatStr(string s, int repeat)
        {
            string rs = "";

            for (int i = 0; i < repeat; i++)
            {
                rs += s;
            }
            return rs;
        }

    }
}
