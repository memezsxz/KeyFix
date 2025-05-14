/// <summary>
/// Represents the base structure of a debug command,
/// including its identifier, description, and format string.
/// </summary>
public class DebugCommandBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DebugCommandBase"/> class.
    /// </summary>
    /// <param name="commandId">The unique ID used to invoke the command.</param>
    /// <param name="commandDescription">A short description of what the command does.</param>
    /// <param name="commandFormat">A string showing the correct format or usage of the command.</param>
    protected DebugCommandBase(string commandId, string commandDescription, string commandFormat)
    {
        CommandId = commandId;
        CommandDescription = commandDescription;
        CommandFormat = commandFormat;
    }

    /// <summary>
    /// Gets the unique identifier used to invoke the command from the debug console.
    /// </summary>
    public string CommandId { get; }

    /// <summary>
    /// Gets a brief explanation of what this command does.
    /// </summary>
    public string CommandDescription { get; }

    /// <summary>
    /// Gets an optional example of how to correctly use the command, including any expected arguments.
    /// </summary>
    public string CommandFormat { get; }
}