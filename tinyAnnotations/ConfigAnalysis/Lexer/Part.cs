using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyAnnotations.Lexer
{
    public enum Part
    {
        NUMBER,
        STRING,
        BOOL,
        NIL,

        Key,
        COLON,
        Commentary,
        LETTERS,
        RANDOM,

        Invisible,
        InvalidPart,
        EOF
    }
}
