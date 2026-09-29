using System.Xml.Serialization;

namespace Tokenizer
{
    // Базовый токен. XmlInclude нужен, чтобы сериализатор знал про наследников.
    [XmlInclude(typeof(Word))]
    [XmlInclude(typeof(Punctuation))]
    public abstract class Token
    {
        public string Text { get; set; }
        public int LineNumber { get; set; }

        public Token() { }
        public Token(string text) { Text = text; }

        public override string ToString() => Text;
    }

    public class Word : Token
    {
        public Word() { }
        public Word(string text) : base(text) { }

        [XmlIgnore]
        public int Length => Text.Length;
    }

    public class Punctuation : Token
    {
        public Punctuation() { }
        public Punctuation(string text) : base(text) { }
    }
}
