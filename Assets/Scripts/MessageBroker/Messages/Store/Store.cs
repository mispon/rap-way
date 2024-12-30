using CharacterCreator2D;

namespace MessageBroker.Messages.Store
{
    public struct ClothesSlotChangedMessage
    {
        public SlotCategory Slot;
        public int          Index;
    }

    public struct ClothesSlotColorChangedMessage
    {
        public SlotCategory Slot;
    }
}