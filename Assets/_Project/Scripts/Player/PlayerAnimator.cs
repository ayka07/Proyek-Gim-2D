using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    [SerializeField] private PlayerMovement movement;
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();

    private void Update()
    {
        if (movement == null || animator == null) return;
        animator.SetBool(IsMoving, movement.Velocity.sqrMagnitude > 0.01f);
        animator.SetFloat(MoveX, movement.Facing.x);
        animator.SetFloat(MoveY, movement.Facing.y);
    }
}
