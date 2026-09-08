
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Character_Will : MonoBehaviour
{
    [SerializeField] AudioClip[] moans;

    public State state;

    [SerializeField] string[] walkAnimVariantNames;
    string walkAnimName;

    public enum State
    {
        Idle,
        Chase,
        Wandering
    }

    [Header("Bool")]
    [SerializeField] public bool canMove;
    [SerializeField] public bool canRotate;

    [Header("References")]
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

    [SerializeField] private Transform eyesPoint;

    private void Awake()
    {
        OnSpawn();
    }

    private void Update()
    {
        UpdateState();

        switch (state)
        {
            case State.Idle:
                break;
            case State.Chase:
                GameObject Player = GameManager.instance.player;
                if (Player != null)
                {
                    MoveTo(Player.transform.position);
                }
                break;
            case State.Wandering:
                WanderingAround();
                break;
        }
        animator.SetFloat("Move", agent.velocity.magnitude / agent.speed);
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
        hasArrivedCooldown = hasArrivedMaxCooldown;
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
        //if (CanMove())
        //{
        //    state = State.Chase;
        //}
        //else if
        //{
            state = State.Wandering;
        //}
        //else
        //{
        //    state = State.Idle;
        //}
    }

    float hasArrivedCooldown = 0.0f;
    [SerializeField] float hasArrivedMaxCooldown;
    void MoveTo(Vector3 InTarget)
    {
        if (canMove)
        {
            if (hasArrivedCooldown < hasArrivedMaxCooldown)
            {
                hasArrivedCooldown += Time.deltaTime;
            }
            agent.SetDestination(InTarget);
            //if (!HasArrived())
            //{
            //    hasArrived = false;
            //}
            //if (HasArrived() && !hasArrived)
            //{
            //    hasArrived = true;
            //    if (hasArrivedCooldown >= hasArrivedMaxCooldown)
            //    {
            //        hasArrivedCooldown = 0.0f;
            //        SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.WilliamArriving, transform, 1.0f, true);
            //        GameManager.instance.player.GetComponent<CameraHandler>().InitLookAt(eyesPoint);
            //    }
            //}
        }
    }

    bool HasArrived()
    {
        return agent.remainingDistance <= agent.stoppingDistance;
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
                Player.GetComponent<HealthSystem>().TakeDamage(damage, Vector3.zero);
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
        state = newAction;
    }

    public float range;

    void WanderingAround()
    {
        if (HasArrived())
        {
            Vector3 randPoint = transform.position + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }
}