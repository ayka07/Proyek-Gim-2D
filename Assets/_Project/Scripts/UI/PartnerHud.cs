using UnityEngine;
using UnityEngine.UI;

public class PartnerHud : MonoBehaviour
{
    [SerializeField] private GameObject partnerPanel;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Image hpFill;
    [SerializeField] private Text hpText;
    [SerializeField] private Image expFill;
    [SerializeField] private Text areaText;
    [SerializeField] private Text potionText;

    private PlayerParty party;
    private MonsterInstance currentPartner;

    public void Construct(PlayerParty playerParty)
    {
        party = playerParty;
        if (party != null)
        {
            party.PartnerChanged += OnPartnerChanged;
            party.PotionsChanged += OnPotionsChanged;
            OnPartnerChanged(party.Partner);
            OnPotionsChanged(party.Potions);
        }
    }

    private void OnDestroy()
    {
        if (party != null)
        {
            party.PartnerChanged -= OnPartnerChanged;
            party.PotionsChanged -= OnPotionsChanged;
        }
    }

    public void SetArea(string areaName)
    {
        if (areaText != null) areaText.text = areaName;
    }

    private void OnPartnerChanged(MonsterInstance monster)
    {
        currentPartner = monster;
        if (monster == null)
        {
            if (partnerPanel != null) partnerPanel.SetActive(false);
            return;
        }

        if (partnerPanel != null) partnerPanel.SetActive(true);
        if (nameText != null) nameText.text = monster.Data.DisplayName;
        if (levelText != null) levelText.text = $"Lv{monster.Level}";
        if (portraitImage != null) portraitImage.sprite = monster.Data.Portrait;

        UpdateHpAndExp();
    }

    private void OnPotionsChanged(int count)
    {
        if (potionText != null) potionText.text = $"Ramuan ×{count}";
    }

    private void Update()
    {
        if (currentPartner != null)
        {
            UpdateHpAndExp();
        }
    }

    private void UpdateHpAndExp()
    {
        if (currentPartner == null) return;

        float hpRatio = currentPartner.MaxHP > 0 ? (float)currentPartner.CurrentHP / currentPartner.MaxHP : 0f;
        if (hpFill != null)
        {
            hpFill.fillAmount = hpRatio;
            hpFill.color = HpColors.For(hpRatio);
        }
        if (hpText != null)
        {
            hpText.text = $"{currentPartner.CurrentHP}/{currentPartner.MaxHP}";
        }

        float expRatio = currentPartner.ExpToNext > 0 ? (float)currentPartner.Exp / currentPartner.ExpToNext : 0f;
        if (expFill != null)
        {
            expFill.fillAmount = expRatio;
            expFill.color = UiTheme.Exp;
        }
    }
}
