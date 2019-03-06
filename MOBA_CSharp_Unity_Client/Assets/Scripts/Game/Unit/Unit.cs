using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] Transform modelRoot;

    //HealthBar
    [SerializeField] GameObject championHealthBar, generalHealthBar;
    GameObject healthBarInstance;

    //Minimap
    [SerializeField] GameObject championDot, generalDot;
    GameObject dotInstance;

    public IGetUnitInfo Info { get; private set; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        Destroy(healthBarInstance);
        Destroy(dotInstance);
    }

    public void Init(IGetUnitInfo info)
    {
        Info = info;
        UnitType unitType = info.GetUnitType();

        Instantiate(UnitTable.Instance.GetUnitModel(unitType).Model, modelRoot);

        //Movement
        GetComponent<Movement>().Init(new Vector3(info.GetXPos(), info.GetYPos(), info.GetZPos()), info.GetRotation());

        //Animation
        IGetAnimInfo animInfo = info as IGetAnimInfo;
        if(animInfo != null)
        {
            GetComponent<Animation>().Init(animInfo.GetAnimType(), animInfo.GetSpeedRate(), animInfo.GetPlayTime());
        }

        //HealthBar
        if (UnitType.Core <= unitType && unitType <= UnitType.Tower)
        {
            BuildingObj obj = info as BuildingObj;

            healthBarInstance = Instantiate(generalHealthBar, GameObject.Find("HealthBars").transform);
            healthBarInstance.GetComponent<GeneralHealthBar>().SetData(new Vector3(transform.position.x, 2.6f, transform.position.z), obj.MaxHP, obj.CurHP, obj.Team);
        }
        else if (UnitType.TowerBullet <= unitType && unitType <= UnitType.PressurisedSteam)
        {
            ActorObj obj = info as ActorObj;
        }
        else if(UnitType.Minion <= unitType && unitType <= UnitType.UltraMonster)
        {
            UnitObj obj = info as UnitObj;

            healthBarInstance = Instantiate(generalHealthBar, GameObject.Find("HealthBars").transform);
            healthBarInstance.GetComponent<GeneralHealthBar>().SetData(new Vector3(transform.position.x, 2.0f, transform.position.z), obj.MaxHP, obj.CurHP, obj.Team);
        }
        else
        {
            ChampionObj obj = info as ChampionObj;

            healthBarInstance = Instantiate(championHealthBar, GameObject.Find("HealthBars").transform);
            SetChampionHealthBarData(obj);
        }

        //Minimap
        if(unitType < UnitType.HatsuneMiku)
        {
            dotInstance = Instantiate(generalDot, GameObject.Find("MinimapUI").transform);
            dotInstance.GetComponent<GeneralDot>().SetData(info.GetTeam(), new Vector2(info.GetXPos(), info.GetZPos()));
        }
        else
        {
            dotInstance = Instantiate(championDot, GameObject.Find("MinimapUI").transform);
            dotInstance.GetComponent<ChampionDot>().SetData(info.GetUnitType(), info.GetTeam(), new Vector2(info.GetXPos(), info.GetZPos()));
        }
    }

    public void UpdateData(IGetUnitInfo info)
    {
        Info = info;
        UnitType unitType = info.GetUnitType();

        //Movement
        GetComponent<Movement>().SetPosition(new Vector3(info.GetXPos(), info.GetYPos(), info.GetZPos()), info.GetRotation(), info.GetWarped());

        //Animation
        IGetAnimInfo animInfo = info as IGetAnimInfo;
        if (animInfo != null)
        {
            GetComponent<Animation>().SetAnime(animInfo.GetAnimType(), animInfo.GetSpeedRate(), animInfo.GetPlayTime());
        }

        if (UnitType.Core <= unitType && unitType <= UnitType.Tower)
        {
            BuildingObj obj = info as BuildingObj;
            healthBarInstance.GetComponent<GeneralHealthBar>().SetData(new Vector3(transform.position.x, 2.6f, transform.position.z), obj.MaxHP, obj.CurHP, obj.Team);
        }
        else if (UnitType.TowerBullet <= unitType && unitType <= UnitType.PressurisedSteam)
        {
            ActorObj obj = info as ActorObj;
        }
        else if (UnitType.Minion <= unitType && unitType <= UnitType.UltraMonster)
        {
            UnitObj obj = info as UnitObj;
            healthBarInstance.GetComponent<GeneralHealthBar>().SetData(new Vector3(transform.position.x, 2.0f, transform.position.z), obj.MaxHP, obj.CurHP, obj.Team);
        }
        else
        {
            ChampionObj obj = info as ChampionObj;
            SetChampionHealthBarData(obj);
        }

        //Minimap
        if (unitType < UnitType.HatsuneMiku)
        {
            dotInstance.GetComponent<GeneralDot>().SetData(info.GetTeam(), new Vector2(info.GetXPos(), info.GetZPos()));
        }
        else
        {
            dotInstance.GetComponent<ChampionDot>().SetData(info.GetUnitType(), info.GetTeam(), new Vector2(info.GetXPos(), info.GetZPos()));
        }

        //Victory
        if(unitType == UnitType.Core && info.GetCurHP() <= 0)
        {
            GameObject.Find("VictoryUI").GetComponent<VictoryUI>().SetVictory(info.GetTeam());
        }
    }

    void SetChampionHealthBarData(ChampionObj championObj)
    {
        healthBarInstance.GetComponent<ChampionHealthBar>().SetData(new Vector3(transform.position.x, 2.0f, transform.position.z), championObj.DisplayName, championObj.Level, championObj.MaxHP, championObj.CurHP, championObj.MaxMP, championObj.CurMP, championObj.Team);
    }
}
