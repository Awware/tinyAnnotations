using System.Collections.Generic;
using TinyAnnotations.Lexer;

namespace TinyAnnotations
{
    public class PartCollection : List<SyntaxPart>
    {
        public PartCollection(List<SyntaxPart> list) : base(list)
        {

        }
        public PartCollection()
        {

        }
        public PartCollection Copy()
        {
            return new PartCollection(this);
        }
    }
}
