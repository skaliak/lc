using System;
using System.Collections.Generic;
using System.Linq;

namespace justify
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var words = new[]{"This", "is", "an", "example", "of", "text", "justification."};
            int maxWidth = 16;

            var justifier = new Justifier(words, maxWidth);

            var result = justifier.Justify();

            foreach(var line in result)
            {
                System.Console.WriteLine(line);
            }

            words = new[]{"Science","is","what","we","understand","well","enough","to","explain","to","a","computer.","Art","is","everything","else","we","do"};
            maxWidth = 20;

            justifier = new Justifier(words, maxWidth);
            result = justifier.Justify();
            System.Console.WriteLine();
            foreach(var line in result)
            {
                System.Console.WriteLine(line);
            }

            words = new[]{"What","must","be","acknowledgment","shall","be"};
            maxWidth = 16;

            justifier = new Justifier(words, maxWidth);
            result = justifier.Justify();
            System.Console.WriteLine();
            foreach(var line in result)
            {
                System.Console.WriteLine(line);
            }
        }
    }

    public class Justifier
    {
        public Queue<string> SourceWords { get; }
        public int MaxWidth { get; }
        public Justifier(IEnumerable<string> words, int maxWidth)
        {
            SourceWords = new Queue<string>(words);
            MaxWidth = maxWidth;
        }

        public IEnumerable<string> Justify()
        {
            string lastWord = GetNextWord();
            while (SourceWords.Count > 0)
            {
                var line = new List<string>();
                while(lastWord != null && !IsOverWidth(line, lastWord))
                {
                    line.Add(lastWord);
                    lastWord = GetNextWord();
                }

                if (lastWord == null)
                    yield return RenderLine(line, true);  //we reached the last line
                else
                    yield return RenderLine(line, false);
            }
            if (lastWord != null)
            {
                yield return RenderLine(new List<string>{lastWord}, true);
            }
        }

        public string RenderLine(List<string> line, bool isLastLine)
        {
            if(isLastLine)
            {
                var str = string.Join(' ', line);
                var diff = MaxWidth - str.Length;
                var pad = "";
                if(diff > 0) 
                {
                    pad = string.Concat(Enumerable.Repeat(" ", diff));
                }
                return str + pad;
            }

            int padding = CalculatePadding(line);
            var padded = AddPadding(line, padding);
            return string.Concat(padded);
        }

        private int CalculatePadding(List<string> line)
        {
            if (line.Count == 1)
            {
                return MaxWidth - line[0].Length;
            }

            return MaxWidth - GetWidth(line, false);
        }

        public static IEnumerable<string> AddPadding(IEnumerable<string> list, int width)
        {
            var outList = list.ToList();
            string lastWord = null;
            if(outList.Count > 1)
            {
                lastWord = outList.Last();
                outList.RemoveAt(outList.Count - 1);
            }
            int idx = 0;
            for (int i = 0; i < width; i++)
            {
                outList[idx] = outList[idx] + " ";
                idx++;
                if (idx == outList.Count)
                    idx = 0;
            }
            if (lastWord != null)
            {
                outList.Add(lastWord);
            }
            return outList;
        }

        public bool IsOverWidth(IEnumerable<string> list, string lastWord)
        {
            var listTotal = GetWidth(list, lastWord);
            return listTotal > MaxWidth;
        }

        public static int GetWidth(IEnumerable<string> list, string lastWord)
        {
            var listTotal = GetWidth(list, true);
            return listTotal + lastWord.Length;
        }

        public static int GetWidth(IEnumerable<string> list, bool withSpace)
        {
            int extra = withSpace ? 1 : 0;
            return list.Aggregate(0, (acc, w) => acc + w.Length + extra);
        }

        public string GetNextWord()
        {
            if(SourceWords.Count > 0)
                return SourceWords.Dequeue();
            
            return null;
        }
    }
}
