using UnityEngine;
using UnityEngine.InputSystem;

public class BoxInteract : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private Transform player;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        inputActions.Player.Interact.performed += OnInteractPerformed;
        inputActions.Enable();
        player = other.transform;

        Debug.Log("Player entered: " + gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (player == other.transform && player.parent != transform.parent)
        {
            player = null;
            inputActions.Player.Interact.performed -= OnInteractPerformed;
            inputActions.Disable();
            Debug.Log("Player exited: " + gameObject.name);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("F pressed: " + gameObject.name);

        if (player == null)
            return;

        Debug.Log("Interact with: " + gameObject.name);

        IInteract interact = GetComponentInParent<IInteract>();

        if (interact != null)
        {
            interact.Interact(player);
        }
        else
        {
            Debug.LogWarning(
                "Không tìm thấy IInteract ở GameObject hoặc Parent!",
                gameObject
            );
        }
    }
}
public interface IInteract
{
    void Interact(Transform interactor);
}