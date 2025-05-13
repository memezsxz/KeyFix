using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class Door : InteractableBase
    {
        public enum DoorType
        {
            Enter,
            Exit
        }

        [SerializeField] private GameManager.Scenes _scene;
        // [SerializeField] private bool isGKey;
        [SerializeField] private DoorType _doorType;


        [SerializeField] private Vector3 forwardPosition;
        private readonly float playerPush = 5;
        public GameManager.Scenes Scene => _scene;

        public DoorType type => _doorType;

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
            // if (isGKey) GameManager.Instance.HandleSceneLoad(_scene, GameManager.GameState.CutScene);
            // else 
            GameManager.Instance.HandleSceneLoad(_scene);
        }

        public void HandleExitDoor()
        {
            var pos = transform.position + forwardPosition * playerPush;
            GameManager.Instance.MovePlayerTo(pos, transform.rotation);
        }
    }
}