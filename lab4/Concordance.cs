using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tokenizer
{
    public class ConcordanceEntry
    {
        public string Word;
        public int Count;
        public SortedSet<int> Lines = new SortedSet<int>();
    }

    // Конкорданс: слова по алфавиту, частота и номера строк вхождения.
    public class Concordance
    {
        public SortedDictionary<string, ConcordanceEntry> Entries =
            new SortedDictionary<string, ConcordanceEntry>(StringComparer.Ordinal);

        public static Concordance Build(Text text)
        {
            var c = new Concordance();
            foreach (var s in text.Sentences)
            {
                foreach (var w in s.Words)
                {
                    string key = w.Text.ToLower();
                    if (!c.Entries.TryGetValue(key, out var entry))
                    {
                        entry = new ConcordanceEntry { Word = key };
                        c.Entries[key] = entry;
                    }
                    entry.Count++;
                    entry.Lines.Add(w.LineNumber);
                }
            }
            return c;
        }

        private static string FormatLine(ConcordanceEntry e)
        {
            string dots = new string('.', Math.Max(1, 25 - e.Word.Length));
            string lines = string.Join(" ", e.Lines);
            return $"{e.Word}{dots}{e.Count}: {lines}";
        }

        public void Print()
        {
            foreach (var e in Entries.Values)
                Console.WriteLine(FormatLine(e));
        }

        public void ExportToFile(string path)
        {
            using var w = new StreamWriter(path);
            foreach (var e in Entries.Values)
                w.WriteLine(FormatLine(e));
        }
    }
}
