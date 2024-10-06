using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using ScriptableObjects;
using Random = UnityEngine.Random;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private const int MIN_FANS_COUNT = 100;

        public void DoAction(RapperInfo rapperInfo, GameSettings settings, ConcertPlacesData concertData)
        {
            switch (ChooseAction())
            {
                case RappersAIActions.Track:
                    DoTrack(rapperInfo, settings);
                    break;

                case RappersAIActions.Clip:
                    DoClip(rapperInfo, settings);
                    break;

                case RappersAIActions.Album:
                    DoAlbum(rapperInfo, settings);
                    break;

                case RappersAIActions.Concert:
                    DoConcert(rapperInfo, settings, concertData);
                    break;

                case RappersAIActions.Eagle:
                    DoEagler(rapperInfo, settings);
                    break;
            }
        }

        private static RappersAIActions ChooseAction()
        {
            return RollDice() switch
            {
                < 20  => RappersAIActions.Track,
                < 35  => RappersAIActions.Clip,
                < 50  => RappersAIActions.Album,
                < 60  => RappersAIActions.Concert,
                < 80  => RappersAIActions.Eagle,
                < 90  => RappersAIActions.Feat,
                < 95  => RappersAIActions.Battle,
                < 100 => RappersAIActions.LeaveLabel,
                // todo: implement actions
                < 101 => RappersAIActions.Diss,
                < 102 => RappersAIActions.Interview,
                // default
                _ => RappersAIActions.Track
            };
        }

        private static int GenWorkPoints(int skill, int days)
        {
            var wp = 0;

            for (var i = 0; i < days / 2; i++)
            {
                wp += Random.Range(0, skill + 1);
            }

            return wp;
        }

        private static int TryDoFeat(RapperInfo rapperInfo)
        {
            if (RollDice() > 10)
            {
                return -1;
            }

            while (true)
            {
                var feat = RappersAPI.Instance.GetRandom();
                if (feat.Id == rapperInfo.Id)
                {
                    continue;
                }

                return feat.Id;
            }
        }

        private static int RollDice()
        {
            return Random.Range(0, 100);
        }
    }
}