namespace RapWay.Domain.Interfaces
{
    /// <summary>
    /// Each save-able entity should implement this interface. 
    /// </summary>
    public interface ISaveable
    {
        string SaveId { get; }
        object CaptureState();
        void RestoreState(object state);
    }
}