using System.Collections;
using UnityEngine;
using Code.Scripts.Managers;

[RequireComponent(typeof(LineRenderer))]
public class Lazer : MonoBehaviour
{
    private LineRenderer lr;
    private bool isActive = true;
    private float timer = 0f;
    private float nextToggleTime;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        SetNextToggleTime();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= nextToggleTime)
        {
            isActive = !isActive;
            lr.enabled = isActive;
            SetNextToggleTime();
            timer = 0f;
        }

        if (!isActive) return; // Skip update if the laser is off

        lr.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.right, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Hit Player");
                GameManager.Instance.ChangeState(GameManager.GameState.GameOver);
            }
            else
            {
                lr.SetPosition(1, hit.point);
            }
        }
        else
        {
            lr.SetPosition(1, transform.position + -transform.right * 50);
        }
    }

    private void SetNextToggleTime()
    {
        nextToggleTime = Random.Range(3f, 5f);
    }
}