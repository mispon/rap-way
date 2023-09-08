using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Rappers
{
    /// <summary>
    /// Элемент списка реальных исполнителей
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class RapperGridItem : MonoBehaviour
    {
        [SerializeField] private Image avatar;
        [SerializeField] private Text nickname;
        
        public RapperInfo Info { get; private set; }

        /// <summary>
        /// Обработчик клика по элементу
        /// </summary>
        public Action<RapperGridItem> onClick = _ => {};

        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() => onClick.Invoke(this));
        }

        /// <summary>
        /// Инициализирует элемент списка 
        /// </summary>
        public void Setup(RapperInfo info)
        {
            Info = info;
            nickname.text = info.Name;
            if (!info.IsCustom)
            {
                avatar.sprite = info.Avatar;
            }
        }
    }
}