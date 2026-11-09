using System.Collections;
using UnityEngine;

public class StarterGiver : Npc
{
    [SerializeField] private MonsterData[] starters;
    [SerializeField] private int starterLevel = 3;
    [SerializeField] private string[] options = new[] { "Pyrit (Pyro)", "Blubi (Aqua)", "Sprig (Sprout)" };

    public static Color ElementColor(Element element)
    {
        switch (element)
        {
            case Element.Pyro: return new Color32(235, 87, 87, 255);
            case Element.Aqua: return new Color32(47, 128, 237, 255);
            case Element.Sprout: return new Color32(39, 174, 96, 255);
            case Element.Volt: return new Color32(242, 201, 76, 255);
            default: return Color.white;
        }
    }

    protected override IEnumerator Talk()
    {
        if (Flags.Has(StoryFlags.HasStarter))
        {
            yield return Dialogue.Say(NpcName, "Bagaimana keadaan partnermu? Berlatihlah di Hutan Rimbun!");
            yield break;
        }

        yield return Dialogue.Say(NpcName, "Ah, kamu datang juga! Hutan di utara penuh monster liar.");
        yield return Dialogue.Say(NpcName, "Tidak ada yang boleh masuk hutan tanpa partner. Pilih satu dari tiga ini.");

        MonsterData pick = null;
        while (pick == null)
        {
            int choice = 0;
            int sure = 0;

            yield return Dialogue.Choose(NpcName, "Siapa yang akan menjadi partnermu?", options, c => choice = c);
            MonsterData m = starters != null && choice >= 0 && choice < starters.Length ? starters[choice] : null;

            if (m != null)
            {
                yield return Dialogue.Choose(NpcName, $"{m.DisplayName} tipe {m.Element}. Yakin?",
                    new[] { "Ya", "Pilih lagi" }, c => sure = c);

                if (sure == 0) pick = m;
            }
        }

        Services.Party.SetPartner(new MonsterInstance(pick, starterLevel));
        Flags.Set(StoryFlags.HasStarter);

        yield return Dialogue.Say("", $"Kamu mendapat {pick.DisplayName}!");
        yield return Dialogue.Say(NpcName, "Rawat dia baik-baik. Menang bertarung membuatnya naik level, lalu berevolusi.");
        yield return Dialogue.Say(NpcName, "Batu Simpan di sebelahku menyimpan perjalananmu dan memulihkan partnermu.");

        Services.Toast?.Show("Penjaga Batas sudah membuka jalan ke hutan");
    }
}
