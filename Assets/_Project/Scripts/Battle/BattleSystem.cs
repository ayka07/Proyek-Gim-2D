using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    private const float secondsPerChar = 0.018f;
    private const float evolveSeconds = 2.4f;

    public MonsterInstance Player { get; private set; }
    public MonsterInstance Enemy { get; private set; }
    public bool IsBoss { get; private set; }
    public PlayerParty Party { get; private set; }

    public int SkillCharges { get; set; } = 3;
    public bool LastHitCritical { get; private set; }
    public Element? LastHitElement { get; private set; }

    private SceneFlow scenes;
    private BattleAction chosen;
    private BattleResult result = BattleResult.None;
    private int enemySkillUses = 0;
    private MonsterInstance guardedMonster;

    public event Action Started;
    public event Action<string> MessageShown;
    public event Action ChoiceRequested;
    public event Action<MonsterInstance> Attacked;
    public event Action<MonsterInstance, int, bool, Element?> Hit;
    public event Action Missed;
    public event Action<MonsterInstance> Fainted;
    public event Action Victory;
    public event Action<MonsterData> LevelUp;
    public event Action<MonsterData, MonsterData> Evolving;
    public event Action<MonsterInstance> Evolved;
    public event Action<BattleResult> Ended;
    public event Action<MonsterInstance> Healed;

    public void Construct(PlayerParty playerParty, BattleSession session, SceneFlow sceneFlow)
    {
        Party = playerParty;
        Player = playerParty != null ? playerParty.Partner : null;
        Enemy = session != null ? session.WildMonster : null;
        IsBoss = session != null && session.IsBoss;
        scenes = sceneFlow;
    }

    public void Begin()
    {
        StartCoroutine(Run());
    }

    public void ChooseAction(BattleAction action)
    {
        chosen = action;
    }

    public void Escape()
    {
        result = BattleResult.Escaped;
    }

    public void SetGuard(MonsterInstance monster)
    {
        guardedMonster = monster;
    }

    public float DamageScale(MonsterInstance attacker)
    {
        float scale = 1.0f;
        // Monster liar biasa damage x 0.8
        if (attacker == Enemy && !IsBoss)
        {
            scale *= 0.8f;
        }

        MonsterInstance defender = attacker == Player ? Enemy : Player;
        if (guardedMonster == defender)
        {
            scale *= 0.5f;
        }

        return scale;
    }

    public void RaiseAttacked(MonsterInstance monster) => Attacked?.Invoke(monster);

    public void RaiseHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        LastHitCritical = critical;
        LastHitElement = element;
        Hit?.Invoke(target, damage, critical, element);
    }

    public void RaiseMissed() => Missed?.Invoke();
    public void RaiseHealed(MonsterInstance target) => Healed?.Invoke(target);

    public IEnumerator Say(string text, float hold = 0.8f)
    {
        MessageShown?.Invoke(text);
        float wait = hold + text.Length * secondsPerChar;
        for (float t = 0f; t < wait; t += Time.deltaTime)
        {
            if (t > text.Length * secondsPerChar && MenuInput.Confirm) break;
            yield return null;
        }
    }

    private BattleAction PickEnemyAction()
    {
        if (enemySkillUses < 2 && UnityEngine.Random.value < 0.35f)
        {
            enemySkillUses++;
            return new SkillAction();
        }
        return new AttackAction();
    }

    private IEnumerator Turn(BattleAction action, MonsterInstance actor, MonsterInstance target)
    {
        if (result != BattleResult.None) yield break;
        if (actor.IsFainted || target.IsFainted) yield break;

        yield return action.Execute(this, actor, target);

        if (target.IsFainted)
        {
            if (target == Enemy)
            {
                yield return Win();
            }
            else
            {
                yield return Lose();
            }
        }
    }

    private IEnumerator Win()
    {
        result = BattleResult.Win;
        Fainted?.Invoke(Enemy);
        yield return new WaitForSeconds(0.6f);

        yield return Say(IsBoss ? $"{Enemy.Data.DisplayName} kalah!" : $"{Enemy.Data.DisplayName} liar kalah!", 0.8f);
        Victory?.Invoke();

        int exp = (10 + Enemy.Level * 8) * (IsBoss ? 2 : 1);
        yield return Say($"{Player.Data.DisplayName} mendapat {exp} EXP.", 0.9f);

        foreach (GrowthEvent growth in Player.GainExp(exp))
        {
            if (growth.Type == GrowthType.LevelUp)
            {
                LevelUp?.Invoke(growth.From);
                yield return Say(growth.Message, 0.9f);
                continue;
            }

            yield return Say($"Apa? {growth.From.DisplayName} mulai bersinar...", 0.9f);
            Evolving?.Invoke(growth.From, growth.To);
            yield return new WaitForSeconds(evolveSeconds);
            Evolved?.Invoke(Player);
            yield return Say(growth.Message, 1.2f);
        }

        if (!IsBoss)
        {
            int healed = Player.Heal(Mathf.RoundToInt(Player.MaxHP * 0.25f));
            if (healed > 0)
            {
                yield return Say($"{Player.Data.DisplayName} beristirahat sejenak dan pulih {healed} HP.", 0.8f);
            }
        }
    }

    private IEnumerator Lose()
    {
        result = BattleResult.Lose;
        Fainted?.Invoke(Player);
        yield return new WaitForSeconds(0.6f);
        yield return Say($"{Player.Data.DisplayName} pingsan... Kamu berlari kembali ke desa.", 1.2f);
        Player.HealFull();
    }

    private IEnumerator Run()
    {
        Started?.Invoke();
        yield return new WaitForSeconds(0.5f);

        yield return Say(IsBoss
            ? $"{Enemy.Data.DisplayName}, penguasa Gua Petir, muncul!"
            : $"{Enemy.Data.DisplayName} liar Lv{Enemy.Level} muncul!", 0.9f);

        yield return Say($"Maju, {Player.Data.DisplayName}!", 0.7f);

        while (result == BattleResult.None)
        {
            guardedMonster = null;
            chosen = null;
            ChoiceRequested?.Invoke();

            yield return new WaitUntil(() => chosen != null);

            if (chosen is RunAction && IsBoss)
            {
                yield return chosen.Execute(this, Player, Enemy);
                continue;
            }

            if (chosen is RunAction)
            {
                yield return chosen.Execute(this, Player, Enemy);
                if (result == BattleResult.Escaped) break;
            }

            BattleAction enemyAction = PickEnemyAction();

            bool playerFirst = chosen.Priority > enemyAction.Priority
                               || (chosen.Priority == enemyAction.Priority && Player.Speed > Enemy.Speed)
                               || (chosen.Priority == enemyAction.Priority && Player.Speed == Enemy.Speed && UnityEngine.Random.value < 0.5f);

            if (playerFirst)
            {
                yield return Turn(chosen, Player, Enemy);
                if (result != BattleResult.None) break;
                yield return Turn(enemyAction, Enemy, Player);
            }
            else
            {
                yield return Turn(enemyAction, Enemy, Player);
                if (result != BattleResult.None) break;
                yield return Turn(chosen, Player, Enemy);
            }
        }

        Ended?.Invoke(result);
        yield return new WaitForSeconds(0.3f);
        scenes?.EndBattle(result);
    }
}
