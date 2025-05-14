using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public SoundData Sounds = new();
    public ProgressData Progress = new();
    public MetaData Meta = new();
    public GraphicData Graphics = new();

    public List<CharacterStateEntry> CharacterStates = new()
    {
        new CharacterStateEntry { Type = CharacterType.Robot, State = new PlayerStateData() }
    };
}


[Serializable]
public class SoundData
{
    public float SoundVolume = 1f;
    public float MusicVolume = 1f;
}

[Serializable]
public class GraphicData
{
    public int ResolutionWidth;
    public int ResolutionHeight;
    public bool Fullscreen = true;
    public string QualityName = "Medium";
}

[Serializable]
public class ProgressData
{
    public bool IsNewGame = true;
    public GameManager.Scenes CurrentScene = GameManager.Scenes.INCIDENT;
    public List<GameManager.Scenes> RepairedKeys = new();
    public SerializableInputBindingSet BindingOverrides = new();
    public int CollectablesCount;
    public int WRoomCount;
    public int ArrowRoomBoardIndex;
}

[Serializable]
public class PlayerStateData
{
    public Vector3 Position;
    public float Yaw; // The Y-axis rotation (Euler angle)
    public Vector3 HallwaysPosition;
    public Quaternion HallwaysRotation;
    public int LivesRemaining = 10; // lives till the player looses the level and the button is damaged puritanically 
}

[Serializable]
public enum CharacterType
{
    Robot,
    Robota
}

[Serializable]
public class MetaData
{
    public string SaveName = "Game1";
    public float PlayTimeSeconds;
    public int ScreenWidth;
    public int ScreenHeight;
    public DateTime LastSaveTime;
}

[Serializable]
public class CharacterStateEntry
{
    public CharacterType Type;
    public PlayerStateData State;
}