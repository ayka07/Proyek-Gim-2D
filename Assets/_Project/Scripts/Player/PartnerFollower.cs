using UnityEngine;

public class PartnerFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float followDistance = 0.9f;
    [SerializeField] private float speed = 4f;

    private MonsterInstance currentMonster;

    public void SetTarget(Transform playerTransform) => target = playerTransform;

    public void SetMonster(MonsterInstance monster)
    {
        currentMonster = monster;
        if (monster != null && monster.Data != null && monster.Data.Frames != null && monster.Data.Frames.Length > 0)
        {
            if (spriteRenderer != null)
                spriteRenderer.sprite = monster.Data.Frames[0];
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (target == null || currentMonster == null || spriteRenderer == null) return;

        // Animated idle / walk
        if (currentMonster.Data != null && currentMonster.Data.Frames != null && currentMonster.Data.Frames.Length > 1)
        {
            int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2;
            spriteRenderer.sprite = currentMonster.Data.Frames[frame];
        }

        Vector3 diff = target.position - transform.position;
        if (diff.magnitude > followDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            if (diff.x > 0.05f) spriteRenderer.flipX = false;
            else if (diff.x < -0.05f) spriteRenderer.flipX = true;
        }
    }
}
