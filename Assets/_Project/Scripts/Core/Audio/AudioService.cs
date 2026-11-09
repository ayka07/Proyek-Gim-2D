using System.Collections;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;
    [SerializeField] private AudioSource sfxSource;

    private AudioClip worldMusic;
    private bool activeIsA = true;
    private Coroutine fadeRoutine;

    public bool Muted { get; private set; }

    private void Awake()
    {
        if (musicSourceA == null) musicSourceA = gameObject.AddComponent<AudioSource>();
        if (musicSourceB == null) musicSourceB = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        musicSourceA.loop = true;
        musicSourceB.loop = true;
        musicSourceA.playOnAwake = false;
        musicSourceB.playOnAwake = false;
        sfxSource.playOnAwake = false;
    }

    private void Update()
    {
        if (MenuInput.Mute)
        {
            ToggleMute();
        }
    }

    public void PlayWorldMusic(AudioClip clip)
    {
        worldMusic = clip;
        PlayMusic(clip);
    }

    public void PlayBattleMusic(AudioClip clip)
    {
        PlayMusic(clip);
    }

    public void ResumeWorldMusic()
    {
        PlayMusic(worldMusic);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource current = activeIsA ? musicSourceA : musicSourceB;
        if (current.isPlaying && current.clip == clip) return;

        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Crossfade(clip));
    }

    private IEnumerator Crossfade(AudioClip nextClip)
    {
        AudioSource current = activeIsA ? musicSourceA : musicSourceB;
        AudioSource next = activeIsA ? musicSourceB : musicSourceA;

        next.clip = nextClip;
        next.volume = 0f;
        next.Play();

        float dur = 1f;
        float elapsed = 0f;
        float startVol = current.volume;

        while (elapsed < dur)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / dur;
            current.volume = Mathf.Lerp(startVol, 0f, t);
            next.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        current.Stop();
        current.volume = 0f;
        next.volume = 1f;
        activeIsA = !activeIsA;
        fadeRoutine = null;
    }

    public void PlaySfx(AudioClip clip, float pitchJitter = 0.05f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        sfxSource.PlayOneShot(clip);
    }

    public bool ToggleMute()
    {
        Muted = !Muted;
        AudioListener.volume = Muted ? 0f : 1f;
        return Muted;
    }
}
