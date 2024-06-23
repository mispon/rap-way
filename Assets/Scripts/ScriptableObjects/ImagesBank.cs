using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ImagesBank", menuName = "Data/ImagesBank")]
    public class ImagesBank : ScriptableObject
    {
        public Sprite[] Index;

        public Sprite GetImageByName(string spriteName)
        {
            foreach (var sprite in Index)
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }

            return Empty;
        }
        
        // @formatter:off
        
        public Sprite Empty;

        [Header("Персонаж")]
        public Sprite MaleAvatar;
        public Sprite FemaleAvatar;

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

        [Header("Rappers")]
        public Sprite CustomRapperAvatar;

        [Header("Labels")]
        public Sprite CustomLabelAvatar;

        [Header("Houses images")]
        public Sprite[] Houses;
        
        // @formatter:on
    }
}