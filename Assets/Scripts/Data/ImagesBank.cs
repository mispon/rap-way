using UnityEngine;

namespace Data
{
    /// <summary>
    /// БД картинок
    /// </summary>
    [CreateAssetMenu(fileName = "ImagesBank", menuName = "Data/ImagesBank")]
    public class ImagesBank : ScriptableObject
    {
        [Header("Иконка стилистики")]
        public Sprite StyleActive;
        public Sprite StyleInactive;

        [Header("Иконки тематик")]
        public Sprite[] ThemesActive;
        public Sprite[] ThemesInactive;

        [Header("Команда")]
        public Sprite BitmakerActive;
        public Sprite BitmakerInactive;
        [Space]
        public Sprite TextwritterActive;
        public Sprite TextwritterInactive;
        [Space]
        public Sprite ProducerActive;
        public Sprite ProducerInactive;
        [Space]
        public Sprite PrManActive;
        public Sprite PrManInactive;

        [Header("Иконки скиллов")]
        [Tooltip("Индекс элемента соответствует значению enum Skills")]
        public Sprite[] Skills;
    }
}