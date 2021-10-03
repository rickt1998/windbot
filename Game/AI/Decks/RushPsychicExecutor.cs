using System.Collections.Generic;

namespace WindBot.Game.AI.Decks
{
    [Deck("RushPsychic", "AI_RushPsychic")]
    public class RushPsychicExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int AmusiHowlingPerformer = 120196029;
            public const int AmusiPerformer = 120140031;
            public const int BlueMedicine = 120130044;
            public const int CAND = 120170002;
            public const int CANDLIVE = 120199035;
            public const int ChemicalCureBlue = 120155017;
            public const int ChemicalCurePurple = 120196030;
            public const int ChemicalCureRed = 120196031;
            public const int EsperaidtheSmashingSuperstar = 120130025;
            public const int Fusion = 120196050;
            public const int Giftarist = 120189003;
            public const int HowlingBird = 120140035;
            public const int MonsterReborn = 120194004;
            public const int PrimaGuitarnatheShiningSuperstar = 120120028;
            public const int PsychicTrapHole = 120189026;
            public const int Psyphickupper = 120199014;
            public const int RedMedicine = 120196032;
            public const int RomancePick = 120130023;
            public const int StarRestart = 120196045;
        }

        public RushPsychicExecutor(GameAI ai, Duel duel) :
            base(ai, duel)
        {
            // Activate multiples of blue/red medicine
            for (int i = 0; i < 3; i++)
                AddExecutor(ExecutorType.Activate, CardId.BlueMedicine);

            for (int i = 0; i < 3; i++)
                AddExecutor(ExecutorType.Activate, CardId.RedMedicine);

            AddExecutor(ExecutorType.Activate, CardId.StarRestart, StarRestartEff);

            AddExecutor(ExecutorType.Summon, CardId.ChemicalCureBlue);
            AddExecutor(ExecutorType.Activate, CardId.ChemicalCureBlue, ChemicalCureBlueEff);
            AddExecutor(ExecutorType.Summon, CardId.ChemicalCureRed);
            AddExecutor(ExecutorType.Activate, CardId.ChemicalCureRed, ChemicalCureRedEff);
            AddExecutor(ExecutorType.Activate, CardId.Fusion, FusionEff);

            AddExecutor(ExecutorType.Summon, CardId.Psyphickupper);
            AddExecutor(ExecutorType.Activate, CardId.Psyphickupper);
            AddExecutor(ExecutorType.Summon, CardId.CAND, CANDSummon);
            AddExecutor(ExecutorType.Activate, CardId.Fusion, FusionEff);

            AddExecutor(ExecutorType.Summon, CardId.AmusiPerformer);
            AddExecutor(ExecutorType.Activate, CardId.AmusiPerformer);
            AddExecutor(ExecutorType.Summon, CardId.HowlingBird, HowlingBirdSummon);
            AddExecutor(ExecutorType.Activate, CardId.Fusion, FusionEff);

            AddExecutor(ExecutorType.Summon, CardId.RomancePick);
            AddExecutor(ExecutorType.Activate, CardId.RomancePick, RomancePickEff);

            AddExecutor(ExecutorType.Summon, CardId.CAND);
            AddExecutor(ExecutorType.Summon, CardId.AmusiPerformer);

            AddExecutor(ExecutorType.Summon, CardId.EsperaidtheSmashingSuperstar, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.EsperaidtheSmashingSuperstar);

            AddExecutor(ExecutorType.Summon, CardId.Giftarist, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.Giftarist);

            AddExecutor(ExecutorType.Summon, CardId.PrimaGuitarnatheShiningSuperstar, Tribute);
            AddExecutor(ExecutorType.Activate, CardId.PrimaGuitarnatheShiningSuperstar);

            AddExecutor(ExecutorType.Activate, CardId.MonsterReborn, MonsterRebornEff);

            AddExecutor(ExecutorType.Activate, CardId.PsychicTrapHole);

            AddExecutor(ExecutorType.SpellSet);
        }

        private bool Tribute()
        {
            int[] lowlevel =
            {
                CardId.ChemicalCureBlue,
                CardId.ChemicalCureRed,
                CardId.Psyphickupper,
                CardId.EsperaidtheSmashingSuperstar,
                CardId.Giftarist,
                CardId.AmusiPerformer,
                CardId.RomancePick,
                CardId.CAND,
                CardId.HowlingBird
            };

            if (Card.IsCode(CardId.Psyphickupper))
            {
                AI.SelectCard(lowlevel);
            }
            else
            {
                AI.SelectCard(lowlevel);
                AI.SelectNextCard(lowlevel);
            }

            return true;
        }

        private bool CANDSummon()
        {
            return Bot.HasInMonstersZone(CardId.Psyphickupper, faceUp: true) && Bot.HasInHand(CardId.Fusion);
        }

        private bool HowlingBirdSummon()
        {
            return Bot.HasInMonstersZone(CardId.AmusiPerformer, faceUp: true) && Bot.HasInHand(CardId.Fusion);
        }

        private bool ChemicalCureBlueEff()
        {
            List<ClientCard> cures = Bot.GetGraveyardSpells().FindAll(x => x.IsCode(CardId.BlueMedicine));

            if (cures.Count == 0)
                return false;
            if (cures.Count >= 1)
                AI.SelectCard(cures);
            if (cures.Count >= 2)
                AI.SelectNextCard(cures);
            if (cures.Count >= 3)
                AI.SelectThirdCard(cures);

            return true;
        }

        private bool ChemicalCureRedEff()
        {
            List<ClientCard> cures = Bot.GetGraveyardSpells().FindAll(x => x.IsCode(CardId.RedMedicine));

            if (cures.Count == 0)
                return false;
            if (cures.Count >= 1)
                AI.SelectCard(cures);
            if (cures.Count >= 2)
                AI.SelectNextCard(cures);
            if (cures.Count >= 3)
                AI.SelectThirdCard(cures);

            return true;
        }

        private bool FusionEff()
        {
            if (Bot.HasInMonstersZone(CardId.CAND, faceUp: true) &&
                Bot.HasInMonstersZone(CardId.Psyphickupper, faceUp: true))
                AI.SelectCard(CardId.CANDLIVE);
            else if (Bot.HasInMonstersZone(CardId.ChemicalCureBlue, faceUp: true) &&
                Bot.HasInMonstersZone(CardId.ChemicalCureRed, faceUp: true))
                AI.SelectCard(CardId.ChemicalCurePurple);
            else if (Bot.HasInMonstersZone(CardId.AmusiPerformer, faceUp: true) &&
                Bot.HasInMonstersZone(CardId.HowlingBird, faceUp: true))
                AI.SelectCard(CardId.AmusiHowlingPerformer);
            else return false;

            return true;
        }

        private bool RomancePickEff()
        {
            int[] targets = {
                CardId.CANDLIVE,
                CardId.ChemicalCurePurple,
                CardId.AmusiHowlingPerformer,
                CardId.ChemicalCureBlue,
                CardId.ChemicalCureRed,
                CardId.Psyphickupper,
                CardId.EsperaidtheSmashingSuperstar,
                CardId.Giftarist,
                CardId.AmusiPerformer,
                CardId.RomancePick,
                CardId.CAND,
                CardId.HowlingBird
            };

            AI.SelectCard(targets);

            return true;
        }

        private bool MonsterRebornEff()
        {
            int[] gytargets = {
                CardId.CANDLIVE,
                CardId.ChemicalCurePurple,
                CardId.AmusiHowlingPerformer,
                CardId.ChemicalCureBlue,
                CardId.ChemicalCureRed,
                CardId.Psyphickupper,
                CardId.EsperaidtheSmashingSuperstar,
                CardId.Giftarist,
                CardId.AmusiPerformer,
                CardId.RomancePick,
                CardId.CAND,
                CardId.HowlingBird
            };

            AI.SelectCard(gytargets);

            return true;
        }

        private bool StarRestartEff()
        {
            int[] normal = { CardId.CAND, CardId.HowlingBird };

            if (!((Bot.HasInGraveyard(CardId.CAND)
              || Bot.HasInGraveyard(CardId.HowlingBird))
             && Bot.HasInGraveyard(CardId.Fusion)))
            {
                return false;
            }

            // Fix such that optimal card is chosen
            AI.SelectCard(Bot.Hand);

            AI.SelectCard(normal);
            AI.SelectCard(CardId.Fusion);

            return true;
        }
    }
}