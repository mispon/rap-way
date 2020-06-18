namespace Models.Production
{
    /// <summary>
    /// Информация о выпущенном клипе
    /// </summary>
    [System.Serializable]
    public class ClipInfo: Production
    {
        public int TrackId;

        public int DirectorPoints;
        public int OperatorPoints;
        public int StylistPoints;

        public int Views;
        public int Likes;
        public int Dislikes;
    }
}