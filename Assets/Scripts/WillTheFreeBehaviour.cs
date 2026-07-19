using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WillTheFreeBehaviour : MonoBehaviour
{
    [SerializeField] AudioClip[] moans;

    public State currentState;

    public enum State
    {
        Idle,
        ChasePlayer,
        WanderingAround
    }

    [SerializeField] Rigidbody rb;

    [Header("Bool")]
    [SerializeField] public bool canMove;
    [SerializeField] public bool canRotate;
    [SerializeField] bool hasArrived = true;
    [SerializeField] bool changedState = false;

    [Header("References")]
    NavMeshPath path;
    Animator animator;

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

    [Header("Wandering Around")]
    [SerializeField] Transform[] wanderingPoints;
    [SerializeField] Vector3 wanderingDestination;
    [SerializeField] Vector2 maxMinWaitTime = new Vector2(5, 10);
    [SerializeField] float currentWaitTime;
    [SerializeField] float timer;

    [Header("Path")]
    [SerializeField] bool pathFound = false;
    private Vector3[] pathPoints;
    [SerializeField] private float stoppingThreshold;

    private void Awake()
    {
        OnSpawn();
        int randWanderingPointIndex = Random.Range(0, wanderingPoints.Length);
        wanderingDestination = wanderingPoints[randWanderingPointIndex].position;
    }

    private void Update()
    {
        UpdateState();
        ApplyBehaviour();
    }

    private void ApplyBehaviour()
    {
        GameObject Player = GameManager.instance.player;

        switch (currentState)
        {
            case State.Idle:
                currentTargetSpeed = 0;
                break;
            case State.ChasePlayer:
                currentTargetSpeed = movementSpeed;
                if (Player != null)
                {
                    CalculatePathTo(Player.transform.position);
                    Vector3 pathDirection = PathDirection();
                    MoveTo(pathDirection);
                    RotateTo(pathDirection);
                }
                break;
            case State.WanderingAround:
                if (Player != null)
                {
                    WanderingAround();
                    CalculatePathTo(wanderingDestination);
                    Vector3 pathDirection = PathDirection();
                    MoveTo(pathDirection);
                    RotateTo(pathDirection);
                }
                break;
        }
    }

    void WanderingAround()
    {
        if (hasArrived)
        {
            timer += Time.deltaTime;
            if (changedState)
            {
                currentWaitTime = Random.Range(maxMinWaitTime.x, maxMinWaitTime.y);
            }
            if (timer >= currentWaitTime)
            {
                currentWaitTime = Random.Range(maxMinWaitTime.x, maxMinWaitTime.y);
                timer = 0;

                int randWanderingPointIndex = Random.Range(0, wanderingPoints.Length);
                wanderingDestination = wanderingPoints[randWanderingPointIndex].position;
            }
        }
    }

    public void OnSpawn()
    {
        animator = GetComponentInChildren<Animator>();
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
        State lastState = currentState;
        if (true)
        {
            currentState = State.WanderingAround;
        }
        else if (CanMove())
        {
            currentState = State.ChasePlayer;
        }
        else
        {
            currentState = State.Idle;
        }

        changedState = lastState != currentState;
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

        velocity = direction * currentSpeed * Time.deltaTime;

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(transform.position + velocity, out hit, 100.0f, NavMesh.AllAreas);

        if (canMove && isValid)
        {
            rb.MovePosition(hit.position);
        }
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            float transitionTime = 0.1f;
            animator.CrossFade("Attack", transitionTime, 0);
            StartCoroutine(CaptureAttackLength(transitionTime));
        }
    }

    IEnumerator CaptureAttackLength(float transitionTime)
    {
        yield return new WaitForSeconds(transitionTime);
        maxAnimTimer = animator.GetCurrentAnimatorStateInfo(0).length;
        while (isAttacking)
        {
            currentAnimTimer += Time.deltaTime;
            if (currentAnimTimer >= maxAnimTimer)
            {
                animator.CrossFade("Walking", 0.1f, 0);
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