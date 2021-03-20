using System;
using Enums;

namespace Models.Info
{
    /// <summary>
    /// Информация по выполнению социального действия
    /// </summary>
    [Serializable]
    public class SocialInfo
    {
        /// <summary>
        /// Тип социального действия
        /// </summary>
        public SocialType Type;
        
        /// <summary>
        /// Основной текст соц. действия
        /// (твит, коммент, пост)
        /// </summary>
        public string MainText;
        
        /// <summary>
        /// Дополнительный текст
        /// (название фонда)
        /// </summary>
        public string AdditionalText;

        /// <summary>
        /// Стиль в контексте соц. действия
        /// (тип фотографии в инсте)
        /// (тип поста в тик-токе)
        /// </summary>
        public int ModeIndex;
        
        /// <summary>
        /// Очки работы
        /// </summary>
        public int WorkPoints;

        /// <summary>
        /// Сумма пожертвования
        /// </summary>
        public int CharityAmount;
        
        /// <summary>
        /// Награда за выполнение соц. действия
        /// </summary>
        public int HypeIncome;

        /// <summary>
        /// Количество лайков
        /// </summary>
        public int Likes;
    }
}