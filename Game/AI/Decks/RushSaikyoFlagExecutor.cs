using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("RushSaikyoFlag", "AI_RushSaikyoFlag")]
    public class RushSaikyoFlagExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int BullBreaker = 120155006;
            public const int CyberFalcon = 120160002;
            public const int AvanWolf = 120193002;
            public const int BrandNew = 120183002;
            public const int AceBreaker = 120181001;
            public const int MirrorInnovator = 120155015;
            public const int AssaultArmored = 120183031;
            public const int CrafterDrone = 120183030;
            public const int SurgeBicorn = 120181011;
            public const int AimEagle = 120193003;
            public const int TensionMax = 120150007;
            public const int GracefulCharity = 120196049;
            public const int IronOnslaught = 120183054;
            public const int BattleDemotion = 120183063;
            public const int FistOfTheBeast = 120155060;
        }

        public RushSaikyoFlagExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.IronOnslaught);
            AddExecutor(ExecutorType.SummonOrSet, CardId.BullBreaker);
            AddExecutor(ExecutorType.SummonOrSet, CardId.CyberFalcon);
            AddExecutor(ExecutorType.SummonOrSet, CardId.AvanWolf);
            AddExecutor(ExecutorType.SummonOrSet, CardId.BrandNew);

            AddExecutor(ExecutorType.SummonOrSet, CardId.AssaultArmored);
            AddExecutor(ExecutorType.Activate, CardId.AssaultArmored, AssaultArmoredEff);

            AddExecutor(ExecutorType.SummonOrSet, CardId.CrafterDrone);
            AddExecutor(ExecutorType.Activate, CardId.CrafterDrone, CrafterDroneEff);

            AddExecutor(ExecutorType.SummonOrSet, CardId.SurgeBicorn);
            AddExecutor(ExecutorType.Activate, CardId.SurgeBicorn, SurgeBicornEff);

            AddExecutor(ExecutorType.SummonOrSet, CardId.AimEagle);
            AddExecutor(ExecutorType.Activate, CardId.AimEagle, AimEagleEff);

            AddExecutor(ExecutorType.Summon, CardId.AceBreaker, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.AceBreaker, AceBreakerEff);

            AddExecutor(ExecutorType.Summon, CardId.MirrorInnovator, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.MirrorInnovator, MirrorInnovatorEff);

            AddExecutor(ExecutorType.Activate, CardId.TensionMax, TensionMaxEff);
            AddExecutor(ExecutorType.Activate, CardId.BattleDemotion);
            AddExecutor(ExecutorType.Activate, CardId.FistOfTheBeast);

            // Set traps
            AddExecutor(ExecutorType.SpellSet, CardId.IronOnslaught);
            AddExecutor(ExecutorType.SpellSet, CardId.BattleDemotion);
            AddExecutor(ExecutorType.SpellSet, CardId.TensionMax);
            AddExecutor(ExecutorType.SpellSet, CardId.GracefulCharity);
            AddExecutor(ExecutorType.SpellSet, CardId.FistOfTheBeast);
        }

        public bool SurgeBicornEffActivated = false;

        private bool Tribute()
        {
            int[] lowLevel = {
                CardId.BullBreaker,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.AvanWolf,
                CardId.AimEagle,
                CardId.CrafterDrone,
                CardId.AssaultArmored
            };

            if (Bot.HasInMonstersZone(CardId.SurgeBicorn, faceUp: true) && SurgeBicornEffActivated)
            {
                AI.SelectCard(CardId.SurgeBicorn);
                AI.SelectYesNo(true);
            }
            else
            {
                AI.SelectCard(lowLevel);
                AI.SelectNextCard(lowLevel);
            }
            return true;
        }

        private bool TensionMaxEff()
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
                CardId.AceBreaker,
                CardId.MirrorInnovator,
                CardId.AvanWolf
            };

            AI.SelectCard(targets);

            return true;
        }

        private bool AceBreakerEff()
        {
            int[] handtargets = {
                CardId.BullBreaker,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.AvanWolf,
                CardId.AimEagle,
                CardId.CrafterDrone,
                CardId.AssaultArmored
            };

            // Select cards from hand to send to gy
            AI.SelectCard(handtargets);
            AI.SelectNextCard(handtargets);

            // Select opponent's monster to destroy
            ClientCard target = Util.GetProblematicEnemyMonster();
            AI.SelectCard(target);

            return true;

        }

        private bool CrafterDroneEff()
        {
            int[] handtargets = {
                CardId.BullBreaker,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.AvanWolf,
                CardId.AimEagle
            };

            AI.SelectCard(handtargets);

            return true;
        }

        private bool SurgeBicornEff()
        {
            if (!Bot.HasInHand(CardId.AceBreaker) && !Bot.HasInHand(CardId.MirrorInnovator))
                return false;

            int[] gytargets = {
                CardId.AvanWolf,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.BullBreaker,
                CardId.AssaultArmored,
                CardId.CrafterDrone,
                CardId.AimEagle
            };

            AI.SelectCard(gytargets);
            AI.SelectNextCard(gytargets);
            AI.SelectThirdCard(gytargets);

            SurgeBicornEffActivated = true;
            Console.WriteLine("Surge Bicorn Eff Activated");

            return true;
        }

        private bool MirrorInnovatorEff()
        {
            if (Util.IsTurn1OrMain2())
                return false;

            int[] gytargets = {
                CardId.AceBreaker,
                CardId.MirrorInnovator,
                CardId.AvanWolf,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.BullBreaker,
                CardId.AssaultArmored,
                CardId.CrafterDrone,
                CardId.AimEagle
            };

            // TODO: stop being lazy and fix this
            if (Enemy.HasDefendingMonster()) // Send one => piercing damage
            {
                AI.SelectCard(gytargets);
            }
            else // Send up to three => bigger number
            {
                int gymonstercount = Bot.Graveyard.GetMonsters().Count;
                if (gymonstercount >= 1)
                    AI.SelectCard(gytargets);
                if (gymonstercount >= 2)
                    AI.SelectNextCard(gytargets);
                if (gymonstercount >= 3)
                    AI.SelectThirdCard(gytargets);
            }

            return true;
        }

        private bool AimEagleEff()
        {
            int[] sendtargets = {
                CardId.BullBreaker,
                CardId.CyberFalcon,
                CardId.BrandNew,
                CardId.AvanWolf,
                CardId.AimEagle,
                CardId.CrafterDrone,
                CardId.AssaultArmored
            };

            AI.SelectCard(sendtargets);
            AI.SelectNextCard(sendtargets);

            return true;
        }

        private bool AssaultArmoredEff()
        {
            ClientCard target = Util.GetProblematicEnemyMonster();
            AI.SelectCard(target);

            return true;
        }
    }
}