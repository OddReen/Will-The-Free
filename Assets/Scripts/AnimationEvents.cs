using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] AudioClip[] steps;
    [SerializeField] Transform stepPos;
    public void Steps()
    {
        //SoundFXManager.instance.PlayerRandomSoundFXClip(steps, stepPos, 1, true);
    }

    public void OnAttackAnimEvent()
    {
        GameObject Player = GameManager.instance.player;
        if (Player != null)
        {
            if (Vector3.Distance(Player.transform.position, transform.parent.position) < GetComponentInParent<EnemyBehaviour>().attackDistance)
            {
                Player.GetComponent<HealthSystem>().TakeDamage(15.0f);
            }
        }
    }
}
