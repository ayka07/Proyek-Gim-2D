using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    [SerializeField] private SpriteVfx prefab;
    [SerializeField] private int initialSize = 5;

    private readonly Queue<SpriteVfx> pool = new Queue<SpriteVfx>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateInstance();
        }
    }

    private SpriteVfx CreateInstance()
    {
        if (prefab == null) return null;
        SpriteVfx obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    public void Play(Vector3 position)
    {
        SpriteVfx vfx = null;
        if (pool.Count > 0)
        {
            vfx = pool.Dequeue();
            pool.Enqueue(vfx);
        }
        else
        {
            vfx = CreateInstance();
        }

        if (vfx != null)
        {
            vfx.Play(position);
        }
    }
}
