using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontUI : MonoBehaviour
{
    enum FrontUIState
    {
        Close,
        ItemUIOpen,
        ClientUIOpen
    }
    FrontUIState state;

    [SerializeField] ItemUI itemUI;
    [SerializeField] ClientUI clientUI;
    // Start is called before the first frame update
    void Start()
    {
        itemUI.gameObject.SetActive(false);
        clientUI.gameObject.SetActive(false);
        state = FrontUIState.Close;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if (state == FrontUIState.Close)
            {
                clientUI.gameObject.SetActive(true);
                state = FrontUIState.ClientUIOpen;
            }
            else if (state == FrontUIState.ItemUIOpen)
            {
                itemUI.gameObject.SetActive(false);
                clientUI.gameObject.SetActive(true);
                state = FrontUIState.ClientUIOpen;
            }
            else
            {
                clientUI.gameObject.SetActive(false);
                state = FrontUIState.Close;
            }
        }
    }

    public void SetPlayerData(PlayerObj playerObj, ChampionObj championObj)
    {
        if(state == FrontUIState.ItemUIOpen)
        {
            itemUI.SetPlayerData(playerObj, championObj);
        }
        else if(state == FrontUIState.ClientUIOpen)
        {

        }
    }

    public void ClickItemUIButton()
    {
        if(state == FrontUIState.Close)
        {
            itemUI.gameObject.SetActive(true);
            state = FrontUIState.ItemUIOpen;
        }
        else if(state == FrontUIState.ItemUIOpen)
        {
            itemUI.gameObject.SetActive(false);
            state = FrontUIState.Close;
        }
        else
        {
            itemUI.gameObject.SetActive(true);
            clientUI.gameObject.SetActive(false);
            state = FrontUIState.ItemUIOpen;
        }
    }

    public void SetData(ClientObj[] clientObjs)
    {
        if(state == FrontUIState.ClientUIOpen)
        {
            clientUI.SetData(clientObjs);
        }
    }
}
