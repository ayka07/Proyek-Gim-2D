using UnityEngine;

public class BattleInstaller : SceneInstaller
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite caveBackground;
    [SerializeField] private Sprite forestBackground;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private BattleView battleView;
    [SerializeField] private BattleAudio battleAudio;
    [SerializeField] private BattleFeel battleFeel;
    [SerializeField] private AudioClip battleBgm;
    [SerializeField] private AudioClip bossBgm;

    public override void Install(GameServices s)
    {
        if (s == null) return;

        bool isBoss = s.Battle != null && s.Battle.IsBoss;
        if (backgroundRenderer != null)
        {
            backgroundRenderer.sprite = isBoss ? caveBackground : forestBackground;
        }

        if (battleSystem != null)
        {
            battleSystem.Construct(s.Party, s.Battle, s.Scenes);
            new BattlePresenter(battleSystem, battleView);

            if (battleAudio != null) battleAudio.Construct(battleSystem, s.Audio);
            if (battleFeel != null) battleFeel.Construct(battleSystem);

            s.Audio?.PlayBattleMusic(isBoss ? bossBgm : battleBgm);
            battleSystem.Begin();
        }
    }
}
