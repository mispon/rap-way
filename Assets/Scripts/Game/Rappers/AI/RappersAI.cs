using System;
using Game.Production.Analyzers;
using Game.Rappers.Desc;
using Game.Settings;
using MessageBroker;
using MessageBroker.Messages.Time;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Rappers.AI
{
    public partial class RappersAI : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private ConcertPlacesData concertData;

        [Header("Analyzers")]
        [SerializeField] private TrackAnalyzer trackAnalyzer;
        [SerializeField] private ClipAnalyzer clipAnalyzer;
        [SerializeField] private AlbumAnalyzer albumAnalyzer;
        [SerializeField] private ConcertAnalyzer concertAnalyzer;

        [Header("Cooldowns")]
        [SerializeField] private int trackCooldown = 30;
        [SerializeField] private int clipCooldown = 30;
        [SerializeField] private int albumCooldown = 50;
        [SerializeField] private int concertCooldown = 100;
        [SerializeField] private int eaglerCooldown = 20;

        [Space]
        [SerializeField] private int actingFrequency = 10;
        [SerializeField] private RapperInfo info;

        private IDisposable _disposable;

        public void Init(RapperInfo rapperInfo)
        {
            name = rapperInfo.Name;
            gameObject.SetActive(true);

            info = rapperInfo;

            _disposable = MsgBroker.Instance
                .Receive<DayLeftMessage>()
                .Subscribe(e => OnDayLeft());
        }

        private void OnDayLeft()
        {
            if (info.Cooldown > 0)
            {
                info.Cooldown -= 1;
                return;
            }

            if (RollDice() > actingFrequency)
            {
                // dice failed, do nothing
                return;
            }

            switch (RollDice())
            {
                case < 30:
                    DoTrack();
                    break;

                case >= 30 and < 50:
                    DoClip();
                    break;

                case >= 50 and < 65:
                    DoAlbum();
                    break;

                case >= 65 and < 75:
                    DoConcert();
                    break;

                case >= 75:
                    DoEalger();
                    break;

                default:
                    // do nothing
            }

        }

        private int GenWorkPoints(int skill, int days)
        {
            int wp = 0;

            for (int i = 0; i < days / 2; i++)
            {
                wp += Random.Range(0, skill + 1);
            }

            return wp;
        }

        private int RollDice()
        {
            return Random.Range(0, 100);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}