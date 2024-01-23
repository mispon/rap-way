using System.ComponentModel;

namespace Enums
{
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