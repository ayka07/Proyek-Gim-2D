using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleInstaller : SceneInstaller
{
    [SerializeField] private TextMenu menu;
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private Image[] starterImages;
    [SerializeField] private MonsterData[] starterDatas;
    [SerializeField] private RectTransform movingBackground;

    private GameServices services;
    private readonly List<string> options = new List<string>();

    public override void Install(GameServices s)
    {
        services = s;
        options.Clear();
        if (s.Save.HasSave()) options.Add("Lanjutkan");
        options.Add("Mulai baru");
        options.Add("Keluar");

        menu.Picked -= OnPicked;
        menu.Picked += OnPicked;
        menu.Open(options.ToArray());

        s.Audio?.PlayWorldMusic(titleMusic);
    }

    private void Update()
    {
        // Animate starters: 2 frames every 0.35s
        if (starterImages != null && starterDatas != null)
        {
            int frame = Mathf.FloorToInt(Time.time / 0.35f) % 2;
            for (int i = 0; i < starterImages.Length && i < starterDatas.Length; i++)
            {
                if (starterImages[i] != null && starterDatas[i] != null && starterDatas[i].Frames != null && starterDatas[i].Frames.Length > 1)
                {
                    starterImages[i].sprite = starterDatas[i].Frames[frame];
                }
            }
        }

        // Panning background: bergeser sin(t / 6) * 240
        if (movingBackground != null)
        {
            float offset = Mathf.Sin(Time.time / 6f) * 240f;
            movingBackground.anchoredPosition = new Vector2(offset, 0);
        }
    }

    private void OnPicked(int index)
    {
        string choice = options[index];
        if (choice == "Lanjutkan")
        {
            Continue();
        }
        else if (choice == "Mulai baru")
        {
            NewGame();
        }
        else if (choice == "Keluar")
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void Continue()
    {
        SaveData data = services.Save.Load();
        if (data == null)
        {
            NewGame();
            return;
        }
        SaveMapper.Apply(data, services);
        services.Save.PendingLoad = data;
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }

    private void NewGame()
    {
        services.Party.Reset();
        services.Flags.Load(new List<string>());
        services.Save.PendingLoad = null;
        services.Scenes.GoTo(SceneFlow.WorldScene);
    }
}
