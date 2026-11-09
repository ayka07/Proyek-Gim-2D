using System.Collections;
using UnityEngine;

public class SpriteVfx : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 15f;

    private void Awake()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    public void Play(Vector3 position, Sprite[] overrideFrames = null)
    {
        transform.position = position;
        gameObject.SetActive(true);
        StartCoroutine(Animate(overrideFrames ?? frames));
    }

    private IEnumerator Animate(Sprite[] animFrames)
    {
        if (animFrames != null && animFrames.Length > 0)
        {
            float delay = 1f / fps;
            for (int i = 0; i < animFrames.Length; i++)
            {
                if (sr != null) sr.sprite = animFrames[i];
                yield return new WaitForSeconds(delay);
            }
        }
        gameObject.SetActive(false);
    }
}
