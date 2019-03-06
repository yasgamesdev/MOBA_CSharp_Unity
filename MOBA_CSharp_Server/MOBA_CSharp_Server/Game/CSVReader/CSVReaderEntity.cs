using CsvHelper;
using MOBA_CSharp_Server.Library.ECS;
using System.Collections.Generic;
using System.IO;

namespace MOBA_CSharp_Server.Game
{
    public class CSVReaderEntity : Entity
    {
        CSVReader reader = new CSVReader();
        Dictionary<string, Dictionary<int, ExpTable>> expTables = new Dictionary<string, Dictionary<int, ExpTable>>();

        public CSVReaderEntity(Entity root) : base(root)
        {
            AddInheritedType(typeof(CSVReaderEntity));
        }

        public Dictionary<int, ExpTable> GetExpTable(UnitType type)
        {
            string path = @"CSV\ExpTables\" + type.ToString() + ".csv";
            if(!File.Exists(path))
            {
                path = @"CSV\ExpTables\Default.csv";
            }

            if (!expTables.ContainsKey(path))
            {
                using (var streamReader = new StreamReader(path))
                using (var csv = new CsvReader(streamReader))
                {
                    Dictionary<int, ExpTable> table = new Dictionary<int, ExpTable>();

                    var rows = csv.GetRecords<ExpTable>();
                    foreach (var r in rows)
                    {
                        table.Add(r.Level, r);
                    }

                    expTables.Add(path, table);
                }
            }

            return expTables[path];
        }

        public ItemTable GetItemTable(CombatType type)
        {
            return reader.GetItemTable(type);
        }
    }
}
