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
            var keys = from a in collection where a.Kind == Part.KEYLETTER select a;
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
                case '#':
                    ReadOperator();
                    break;
                case '$':
                    Pos.Position++;
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

        private void ReadKey()
        {
            while (char.IsLetterOrDigit(Current) && Current != ':')
                Pos.Position++;
            string key = Content.Substring(Pos.StartPosition, EndPositionOrLen);
            Value = key;
            PartKind = Part.KEYLETTER;
        }

        private void ReadOperator()
        {
            if (Current is ':')
                PartKind = Part.COLON;
            else if (Current is '#')
                PartKind = Part.SHARP;
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
            bool escape = false;
            string value = "";
            Pos.Position++;
            while (Current != '"' || escape == true)
            {
                if (escape)
                {
                    escape = false;
                    switch (Current)
                    {
                        case 'b':
                            value += '\b';
                            goto end;
                        case 'f':
                            value += '\f';
                            goto end;
                        case 'n':
                            value += '\n';
                            goto end;
                        case 'r':
                            value += '\r';
                            goto end;
                        case 't':
                            value += '\t';
                            goto end;
                        case 'u':
                            string unicode_value = "";
                            for (var i = 0; i < 4; i++)
                            {
                                Pos.Position++;
                                unicode_value += Current;
                            }
                            value += (char)Convert.ToInt32(unicode_value, 16);
                            goto end;
                        case '"':
                            value += '\"';
                            goto end;
                        case '\\':
                            value += '\\';
                            goto end;
                        default:
                            Errors.Add($"Unidentified escape sequence \\{Current} at position {Pos.Position}.");
                            break;
                    }
                }
                if (Current == '\\')
                {
                    escape = true;
                    goto end;
                }

                value += Current;

                end:
                Pos.Position++;
            }
            Pos.Position++;
            Value = value;
            PartKind = Part.STRING;
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
