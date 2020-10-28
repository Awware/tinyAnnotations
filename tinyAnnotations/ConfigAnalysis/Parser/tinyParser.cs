using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tinyAnnotations.Lexer;

namespace tinyAnnotations.ConfigAnalysis.Parser
{
    public class tiny : Dictionary<string, object>
    {
        private int CurrentPos = 0;
        private PartCollection parts;
        private static Random r = new Random();
        public tiny(string config) : this(new tinyLexer().GetParts(config, out HashSet<string> Errors))
        {
            if (Errors.Count > 0) {
                foreach (var error in Errors)
                {
                    Console.WriteLine($"[ERROR] {error}");
                }
                return;
            }
        }
        public tiny(PartCollection collection)
        {
            parts = collection;
            Parse();
        }
        private void Parse()
        {
            for (CurrentPos = 0; CurrentPos < parts.Count; CurrentPos++)
            {
                var part = parts[CurrentPos];
                if (part.Kind == Part.COLON)
                    AddConfigKey();
            }
        }
        private void AddConfigKey()
        {
            string key = parts[CurrentPos - 1].Value.ToString().Remove(0, 1);
            SyntaxPart part = parts[CurrentPos + 1];
            object value = new object();
            if (part.Kind == Part.RANDOM)
            {
                int x = (int)parts[CurrentPos + 2].Value;
                int y = (int)parts[CurrentPos + 3].Value;
                value = r.Next(x, y);
            }
            else
                value = part.Value;
            Add(key, value);
            CurrentPos++;
        }
        public T CastAndGet<T>(string key)
        {
            if (ContainsKey(key))
                return (T)this[key];
            return default;
        }
    }
}
