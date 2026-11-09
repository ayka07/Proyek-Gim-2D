using UnityEngine;
using UnityEngine.UI;

public class EndInstaller : SceneInstaller
{
    [SerializeField] private Text partnerText;
    [SerializeField] private Text timeText;
    [SerializeField] private Image partnerImage;
    [SerializeField] private Button clickCatcher;

    private GameServices services;
    private MonsterInstance partner;
    private float ready;
    private bool leaving;

    public override void Install(GameServices s)
    {
        services = s;
        partner = s.Party != null ? s.Party.Partner : null;

        int total = s.Party != null ? Mathf.FloorToInt(s.Party.Playtime) : 0;
        if (partnerText != null)
        {
            partnerText.text = partner != null && partner.Data != null
                ? $"{partner.Data.DisplayName} · Lv {partner.Level}"
                : "";
        }

        if (timeText != null)
        {
            timeText.text = $"Waktu bermain {total / 60} menit {total % 60} detik";
        }

        if (clickCatcher != null)
        {
            clickCatcher.onClick.RemoveAllListeners();
            clickCatcher.onClick.AddListener(Leave);
        }

        ready = Time.unscaledTime + 0.5f;
    }

    private void Update()
    {
        if (partner != null && partnerImage != null && partner.Data != null && partner.Data.Frames != null && partner.Data.Frames.Length > 1)
        {
            int frame = Mathf.FloorToInt(Time.unscaledTime / 0.35f) % 2;
            partnerImage.sprite = partner.Data.Frames[frame];
        }

        if (MenuInput.Confirm)
        {
            Leave();
        }
    }

    private void Leave()
    {
        if (leaving || Time.unscaledTime < ready) return;
        leaving = true;
        services?.Scenes?.GoTo(SceneFlow.TitleScene);
    }
}
