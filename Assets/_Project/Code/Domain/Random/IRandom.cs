namespace RapWay.Domain.Random
{
    public interface IRandom
    {
        int NextInt(int minimumInclusive, int maximumExclusive);

        bool Chance(int successfulWeight, int totalWeight);
    }
}
