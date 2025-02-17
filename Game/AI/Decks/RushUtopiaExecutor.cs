﻿using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;

namespace WindBot.Game.AI.Decks
{
    [Deck("RushUtopia", "AI_RushUtopia")]
    public class RushUtopiaExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int FireGuardian = 120110003;
            public const int AchachaCatcher = 120199005;
            public const int GagagaOutfielder = 120199004;
            public const int ZubabaBatter = 120199006;
            public const int BaseballKing = 120199028;
            public const int BaseballDragon = 120199027;
            public const int DododoSecond = 120199025;
            public const int GogogoUmpire = 120199026;
            public const int TerrorsaurSternptera = 120151011;
            public const int PhoenixDragon = 120110009;
            public const int TensionMax = 120150007;
            public const int HammerCrush = 120120041;
            public const int MonsterReborn = 120194004;
            public const int SpiritsStadium = 120199047;
            public const int DeadlyDuel = 120183060;
            public const int Request9 = 120199061;
            public const int TrumpetToMarch = 120184005;
        }

        public RushUtopiaExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Field Spell
            AddExecutor(ExecutorType.Activate, CardId.SpiritsStadium);

            AddExecutor(ExecutorType.Activate, CardId.HammerCrush);

            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEff);

            // Dododo Second
            // Treat as two tributes for Utopia / Dragon
            AddExecutor(ExecutorType.SummonOrSet, CardId.DododoSecond);
            AddExecutor(ExecutorType.Activate, CardId.DododoSecond, DododoEff);

            // Gogogo Umpire
            // Recycle Field Spell
            AddExecutor(ExecutorType.SummonOrSet, CardId.GogogoUmpire);
            AddExecutor(ExecutorType.Activate, CardId.GogogoUmpire, GogogoEff);

            // Terrorsaur Sternptera
            // Forceful Sentry go brr
            AddExecutor(ExecutorType.SummonOrSet, CardId.TerrorsaurSternptera);
            AddExecutor(ExecutorType.Activate, CardId.TerrorsaurSternptera);

            // Phoenix Dragon
            // Recycle high level dragon
            AddExecutor(ExecutorType.SummonOrSet, CardId.PhoenixDragon);
            AddExecutor(ExecutorType.Activate, CardId.PhoenixDragon, PhoenixEff);

            // Normal Monsters in order of ATK value
            AddExecutor(ExecutorType.SummonOrSet, CardId.ZubabaBatter);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AchachaCatcher);
            AddExecutor(ExecutorType.SummonOrSet, CardId.GagagaOutfielder);

            // Utopia
            // Send 2 Monsters to GY to gain ATK 
            AddExecutor(ExecutorType.Summon, CardId.BaseballKing, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.BaseballKing, BaseballKingEff);

            // Utopic Dragon
            // Recycle monsters + SS Utopia
            AddExecutor(ExecutorType.Summon, CardId.BaseballDragon, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.BaseballDragon, BaseballDragonEff);

            // Big number
            AddExecutor(ExecutorType.Summon, CardId.FireGuardian, Tribute);

            // 10sion Max
            AddExecutor(ExecutorType.Activate, CardId.TensionMax, TensionMax);

            // Normal Trap
            AddExecutor(ExecutorType.Activate, CardId.DeadlyDuel);
            AddExecutor(ExecutorType.Activate, CardId.Request9);
            AddExecutor(ExecutorType.Activate, CardId.TrumpetToMarch);

            // Set Spells/Traps to maximise advantage next turn
            AddExecutor(ExecutorType.SpellSet);
        }

        public bool DododoEffActivated = false;

        public override void OnNewTurn()
        {
            DododoEffActivated = false;
            base.OnNewTurn();
        }

        // Tribute summon handler
        private bool Tribute()
        {
            int[] lowLevel = {
                CardId.PhoenixDragon,
                CardId.TerrorsaurSternptera,
                CardId.GogogoUmpire,
                CardId.AchachaCatcher,
                CardId.GagagaOutfielder,
                CardId.ZubabaBatter,
                CardId.DododoSecond
            };

            if (Card.IsCode(CardId.BaseballDragon) || Card.IsCode(CardId.BaseballKing))
            {
                if (Bot.HasInMonstersZone(CardId.DododoSecond) && DododoEffActivated)
                {
                    // Summon with effect of Dododo Second
                    AI.SelectMaterials(CardId.DododoSecond);
                    AI.SelectYesNo(true);
                    DododoEffActivated = false;
                }
                else
                {
                    AI.SelectMaterials(lowLevel);
                    AI.SelectMaterials(lowLevel);
                }
            }
            else // It's Fire Guardian
            {
                AI.SelectMaterials(lowLevel);
            }

            return true;
        }

        // Only use on targets that are suitable
        private bool TensionMax()
        {
            if (Util.IsTurn1OrMain2())
                return false;

            // If all enemy monsters are better, test for each player monster if it would be able to hit over
            if (Util.IsAllEnemyBetter())
            {
                foreach (ClientCard mymonster in Bot.GetMonsters())
                {
                    if (mymonster.Attack + 400 > Util.GetBestEnemyCard().Attack)
                    {
                        AI.SelectCard(mymonster);
                        return true;
                    }
                }

                return false;
            }

            int[] targets = {
                CardId.BaseballDragon,
                CardId.BaseballKing,
                CardId.FireGuardian,
                CardId.ZubabaBatter,
                CardId.AchachaCatcher,
                CardId.GagagaOutfielder
            };

            AI.SelectCard(targets);

            return true;
        }

        private bool GogogoEff()
        {
            int[] gytargets = {
                CardId.FireGuardian,
                CardId.ZubabaBatter,
                CardId.AchachaCatcher,
                CardId.GagagaOutfielder
            };

            AI.SelectCard(gytargets);

            return true;
        }

        private bool DododoEff()
        {
            // Don't activate it if it's already activated
            if (DododoEffActivated)
                return false;

            if (!Bot.HasInHand(CardId.BaseballKing) && !Bot.HasInHand(CardId.BaseballDragon))
                return false;

            DododoEffActivated = true;

            return true;
        }

        private bool MonsterRebornEff()
        {
            int[] gytargets = {
                CardId.BaseballDragon,
                CardId.BaseballKing,
                CardId.FireGuardian,
                CardId.DododoSecond,
                CardId.GogogoUmpire,
                CardId.TerrorsaurSternptera,
                CardId.PhoenixDragon,
                CardId.ZubabaBatter,
                CardId.AchachaCatcher,
                CardId.GagagaOutfielder
            };

            AI.SelectCard(gytargets);

            return true;
        }

        private bool BaseballKingEff()
        {
            if (Util.IsTurn1OrMain2())
                return false;

            int[] targets = {
                CardId.DododoSecond,
                CardId.PhoenixDragon,
                CardId.TerrorsaurSternptera,
                CardId.GogogoUmpire,
                CardId.GagagaOutfielder,
                CardId.AchachaCatcher,
                CardId.ZubabaBatter
            };

            AI.SelectCard(targets);
            AI.SelectNextCard(targets);

            return true;
        }

        private bool PhoenixEff()
        {
            int[] targets = {
                CardId.BaseballDragon,
                CardId.FireGuardian
            };

            AI.SelectCard(targets);

            return true;
        }

        private bool BaseballDragonEff()
        {
            if (!Bot.HasInGraveyard(CardId.BaseballKing))
                return false;

            int[] targets = {
                CardId.BaseballDragon,
                CardId.DododoSecond,
                CardId.BaseballKing,
                CardId.GogogoUmpire,
                CardId.TerrorsaurSternptera,
                CardId.PhoenixDragon,
                CardId.FireGuardian,
                CardId.ZubabaBatter,
                CardId.AchachaCatcher,
                CardId.GagagaOutfielder,
            };

            AI.SelectCard(targets);
            AI.SelectNextCard(targets);
            AI.SelectThirdCard(targets);

            return true;
        }
    }
}
