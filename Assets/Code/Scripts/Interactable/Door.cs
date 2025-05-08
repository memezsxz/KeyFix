using System;
using System.Linq;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Code.Scripts.Interactable
{
    public class Door : InteractableBase
    {
       public enum DoorType
        {
            Enter,
            Exit,
        }

        [SerializeField] GameManager.Scenes _scene;
      public  GameManager.Scenes Scene => _scene;
        [SerializeField] private bool isGKey = false;
        [SerializeField] DoorType _doorType;

        public DoorType type => _doorType;
        

        [SerializeField] Vector3 forwardPosition;
        private float playerPush = 5;

        private void Start()
        {
            if (_doorType == DoorType.Exit) gameObject.layer = LayerMask.NameToLayer("Default");
        }

        public override void Interact()
        {
            if (_doorType == DoorType.Enter) HandleEnterDoor();
            else HandleExitDoor();
        }

        private void HandleEnterDoor()
        {
            SaveManager.Instance.SaveHallwayPosition();
            if (isGKey) GameManager.Instance.HandleSceneLoad(_scene, GameManager.GameState.CutScene);
            else GameManager.Instance.HandleSceneLoad(_scene);
        }

        public void HandleExitDoor()
        {
            var pos = transform.position + (forwardPosition * playerPush);
            GameManager.Instance.MovePlayerTo(pos, transform.rotation);
        }
    }
}