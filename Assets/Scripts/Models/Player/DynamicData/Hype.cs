using System;

namespace Models.Player.DynamicData
{
    /// <summary>
    /// Величина Хайпа
    /// </summary>
    [Serializable]
    public struct Hype
    {
        public int Value;

        public override string ToString()
        {
            //ToDo: написать override
            return base.ToString();
        }
    }
}