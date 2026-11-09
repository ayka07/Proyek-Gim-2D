using System.Collections;
using UnityEngine;

public abstract class BattleAction
{
    public virtual int Priority => 0;
    public abstract IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target);
}

public class AttackAction : BattleAction
{
    protected virtual int Power => 14;
    protected virtual string Move(MonsterInstance user) => null;
    protected virtual float ElementBonus(MonsterInstance user, MonsterInstance target) => 1.0f;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        string who = (user == battle.Enemy && !battle.IsBoss ? "Lawan " : "") + user.Data.DisplayName;
        string move = Move(user);

        yield return battle.Say(move == null ? $"{who} menyerang!" : $"{who} memakai {move}!", 0.7f);

        battle.RaiseAttacked(user);
        yield return new WaitForSeconds(0.2f);

        if (Random.value < 0.06f)
        {
            battle.RaiseMissed();
            yield return battle.Say("Serangan meleset!", 0.8f);
            yield break;
        }

        float bonus = ElementBonus(user, target);
        int damage = DamageFormula.Calculate(user, target, Power, bonus * battle.DamageScale(user), out bool critical);

        target.TakeDamage(damage);
        battle.RaiseHit(target, damage, critical, move != null ? user.Data.Element : (Element?)null);
        yield return new WaitForSeconds(0.45f);

        if (critical) yield return battle.Say("Serangan kritis!", 0.7f);
        if (bonus > 1f) yield return battle.Say("Sangat efektif!", 0.7f);
        else if (bonus < 1f && move != null) yield return battle.Say("Kurang efektif...", 0.7f);
    }
}

public class SkillAction : AttackAction
{
    protected override int Power => 22;
    protected override string Move(MonsterInstance user) => user.Data.SkillName;
    protected override float ElementBonus(MonsterInstance user, MonsterInstance target)
    {
        return ElementChart.GetMultiplier(user.Data.Element, target.Data.Element);
    }
}

public class PotionAction : BattleAction
{
    public override int Priority => 1;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (battle.Party != null && battle.Party.UsePotion())
        {
            int healAmount = Mathf.RoundToInt(user.MaxHP * 0.4f);
            int actual = user.Heal(healAmount);
            battle.RaiseHealed(user);
            yield return battle.Say($"{user.Data.DisplayName} menggunakan Ramuan dan pulih {actual} HP!", 0.8f);
        }
        else
        {
            yield return battle.Say("Ramuan habis!", 0.8f);
        }
    }
}

public class RunAction : BattleAction
{
    public override int Priority => 2;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        if (battle.IsBoss)
        {
            yield return battle.Say("Tidak bisa kabur dari pertempuran ini!", 0.8f);
            yield break;
        }

        if (Random.value < 0.70f)
        {
            yield return battle.Say("Berhasil melarikan diri!", 0.8f);
            battle.Escape();
        }
        else
        {
            yield return battle.Say("Gagal melarikan diri!", 0.8f);
        }
    }
}

public class GuardAction : BattleAction
{
    public override int Priority => 1;

    public override IEnumerator Execute(BattleSystem battle, MonsterInstance user, MonsterInstance target)
    {
        battle.SetGuard(user);
        yield return battle.Say($"{user.Data.DisplayName} bersiap bertahan!", 0.7f);
    }
}
