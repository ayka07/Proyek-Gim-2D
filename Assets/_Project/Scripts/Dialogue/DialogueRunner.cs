using System;
using System.Collections;
using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [SerializeField] private DialogueView view;
    [SerializeField] private AudioClip blipSfx;

    private InputGate gate;
    private AudioService audioService;
    private bool clicked;
    private int pickedIndex = -1;

    public bool IsRunning { get; private set; }

    public void Construct(InputGate inputGate, AudioService audio)
    {
        gate = inputGate;
        audioService = audio;
    }

    private void Awake()
    {
        if (view == null) view = GetComponent<DialogueView>();
        if (view != null)
        {
            view.Clicked += OnClicked;
        }
    }

    private void OnDestroy()
    {
        if (view != null)
        {
            view.Clicked -= OnClicked;
        }
    }

    private void OnClicked()
    {
        clicked = true;
    }

    private bool AdvancePressed()
    {
        bool pressed = clicked || MenuInput.Confirm;
        clicked = false;
        return pressed;
    }

    public void Run(IEnumerator sequence)
    {
        if (!IsRunning && sequence != null)
        {
            StartCoroutine(RunRoutine(sequence));
        }
    }

    private IEnumerator RunRoutine(IEnumerator sequence)
    {
        IsRunning = true;
        gate?.Lock();
        yield return null; // lewati tombol pembuka
        clicked = false;
        yield return sequence;
        view.Hide();
        yield return null; // cegah dialog terbuka lagi
        gate?.Unlock();
        IsRunning = false;
    }

    public IEnumerator Say(string speaker, string text)
    {
        view.Show(false);
        yield return TypeLine(speaker, text, true);
    }

    public IEnumerator Narrate(string text)
    {
        view.Show(true);
        yield return TypeLine("", text, true);
    }

    public IEnumerator Choose(string speaker, string prompt, string[] options, Action<int> result)
    {
        view.Show(false);
        yield return TypeLine(speaker, prompt, false);
        pickedIndex = -1;
        view.ShowChoices(options, idx => pickedIndex = idx);
        yield return new WaitUntil(() => pickedIndex >= 0);
        view.HideChoices();
        result?.Invoke(pickedIndex);
    }

    public IEnumerator PlayData(DialogueData data)
    {
        if (data == null || data.lines == null) yield break;
        for (int i = 0; i < data.lines.Length; i++)
        {
            yield return Say(data.lines[i].speaker, data.lines[i].text);
        }
    }

    private IEnumerator TypeLine(string speaker, string text, bool showHint)
    {
        view.SetSpeaker(speaker);
        view.SetContinueHint(false);
        view.SetBody("");

        const float secondsPerChar = 0.018f; // ~55 chars / sec
        clicked = false;

        for (int i = 1; i <= text.Length; i++)
        {
            view.SetBody(text.Substring(0, i));

            if (i % 3 == 0 && audioService != null && blipSfx != null)
            {
                audioService.PlaySfx(blipSfx, 0.08f);
            }

            if (AdvancePressed())
            {
                // Instantly complete typing
                view.SetBody(text);
                break;
            }

            yield return new WaitForSecondsRealtime(secondsPerChar);
        }

        view.SetBody(text);
        if (showHint) view.SetContinueHint(true);

        yield return null;
        clicked = false;

        while (!AdvancePressed())
        {
            yield return null;
        }

        if (showHint) view.SetContinueHint(false);
    }
}
