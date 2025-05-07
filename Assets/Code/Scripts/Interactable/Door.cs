using System.Linq;
using Code.Scripts.Managers;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Code.Scripts.Interactable
{
    public class Door : InteractableBase
    {
        enum DoorType
        {
            Enter,
            Exit,
        }

        [SerializeField] GameManager.Scenes _scene;
        [SerializeField] private bool isGKey = false;
        [SerializeField] DoorType _doorType;


        [SerializeField] Vector3 forwardPosition;
        private float playerPush = 5;

        public override void Interact()
        {
            if (_doorType == DoorType.Enter) HandleEnterDoor();
            else HandleExitDoor(GameObject.FindGameObjectWithTag("Player").transform);
        }

        private void HandleEnterDoor()
        {
            SaveManager.Instance.GetCharacterData(CharacterType.Robot).HallwaysPosition =
                GameManager.Instance.GetPlayerTransform();
            
            if (isGKey) GameManager.Instance.HandleSceneLoad(_scene, GameManager.GameState.CutScene);
            else GameManager.Instance.HandleSceneLoad(_scene);
        }

        private void HandleExitDoor(Transform playerTransform)
        {
            gameObject.layer = LayerMask.NameToLayer("Default");

            GameManager.Instance.MovePlayerToExitDoorPosition(transform, forwardPosition * playerPush);
            // playerTransform.position = transform.position + (forwardPosition * playerPush);
        }
    }
}