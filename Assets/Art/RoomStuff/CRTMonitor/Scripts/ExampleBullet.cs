using UnityEngine;

public class ExampleBullet : MonoBehaviour
{
    public GameObject bullet;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            bullet.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 2, ForceMode.Impulse);
            bullet.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}