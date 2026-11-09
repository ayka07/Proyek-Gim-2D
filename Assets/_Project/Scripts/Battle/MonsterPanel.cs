using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterPanel : MonoBehaviour
{
    [SerializeField] private SpriteRenderer body;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text elementText;
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expFill;
    [SerializeField] private bool faceRight = false;

    private MonsterInstance shown;
    private MonsterData shownData;
    private Vector3 home;
    private float hpShow;
    private float lungeStart = -10f;
    private bool hurting;
    private bool evolving;

    public SpriteRenderer BodyRenderer => body;

    private void Awake()
    {
        if (body != null) home = body.transform.localPosition;
    }

    public void Show(MonsterInstance monster, bool isBoss = false, MonsterData displayAs = null)
    {
        shown = monster;
        if (monster == null) return;

        shownData = displayAs != null ? displayAs : monster.Data;
        hpShow = monster.CurrentHP;

        if (nameText != null)
        {
            nameText.text = (isBoss ? "★ " : "") + shownData.DisplayName;
        }

        if (levelText != null)
        {
            levelText.text = $"Lv{monster.Level}";
        }

        if (elementText != null)
        {
            elementText.text = shownData.Element.ToString();
        }

        if (expFill != null)
        {
            float expRatio = monster.ExpToNext > 0 ? (float)monster.Exp / monster.ExpToNext : 0f;
            expFill.fillAmount = expRatio;
            expFill.color = UiTheme.Exp;
        }

        if (body != null && shownData.Frames != null && shownData.Frames.Length > 0)
        {
            body.sprite = shownData.Frames[0];
            body.color = Color.white;
            body.transform.localPosition = home;
        }
    }

    private void Update()
    {
        if (shown == null || shownData == null) return;

        if (!hurting && !evolving && body != null && shownData.Frames != null && shownData.Frames.Length > 1)
        {
            body.sprite = shownData.Frames[Mathf.FloorToInt(Time.time / 0.35f) % 2];
        }

        float k = 1f - Mathf.Pow(0.85f, Time.deltaTime * 60f);
        hpShow = Mathf.Lerp(hpShow, shown.CurrentHP, k);

        if (hpFill != null)
        {
            hpFill.fillAmount = shown.CurrentHP <= 0 ? 0f : hpShow / shown.MaxHP;
            hpFill.color = HpColors.For(hpFill.fillAmount);
        }

        if (hpText != null)
        {
            hpText.text = $"{Mathf.RoundToInt(hpShow)}/{shown.MaxHP}";
        }

        if (body != null)
        {
            float p = (Time.time - lungeStart) / 0.26f;
            float dx = p < 1f ? Mathf.Sin(p * Mathf.PI) * 0.875f * (faceRight ? 1f : -1f) : 0f;
            body.transform.localPosition = home + new Vector3(dx, 0f, 0f);
        }
    }

    public void PlayLunge()
    {
        lungeStart = Time.time;
    }

    public void PlayHurt()
    {
        StartCoroutine(HurtRoutine());
    }

    private IEnumerator HurtRoutine()
    {
        hurting = true;
        if (body != null)
        {
            Color orig = body.color;
            for (int i = 0; i < 4; i++)
            {
                body.color = new Color(1f, 1f, 1f, 0.2f);
                yield return new WaitForSeconds(0.05f);
                body.color = orig;
                yield return new WaitForSeconds(0.05f);
            }
        }
        hurting = false;
    }

    public void PlayFaint()
    {
        StartCoroutine(FaintRoutine());
    }

    private IEnumerator FaintRoutine()
    {
        if (body == null) yield break;
        Vector3 startPos = body.transform.localPosition;
        Vector3 endPos = startPos - new Vector3(0, 1.25f, 0); // ~20 px
        Color startCol = body.color;
        float dur = 0.5f;
        float elapsed = 0f;

        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dur;
            body.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            body.color = new Color(startCol.r, startCol.g, startCol.b, 1f - t);
            yield return null;
        }

        body.gameObject.SetActive(false);
    }

    public void PlayEvolve(MonsterData from, MonsterData to, float duration)
    {
        StartCoroutine(EvolveRoutine(from, to, duration));
    }

    private IEnumerator EvolveRoutine(MonsterData from, MonsterData to, float duration)
    {
        evolving = true;
        float elapsed = 0f;
        float interval = 0.2f;

        while (elapsed < duration)
        {
            elapsed += interval;
            body.sprite = from.Frames[0];
            yield return new WaitForSeconds(interval);

            body.sprite = to.Frames[0];
            yield return new WaitForSeconds(interval);

            interval = Mathf.Max(0.04f, interval * 0.8f);
        }

        evolving = false;
        shownData = to;
    }
}
