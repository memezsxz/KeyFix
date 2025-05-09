using GLTFast.Schema;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 10f;
    public float stopDistance = 2f;

    public Transform firePoint;              
    public GameObject projectilePrefab;       
    public float fireForce = 10f;             
    public float attackCooldown = 2f;         

    private float lastAttackTime = 0f;

    private NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            // Rotate smoothly toward the player
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            // Stop if too close
            if (distance <= stopDistance)
            {
                agent.isStopped = true;
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    animator.SetTrigger("Throw");
                    ThrowBall();
                    lastAttackTime = Time.time;
                }
            }
            else
                agent.isStopped = false;
        }
        else
        {
            agent.isStopped = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void ThrowBall()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * fireForce;
        }
    }

}
