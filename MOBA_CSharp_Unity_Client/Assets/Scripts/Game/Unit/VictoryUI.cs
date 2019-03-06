using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class VictoryUI : MonoBehaviour
{
    [SerializeField] TMP_Text victoryText;
    bool activated = false;

    public void SetVictory(Team team)
    {
        if(!activated)
        {
            activated = true;
            victoryText.text = "Victory";
            if(team == Team.Blue)
            {
                victoryText.color = new Color(1, 0, 0);
            }
            else
            {
                victoryText.color = new Color(0, 0, 1);
            }
        }
    }
}
