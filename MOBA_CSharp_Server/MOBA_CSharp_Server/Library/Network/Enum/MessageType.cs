public enum MessageType
{
    Connect,
    Disconnect,
    Timeout,
    Snapshot,
    Move,
}

public class MessageConfig
{
    public const int MESSAGE_LEN = 2;
}