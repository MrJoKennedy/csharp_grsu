using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Tokenizer
{
    // Текст - коллекция предложений.
    [XmlRoot("Text")]
    public class Text
    {
        public List<Sentence> Sentences { get; set; } = new List<Sentence>();

        private const string Vowels = "aeiouyаеёиоуыэюя";

        public List<Sentence> SortByWordCount()
            => Sentences.OrderBy(s => s.WordCount).ToList();

        public List<Sentence> SortByLength()
            => Sentences.OrderBy(s => s.Length).ToList();

        // Слова заданной длины во всех вопросительных предложениях, без повторов.
        public List<string> FindWordsInQuestions(int length)
        {
            var result = new List<string>();
            foreach (var s in Sentences.Where(s => s.IsQuestion))
                foreach (var w in s.Words.Where(w => w.Length == length))
                    if (!result.Contains(w.Text, StringComparer.OrdinalIgnoreCase))
                        result.Add(w.Text);
            return result;
        }

        // Удалить слова заданной длины, начинающиеся с согласной буквы.
        public int RemoveWordsStartingWithConsonant(int length)
        {
            int removed = 0;
            foreach (var s in Sentences)
                removed += s.Tokens.RemoveAll(t =>
                    t is Word w && w.Length == length && !Vowels.Contains(char.ToLower(w.Text[0])));
            return removed;
        }

        // Заменить слова заданной длины в конкретном предложении на подстроку.
        public bool ReplaceWordsInSentence(int sentenceIndex, int length, string replacement)
        {
            if (sentenceIndex < 0 || sentenceIndex >= Sentences.Count) return false;
            var tokens = Sentences[sentenceIndex].Tokens;
            bool changed = false;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is Word w && w.Length == length)
                {
                    tokens[i] = new Word(replacement);
                    changed = true;
                }
            }
            return changed;
        }

        // Удалить стоп-слова (регистр не важен).
        public int RemoveStopWords(HashSet<string> stopWords)
        {
            int removed = 0;
            foreach (var s in Sentences)
                removed += s.Tokens.RemoveAll(t => t is Word w && stopWords.Contains(w.Text.ToLower()));
            return removed;
        }

        public void ExportToXml(string path)
        {
            var serializer = new XmlSerializer(typeof(Text));
            using var writer = new StreamWriter(path);
            serializer.Serialize(writer, this);
        }
    }
}
