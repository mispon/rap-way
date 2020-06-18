using System;

namespace Models.Player.DynamicData
{
    /// <summary>
    /// Число фанатов
    /// </summary>
    [Serializable]
    public struct Fans
    {
        public int Value;

        public override string ToString()
        {
            //ToDo: написать override
            return base.ToString();
        }
    }
}