using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MOBA_CSharp_Server.Config
{
    public class ConfigReader
    {
        Dictionary<string, string> config = new Dictionary<string, string>();

        public ConfigReader(string path)
        {
            StreamReader reader = new StreamReader(path);

            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if(Regex.IsMatch(line, @"[^=]+=[^=]+"))
                {
                    string[] words = line.Split('=');
                    config.Add(words[0], words[1]);
                }
            }

            reader.Close();
        }

        public string GetString(string key)
        {
            return config[key];
        }

        public int GetInt(string key)
        {
            return int.Parse(config[key]);
        }

        public float GetFloat(string key)
        {
            return float.Parse(config[key]);
        }

        public bool GetBool(string key)
        {
            return bool.Parse(config[key]);
        }
    }
}
