using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 minBounds = new Vector2(10f, 5.625f);
    [SerializeField] private Vector2 maxBounds = new Vector2(30f, 30.375f);
    [SerializeField] private float damping = 0.5f;

    private Vector3 currentVelocity;

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
        if (target != null)
        {
            SnapToTarget();
        }
    }

    public void SetBounds(float mapWidth, float mapHeight, float orthoSize, float aspect)
    {
        float vertExtent = orthoSize;
        float horzExtent = vertExtent * aspect;

        minBounds = new Vector2(horzExtent, vertExtent);
        maxBounds = new Vector2(mapWidth - horzExtent, mapHeight - vertExtent);
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 dest = target.position;
        float clampedX = Mathf.Clamp(dest.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(dest.y, minBounds.y, maxBounds.y);
        Vector3 targetPos = new Vector3(clampedX, clampedY, -10f);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, damping);
    }

    public void SnapToTarget()
    {
        if (target == null) return;
        float clampedX = Mathf.Clamp(target.position.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(target.position.y, minBounds.y, maxBounds.y);
        transform.position = new Vector3(clampedX, clampedY, -10f);
        currentVelocity = Vector3.zero;
    }
}
