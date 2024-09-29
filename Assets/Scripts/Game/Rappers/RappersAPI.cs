using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Game.Rappers.Desc;
using Models.Production;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Rappers
{
    public partial class RappersPackage
    {
        public void AddCustom(RapperInfo info)
        {
            _customRappers.Add(info);
        }

        public void RemoveCustom(RapperInfo info)
        {
            _customRappers.Remove(info);
        }

        public int MaxCustomRapperID()
        {
            const int minID = 0;
            return _customRappers.Any() ? _customRappers.Max(e => e.Id) : minID;
        }

        public RapperInfo Get(int rapperId)
        {
            foreach (var rapperInfo in _rappers)
            {
                if (rapperInfo.Id == rapperId)
                {
                    return rapperInfo;
                }
            }

            foreach (var rapperInfo in _customRappers)
            {
                if (rapperInfo.Id == rapperId)
                {
                    return rapperInfo;
                }
            }

            return null;
        }

        public RapperInfo GetRandom()
        {
            var rappers = GetAll().ToArray();
            int dice = Random.Range(0, rappers.Length);
            return rappers[dice];
        }

        public IEnumerable<RapperInfo> GetAll()
        {
            foreach (var rapperInfo in _rappers)
            {
                rapperInfo.Avatar = SpritesManager.Instance.TryGetByName(rapperInfo.AvatarName, out var avatar)
                   ? avatar
                   : imagesBank.CustomRapperAvatar;

                yield return rapperInfo;
            }

            foreach (var rapperInfo in _customRappers)
            {
                rapperInfo.Avatar = imagesBank.CustomRapperAvatar;
                rapperInfo.AvatarName = imagesBank.CustomRapperAvatar.name;

                yield return rapperInfo;
            }
        }

        public IEnumerable<RapperInfo> GetFromLabel(string label)
        {
            var rSource = _rappers.Where(rapperInfo => rapperInfo.Label == label);
            foreach (var rapperInfo in rSource)
            {
                yield return rapperInfo;
            }

            var crSource = _customRappers.Where(rapperInfo => rapperInfo.Label == label);
            foreach (var rapperInfo in crSource)
            {
                yield return rapperInfo;
            }
        }

        public static int GetRapperScore(RapperInfo rapper)
        {
            const int maxRapperScore = 100;
            const int maxValuableFans = 50_000_000;

            var score = Convert.ToInt32(1f * rapper.Fans / maxValuableFans * maxRapperScore);

            return Mathf.Min(score, maxRapperScore);
        }

        public static float GetRapperPrestige(RapperInfo rapper)
        {
            int score = GetRapperScore(rapper);

            return score switch
            {
                >= 90 => 5.0f,
                >= 80 => 4.5f,
                >= 70 => 4.0f,
                >= 60 => 3.5f,
                >= 50 => 3.0f,
                >= 40 => 2.5f,
                >= 30 => 2.0f,
                >= 20 => 1.5f,
                >= 10 => 1.0f,
                >= 5 => 0.5f,
                _ => 0
            };
        }

        public TrackInfo GetTrack(int rapperId, int trackId)
        {
            return Get(rapperId)
                .History.TrackList
                .First(e => e.Id == trackId);
        }

        public AlbumInfo GetAlbum(int rapperId, int albumId)
        {
            return Get(rapperId)
                .History.AlbumList
                .First(e => e.Id == albumId);
        }

        public bool IsNameAlreadyTaken(string nickname)
        {
            foreach (var rapper in GetAll())
            {
                if (string.Equals(nickname, rapper.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
    }
}