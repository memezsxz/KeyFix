using System;
using UnityEngine;

namespace Code.Scripts.Managers
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        private DebugController _debugController;
        public static event Action<GameState> OnBeforeGameStateChanged;
        public static event Action<GameState> OnAfterGameStateChanged;

        private GameState State { get; set; }

        void Start()
        {
            _debugController = GameObject.FindGameObjectWithTag("DebugController").GetComponent<DebugController>();
            GameManager.Instance.AddDebugCommand(new DebugCommand("gm_test", "testing from the game manager", "",
                () => Debug.Log("working in game manager"))); // command format hint in case of args
            ChangeState(GameState.Initial);
        }

        /// <summary>
        /// Add a debug command to the debugger
        /// </summary>
        /// <param name="command">The command to be added</param>
        /// <code>AddDebugCommand(
        ///      new DebugCommand(
        ///      "delete_all_enemies",
        ///      "deletes all enemies",
        ///      "",
        ///      () => { print("deleted all"); })
        /// );</code>
        public void AddDebugCommand(DebugCommand command)
        {
            if (_debugController != null)
            {
                _debugController.RegisterCommand(command);
                // Debug.Log($"Command {command.CommandId} added."); // // for testing only
            }
            else
            {
                // Debug.LogWarning("No debug controller found."); // for testing only
            }
        }

        /// <summary>
        /// Change the state of the game play
        /// </summary>
        /// <param name="newState">The new state</param>
        /// <exception cref="ArgumentOutOfRangeException">If the new state is not valid</exception>
        public void ChangeState(GameState newState)
        {
            if (State == newState) return;

            OnBeforeGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Initial:
                    HandelInitialState();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }

            State = newState;

            OnAfterGameStateChanged?.Invoke(newState);
        }

        // TODO Maryam: Make sure all scripts unsubscribe to events when they are done with them, to avoid memory leaks // in the OnDestroy method or when the state is never coming back to 
        private void HandelInitialState()
        {
        }
    }


    public enum GameState
    {
        /// <summary>
        /// load the data on OnBeforeGameStateChanged command
        /// </summary>
        Initial
    }
}