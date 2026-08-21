namespace RapWay.Domain.Character
{
    public sealed class HypeState
    {
        public HypeState(BoundedResource intensity)
        {
            Intensity = intensity;
        }

        public BoundedResource Intensity { get; }

        public static HypeState Empty { get; } = new(new BoundedResource(0, CharacterResources.DefaultMaximum));

        internal HypeState Copy()
        {
            return new HypeState(Intensity);
        }

        internal void Validate()
        {
            _ = new BoundedResource(Intensity.Current, Intensity.Maximum);
        }
    }
}
