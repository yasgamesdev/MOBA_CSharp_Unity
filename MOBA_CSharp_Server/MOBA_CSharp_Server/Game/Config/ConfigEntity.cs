using Config;
using ECS;

namespace MOBA_CSharp_Server.Game
{
    public class ConfigEntity : Entity
    {
        readonly string path = "ServerConfig.txt";

        ConfigReader config;

        public ConfigEntity(RootEntity root) : base(root)
        {
            config = new ConfigReader(path);
        }

        public string GetString(string key)
        {
            return config.GetString(key);
        }

        public int GetInt(string key)
        {
            return config.GetInt(key);
        }

        public float GetFloat(string key)
        {
            return config.GetFloat(key);
        }

        public bool GetBool(string key)
        {
            return config.GetBool(key);
        }
    }
}
