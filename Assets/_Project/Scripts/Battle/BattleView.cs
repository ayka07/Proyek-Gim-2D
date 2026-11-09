using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    [SerializeField] private MonsterPanel enemyPanel;
    [SerializeField] private MonsterPanel playerPanel;
    [SerializeField] private Text messageText;
    [SerializeField] private Text hintText;
    [SerializeField] private TextMenu menu;
    [SerializeField] private Image flashImage;
    [SerializeField] private Image wipeTop;
    [SerializeField] private Image wipeBottom;
    [SerializeField] private Image[] ringImages;

    private Coroutine typing;
    private int lastPick = 0;

    public MonsterPanel EnemyPanel => enemyPanel;
    public MonsterPanel PlayerPanel => playerPanel;
    public event Action<int> ActionClicked;

    private void Awake()
    {
        if (menu != null)
        {
            menu.Picked += (idx) =>
            {
                lastPick = idx;
                menu.Close();
                ActionClicked?.Invoke(idx);
            };
        }

        if (flashImage != null) flashImage.color = new Color(1, 1, 1, 0);
        if (wipeTop != null) wipeTop.gameObject.SetActive(false);
        if (wipeBottom != null) wipeBottom.gameObject.SetActive(false);
    }

    public void ShowMessage(string text)
    {
        if (hintText != null) hintText.enabled = false;
        if (typing != null) StopCoroutine(typing);
        typing = StartCoroutine(Type(text));
    }

    private IEnumerator Type(string text)
    {
        if (messageText == null) yield break;
        messageText.text = "";
        const float secPerChar = 0.018f;
        for (int i = 1; i <= text.Length; i++)
        {
            messageText.text = text.Substring(0, i);
            yield return new WaitForSeconds(secPerChar);
        }
        typing = null;
    }

    public void ShowMenu(IReadOnlyList<string> labels, IReadOnlyList<bool> usable, string prompt, string hint)
    {
        if (typing != null) StopCoroutine(typing);
        if (messageText != null) messageText.text = prompt;
        if (hintText != null)
        {
            hintText.text = hint;
            hintText.enabled = true;
        }

        int start = (lastPick >= 0 && lastPick < usable.Count && usable[lastPick]) ? lastPick : 0;
        if (menu != null)
        {
            menu.Open(new List<string>(labels).ToArray(), new List<bool>(usable).ToArray(), start);
        }
    }

    public void PlayIntro()
    {
        StartCoroutine(IntroWipeRoutine());
    }

    private IEnumerator IntroWipeRoutine()
    {
        if (wipeTop == null || wipeBottom == null) yield break;
        wipeTop.gameObject.SetActive(true);
        wipeBottom.gameObject.SetActive(true);

        RectTransform topRect = wipeTop.rectTransform;
        RectTransform botRect = wipeBottom.rectTransform;

        float dur = 0.48f;
        float elapsed = 0f;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dur;
            topRect.anchorMax = new Vector2(1, 1);
            topRect.anchorMin = new Vector2(0, 1f - 0.5f * (1f - t));
            botRect.anchorMin = new Vector2(0, 0);
            botRect.anchorMax = new Vector2(1, 0.5f * (1f - t));
            yield return null;
        }

        wipeTop.gameObject.SetActive(false);
        wipeBottom.gameObject.SetActive(false);
    }

    public void PlayFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (flashImage == null) yield break;
        float dur = 0.5f;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsed / dur);
            flashImage.color = new Color(1, 1, 1, a);
            yield return null;
        }
        flashImage.color = new Color(1, 1, 1, 0);
    }

    public void PlayEvolveRings(float dur)
    {
        StartCoroutine(EvolveRingsRoutine(dur));
    }

    private IEnumerator EvolveRingsRoutine(float dur)
    {
        if (ringImages == null || ringImages.Length == 0) yield break;
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            for (int i = 0; i < ringImages.Length; i++)
            {
                if (ringImages[i] != null)
                {
                    float scale = 1f + Mathf.Repeat(elapsed * 2f + i * 0.3f, 2f);
                    ringImages[i].transform.localScale = Vector3.one * scale;
                }
            }
            yield return null;
        }
    }
}
