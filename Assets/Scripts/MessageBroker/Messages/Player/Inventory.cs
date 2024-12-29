using Game.Player.Inventory.Desc;

namespace MessageBroker.Messages.Player
{
    public struct AddInventoryItemMessage
    {
        public string        Name;
        public InventoryType Type;
        public object        Raw;
    }

    public struct InventoryItemExistsRequest
    {
        public string        Name;
        public InventoryType Type;
        public bool          IsNoAds;
    }

    public struct InventoryItemExistsResponse
    {
        public bool Status;
    }
}