using System.Collections;
using UnityEngine;

public class WorldInstaller : SceneInstaller
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private PartnerFollower partnerFollower;
    [SerializeField] private PartnerHud partnerHud;
    [SerializeField] private Vector2 respawnPoint = new Vector2(21.5f, 6f);
    [SerializeField] private AudioClip villageBgm;

    private GameServices services;

    public override void Install(GameServices s)
    {
        services = s;
        if (s == null) return;

        if (player != null)
        {
            player.Construct(s.Input);
            if (cameraFollow != null) cameraFollow.SetTarget(player.transform);
            if (partnerFollower != null)
            {
                partnerFollower.SetTarget(player.transform);
                s.Party.PartnerChanged -= partnerFollower.SetMonster;
                s.Party.PartnerChanged += partnerFollower.SetMonster;
                partnerFollower.SetMonster(s.Party.Partner);
            }
        }

        if (interactor != null)
        {
            interactor.Construct(s.Input);
        }

        if (partnerHud != null)
        {
            partnerHud.Construct(s.Party);
            partnerHud.SetArea("Desa Awal");
        }

        // Inject into NPCs
        Npc[] npcs = FindObjectsByType<Npc>(FindObjectsSortMode.None);
        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i].Construct(s);
        }

        // Inject into SaveStones
        SaveStone[] stones = FindObjectsByType<SaveStone>(FindObjectsSortMode.None);
        for (int i = 0; i < stones.Length; i++)
        {
            stones[i].Construct(s);
        }

        // Inject into FlagToggles
        FlagToggle[] toggles = FindObjectsByType<FlagToggle>(FindObjectsSortMode.None);
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].Construct(s.Flags);
        }

        // Inject into AreaZones
        AreaZone[] areas = FindObjectsByType<AreaZone>(FindObjectsSortMode.None);
        for (int i = 0; i < areas.Length; i++)
        {
            areas[i].Construct(partnerHud, s.Audio);
        }

        // Inject into Wild Monsters
        WildMonster[] wildMonsters = FindObjectsByType<WildMonster>(FindObjectsSortMode.None);
        for (int i = 0; i < wildMonsters.Length; i++)
        {
            wildMonsters[i].Construct(s, player != null ? player.transform : null);
        }

        // Listen for battle ends
        if (s.Battle != null)
        {
            s.Battle.BattleEnded -= OnBattleEnded;
            s.Battle.BattleEnded += OnBattleEnded;
        }

        s.Audio?.PlayWorldMusic(villageBgm);

        // Handle PendingLoad or Intro
        if (s.Save != null && s.Save.PendingLoad != null)
        {
            if (player != null) player.Teleport(s.Save.PendingLoad.playerPosition, Vector2.down);
            s.Save.PendingLoad = null;
            s.Toast?.Show("Perjalanan dilanjutkan");
        }
        else if (s.Flags != null && !s.Flags.Has(StoryFlags.IntroSeen))
        {
            if (s.Scenes != null) s.Scenes.HoldFade = true;
            StartCoroutine(Intro());
        }
    }

    private void OnDestroy()
    {
        if (services != null && services.Battle != null)
        {
            services.Battle.BattleEnded -= OnBattleEnded;
        }
        if (services != null && services.Party != null && partnerFollower != null)
        {
            services.Party.PartnerChanged -= partnerFollower.SetMonster;
        }
    }

    private void OnBattleEnded(BattleResult result, bool wasBoss)
    {
        if (result == BattleResult.Lose)
        {
            if (player != null) player.Teleport(respawnPoint, Vector2.up);
            services?.Toast?.Show("Kamu sadar di depan Batu Simpan.");
        }
        else if (result == BattleResult.Win && wasBoss)
        {
            StartCoroutine(Ending());
        }
    }

    private IEnumerator Intro()
    {
        yield return services.Dialogue.Narrate("Di ujung Pulau Batam ada desa kecil yang hidup berdampingan dengan monster.");
        yield return services.Dialogue.Narrate("Hari ini adalah hari di mana kamu akan memilih monstermu yang pertama.");

        services.Flags.Set(StoryFlags.IntroSeen);
        yield return services.Scenes.ReleaseFade();
        services.Toast?.Show("Temui Tetua Rahmat (kanan jalan desa)");
    }

    private IEnumerator Ending()
    {
        while (services.Scenes.IsBusy) yield return null;

        yield return services.Dialogue.Narrate("Raijugar menunduk dan membiarkanmu lewat...");
        yield return services.Dialogue.Narrate("Kamu berhasil menaklukkan Gua Petir dan membuktikan kekuatan ikatanmu dengan partnermu!");

        services.Flags.Set(StoryFlags.BossDefeated);
        if (player != null)
        {
            services.Save.Save(SaveMapper.Capture(services, player.transform.position));
        }

        services.Scenes.GoTo(SceneFlow.EndScene);
    }
}
