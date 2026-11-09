using System.Collections;
using UnityEngine;

public class MerchantNpc : Npc
{
    [SerializeField] private AudioClip giftSfx;

    protected override IEnumerator Talk()
    {
        if (!Flags.Has(StoryFlags.Gift))
        {
            yield return Dialogue.Say(NpcName, "Petualang baru? Bawa 3 Ramuan ini. Gratis untuk hari pertama!");
            Services.Party?.AddPotions(3);
            Flags.Set(StoryFlags.Gift);
            if (giftSfx != null && Services.Audio != null)
            {
                Services.Audio.PlaySfx(giftSfx);
            }
            Services.Toast?.Show("Mendapat 3 Ramuan!");
        }
        else
        {
            yield return Dialogue.Say(NpcName, "Gunakan ramuanmu dengan bijak saat bertualang!");
        }
    }
}
