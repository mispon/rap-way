using System;

namespace Models.Player.DynamicData
{
    /// <summary>
    /// Количество денег
    /// </summary>
    [Serializable]
    public struct Money
    {
        public int Value;

        public override string ToString()
        {
            //ToDo: написать override
            return base.ToString();
        }
    }
}