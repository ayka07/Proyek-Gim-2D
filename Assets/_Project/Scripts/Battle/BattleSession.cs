using System;
using UnityEngine;

public class BattleSession
{
    public MonsterInstance WildMonster { get; private set; }
    public bool IsBoss { get; private set; }
    public float SafeTimeUntil { get; set; } = 0f;

    public event Action<BattleResult, bool> BattleEnded;

    public void StartBattle(MonsterInstance enemy, bool isBoss = false)
    {
        WildMonster = enemy;
        IsBoss = isBoss;
    }

    public void EndBattle(BattleResult result)
    {
        SafeTimeUntil = Time.time + 3.0f;
        BattleEnded?.Invoke(result, IsBoss);
    }
}
