using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerConfig config;
    [SerializeField] private Rigidbody2D rb;

    private IMoveInput input;
    private Vector2 velocity;

    public Vector2 Velocity => velocity;
    public Vector2 Facing { get; private set; } = Vector2.down;

    public void Construct(IMoveInput moveInput)
    {
        input = moveInput;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (input == null) return;
        Vector2 move = input.Move;
        float speed = input.IsRunning ? (config != null ? config.RunSpeed : 6f) : (config != null ? config.WalkSpeed : 3.75f);
        velocity = move * speed;
        if (move.sqrMagnitude > 0.01f) Facing = ToCardinal(move);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    public void Teleport(Vector2 pos, Vector2 facing)
    {
        transform.position = pos;
        Facing = facing;
        velocity = Vector2.zero;
        if (rb != null)
        {
            rb.position = pos;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private static Vector2 ToCardinal(Vector2 v) =>
        Mathf.Abs(v.x) > Mathf.Abs(v.y)
            ? new Vector2(Mathf.Sign(v.x), 0f)
            : new Vector2(0f, Mathf.Sign(v.y));
}
