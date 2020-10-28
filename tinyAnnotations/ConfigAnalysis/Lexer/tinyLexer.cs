using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyAnnotations.Lexer
{
    public class tinyLexer
    {
        private PositionThat Pos;
        public HashSet<string> Errors;
        private string Content;
        private int ContentLen => Content.Length;
        private int EndPositionOrLen => Pos.Position - Pos.StartPosition;

        private Part PartKind;
        private object Value;
        public tinyLexer()
        {
            Pos = new PositionThat();
        }
        private char Current => Peek();
        private char Lookahead => Peek(1);
        private char Peek(int offset = 0)
        {
            int index = Pos.Position + offset;
            if (index >= ContentLen)
                return '\0';
            return Content[index];
        }
        public PartCollection GetParts(string content, out HashSet<string> errors)
        {
            Errors = new HashSet<string>();
            Content = content;
            PartCollection collection = new PartCollection();
            SyntaxPart part;
            for (int i = 0; i < ContentLen; i++)
            {
                do
                {
                    part = Get();
                    if(part.Kind == Part.InvalidPart)
                        Errors.Add($"Unknown character: <'{Current}'> on pos <{Pos.Position}>");
                    if (part.Kind != Part.Invisible && part.Kind != Part.InvalidPart && part.Kind != Part.EOF)
                        collection.Add(part);
                }
                while (part.Kind != Part.EOF);
            }
            var keys = from a in collection where a.Kind == Part.Key select a;
            foreach (var p in keys)
                if (keys.Where(a => a.Value.Equals(p.Value)).Count() > 1)
                    Errors.Add($"Key `{p.Value}` already exists!");

            PartKind = Part.InvalidPart;
            Pos = new PositionThat();
            Value = null;
            errors = Errors;
            return collection;
        }
        private SyntaxPart Get()
        {
            Pos.StartPosition = Pos.Position;
            PartKind = Part.InvalidPart;
            Value = null;
            switch (Current)
            {
                case '\0':
                    PartKind = Part.EOF;
                    break;
                case '"':
                    ReadString();
                    break;
                case ':':
                    ReadOperator();
                    break;
                case '#':
                    ReadComment();
                    break;
                case '$':
                    ReadKey();
                    break;
                default:
                    if (char.IsDigit(Current) || Current == '-' || Current == '+')
                        ReadNumber();
                    else if (char.IsLetter(Current))
                        ReadOther();
                    else if (char.IsWhiteSpace(Current))
                        ReadWhiteSpace();
                    else
                        Errors.Add($"Undefined symbol `{Current}`.");
                    break;
            }
            string Text = Content.Substring(Pos.StartPosition, EndPositionOrLen);
            return new SyntaxPart(PartKind, Text, new PositionThat(Pos.StartPosition, EndPositionOrLen), Value);
        }
        private void ReadComment()
        {
            Pos.Position++;
            bool flag = false;
            while (!flag)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        flag = true;
                        break;
                    default:
                        Pos.Position++;
                        break;
                }
            }
            PartKind = Part.Commentary;
        }
        private void ReadKey()
        {
            Pos.Position++;
            while (Current != ':')
                Pos.Position++;
            string key = Content.Substring(Pos.StartPosition + 1, EndPositionOrLen - 1);
            Value = key;
            PartKind = Part.Key;
        }

        private void ReadOperator()
        {
            if (Current is ':')
                PartKind = Part.COLON;
            Pos.Position++;
        }

        private void ReadNumber()
        {
            bool isDouble = false;
            while (char.IsDigit(Current) || Current == '-' || Current == '+')
            {
                if (Lookahead is '.' || Lookahead == ',')
                {
                    Pos.Position++;
                    isDouble = true;
                }
                Pos.Position++;
            }
            string str = Content.Substring(Pos.StartPosition, EndPositionOrLen);
            if(!isDouble)
            {
                if (int.TryParse(str, out int resultInt))
                    Value = resultInt;
                else if (long.TryParse(str, out long resultLong))
                    Value = resultLong;
                else
                    Errors.Add($"Invalid number type `{str}`");
            }
            else
            {
                if (!double.TryParse(str.Replace('.', ','), out double value))
                    Errors.Add($"Invalid double number `{str}`");
                Value = value;
            }
            PartKind = Part.NUMBER;
        }
        private void ReadString()
        {
            Pos.Position++;
            bool escape = false;
            StringBuilder value = new StringBuilder();
            while (!escape)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        escape = true;
                        Errors.Add($"Unterminated string `{Pos.StartPosition}`");
                        break;
                    case '"':
                        if(Lookahead == '"')
                        {
                            value.Append(Current);
                            Pos.Position += 2;
                        }
                        else
                        {
                            Pos.Position += 2;
                            escape = true;
                        }
                        break;
                    default:
                        value.Append(Current);
                        Pos.Position++;
                        break;
                }
            }
            PartKind = Part.STRING;
            Value = value.ToString();
        }
        private void ReadOther()
        {
            while (char.IsLetterOrDigit(Current))
                Pos.Position++;
            string other = Content.Substring(Pos.StartPosition, EndPositionOrLen);
            PartKind = Part.LETTERS;
            Value = other;

            if (bool.TryParse(other, out bool result))
            {
                Value = result;
                PartKind = Part.BOOL;
            }
            else if(other == "rand")
            {
                PartKind = Part.RANDOM;
            }
        }
        private void ReadWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
                Pos.Position++;
            PartKind = Part.Invisible;
        }
    }
}
