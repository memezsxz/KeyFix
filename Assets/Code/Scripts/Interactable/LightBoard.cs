using System;
using Code.Scripts.Managers;
using UnityEngine;


public class LightBoard : InteractableBase
{
    [SerializeField] MeshRenderer bulbRenderer; // for color changes
    [SerializeField] Material bulbRedMaterial;
    [SerializeField] GameObject canvas;
    [SerializeField] BulbColorController bulbColorController;

    private bool isActive = false;

    private void Start()
    {
        InteractMessage = "E";
    }

    public override void Interact()
    {
        isActive = canvas.gameObject.activeSelf;
        
        if (!isActive)
        {
            canvas.SetActive(true);
            bulbColorController.showArrowChalange();
            GameManager.Instance.TogglePlayerMovement(false);
        }
        else
        {
            canvas.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
        }
    }
}