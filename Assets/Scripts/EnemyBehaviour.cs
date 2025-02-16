using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public Action action;

    public enum Action
    {
        Idle,
        Chase,
        LookingForFood,
        FuellingCampfire,
        Patrolling
    }

    [Header("Bool")]
    [SerializeField] public bool canMove;
    [SerializeField] public bool canRotate;
    [SerializeField] bool hasArrived = true;

    [Header("References")]
    NavMeshPath path;
    Animator animator;

    [Header("Movement")]
    [SerializeField] MovementState movementState;
    enum MovementState
    {
        Idle,
        Walk,
        Run
    }

    [SerializeField] private float speed;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float movementTransitionSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotateSpeed;
    private Vector3 targetDirection;
    private Vector3 direction;
    private Vector3 velocity;

    [Header("Patrol")]
    [SerializeField] Transform randPointOrigin;
    [SerializeField] float randPointRange;
    [SerializeField] float waitTimer = 0;
    [SerializeField] float randomWaitTime;
    [SerializeField] float minWaitingTime;
    [SerializeField] float maxWaitingTime;
    Vector3 destination;
    [SerializeField] bool pathFound = false;
    private Vector3[] pathPoints;
    [SerializeField] private float stoppingThreshold;

    [Header("Detect Characters")]
    [SerializeField] float distanceToCharacter;

    bool actionStarted = false;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        path = new NavMeshPath();
        hasArrived = false;
    }
    private void Update()
    {
        targetSpeed = NPCTargetSpeed();

        switch (action)
        {
            case Action.Chase:
                movementState = MovementState.Walk;
                Chase();
                break;
            case Action.Idle:
                break;
        }
        targetDirection = NPCDirection();


        MoveTo(targetDirection);
        RotateTo(targetDirection);

        Animation();
    }

    float NPCTargetSpeed()
    {
        if (movementState == MovementState.Run)
        {
            return runSpeed;
        }
        else if (movementState == MovementState.Walk)
        {
            movementState = MovementState.Walk;
            return walkSpeed;
        }
        else
        {
            movementState = MovementState.Idle;
            return 0;
        }
    }
    Vector3 NPCDirection()
    {
        if (pathFound && path.corners.Length >= 2)
        {
            pathPoints = path.corners;

            return (pathPoints[1] - transform.position).normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }

    void RotateTo(Vector3 targetDirection)
    {
        if (targetDirection != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

            float smoothRotation = Mathf.LerpAngle(transform.eulerAngles.y, targetRotation, rotateSpeed * Time.deltaTime);

            if (canRotate)
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, smoothRotation, transform.eulerAngles.z);
        }
    }
    void MoveTo(Vector3 targetDirection)
    {
        direction = Vector3.MoveTowards(direction, targetDirection, movementTransitionSpeed * Time.deltaTime);

        speed = Mathf.MoveTowards(speed, targetSpeed, movementTransitionSpeed * Time.deltaTime);

        velocity = transform.position + direction * speed * Time.deltaTime;

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(velocity, out hit, 100.0f, NavMesh.AllAreas);

        if (canMove && isValid)
        {
            transform.position = hit.position;
        }
    }
    void Animation()
    {
        //Vector3 deltaVelocity = (transform.position - lastPosition) / Time.deltaTime;

        //animator.SetFloat("Movement", deltaVelocity.magnitude);

        //animator.SetFloat("Movement", speed);
    }
    void CalculatePath()
    {
        pathFound = NavMesh.CalculatePath(transform.position, destination, -1, path);
    }
    public void ChangeState(Action newAction)
    {
        hasArrived = false;
        actionStarted = false;
        action = newAction;
    }

    void Chase()
    {
        if (!actionStarted)
        {
            targetSpeed = runSpeed;
            actionStarted = true;
        }

        NavMeshHit hit;
        bool isValid = NavMesh.SamplePosition(GameManager.instance.player.transform.position, out hit, 100.0f, NavMesh.AllAreas);
        if (isValid)
        {
            destination = hit.position;
        }

        if (hasArrived)
        {
            hasArrived = false;
        }
        else
        {
            CalculatePath();
            hasArrived = path.corners.Length >= 2 && Vector3.Distance(transform.position, destination) < stoppingThreshold;
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
        if (randPointOrigin != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(randPointOrigin.position, randPointRange);
        }
    }
}