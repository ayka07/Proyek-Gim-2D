using System;
using UnityEngine;

public class PlayerParty
{
    public MonsterInstance Partner { get; private set; }
    public int Potions { get; private set; } = 3;
    public float Playtime { get; set; } = 0f;

    public event Action<MonsterInstance> PartnerChanged;
    public event Action<int> PotionsChanged;

    public void SetPartner(MonsterInstance monster)
    {
        Partner = monster;
        PartnerChanged?.Invoke(Partner);
    }

    public void SetPotions(int count)
    {
        Potions = Mathf.Max(0, count);
        PotionsChanged?.Invoke(Potions);
    }

    public void AddPotions(int amount)
    {
        Potions = Mathf.Max(0, Potions + amount);
        PotionsChanged?.Invoke(Potions);
    }

    public bool UsePotion()
    {
        if (Potions <= 0) return false;
        Potions--;
        PotionsChanged?.Invoke(Potions);
        return true;
    }

    public void Reset()
    {
        Partner = null;
        Potions = 3;
        Playtime = 0f;
        PartnerChanged?.Invoke(null);
        PotionsChanged?.Invoke(3);
    }

    public void TickPlaytime(float dt)
    {
        Playtime += dt;
    }
}
