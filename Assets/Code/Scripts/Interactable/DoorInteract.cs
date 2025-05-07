using Code.Scripts.Managers;
using UnityEngine;


    public class DoorInteract : InteractableBase
    {
        public override void Interact()
        {
           GameManager.Instance.ChangeState(GameManager.GameState.Victory);
        }
    }
