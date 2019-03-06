using MOBA_CSharp_Server.Library.DataReader;
using MOBA_CSharp_Server.Library.ECS;

namespace MOBA_CSharp_Server.Game
{
    public class DataReaderEntity : Entity
    {
        YAMLReader reader = new YAMLReader();

        public DataReaderEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(DataReaderEntity));
        }

        public YAMLObject GetYAMLObject(string path)
        {
            return reader.GetYAMLObject(path);
        }

        public YAMLObject GetYAMLObject(UnitType type)
        {
            return GetYAMLObject(@"YAML\Units\" + type.ToString() + ".yml");
        }

        public YAMLObject GetYAMLObject(CombatType type)
        {
            return GetYAMLObject(@"YAML\Combats\" + type.ToString() + ".yml");
        }
    }
}
