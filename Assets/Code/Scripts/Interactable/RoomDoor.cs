using Code.Scripts.Managers;

namespace Code.Scripts.Interactable
{
    public class RoomDoor : InteractableBase
    {
        private bool didInteract;

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