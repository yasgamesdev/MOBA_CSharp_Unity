public enum MessageType
{
    Connect,
    Disconnect,
    Timeout,
    Snapshot,
    Move,
    Attack,
    Recall,
    BuyItem,
    SellItem,
    UseItem,
    Change,
    Cast,
    Chat,
    Broadcast,
    Select,
    Lobby
}

public class MessageConfig
{
    public const int MESSAGE_LEN = 2;
}