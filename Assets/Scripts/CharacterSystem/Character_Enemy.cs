using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Character_Enemy : MonoBehaviour
{
    public State state;

    [SerializeField] string[] walkAnimVariantNames;
    string walkAnimName;

    public float[] speeds;

    public enum State
    {
        Idle,
        Chase,
        Attack,
        Knockbacked
    }

    [Header("References")]
    Animator animator;
    NavMeshAgent agent;

    [Header("Attack")]
    bool isAttacking;
    public float damage;
    public float attackDistance = 2.0f;
    public float currentAnimTimer;
    public float maxAnimTimer;
    [SerializeField] private bool isKnockbacked = false;

    [Header("Moans")]
    public float minMoanTime;
    public float maxMoanTime;

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
            case State.Knockbacked:

                break;
            case State.Attack:
                if (Player != null)
                {
                    Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
                }
                InitAttack();
                break;
            case State.Chase:
                if (Player != null)
                {
                    MoveTo(Player.transform.position);
                }
                break;
        }
    }

    public void OnSpawn()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("Offset", Random.value);

        agent = GetComponent<NavMeshAgent>();
        agent.speed = speeds[Random.Range(0, speeds.Length)];
        if (walkAnimVariantNames.Length != 0)
        {
            walkAnimName = walkAnimVariantNames[Random.Range(0, walkAnimVariantNames.Length)];
            animator.Play(walkAnimName);
        }
        StartCoroutine(Sounds());
    }

    public void ApplyKnockback(Vector3 InSourcePosition, float InKnockbackForce, float InKnockbackDuration)
    {
        if (isKnockbacked)
            return;

        Vector3 direction = transform.position - InSourcePosition;
        direction.y = 0f;
        direction.Normalize();

        StartCoroutine(Knockback(direction, InKnockbackForce, InKnockbackDuration));
    }

    IEnumerator Knockback(Vector3 InDirection, float InKnockbackForce, float InKnockbackDuration)
    {
        isKnockbacked = true;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;

            float timer = .0f;

            while (timer < InKnockbackDuration)
            {
                float t = timer / InKnockbackDuration;
                float strength = Mathf.Lerp(InKnockbackForce, 0f, t);

                agent.Move(InDirection * strength * Time.deltaTime);

                timer += Time.deltaTime;
                yield return null;
            }

            agent.isStopped = false;
        }

        isKnockbacked = false;
    }

    IEnumerator Sounds()
    {
        while (true)
        {
            float rand = Random.Range(minMoanTime, maxMoanTime);
            yield return new WaitForSeconds(rand);
            SoundFXManager.instance.TriggerRandomSoundFX(SoundFXManager.SoundCategory.EnemyMoans, transform, 1.0f, true);
        }
    }

    void UpdateState()
    {
        if (isKnockbacked)
        {
            state = State.Knockbacked;
        }
        else if (IsTargetInReachForAttack() || isAttacking)
        {
            state = State.Attack;
        }
        else if (true)
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
        agent.SetDestination(targetDirection);
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

    public void ChangeState(State newAction)
    {
        state = newAction;
    }
}