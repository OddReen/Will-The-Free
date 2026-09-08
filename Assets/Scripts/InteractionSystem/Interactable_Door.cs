using UnityEngine;

public class Interactable_Door : Interactable
{
    [SerializeField] Animator animator;

    public override void ExecuteAction()
    {
        animator.Play("Falling");
    }
}
