using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Имущество игрока
    /// </summary>
    public enum GoodsType
    {
        [Description("GoodsType_Micro")]
        Micro, 
        [Description("GoodsType_AudioCard")]
        AudioCard, 
        [Description("GoodsType_FxMixer")]
        FxMixer, 
        [Description("GoodsType_Acoustic")]
        Acoustic,
        [Description("GoodsType_Car")]
        Car, 
        [Description("GoodsType_Apartments")]
        Apartments, 
        [Description("GoodsType_Swatches")]
        Swatches, 
        [Description("GoodsType_Chain")]
        Chain, 
        [Description("GoodsType_Grillz")]
        Grillz
    }
}