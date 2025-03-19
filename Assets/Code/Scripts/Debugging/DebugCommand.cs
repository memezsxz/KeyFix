using System;

public class DebugCommand : DebugCommandBase
{
    private readonly Action _command;
    private readonly Action<string[]> _commandWithArgs;  // For commands with parameters
    private readonly bool _expectsArguments;

    public DebugCommand(string commandId, string commandDescription, string commandFormat, Action command) : base(
        commandId, commandDescription, commandFormat)
    {
        _command = command;
    }

    public DebugCommand(string commandId, string commandDescription, string commandFormat, Action<string[]> commandWithArgs)
        : base(commandId, commandDescription, commandFormat)
    {
        _commandWithArgs = commandWithArgs;
        _expectsArguments = true;
    }

    /// <summary>
    /// Invoke a command with or without arguments
    /// could be used for methods with arguments or to handel multiple input values at the same time
    /// </summary>
    /// <param name="args">array of string arguments</param>
    /// <exception cref="InvalidOperationException">throw when the command is not found</exception>
    /*  example in a Player script:
     GameManager.Instance.AddDebugCommand(new DebugCommand(
               "set_player_attributes",
               "Updates the player's attributes",
               "set_player_attributes <name> <health> <speed> <inventoryItem1> <inventoryItem2> ...",
               args =>
               {
                   if (args.Length < 3)
                   {
                       Debug.LogWarning("Usage: set_player_attributes <name> <health> <speed> <inventory items...>");
                       return;
                   }

                   string playerName = args[0];
                   if (!int.TryParse(args[1], out int health))
                   {
                       Debug.LogWarning("Invalid health value. It must be an integer.");
                       return;
                   }

                   if (!float.TryParse(args[2], out float speed))
                   {
                       Debug.LogWarning("Invalid speed value. It must be a float.");
                       return;
                   }

                   string[] inventoryItems = args.Length > 3 ? args[3..] : new string[0]; // Remaining items

                   UpdateAttributes(playerName, health, speed, inventoryItems);
               }));

           v = -1; // Ensure it only runs once
       }
       public void UpdateAttributes(string name, int health, float speed, string[] inventoryItems)
       {
           Name = name;
           Health = health;
           Speed = speed;
           Inventory = new List<string>(inventoryItems);

           Debug.Log($"Player Updated: Name={Name}, Health={Health}, Speed={Speed}, Inventory=[{string.Join(", ", Inventory)}]");
       }*/
    public void Invoke(params string[] args)
    {
        if (_expectsArguments && _commandWithArgs != null)
        {
            _commandWithArgs.Invoke(args);
        }
        else if (!_expectsArguments && _command != null)
        {
            _command.Invoke();
        }
        else
        {
            throw new InvalidOperationException($"Command {CommandId} was not set up correctly.");
        }
    }
}