using System.Collections.Generic;
using tinyAnnotations.Lexer;

namespace tinyAnnotations
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
