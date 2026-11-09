using UnityEngine;

public class BattleAudio : MonoBehaviour
{
    [SerializeField] private AudioClip hitSfx;
    [SerializeField] private AudioClip hitCriticalSfx;
    [SerializeField] private AudioClip missSfx;
    [SerializeField] private AudioClip faintSfx;
    [SerializeField] private AudioClip victorySfx;
    [SerializeField] private AudioClip levelUpSfx;
    [SerializeField] private AudioClip evolveSfx;
    [SerializeField] private AudioClip healSfx;

    private BattleSystem battle;
    private AudioService audioService;

    public void Construct(BattleSystem battleSystem, AudioService audio)
    {
        battle = battleSystem;
        audioService = audio;

        if (battle != null)
        {
            battle.Hit += OnHit;
            battle.Missed += OnMissed;
            battle.Fainted += OnFainted;
            battle.Victory += OnVictory;
            battle.LevelUp += OnLevelUp;
            battle.Evolving += OnEvolving;
            battle.Healed += OnHealed;
        }
    }

    private void OnDestroy()
    {
        if (battle != null)
        {
            battle.Hit -= OnHit;
            battle.Missed -= OnMissed;
            battle.Fainted -= OnFainted;
            battle.Victory -= OnVictory;
            battle.LevelUp -= OnLevelUp;
            battle.Evolving -= OnEvolving;
            battle.Healed -= OnHealed;
        }
    }

    private void OnHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        audioService?.PlaySfx(critical ? hitCriticalSfx : hitSfx);
    }

    private void OnMissed()
    {
        audioService?.PlaySfx(missSfx);
    }

    private void OnFainted(MonsterInstance target)
    {
        audioService?.PlaySfx(faintSfx);
    }

    private void OnVictory()
    {
        audioService?.PlaySfx(victorySfx);
    }

    private void OnLevelUp(MonsterData data)
    {
        audioService?.PlaySfx(levelUpSfx);
    }

    private void OnEvolving(MonsterData from, MonsterData to)
    {
        audioService?.PlaySfx(evolveSfx);
    }

    private void OnHealed(MonsterInstance target)
    {
        audioService?.PlaySfx(healSfx);
    }
}
