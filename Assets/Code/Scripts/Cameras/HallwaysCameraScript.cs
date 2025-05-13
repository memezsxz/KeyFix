using UnityEngine;

public class HallwaysCameraScript : MonoBehaviour
{
    [SerializeField] private Camera vcCamera;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("HallwaysCameraScript started!");
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Camera view on collided!");

        if (collision.gameObject.CompareTag("Player"))
            // Assuming you have a method to change the camera view
            ChangeCameraView();
    }

    private void onTriggerEnter(Collider other)
    {
        Debug.Log("Camera view on triggered!");

        if (other.CompareTag("Player"))
            // Assuming you have a method to change the camera view
            ChangeCameraView();
    }

    private void ChangeCameraView()
    {
        gameObject.SetActive(false);
        vcCamera.gameObject.SetActive(true);
        Debug.Log("Camera view changed!");
    }
}