using System;
using System.Collections.Generic;

public class StoryFlags
{
    public const string HasStarter = "has_starter";
    public const string Gift = "gift";
    public const string IntroSeen = "intro_seen";
    public const string BossDefeated = "boss_defeated";

    private readonly HashSet<string> flags = new HashSet<string>();

    public event Action<string> Changed;

    public bool Has(string flag) => flags.Contains(flag);

    public void Set(string flag)
    {
        if (flags.Add(flag))
        {
            Changed?.Invoke(flag);
        }
    }

    public void Clear(string flag)
    {
        if (flags.Remove(flag))
        {
            Changed?.Invoke(flag);
        }
    }

    public List<string> ToList() => new List<string>(flags);

    public void Load(IEnumerable<string> savedFlags)
    {
        flags.Clear();
        if (savedFlags != null)
        {
            foreach (var f in savedFlags)
            {
                flags.Add(f);
                Changed?.Invoke(f);
            }
        }
    }
}
