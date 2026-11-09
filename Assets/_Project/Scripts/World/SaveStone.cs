using System.Collections;
using UnityEngine;

public class SaveStone : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip saveClip;
    [SerializeField] private PixelBurst burst;

    private GameServices services;

    public void Construct(GameServices s)
    {
        services = s;
    }

    public void Interact(PlayerInteractor who)
    {
        if (services != null && services.Dialogue != null && !services.Dialogue.IsRunning)
        {
            services.Dialogue.Run(Use(who.transform));
        }
    }

    private IEnumerator Use(Transform player)
    {
        if (services.Audio != null && saveClip != null)
        {
            services.Audio.PlaySfx(saveClip, 0f);
        }

        if (services.Party != null && services.Party.Partner != null)
        {
            services.Party.Partner.HealFull();
            if (burst != null)
            {
                burst.Play(player.position + Vector3.up * 0.5f, new Color32(110, 231, 242, 255), 16, 50f, 0.7f);
            }
        }

        bool saved = services.Save != null && services.Save.Save(SaveMapper.Capture(services, player.position));

        yield return services.Dialogue.Say("", saved
            ? "Batu Simpan bersinar. HP partner pulih dan perjalanan tersimpan."
            : "Batu Simpan bersinar. HP partner pulih, tetapi penyimpanan gagal.");
    }
}
