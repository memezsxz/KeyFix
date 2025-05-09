using Code.Scripts.Managers;
using UnityEngine;

namespace Code.Scripts.Interactable
{
    public class RoomDoor : InteractableBase
    {
        bool didInteract = false;
        public override void Interact()
        {
        }

        public override void InteractH()
        {
            if (didInteract) return;
            didInteract = true;
            GameManager.Instance.HandleSceneLoad(GameManager.Scenes.HALLWAYS, GameManager.GameState.Playing);
        }
    }
}