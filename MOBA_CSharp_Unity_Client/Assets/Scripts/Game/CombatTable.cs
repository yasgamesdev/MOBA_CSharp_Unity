using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MOBA_CSharp/Create CombatTable", fileName = "CombatTable")]
public class CombatTable : ScriptableObject
{
    private static readonly string RESOURCE_PATH = "CombatTable";

    private static CombatTable s_instance = null;
    public static CombatTable Instance
    {
        get
        {
            if (s_instance == null)
            {
                var asset = Resources.Load(RESOURCE_PATH) as CombatTable;
                if (asset == null)
                {
                    Debug.AssertFormat(false, "Missing CombatTable! path={0}", RESOURCE_PATH);
                    asset = CreateInstance<CombatTable>();
                }

                s_instance = asset;
            }

            return s_instance;
        }
    }

    [SerializeField]
    public CombatModel[] table;

    Dictionary<CombatType, CombatModel> cache = new Dictionary<CombatType, CombatModel>();

    public CombatModel GetCombatData(CombatType type)
    {
        if(cache.ContainsKey(type))
        {
            return cache[type];
        }
        else
        {
            CombatModel data = table.First(x => x.Type == type);
            cache.Add(data.Type, data);
            return data;
        }
    }
}