using System.Collections.Generic;
using UnityEngine;

public class PixelBurst : MonoBehaviour
{
    private class Particle
    {
        public SpriteRenderer sr;
        public Vector2 velocity;
        public float life;
        public float max;
    }

    [SerializeField] private Sprite pixel;
    [SerializeField] private float gravity = 2.5f;

    private readonly List<Particle> active = new List<Particle>();
    private readonly Stack<Particle> free = new Stack<Particle>();

    private Particle Create()
    {
        var go = new GameObject("BurstParticle");
        go.transform.SetParent(transform);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixel;
        sr.sortingLayerName = "Characters";
        sr.sortingOrder = 100;

        return new Particle { sr = sr };
    }

    public void Play(Vector3 pos, Color color, int count, float speed, float life)
    {
        for (int i = 0; i < count; i++)
        {
            Particle p = free.Count > 0 ? free.Pop() : Create();
            float a = Random.Range(0f, Mathf.PI * 2f);
            float s = Random.Range(speed * 0.3f, speed) / 16f; // Divided by PPU
            p.velocity = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s;
            p.life = p.max = life;
            p.sr.color = color;
            p.sr.transform.position = pos;
            p.sr.gameObject.SetActive(true);
            active.Add(p);
        }
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        for (int i = active.Count - 1; i >= 0; i--)
        {
            Particle p = active[i];
            p.life -= dt;
            if (p.life <= 0f)
            {
                p.sr.gameObject.SetActive(false);
                active.RemoveAt(i);
                free.Push(p);
                continue;
            }

            p.velocity.y -= gravity * dt;
            p.sr.transform.position += (Vector3)(p.velocity * dt);
            Color c = p.sr.color;
            c.a = p.life / p.max;
            p.sr.color = c;
        }
    }
}
