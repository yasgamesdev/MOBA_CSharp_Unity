using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using MessagePack;

public class ClientUI : MonoBehaviour
{
    [SerializeField] GameObject clientUINodePrefab;
    [SerializeField] RectTransform blueTeamTransform, redTeamTransform;
    List<GameObject> blueClientNodeInstances = new List<GameObject>();
    List<GameObject> redClientNodeInstances = new List<GameObject>();

    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_Dropdown teamDropdown, unitTypeDropdown;
    [SerializeField] Button changeButton;

    Network network;

    void Start()
    {
        network = GameObject.Find("Network").GetComponent<Network>();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
        {
            if (type >= UnitType.HatsuneMiku)
            {
                options.Add(new TMP_Dropdown.OptionData(type.ToString()));
            }
        }
        unitTypeDropdown.options = options;

        SnapshotObj snapshotObj = GameObject.Find("UnitManager").GetComponent<UnitManager>().GetSnapshotObj();
        if(snapshotObj != null)
        {
            ChampionObj championObj = snapshotObj.ChampionObjs.ToList().FirstOrDefault(x => x.UnitID == snapshotObj.PlayerObj.UnitID);
            if(championObj != null)
            {
                teamDropdown.value = teamDropdown.options.FindIndex(x => x.text == championObj.Team.ToString());
                nameInputField.text = championObj.DisplayName;
                unitTypeDropdown.value = unitTypeDropdown.options.FindIndex(x => x.text == championObj.Type.ToString());
            }
        }
    }

    public void SetData(ClientObj[] clientObjs)
    {
        int blueCount = clientObjs.ToList().Count(x => x.Team == Team.Blue);
        int redCount = clientObjs.Length - blueCount;

        int deleteBlueCount = blueClientNodeInstances.Count - blueCount;
        int deleteRedCount = redClientNodeInstances.Count - redCount;

        for(int i=0; i<deleteBlueCount; i++)
        {
            GameObject blueClientNodeInstance = blueClientNodeInstances.Last();
            Destroy(blueClientNodeInstance);
            blueClientNodeInstances.Remove(blueClientNodeInstance);
        }

        for (int i = 0; i < deleteRedCount; i++)
        {
            GameObject redClientNodeInstance = redClientNodeInstances.Last();
            Destroy(redClientNodeInstance);
            blueClientNodeInstances.Remove(redClientNodeInstance);
        }

        int addBlueCount = blueCount - blueClientNodeInstances.Count;
        int addRedCount = redCount - redClientNodeInstances.Count;

        for(int i=0; i<addBlueCount; i++)
        {
            GameObject instance = Instantiate(clientUINodePrefab, blueTeamTransform);
            blueClientNodeInstances.Add(instance);
        }

        for(int i=0; i<addRedCount; i++)
        {
            GameObject instance = Instantiate(clientUINodePrefab, redTeamTransform);
            redClientNodeInstances.Add(instance);
        }

        int blueIndex = 0;
        int redIndex = 0;
        foreach(ClientObj clientObj in clientObjs)
        {
            if(clientObj.Team == Team.Blue)
            {
                blueClientNodeInstances[blueIndex].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                blueClientNodeInstances[blueIndex].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                blueClientNodeInstances[blueIndex].GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                blueClientNodeInstances[blueIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -blueIndex * 100);
                blueClientNodeInstances[blueIndex].GetComponent<ClientUINode>().SetData(clientObj.Type, clientObj.Team, clientObj.Name, clientObj.Level);

                blueIndex++;
            }
            else
            {
                redClientNodeInstances[redIndex].GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                redClientNodeInstances[redIndex].GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                redClientNodeInstances[redIndex].GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                redClientNodeInstances[redIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -redIndex * 100);
                redClientNodeInstances[redIndex].GetComponent<ClientUINode>().SetData(clientObj.Type, clientObj.Team, clientObj.Name, clientObj.Level);

                redIndex++;
            }
        }
    }

    public void ChangeButtonClicked()
    {
        Team team;
        Enum.TryParse(teamDropdown.options[teamDropdown.value].text, out team);
        UnitType unitType;
        Enum.TryParse(unitTypeDropdown.options[unitTypeDropdown.value].text, out unitType);

        ChangeObj changeObj = new ChangeObj()
        {
            Team = team,
            Name = nameInputField.text,
            Type = unitType
        };

        network.Send(MessageType.Change, MessagePackSerializer.Serialize(changeObj), ENet.PacketFlags.Reliable);
    }
}
