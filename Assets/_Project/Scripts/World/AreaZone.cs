using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AreaZone : MonoBehaviour
{
    [SerializeField] private string areaName = "Desa Awal";
    [SerializeField] private AudioClip areaBgm;

    private PartnerHud hud;
    private AudioService audioService;

    public string AreaName => areaName;
    public AudioClip AreaBgm => areaBgm;

    public void Construct(PartnerHud partnerHud, AudioService audio)
    {
        hud = partnerHud;
        audioService = audio;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (hud != null) hud.SetArea(areaName);
        if (audioService != null && areaBgm != null)
        {
            audioService.PlayWorldMusic(areaBgm);
        }
    }
}
