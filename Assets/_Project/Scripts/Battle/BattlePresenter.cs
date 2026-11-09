using System.Collections.Generic;
using UnityEngine;

public class BattlePresenter
{
    private readonly BattleSystem battle;
    private readonly BattleView view;

    public BattlePresenter(BattleSystem battleSystem, BattleView battleView)
    {
        battle = battleSystem;
        view = battleView;

        battle.Started += OnStarted;
        battle.MessageShown += view.ShowMessage;
        battle.ChoiceRequested += OnChoiceRequested;
        battle.Attacked += m => PanelOf(m).PlayLunge();
        battle.Hit += OnHit;
        battle.Fainted += m => PanelOf(m).PlayFaint();
        battle.LevelUp += shown => view.PlayerPanel.Show(battle.Player, false, shown);
        battle.Evolving += (from, to) =>
        {
            view.PlayerPanel.PlayEvolve(from, to, 2.4f);
            view.PlayEvolveRings(2.4f);
        };
        battle.Evolved += m =>
        {
            view.PlayFlash();
            view.PlayerPanel.Show(m);
        };

        view.ActionClicked += OnActionClicked;
    }

    private MonsterPanel PanelOf(MonsterInstance monster)
    {
        return monster == battle.Player ? view.PlayerPanel : view.EnemyPanel;
    }

    private void OnStarted()
    {
        view.EnemyPanel.Show(battle.Enemy, battle.IsBoss);
        view.PlayerPanel.Show(battle.Player, false);
        view.PlayIntro();
    }

    private void OnChoiceRequested()
    {
        string[] labels = new string[4];
        labels[0] = "Serang";
        labels[1] = $"Jurus ({battle.SkillCharges})";
        labels[2] = $"Ramuan ({battle.Party.Potions})";
        labels[3] = "Kabur";

        bool[] usable = new bool[4];
        usable[0] = true;
        usable[1] = battle.SkillCharges > 0;
        usable[2] = battle.Party.Potions > 0;
        usable[3] = !battle.IsBoss;

        string prompt = $"Apa yang akan {battle.Player.Data.DisplayName} lakukan?";
        string hint = $"Jurus elemen: {battle.Player.Data.SkillName}";

        view.ShowMenu(labels, usable, prompt, hint);
    }

    private void OnActionClicked(int index)
    {
        switch (index)
        {
            case 0:
                battle.ChooseAction(new AttackAction());
                break;
            case 1:
                battle.SkillCharges--;
                battle.ChooseAction(new SkillAction());
                break;
            case 2:
                battle.ChooseAction(new PotionAction());
                break;
            case 3:
                battle.ChooseAction(new RunAction());
                break;
            default:
                battle.ChooseAction(new AttackAction());
                break;
        }
    }

    private void OnHit(MonsterInstance target, int damage, bool critical, Element? element)
    {
        PanelOf(target).PlayHurt();
    }
}
