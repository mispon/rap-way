using System;

namespace RapWay.Domain.Character
{
    public sealed class CharacterResources
    {
        public const int DefaultMaximum = 1_000;

        public CharacterResources(BoundedResource energy, BoundedResource satiety, BoundedResource motivation)
        {
            Energy = energy;
            Satiety = satiety;
            Motivation = motivation;
        }

        public BoundedResource Energy { get; }

        public BoundedResource Satiety { get; }

        public BoundedResource Motivation { get; }

        public static CharacterResources CreateFull()
        {
            BoundedResource full = new(DefaultMaximum, DefaultMaximum);
            return new CharacterResources(full, full, full);
        }

        internal CharacterResources Copy()
        {
            return new CharacterResources(Energy, Satiety, Motivation);
        }

        internal CharacterResources ApplyChanges(int energyChange, int satietyChange, int motivationChange)
        {
            return new CharacterResources(
                ApplyChange(Energy, energyChange),
                ApplyChange(Satiety, satietyChange),
                ApplyChange(Motivation, motivationChange));
        }

        internal void Validate()
        {
            _ = new BoundedResource(Energy.Current, Energy.Maximum);
            _ = new BoundedResource(Satiety.Current, Satiety.Maximum);
            _ = new BoundedResource(Motivation.Current, Motivation.Maximum);
        }

        private static BoundedResource ApplyChange(BoundedResource resource, int change)
        {
            return change >= 0
                ? resource.Gain(change)
                : resource.Spend(checked(-change));
        }
    }
}
