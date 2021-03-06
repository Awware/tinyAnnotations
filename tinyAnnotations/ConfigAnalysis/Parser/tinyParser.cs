﻿using System;
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
        public tiny(string config)
        {
            tinyLexer lex = new tinyLexer();
            parts = lex.GetParts(config, out HashSet<string> Errors);
            if (Errors.Any()) {
                foreach (var error in Errors)
                    Console.WriteLine($"[ERROR] {error}");
                return;
            }
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
            string key = parts[CurrentPos - 1].Value.ToString();
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
        public bool ValidConfig(string importantKeys)
        {
            string[] keys = importantKeys.Split(';');
            foreach (string key in keys)
                if (!ContainsKey(key))
                    return false;
            return true;
        }
    }
}
