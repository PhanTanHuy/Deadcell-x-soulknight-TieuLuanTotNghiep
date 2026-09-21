using UnityEngine;

public class BoxInteract : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        IInteract interact = GetComponentInParent<IInteract>();

        if (interact != null)
        {
            interact.Interact(other.transform);
        }
    }
}
public interface IInteract
{
    void Interact(Transform interactor);
}