using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip[] moans;

    public State state;

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

    [Header("Attack")]
    bool isAttacking;
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

    public void OnSpawn()
    {
        animator = GetComponentInChildren<Animator>();
        path = new NavMeshPath();
        hasArrived = false;

        StartCoroutine(Sounds());
    }
    IEnumerator Sounds()
    {
        while (true)
        {
            float rand = Random.Range(0, 15);
            yield return new WaitForSeconds(rand);
            //SoundFXManager.instance.PlayerRandomSoundFXClip(moans, transform, 1, true);
        }
    }
    private void Awake()
    {
        OnSpawn();
    }
    private void Update()
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
                    CalculatePathTo(Player.transform.position);
                    Vector3 pathDirection = PathDirection();
                    MoveTo(pathDirection);
                    RotateTo(pathDirection);
                }
                break;
            case State.Attack:
                Attack();
                if (Player != null)
                {
                    Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
                    RotateTo(directionToPlayer);
                }
                break;
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

    Vector3 PathDirection()
    {
        if (pathFound && path.corners.Length >= 2)
        {
            pathPoints = path.corners;

            return (pathPoints[1] - transform.position).normalized;
        }
        else
        {
            return transform.forward;
        }
    }

    void RotateTo(Vector3 targetDirection)
    {
        if (targetDirection != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            float smoothRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation, rotationSpeed * Time.deltaTime);

            if (canRotate)
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, smoothRotation, transform.eulerAngles.z);
        }
    }

    void MoveTo(Vector3 targetDirection)
    {
        direction = Vector3.MoveTowards(direction, targetDirection, movementTransitionSpeed * Time.deltaTime);

        currentSpeed = Mathf.MoveTowards(currentSpeed, currentTargetSpeed, movementTransitionSpeed * Time.deltaTime);

        velocity = transform.position + direction * currentSpeed * Time.deltaTime;

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(velocity, out hit, 100.0f, NavMesh.AllAreas);

        if (canMove && isValid)
        {
            transform.position = hit.position;
        }
    }
    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animator.CrossFade("Attack", 0.1f, 0);
            maxAnimTimer = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        }
        else
        {
            currentAnimTimer += Time.deltaTime;
            if (currentAnimTimer >= maxAnimTimer)
            {
                animator.CrossFade("Walking", 0.1f, 0);
                currentAnimTimer = 0;
                isAttacking = false;
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