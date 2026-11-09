public enum Element
{
    Pyro,
    Aqua,
    Sprout,
    Volt
}

public static class ElementChart
{
    public static float GetMultiplier(Element attacker, Element defender)
    {
        if (attacker == Element.Pyro && defender == Element.Sprout) return 1.5f;
        if (attacker == Element.Sprout && defender == Element.Aqua) return 1.5f;
        if (attacker == Element.Aqua && defender == Element.Pyro) return 1.5f;
        if (attacker == Element.Volt && defender == Element.Aqua) return 1.5f;

        if (attacker == Element.Pyro && defender == Element.Aqua) return 0.75f;
        if (attacker == Element.Aqua && defender == Element.Sprout) return 0.75f;
        if (attacker == Element.Sprout && defender == Element.Pyro) return 0.75f;
        if (attacker == Element.Aqua && defender == Element.Volt) return 0.75f;

        return 1.0f;
    }
}
