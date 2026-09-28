using System.IO;
using System.Text.RegularExpressions;

namespace Tokenizer
{
    // Парсер 2: разбивает текст на токены через regex и собирает предложения.
    public class TextParser
    {
        private static readonly Regex TokenPattern =
            new Regex(@"[\p{L}\p{N}]+(?:['’][\p{L}]+)*|[^\s\p{L}\p{N}]", RegexOptions.Compiled);

        private static readonly string SentenceEnders = ".!?…";

        public Text Parse(string content)
        {
            var text = new Text();
            var sentence = new Sentence();

            foreach (Match m in TokenPattern.Matches(content))
            {
                string value = m.Value;
                bool isWord = char.IsLetterOrDigit(value[0]);
                Token token = isWord ? new Word(value) : new Punctuation(value);
                sentence.Tokens.Add(token);

                if (!isWord && SentenceEnders.Contains(value))
                {
                    text.Sentences.Add(sentence);
                    sentence = new Sentence();
                }
            }

            if (sentence.Tokens.Count > 0)
                text.Sentences.Add(sentence);

            return text;
        }

        public Text ParseFile(string path) => Parse(File.ReadAllText(path));
    }
}
