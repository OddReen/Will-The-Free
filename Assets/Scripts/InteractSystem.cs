using UnityEngine;

public class InteractSystem : MonoBehaviour
{
    [SerializeField] InputHandler inputHandler;
    private void Start()
    {
        inputHandler.OnInteract += DetectObject;
    }
    private void DetectObject()
    {
        RaycastHit hit;
        if (Physics.SphereCast(Camera.main.transform.position, .25f, Camera.main.transform.forward, out hit, 2))
        {
            Interactable interactable_Door = hit.collider.GetComponentInParent<Interactable>();
            if (interactable_Door != null)
            {
                interactable_Door.ExecuteAction();
            }
        }
    }
}
