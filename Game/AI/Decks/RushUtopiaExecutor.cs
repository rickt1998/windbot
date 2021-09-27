using YGOSharp.OCGWrapper.Enums;
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
            public const int HammerCrush = 120103032;
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
            AddExecutor(ExecutorType.Summon, CardId.DododoSecond);
            AddExecutor(ExecutorType.Activate, CardId.DododoSecond, DododoEff);

            // Gogogo Umpire
            // Recycle Field Spell
            AddExecutor(ExecutorType.Summon, CardId.GogogoUmpire);
            AddExecutor(ExecutorType.Activate, CardId.GogogoUmpire, GogogoEff);

            // Terrorsaur Sternptera
            // Forceful Sentry go brr
            AddExecutor(ExecutorType.Summon, CardId.TerrorsaurSternptera);
            AddExecutor(ExecutorType.Activate, CardId.TerrorsaurSternptera);

            // Phoenix Dragon
            // Recycle high level dragon
            AddExecutor(ExecutorType.Summon, CardId.PhoenixDragon);
            AddExecutor(ExecutorType.Activate, CardId.PhoenixDragon, PhoenixEff);

            // Normal Monsters in order of ATK value
            AddExecutor(ExecutorType.Summon, CardId.ZubabaBatter);
            AddExecutor(ExecutorType.Summon, CardId.AchachaCatcher);
            AddExecutor(ExecutorType.Summon, CardId.GagagaOutfielder);

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
            AddExecutor(ExecutorType.SpellSet, CardId.MonsterReborn);
            AddExecutor(ExecutorType.SpellSet, CardId.HammerCrush);
            AddExecutor(ExecutorType.SpellSet, CardId.TensionMax);
            AddExecutor(ExecutorType.SpellSet, CardId.DeadlyDuel);
            AddExecutor(ExecutorType.SpellSet, CardId.Request9);
            AddExecutor(ExecutorType.SpellSet, CardId.TrumpetToMarch);
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
                if (Bot.HasInMonstersZone(CardId.DododoSecond, faceUp: true) && DododoEffActivated)
                {
                    AI.SelectCard(CardId.DododoSecond);
                    AI.SelectYesNo(true);
                    return true;
                }
                else
                {
                    AI.SelectCard(lowLevel);
                    AI.SelectNextCard(lowLevel);
                    return true;
                }
            }
            else // It's Fire Guardian
            {
                AI.SelectCard(lowLevel);
                return true;
            }
        }

        // Only use on targets that are suitable
        private bool TensionMax()
        {
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
            {
                return false;
            }
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
