using Code.Scripts.Managers;
using UnityEngine;

[RequireComponent(typeof(BulbColorController))]
public class LightBoard : InteractableBase
{
    [SerializeField] private MeshRenderer bulbRenderer;
    [SerializeField] private Material bulbRedMaterial;
    [SerializeField] private GameObject canvas;

    private BulbColorController bulbColorController;
    private bool isActive;

    private void Awake()
    {
        bulbColorController = gameObject.GetComponent<BulbColorController>();
        if (canvas != null) canvas.SetActive(false); // Always hide the canvas initially
    }

    private void Start()
    {
        InteractMessage = "E";
    }

    public override void Interact()
    {
        if (bulbColorController.IsDone)
        {
            canvas.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
            return;
        }

        isActive = canvas.gameObject.activeSelf;

        if (!isActive)
        {
            canvas.SetActive(true);
            bulbColorController.ShowArrowChallenge();
            GameManager.Instance.TogglePlayerMovement(false);
        }
        else
        {
            canvas.SetActive(false);
            GameManager.Instance.TogglePlayerMovement(true);
        }
    }

    public void ToggleCanvas(bool value)
    {
        bulbColorController.enabled = value;
        // Do NOT enable the canvas based on BulbColorController directly
        if (!value)
            // If we are disabling, make sure canvas is hidden too
            if (canvas != null)
                canvas.SetActive(false);
    }
}