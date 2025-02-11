using Enums;
using Game.Rappers.Desc;
using Game.Settings;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;
using RappersAPI = Game.Rappers.RappersPackage;

namespace Game.Rappers.AI
{
    public partial class RappersAI
    {
        private const int MIN_FANS_COUNT = 100;

        public void DoAction(RapperInfo rapperInfo, GameSettings settings, ImagesBank imagesBank, ConcertPlacesData concertData)
        {
            var action = ChooseAction();

            switch (action)
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

                case RappersAIActions.LeaveLabel:
                    TryLeaveLabel(rapperInfo, settings);
                    break;

                case RappersAIActions.Feat:
                    ProposeFeat(rapperInfo, settings);
                    break;

                case RappersAIActions.Battle:
                    ProposeBattle(rapperInfo, settings, imagesBank);
                    break;

                case RappersAIActions.Diss:
                    break;
                case RappersAIActions.Interview:
                    break;

                default:
                    Debug.LogError($"Received unexpected action type: {action}");
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
                < 90  => RappersAIActions.LeaveLabel,
                < 95  => RappersAIActions.Feat,
                < 100 => RappersAIActions.Battle,

                // todo: implement actions
                < 103 => RappersAIActions.Diss,
                < 104 => RappersAIActions.Interview,

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

        private static bool CanInteractPlayer(int rapperFans, int playerFans)
        {
            if (playerFans >= rapperFans)
            {
                return true;
            }

            const float maxDiff = 0.1f;

            var diff = 1f * (playerFans - rapperFans) / rapperFans;
            return Mathf.Abs(diff) <= maxDiff;
        }

        private static int TryDoFeat(RapperInfo rapperInfo, int chance)
        {
            if (RollDice() > chance)
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