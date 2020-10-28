using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tinyAnnotations.ConfigCreator
{
    public class IOConfig
    {
        private string RawContent;
        public IOConfig BuildAttribute(string key, object value = default, string desc = "")
        {
            RawContent += $"${key}:{FormatToTinyConfig(value)}{(string.IsNullOrEmpty(desc) ? "" : $" #{desc.Replace("\n", " ")}")}\n";
            return this;
        }
        public string Generate(string configName = "config.tiny")
        {
            File.WriteAllText(configName, RawContent);
            return RawContent;
        }
        private string FormatToTinyConfig(object value)
        {
            if (value is null)
                return "";
            else if (value is string)
                return $"\"{value}\"";
            else if (value is bool)
                return value.ToString().ToLower();
            else if (value is RandStructure)
                return (value as RandStructure).ToString();
            else
                return string.IsNullOrEmpty(value.ToString()) ? "0.0" : value.ToString();
        }
    }
}
