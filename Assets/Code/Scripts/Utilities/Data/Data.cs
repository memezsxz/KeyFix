using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public SettingsData Settings = new();
    public ProgressData Progress = new();
    public InputData Input = new();
    public MetaData Meta = new();

    public List<CharacterStateEntry> CharacterStates = new()
    {
        new CharacterStateEntry { Type = CharacterType.Robot, State = new PlayerStateData() }
    };
}


[Serializable]
public class SettingsData
{
    public bool Music = true;
    public bool Sound = true;
    public bool Vibrate = true;
    public float SoundVolume = 1.0f;
    public float MusicVolume = 1.0f;
}

[Serializable]
public class ProgressData
{
    public int CurrentScene;

    public string LastCheckpointId = ""; // TODO Maryam: see how will the checkpoint system work. // optional: for respawn

    public List<string> RepairedKeys = new(); // e.g. ["W", "A"]
    public List<string> DeadKeys = new(); // e.g. ["W", "A"]
}

[Serializable]
public class PlayerStateData
{
    public Vector3 Position;
    public float Yaw; // The Y-axis rotation (Euler angle)
    public int LivesRemaining = 10; // lives till the player looses the level and the button is damaged puritanically 
    public int HitsRemaining = 10; // hits till the player is reseted to the last checkpoint
    public PlayerBindingManage Bindings;
}

[Serializable]
public enum CharacterType
{
    Robot,
    Robota
}

[Serializable]
public class InputData
{
    public Dictionary<string, string> BindingOverrides = new(); // e.g. {"left", "<Keyboard>/a"}
}

[Serializable]
public class MetaData
{
    public string SaveName = "Game1";
    public float PlayTimeSeconds;
    public int ScreenWidth;
    public int ScreenHeight;
    public bool Fullscreen;
    public DateTime LastSaveTime;
}

[Serializable]
public class CharacterStateEntry
{
    public CharacterType Type;
    public PlayerStateData State;
}