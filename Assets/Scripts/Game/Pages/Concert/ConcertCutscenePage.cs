using Models.Info.Production;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Concert
{
    public class ConcertCutscenePage : Page
    {
        [Header("Кол-во мер заполненности зала")] 
        [SerializeField, Range(3, 5)] private int locationOccupancies;

        [Header("Сцена")] 
        [SerializeField] private GameObject artist;
        //ToDo: Освещение?
        
        [Header("Настройки танцпола")] 
        [SerializeField, Tooltip("Один единственный чувак на весь зал")] 
        private GameObject flexSingleTemplate;
        [SerializeField, Tooltip("Группа из чуваков на сцене")] 
        private GameObject flexGroupTemplate;
        [SerializeField, 
         Tooltip("Контейнер, куда мы будем грузить наших чуваков. В теории обладает комопнентом HorizontalLayoutGroup, " +
                 "и нам не нужно запараитьвася над координатами расстановки")]
        private Transform container;

        [Header("Пропуск катсцены")] 
        [SerializeField] private Button skipButton;
        
        private GameObject[] flexingUnits;
        private ConcertInfo _concert;
        
        /// <summary>
        /// Открытие страницы с передаче информации о проведенном концерте
        /// </summary>
        public void Show(ConcertInfo concert)
        {
            _concert = concert;
            Open();
        }

        /// <summary>
        /// Устанавливаем настройки сцены
        /// </summary>
        private void SetUpScene()
        {
            //ToDo: пока оставим просто вклбчение обхекта персонажа, но тут мы должны укзаать, что он начинает флексить
            //ToDo: тоже самое со светом, если будем над ним запариваться: бошки начниают кружиться
            
            artist.SetActive(true);
        }
        
        /// <summary>
        /// Располагаем фанатов на танцполе в завивсмости от заполненности зала
        /// </summary>
        private void FillDanceFloor()
        {
            var occupancyRatio = _concert.TicketsSold / (float) _concert.LocationCapacity;
            var locationOccupanciesRatio = 1 / (float) locationOccupancies;
            //Мера заполненности зала. От нее мы рисуем нужжное кол-во флексящих чуваков на сцене
            var occupancyMeasure = Mathf.RoundToInt(occupancyRatio / locationOccupanciesRatio);
            
            if(occupancyMeasure == 0)
                InstantiateFlexing(flexSingleTemplate, 1);
            else
                InstantiateFlexing(flexGroupTemplate, occupancyMeasure);
        }

        /// <summary>
        /// Создаем группы фанатов, располагая их в контейнере
        /// </summary>
        private void InstantiateFlexing(GameObject unit, int count)
        {
            flexingUnits = new GameObject[count];
            
            for (var i = 0; i < count; i++)
            {
                flexingUnits[i] = Instantiate(unit, container);
                flexingUnits[i].SetActive(true);
            }
        }
        
        

        #region PAGE CALLBACKS

        protected override void BeforePageOpen()
        {
            FillDanceFloor();
        }

        protected override void AfterPageClose()
        {
            //ToDo: диспозим артиста и освещениеЮ если оно будет
            artist.SetActive(false);
            
            //ToDo: диспозим группы флексящих
            for (var i = 0; i < flexingUnits.Length; i++)
                Destroy(flexingUnits[i]);

            flexingUnits = null;
            _concert = null;
        }

        #endregion
    }
}