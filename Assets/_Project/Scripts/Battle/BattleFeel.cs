using System.Collections;
using UnityEngine;

public class BattleFeel : MonoBehaviour
{
    [SerializeField] private Transform cameraRig;
    [SerializeField] private SpriteRenderer playerBody;
    [SerializeField] private SpriteRenderer enemyBody;
    [SerializeField] private VfxPool hitVfx;
    [SerializeField] private PixelBurst burst;

    private BattleSystem battle;
    private Vector3 cameraHome;

    public void Construct(BattleSystem battleSystem)
    {
        battle = battleSystem;
        if (cameraRig != null) cameraHome = cameraRig.localPosition;
        else if (Camera.main != null)
        {
            cameraRig = Camera.main.transform;
            cameraHome = cameraRig.localPosition;
        }

        if (battle != null)
        {
            battle.Hit += OnHit;
            battle.Healed += OnHealed;
        }
    }

    private void OnHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        SpriteRenderer body = target == battle.Player ? playerBody : enemyBody;
        Vector3 pos = body != null ? body.bounds.center : Vector3.zero;

        if (hitVfx != null) hitVfx.Play(pos);

        Color c = battle.LastHitElement.HasValue
            ? GetElementColor(battle.LastHitElement.Value)
            : Color.white;

        if (burst != null)
        {
            burst.Play(pos, c, battle.LastHitElement.HasValue ? 22 : 12, 90f, 0.5f);
        }

        StartCoroutine(Shake(battle.LastHitCritical ? 0.35f : 0.2f));
        StartCoroutine(HitStop());
    }

    private Color GetElementColor(Element element)
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

    private IEnumerator Shake(float duration)
    {
        if (cameraRig == null) yield break;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float strength = (1f - (elapsed / duration)) * 0.15f;
            cameraRig.localPosition = cameraHome + (Vector3)(Random.insideUnitCircle * strength);
            yield return null;
        }
        cameraRig.localPosition = cameraHome;
    }

    private IEnumerator HitStop()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSecondsRealtime(0.06f);
        Time.timeScale = 1f;
    }

    private void OnHealed(MonsterInstance t)
    {
        if (burst != null && playerBody != null)
        {
            burst.Play(playerBody.bounds.center, HpColors.High, 16, 50f, 0.6f);
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        if (battle != null)
        {
            battle.Hit -= OnHit;
            battle.Healed -= OnHealed;
        }
    }
}
