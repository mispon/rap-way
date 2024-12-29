using System.ComponentModel;

namespace Game.Player.Inventory.Desc
{
    public enum InventoryType
    {
        [Description("InventoryType_Micro")]
        Micro,

        [Description("InventoryType_AudioCard")]
        AudioCard,

        [Description("InventoryType_FxMixer")]
        FxMixer,

        [Description("InventoryType_Acoustic")]
        Acoustic,

        [Description("InventoryType_Car")]
        Car,

        [Description("InventoryType_House")]
        House,

        [Description("InventoryType_Swatches")]
        Swatches,

        [Description("InventoryType_Chain")]
        Chain,

        [Description("InventoryType_Grillz")]
        Grillz,

        [Description("InventoryType_Clothes")]
        Clothes,

        [Description("InventoryType_DonateCoins")]
        DonateCoins,

        [Description("InventoryType_NoAds")]
        NoAds
    }
}