using System;
using UnityEngine;

public static class SaveMapper
{
    public static SaveData Capture(GameServices services, Vector2 playerPosition)
    {
        var data = new SaveData
        {
            version = SaveService.CurrentVersion,
            savedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            scene = SceneFlow.WorldScene,
            playerPosition = playerPosition,
            potions = services != null && services.Party != null ? services.Party.Potions : 3,
            playtime = services != null && services.Party != null ? services.Party.Playtime : 0f,
            flags = services != null && services.Flags != null ? services.Flags.ToList() : new System.Collections.Generic.List<string>()
        };

        if (services != null && services.Party != null && services.Party.Partner != null)
        {
            MonsterInstance p = services.Party.Partner;
            data.partnerId = p.Data != null ? p.Data.Id : "";
            data.partnerLevel = p.Level;
            data.partnerExp = p.Exp;
            data.partnerHp = p.CurrentHP;
        }

        return data;
    }

    public static void Apply(SaveData data, GameServices services)
    {
        if (data == null || services == null) return;

        if (services.Party != null)
        {
            services.Party.SetPotions(data.potions);
            services.Party.Playtime = data.playtime;

            if (!string.IsNullOrEmpty(data.partnerId) && services.Database != null)
            {
                MonsterData template = services.Database.GetById(data.partnerId);
                if (template != null)
                {
                    var instance = new MonsterInstance(template, data.partnerLevel);
                    instance.RestoreState(template, data.partnerLevel, data.partnerHp, data.partnerExp);
                    services.Party.SetPartner(instance);
                }
            }
            else
            {
                services.Party.SetPartner(null);
            }
        }

        if (services.Flags != null)
        {
            services.Flags.Load(data.flags);
        }
    }
}
