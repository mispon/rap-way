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
        
        public RapperInfo Info { get; private set; }

        /// <summary>
        /// Обработчик клика по элементу
        /// </summary>
        public Action<RapperGridItem> onClick = item => {};

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
            avatar.sprite = info.Avatar;
        }
    }
}