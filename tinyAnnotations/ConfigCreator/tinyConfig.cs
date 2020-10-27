using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyAnnotations.ConfigAnalysis.Parser;

namespace TinyAnnotations.ConfigCreator
{
    public class tinyConfig
    {
        private tiny Parsed;
        public tinyConfig()
        {

        }
        public tinyConfig(string content)
        {
            Parsed = new tiny(content);
        }
        public object this[string key]
        {
            get => Parsed[key];
        }
    }
}
