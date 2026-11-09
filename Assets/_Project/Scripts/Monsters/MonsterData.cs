using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/MonsterData")]
public class MonsterData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private Element element;
    [SerializeField] private int stage = 1;
    [SerializeField] private int baseHP = 40;
    [SerializeField] private int baseATK = 10;
    [SerializeField] private int baseDEF = 10;
    [SerializeField] private int baseSPD = 10;
    [SerializeField] private MonsterData evolvesTo;
    [SerializeField] private int evolveLevel = 0;
    [SerializeField] private Element strongAgainst;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private Sprite portrait;
    [SerializeField] private string customSkillName;

    public string Id { get => id; set => id = value; }
    public string DisplayName { get => displayName; set => displayName = value; }
    public Element Element { get => element; set => element = value; }
    public int Stage { get => stage; set => stage = value; }
    public int BaseHP { get => baseHP; set => baseHP = value; }
    public int BaseATK { get => baseATK; set => baseATK = value; }
    public int BaseDEF { get => baseDEF; set => baseDEF = value; }
    public int BaseSPD { get => baseSPD; set => baseSPD = value; }
    public MonsterData EvolvesTo { get => evolvesTo; set => evolvesTo = value; }
    public int EvolveLevel { get => evolveLevel; set => evolveLevel = value; }
    public Element StrongAgainst { get => strongAgainst; set => strongAgainst = value; }
    public Sprite[] Frames { get => frames; set => frames = value; }
    public Sprite Portrait { get => portrait != null ? portrait : (frames != null && frames.Length > 0 ? frames[0] : null); set => portrait = value; }

    public string SkillName
    {
        get
        {
            if (!string.IsNullOrEmpty(customSkillName)) return customSkillName;
            switch (element)
            {
                case Element.Pyro: return "Semburan Api";
                case Element.Aqua: return "Semprotan Air";
                case Element.Sprout: return "Cambuk Daun";
                case Element.Volt: return "Sengatan Petir";
                default: return "Serangan Elemen";
            }
        }
    }
}
