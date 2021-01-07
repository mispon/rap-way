using Data;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Battle
{
    /// <summary>
    /// Рабочая страница батла
    /// </summary>
    public class BattleWorkingPage : BaseWorkingPage
    {
        [Header("Контролы")] 
        [SerializeField] private Text header;
        [Space]
        [SerializeField] private Text playerPointsLabel;
        [SerializeField] private WorkPoints playerWorkPoints;
        [Space]
        [SerializeField] private Image rapperAvatar;
        [SerializeField] private Text rapperPointsLabel;
        [SerializeField] private WorkPoints rapperWorkPoints;

        [Header("Очки работы от скилов")]
        [SerializeField] private int skillChance;
        [SerializeField] private WorkPoints doubleTimePoint;
        [SerializeField] private WorkPoints shoutOutPoint;
        [SerializeField] private WorkPoints freestylePoint;
        [SerializeField] private WorkPoints punchPoint;
        [SerializeField] private WorkPoints flipPoint;

        [Header("Страница результата")] 
        [SerializeField] private BattleResultPage battleResult;

        private RapperInfo _rapper;
        private int _playerPoints;
        private int _rapperPoints;
        
        /// <summary>
        /// Начинает выполнение работы 
        /// </summary>
        public override void StartWork(params object[] args)
        {
            _rapper = (RapperInfo) args[0];
            Open();
        }

        /// <summary>
        /// Работа, выполняемая за один день
        /// </summary>
        protected override void DoDayWork()
        {
            GenerateWorkPoints();
            DisplayWorkPoints();
        }

        /// <summary>
        /// Обработчик завершения работы
        /// </summary>
        protected override void FinishWork()
        {
            battleResult.Show(_rapper, _playerPoints, _rapperPoints);
            Close();
        }

        /// <summary>
        /// Генерирует очки работы игрока и рэпера
        /// </summary>
        private void GenerateWorkPoints()
        {
            int playerPoints = Random.Range(1, PlayerManager.Data.Stats.Vocobulary.Value + 1);
            playerPoints += GenerateSkillsPoints();
            playerWorkPoints.Show(playerPoints);
            _playerPoints += playerPoints;

            int rapperPoints = Random.Range(1, _rapper.Vocobulary + 1);
            rapperWorkPoints.Show(rapperPoints);
            _rapperPoints += rapperPoints;
        }

        /// <summary>
        /// Генерирует рабочие очки с умений 
        /// </summary>
        private int GenerateSkillsPoints()
        {
            int GeneratePoint(Skills skill, WorkPoints workPoints)
            {
                if (!PlayerManager.Data.Skills.Contains(skill) || Random.Range(0, 100) > skillChance)
                    return 0;
                
                workPoints.Show(1);
                return 1;
            }

            var result = GeneratePoint(Skills.DoubleTime, doubleTimePoint);
            result += GeneratePoint(Skills.ShoutOut, shoutOutPoint);
            result += GeneratePoint(Skills.Freestyle, freestylePoint);
            result += GeneratePoint(Skills.Punch, punchPoint);
            result += GeneratePoint(Skills.Flip, flipPoint);

            return result;
        }

        /// <summary>
        /// Отображает количество очков в интерфейсе
        /// </summary>
        private void DisplayWorkPoints()
        {
            playerPointsLabel.text = _playerPoints.ToString();
            rapperPointsLabel.text = _rapperPoints.ToString();
        }

        /// <summary>
        /// Отображает иконки активных умений
        /// </summary>
        private void DisplayAvailableSkills()
        {
            void DisplayIfAvailable(Skills skill, WorkPoints workPoint)
            {
                var icon = workPoint.transform.parent.GetComponent<Image>();
                icon.enabled = PlayerManager.Data.Skills.Contains(skill);
            }
            
            DisplayIfAvailable(Skills.DoubleTime, doubleTimePoint);
            DisplayIfAvailable(Skills.ShoutOut, shoutOutPoint);
            DisplayIfAvailable(Skills.Freestyle, freestylePoint);
            DisplayIfAvailable(Skills.Punch, punchPoint);
            DisplayIfAvailable(Skills.Flip, flipPoint);
        }
        
        protected override void BeforePageOpen()
        {
            header.text = $"{PlayerManager.Data.Info.NickName} VS {_rapper.Name}";
            playerPointsLabel.text = rapperPointsLabel.text = "0";

            DisplayAvailableSkills();
            
            rapperAvatar.sprite = _rapper.Avatar;
        }

        protected override void BeforePageClose()
        {
            base.BeforePageClose();
            _rapper = null;
        }
    }
}