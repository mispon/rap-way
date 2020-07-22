using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Типы тиммейтов игрока
    /// </summary>
    public enum TeammateType
    {
        [Description("teammate_bitmaker")]
        BitMaker,
        [Description("teammate_textwritter")]
        TextWriter,
        [Description("teammate_manager")]
        Manager, 
        [Description("teammate_prman")]
        PrMan
    }
}