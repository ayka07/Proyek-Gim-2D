using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject dialogueRoot;
    [SerializeField] private Button clickCatcher;
    [SerializeField] private Image boxImage;
    [SerializeField] private Text bodyText;
    [SerializeField] private GameObject speakerTag;
    [SerializeField] private Text speakerText;
    [SerializeField] private RectTransform speakerTagRect;
    [SerializeField] private GameObject continueHint;
    [SerializeField] private TextMenu choicesMenu;
    [SerializeField] private Sprite navyPanelSprite;
    [SerializeField] private Sprite narrPanelSprite;

    public event Action Clicked;

    private void Awake()
    {
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (clickCatcher != null)
        {
            clickCatcher.onClick.RemoveAllListeners();
            clickCatcher.onClick.AddListener(() => Clicked?.Invoke());
        }
        Hide();
    }

    private void Update()
    {
        if (continueHint != null && continueHint.activeSelf)
        {
            bool visible = Mathf.FloorToInt(Time.unscaledTime / 0.3f) % 2 == 0;
            continueHint.SetActive(visible);
        }
    }

    public void Show(bool narrate)
    {
        if (canvas != null)
        {
            canvas.sortingOrder = narrate ? 110 : 20;
        }

        if (dialogueRoot != null) dialogueRoot.SetActive(true);
        if (boxImage != null)
        {
            boxImage.sprite = narrate ? narrPanelSprite : navyPanelSprite;
        }

        if (speakerTag != null)
        {
            speakerTag.SetActive(!narrate);
        }

        if (continueHint != null) continueHint.SetActive(false);
        if (choicesMenu != null) choicesMenu.Close();
    }

    public void Hide()
    {
        if (dialogueRoot != null) dialogueRoot.SetActive(false);
        if (continueHint != null) continueHint.SetActive(false);
        if (choicesMenu != null) choicesMenu.Close();
    }

    public void SetSpeaker(string speaker)
    {
        if (speakerTag != null)
        {
            speakerTag.SetActive(!string.IsNullOrEmpty(speaker));
        }

        if (speakerText != null)
        {
            speakerText.text = speaker;
        }

        if (speakerTagRect != null && !string.IsNullOrEmpty(speaker))
        {
            float width = Mathf.Min(488f, speaker.Length * 20f + 64f);
            speakerTagRect.sizeDelta = new Vector2(width, speakerTagRect.sizeDelta.y);
        }
    }

    public void SetBody(string text)
    {
        if (bodyText != null) bodyText.text = text;
    }

    public void SetContinueHint(bool active)
    {
        if (continueHint != null) continueHint.SetActive(active);
    }

    public void ShowChoices(string[] options, Action<int> onPicked)
    {
        if (choicesMenu == null) return;
        choicesMenu.Picked -= onPicked;
        choicesMenu.Picked += onPicked;
        choicesMenu.Open(options);
    }

    public void HideChoices()
    {
        if (choicesMenu != null) choicesMenu.Close();
    }
}
