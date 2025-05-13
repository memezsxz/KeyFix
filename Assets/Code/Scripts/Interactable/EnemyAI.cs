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
    public Animator animator;

    private NavMeshAgent agent;

    private float lastAttackTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        var distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);

            // Rotate smoothly toward the player
            var direction = (player.position - transform.position).normalized;
            var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
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
            {
                agent.isStopped = false;
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void ThrowBall()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        var projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        var rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            var direction = (player.position - firePoint.position).normalized;
            rb.velocity = direction * fireForce;
        }
    }
}