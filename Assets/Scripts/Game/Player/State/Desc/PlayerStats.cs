using System;
using Models.Game;

namespace Game.Player.State.Desc
{
    [Serializable]
    public class PlayerStats
    {
        private const int MIN_VALUE = 1;

        public ExpValue Vocobulary;
        public ExpValue Bitmaking;
        public ExpValue Flow;
        public ExpValue Charisma;
        public ExpValue Management;
        public ExpValue Marketing;

        public static PlayerStats New => new()
        {
            Vocobulary = {Value = MIN_VALUE},
            Bitmaking = {Value = MIN_VALUE},
            Flow = {Value = MIN_VALUE},
            Charisma = {Value = MIN_VALUE},
            Management = {Value = MIN_VALUE},
            Marketing = {Value = MIN_VALUE}
        };
        
        public ExpValue[] Values => new[]
        {
            Vocobulary,
            Bitmaking,
            Flow,
            Charisma,
            Management,
            Marketing
        };
    }
}