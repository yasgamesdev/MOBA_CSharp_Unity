using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MOBA_CSharp_Client.ClientNetwork;
using MessagePack;

public class ChatUI : MonoBehaviour
{
    ClientNetwork client;

    private string Logs = "";
    private string oldLogs = "";
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] Text textLog;
    [SerializeField] InputField inputField;
    [SerializeField] Text inputText;

    bool allMode = true;

    void Start()
    {
        
    }

    public void SetClientNetwork(ClientNetwork client)
    {
        this.client = client;
    }

    // Update is called once per frame
    void Update()
    {
        if (scrollRect != null && Logs != oldLogs)
        {
            textLog.text = Logs;
            StartCoroutine(DelayMethod(5, () =>
            {
                scrollRect.verticalNormalizedPosition = 0;
            }));
            oldLogs = Logs;
        }

        if (inputText.gameObject.activeInHierarchy  && Input.GetKeyDown(KeyCode.Return))
        {
            Send();
        }
    }

    public bool InputTextActiveInHierarchy()
    {
        return inputField.isFocused;
    }

    private IEnumerator DelayMethod(int delayFrameCount, Action action)
    {
        for (var i = 0; i < delayFrameCount; i++)
        {
            yield return null;
        }
        action();
    }

    public void Log(Team team, string logText)
    {
        if(team == Team.Blue)
        {
            Logs += ("<color=blue>" + logText + "</color>\n");
        }
        else if(team == Team.Red)
        {
            Logs += ("<color=red>" + logText + "</color>\n");
        }
        else
        {
            Logs += (logText + "\n");
        }
    }

    public void Send()
    {
        if(inputField.text != "" && client != null)
        {
            if(inputField.text.StartsWith("/all "))
            {
                allMode = true;
                client.Send(MessageType.Chat, MessagePackSerializer.Serialize(inputField.text), ENet.PacketFlags.Reliable);
            }
            else if(inputField.text.StartsWith("/team "))
            {
                allMode = false;
                client.Send(MessageType.Chat, MessagePackSerializer.Serialize(inputField.text), ENet.PacketFlags.Reliable);
            }
            else
            {
                client.Send(MessageType.Chat, MessagePackSerializer.Serialize((allMode ? "/all " : "/team ") + inputField.text), ENet.PacketFlags.Reliable);
            }

            inputField.text = "";
        }
    }
}
