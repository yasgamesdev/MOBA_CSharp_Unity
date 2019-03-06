using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MOBA_CSharp/Create UnitTable", fileName = "UnitTable")]
public class UnitTable : ScriptableObject
{
    private static readonly string RESOURCE_PATH = "UnitTable";

    private static UnitTable s_instance = null;
    public static UnitTable Instance
    {
        get
        {
            if (s_instance == null)
            {
                var asset = Resources.Load(RESOURCE_PATH) as UnitTable;
                if (asset == null)
                {
                    Debug.AssertFormat(false, "Missing CombatTable! path={0}", RESOURCE_PATH);
                    asset = CreateInstance<UnitTable>();
                }

                s_instance = asset;
            }

            return s_instance;
        }
    }

    [SerializeField]
    public UnitModel[] table;

    Dictionary<UnitType, UnitModel> cache = new Dictionary<UnitType, UnitModel>();

    public UnitModel GetUnitModel(UnitType type)
    {
        if(cache.ContainsKey(type))
        {
            return cache[type];
        }
        else
        {
            UnitModel data = table.First(x => x.Type == type);
            cache.Add(data.Type, data);
            return data;
        }
    }
}