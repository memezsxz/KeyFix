using System;

public class DebugCommand : DebugCommandBase
{
    private readonly Action _command;

    public DebugCommand(string commandId, string commandDescription, string commandFormat, Action command) : base(
        commandId, commandDescription, commandFormat)
    {
        _command = command;
    }

    public void Invoke()
    {
        _command.Invoke();
    }
}