using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextMenu : MonoBehaviour
{
    [SerializeField] private Text[] rows;
    [SerializeField] private int columns = 1;
    [SerializeField] private AudioClip moveSfx;
    [SerializeField] private AudioClip confirmSfx;
    [SerializeField] private AudioClip cancelSfx;

    private string[] labels;
    private bool[] usable;
    private int count;
    private bool acceptInput;

    public int Selected { get; private set; }
    public event Action<int> Picked;
    public event Action Cancelled;

    private Color normalColor = UiTheme.Text;
    private Color selectedColor = UiTheme.Gold;
    private Color disabledColor = UiTheme.Disabled;

    private void Awake()
    {
        if (rows == null || rows.Length == 0)
        {
            rows = GetComponentsInChildren<Text>(true);
        }

        for (int i = 0; i < rows.Length; i++)
        {
            int index = i;
            Button btn = rows[i].GetComponent<Button>();
            if (btn == null) btn = rows[i].gameObject.AddComponent<Button>();
            btn.transition = Selectable.Transition.None;
            var nav = btn.navigation;
            nav.mode = Navigation.Mode.None;
            btn.navigation = nav;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => Click(index));

            EventTrigger trigger = rows[i].GetComponent<EventTrigger>();
            if (trigger == null) trigger = rows[i].gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener((_) => Hover(index));
            trigger.triggers.Clear();
            trigger.triggers.Add(entry);
        }
    }

    public void Open(string[] options, bool[] enabled = null, int start = 0)
    {
        labels = options;
        count = Mathf.Min(options.Length, rows.Length);
        usable = enabled ?? new bool[count];
        if (enabled == null)
        {
            for (int i = 0; i < count; i++) usable[i] = true;
        }

        for (int i = 0; i < rows.Length; i++)
        {
            rows[i].gameObject.SetActive(i < count);
        }

        Selected = Mathf.Clamp(start, 0, Mathf.Max(0, count - 1));
        if (count > 0 && !usable[Selected]) Selected = FirstUsable();

        gameObject.SetActive(true);
        acceptInput = false;
        Redraw();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private int FirstUsable()
    {
        for (int i = 0; i < count; i++)
        {
            if (usable[i]) return i;
        }
        return 0;
    }

    private void Update()
    {
        if (count == 0) return;
        if (!acceptInput)
        {
            acceptInput = true;
            return;
        }

        if (columns <= 1)
        {
            if (MenuInput.Down) MoveSelection(+1);
            else if (MenuInput.Up) MoveSelection(-1);
        }
        else
        {
            if (MenuInput.Down) MoveSelection(+columns);
            else if (MenuInput.Up) MoveSelection(-columns);
            else if (MenuInput.Right) MoveSelection(+1);
            else if (MenuInput.Left) MoveSelection(-1);
        }

        if (MenuInput.Confirm)
        {
            Pick(Selected);
        }
        else if (MenuInput.Cancel)
        {
            Cancelled?.Invoke();
        }
    }

    private void MoveSelection(int delta)
    {
        if (count <= 1) return;
        int next = (Selected + delta + count) % count;
        if (next != Selected)
        {
            Selected = next;
            Redraw();
            PlayAudio(moveSfx);
        }
    }

    public void Hover(int index)
    {
        if (index >= 0 && index < count && index != Selected)
        {
            Selected = index;
            Redraw();
            PlayAudio(moveSfx);
        }
    }

    public void Click(int index)
    {
        if (index >= 0 && index < count)
        {
            Selected = index;
            Redraw();
            Pick(Selected);
        }
    }

    private void Pick(int index)
    {
        if (index < 0 || index >= count) return;
        if (!usable[index])
        {
            PlayAudio(cancelSfx);
            return;
        }

        PlayAudio(confirmSfx);
        Picked?.Invoke(index);
    }

    private void Redraw()
    {
        for (int i = 0; i < count; i++)
        {
            bool on = i == Selected;
            rows[i].text = (on ? "▶ " : "  ") + labels[i];
            rows[i].color = !usable[i] ? disabledColor : (on ? selectedColor : normalColor);
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main != null ? Camera.main.transform.position : Vector3.zero);
        }
    }
}
