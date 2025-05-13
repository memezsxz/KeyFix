public class DebugCommandBase
{
    protected DebugCommandBase(string commandId, string commandDescription, string commandFormat)
    {
        CommandId = commandId;
        CommandDescription = commandDescription;
        CommandFormat = commandFormat;
    }

    public string CommandId { get; }

    public string CommandDescription { get; }
    public string CommandFormat { get; }
}