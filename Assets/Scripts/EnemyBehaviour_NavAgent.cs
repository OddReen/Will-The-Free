using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour_NavAgent : MonoBehaviour
{
    [SerializeField] AudioClip[] moans;

    public State state;

    [SerializeField] string[] walkAnimVariantNames;
    string walkAnimName;

    public enum State
    {
        Idle,
        Chase,
        Attack
    }

    [Header("Bool")]
    [SerializeField] public bool canMove;
    [SerializeField] public bool canRotate;
    [SerializeField] bool hasArrived = true;

    [Header("References")]
    NavMeshPath path;
    Animator animator;
    NavMeshAgent agent;

    [Header("Attack")]
    bool isAttacking;
    public float damage;
    public float attackDistance = 2.0f;
    public float currentAnimTimer;
    public float maxAnimTimer;

    [Header("Movement")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentTargetSpeed;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float movementTransitionSpeed;
    [SerializeField] private float rotationSpeed;
    private Vector3 direction;
    private Vector3 velocity;

    [Header("Path")]
    [SerializeField] bool pathFound = false;
    private Vector3[] pathPoints;
    [SerializeField] private float stoppingThreshold;

    private void Awake()
    {
        OnSpawn();
    }

    private void FixedUpdate()
    {
        UpdateState();

        GameObject Player = GameManager.instance.player;

        switch (state)
        {
            case State.Idle:
                currentTargetSpeed = 0;
                break;
            case State.Chase:
                currentTargetSpeed = movementSpeed;
                if (Player != null)
                {
                    MoveTo(Player.transform.position);
                }
                break;
            case State.Attack:
                if (Player != null)
                {
                    Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
                }
                InitAttack();
                break;
        }
    }

    public void OnSpawn()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (walkAnimVariantNames.Length != 0)
        {
            walkAnimName = walkAnimVariantNames[Random.Range(0, walkAnimVariantNames.Length)];
            animator.Play(walkAnimName);
        }
        path = new NavMeshPath();
        hasArrived = false;
    }

    IEnumerator Sounds()
    {
        while (true)
        {
            float rand = Random.Range(0, 15);
            yield return new WaitForSeconds(rand);
        }
    }

    void UpdateState()
    {
        if (IsTargetInReachForAttack() || isAttacking)
        {
            state = State.Attack;
        }
        else if (CanMove())
        {
            state = State.Chase;
        }
        else
        {
            state = State.Idle;
        }
    }

    void MoveTo(Vector3 targetDirection)
    {
        if (canMove)
        {
            agent.SetDestination(targetDirection);
        }
    }

    void InitAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.Play("Attack");
            animator.Update(0.0f);
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        maxAnimTimer = animator.GetCurrentAnimatorStateInfo(0).length;
        while (isAttacking)
        {
            currentAnimTimer += Time.deltaTime;
            if (currentAnimTimer >= maxAnimTimer)
            {
                animator.Play(walkAnimName);
                currentAnimTimer = 0f;
                isAttacking = false;
            }
            yield return null;
        }
    }

    public void OnAttackAnimEvent()
    {
        GameObject Player = GameManager.instance.player;
        if (Player != null)
        {
            if (Vector3.Distance(Player.transform.position, transform.position) < attackDistance)
            {
                Player.GetComponent<HealthSystem>().TakeDamage(damage);
            }
        }
    }

    bool IsTargetInReachForAttack()
    {
        bool OutBool = false;

        GameObject target = GameManager.instance.player;
        if (target != null)
        {
            OutBool = Vector3.Distance(transform.position, target.transform.position) < attackDistance;
        }

        return OutBool;
    }

    bool CanMove()
    {
        return true;
    }

    public void ChangeState(State newAction)
    {
        hasArrived = false;
        state = newAction;
    }

    void CalculatePathTo(Vector3 InDestination)
    {
        // Update Destination
        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(InDestination, out hit, 100.0f, NavMesh.AllAreas);
        if (isValid)
        {
            Vector3 destination = hit.position;

            // Not arrived? Then recalculate path
            if (!hasArrived)
            {
                pathFound = NavMesh.CalculatePath(transform.position, destination, -1, path);
                hasArrived = path.corners.Length >= 2 && Vector3.Distance(transform.position, destination) < stoppingThreshold;
            }
            else
            {
            hasArrived = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pathFound)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.corners.Length; i++)
            {
                if (i == path.corners.Length - 1)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(path.corners[i], .5f);
                }
                else
                {
                    Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
                    Gizmos.DrawSphere(path.corners[i], .25f);
                }
            }
        }
    }
}