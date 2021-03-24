using System.Collections;
using Game;
using Game.Analyzers;
using Models.Info;
using Models.Info.Production;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(TrackAnalyzer))]
    [RequireComponent(typeof(AlbumAnalyzer))]
    [RequireComponent(typeof(ClipAnalyzer))]
    [RequireComponent(typeof(ConcertAnalyzer))]
    [RequireComponent(typeof(SocialAnalyzer))]
    public class AnalyzersBalancer : MonoBehaviour
    {
        private TrackAnalyzer _trackAnalyzer;
        private AlbumAnalyzer _albumAnalyzer;
        private ClipAnalyzer _clipAnalyzer;
        private ConcertAnalyzer _concertAnalyzer;
        private SocialAnalyzer _socialAnalyzer;

        [SerializeField, Range(100, 500_000_000)] private int fans;
        [Space]
        [SerializeField] private TrackInfo track;
        [SerializeField] private AlbumInfo album;
        [SerializeField] private ClipInfo clip;
        [SerializeField] private ConcertInfo concert;
        [SerializeField] private SocialInfo social;

        private IEnumerator Start()
        {
            _trackAnalyzer = GetComponent<TrackAnalyzer>();
            _albumAnalyzer = GetComponent<AlbumAnalyzer>();
            _clipAnalyzer = GetComponent<ClipAnalyzer>();
            _concertAnalyzer = GetComponent<ConcertAnalyzer>();
            _socialAnalyzer = GetComponent<SocialAnalyzer>();

            yield return new WaitWhile(() => GameManager.Instance.IsReady == false);
            GameManager.Instance.PlayerData.Fans = fans;
        }

        public void AnalyzeTrack()
        {
            track.Id += 1;
            track.TextPoints = Random.Range(10, 100);
            track.BitPoints = Random.Range(10, 100);

            _trackAnalyzer.Analyze(track);
            PlayerManager.Instance.GiveReward(track.FansIncome, track.MoneyIncome, 100);
        }

        public void AnalyzeAlbum()
        {
            album.Id += 1;
            album.TextPoints = Random.Range(10, 150);
            album.BitPoints = Random.Range(10, 150);

            _albumAnalyzer.Analyze(album);
            PlayerManager.Instance.GiveReward(album.FansIncome, album.MoneyIncome, 200);
        }

        public void AnalyzeClip()
        {
            clip.Id += 1;
            clip.DirectorPoints = Random.Range(10, 125);
            clip.OperatorPoints = Random.Range(10, 125);

            _clipAnalyzer.Analyze(clip);
            PlayerManager.Instance.GiveReward(clip.FansIncome, clip.MoneyIncome, 150);
        }

        public void AnalyzeConcert()
        {
            _concertAnalyzer.Analyze(concert);
        }

        public void AnalyzeSocial()
        {
            _socialAnalyzer.Analyze(social);
        }
    }
}