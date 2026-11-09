using System.Collections.Generic;
using UnityEngine;

public enum GrowthType
{
    LevelUp,
    Evolve
}

public struct GrowthEvent
{
    public GrowthType Type;
    public MonsterData From;
    public MonsterData To;
    public string Message;
}

[System.Serializable]
public class MonsterInstance
{
    [SerializeField] private MonsterData data;
    [SerializeField] private int level;
    [SerializeField] private int currentHP;
    [SerializeField] private int exp;

    public MonsterData Data => data;
    public int Level => level;
    public int CurrentHP => currentHP;
    public int Exp => exp;

    public float Scale => 1f + (level - 1) * 0.1f;
    public int MaxHP => data != null ? Mathf.RoundToInt(data.BaseHP * Scale) + level * 2 : 1;
    public int Attack => data != null ? Mathf.RoundToInt(data.BaseATK * Scale) : 1;
    public int Defense => data != null ? Mathf.RoundToInt(data.BaseDEF * Scale) : 1;
    public int Speed => data != null ? Mathf.RoundToInt(data.BaseSPD * Scale) : 1;
    public int ExpToNext => level * 10;
    public bool IsFainted => currentHP <= 0;

    public MonsterInstance(MonsterData templateData, int initialLevel = 3)
    {
        data = templateData;
        level = Mathf.Max(1, initialLevel);
        currentHP = MaxHP;
        exp = 0;
    }

    public void RestoreState(MonsterData templateData, int lvl, int hp, int xp)
    {
        data = templateData;
        level = Mathf.Max(1, lvl);
        currentHP = Mathf.Clamp(hp, 0, MaxHP);
        exp = xp;
    }

    public int TakeDamage(int damage)
    {
        int actual = Mathf.Min(currentHP, damage);
        currentHP = Mathf.Clamp(currentHP - damage, 0, MaxHP);
        return actual;
    }

    public int Heal(int amount)
    {
        int prev = currentHP;
        currentHP = Mathf.Clamp(currentHP + amount, 0, MaxHP);
        return currentHP - prev;
    }

    public void HealFull()
    {
        currentHP = MaxHP;
    }

    public List<GrowthEvent> GainExp(int amount)
    {
        var events = new List<GrowthEvent>();
        exp += amount;

        while (exp >= ExpToNext)
        {
            exp -= ExpToNext;
            int prevMaxHP = MaxHP;
            level++;
            int hpIncrease = MaxHP - prevMaxHP;
            currentHP += hpIncrease;

            events.Add(new GrowthEvent
            {
                Type = GrowthType.LevelUp,
                From = data,
                To = data,
                Message = $"{data.DisplayName} naik ke Lv {level}!"
            });

            // Check evolution
            if (data.EvolvesTo != null && level >= data.EvolveLevel && data.EvolveLevel > 0)
            {
                MonsterData prevData = data;
                data = data.EvolvesTo;
                int afterEvolveMaxHP = MaxHP;
                currentHP += (afterEvolveMaxHP - prevMaxHP);

                events.Add(new GrowthEvent
                {
                    Type = GrowthType.Evolve,
                    From = prevData,
                    To = data,
                    Message = $"{prevData.DisplayName} berevolusi menjadi {data.DisplayName}!"
                });
            }
        }

        return events;
    }
}
