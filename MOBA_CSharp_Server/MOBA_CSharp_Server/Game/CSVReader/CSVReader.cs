using CsvHelper;
using System.Collections.Generic;
using System.IO;

public class CSVReader
{
    Dictionary<CombatType, ItemTable> items;

    public ItemTable GetItemTable(CombatType type)
    {
        if (items == null)
        {
            using (var streamReader = new StreamReader(@"CSV\Items.csv"))
            using (var csv = new CsvReader(streamReader))
            {
                items = new Dictionary<CombatType, ItemTable>();

                var rows = csv.GetRecords<ItemTable>();
                foreach (var r in rows)
                {
                    items.Add(r.Type, r);
                }
            }
        }

        return items[type];
    }
}