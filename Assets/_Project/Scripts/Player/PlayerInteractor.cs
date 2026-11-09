using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteractor who);
}

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private float reach = 0.6f;
    [SerializeField] private float radius = 0.3f;
    [SerializeField] private LayerMask interactableMask;
    private IActionInput input;

    public void Construct(IActionInput actionInput) => input = actionInput;
    public Vector2 ProbePoint => (Vector2)transform.position + (movement != null ? movement.Facing : Vector2.down) * reach;

    private void Update()
    {
        if (input == null || !input.InteractPressed) return;
        Collider2D hit = Physics2D.OverlapCircle(ProbePoint, radius, interactableMask);
        if (hit != null && hit.TryGetComponent(out IInteractable target))
        {
            target.Interact(this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ProbePoint, radius);
    }
}
