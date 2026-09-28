using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Tokenizer
{
    // Предложение - упорядоченная коллекция слов и знаков препинания.
    public class Sentence
    {
        [XmlElement("Token")]
        public List<Token> Tokens { get; set; } = new List<Token>();

        [XmlIgnore]
        public IEnumerable<Word> Words => Tokens.OfType<Word>();

        [XmlIgnore]
        public int WordCount => Words.Count();

        [XmlIgnore]
        public int Length => ToString().Length;

        [XmlIgnore]
        public bool IsQuestion => ToString().TrimEnd().EndsWith("?");

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var t in Tokens)
            {
                if (t is Word && sb.Length > 0)
                    sb.Append(' ');
                sb.Append(t.Text);
            }
            return sb.ToString();
        }
    }
}
