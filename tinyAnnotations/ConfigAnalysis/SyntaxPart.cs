using tinyAnnotations.Lexer;

namespace tinyAnnotations
{
    public class SyntaxPart
    {
        public Part Kind { get; }
        public PositionThat Position { get; }
        public object Value { get; }
        public string Text { get; }
        public SyntaxPart(Part kind, string text, PositionThat position, object value)
        {
            Kind = kind;
            Position = position;
            Text = text ?? string.Empty;
#pragma warning disable CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
            Value = value;
#pragma warning restore CS8601 // Возможно, назначение-ссылка, допускающее значение NULL.
        }
        public override string ToString()
        {
            return Text + " is " + Kind;
        }
    }
}
