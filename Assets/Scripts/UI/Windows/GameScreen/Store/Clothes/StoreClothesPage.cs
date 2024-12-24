using Extensions;
using MessageBroker;
using MessageBroker.Messages.Player.State;
using ScriptableObjects;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.GameScreen.Store.Clothes
{
    public class StoreClothesPage : Page
    {
        [Header("Data")]
        [SerializeField] private GoodsData data;

        [Header("Header")]
        [SerializeField] private Text gameBalance;
        [SerializeField] private Text donateBalance;

        private readonly CompositeDisposable _disposable = new();

        protected override void BeforeShow(object ctx = null)
        {
            MsgBroker.Instance
                .Receive<MoneyChangedMessage>()
                .Subscribe(e => UpdateGameBalance(e.NewVal))
                .AddTo(_disposable);
            MsgBroker.Instance
                .Receive<FullStateResponse>()
                .Subscribe(UpdateHUD)
                .AddTo(_disposable);
            MsgBroker.Instance.Publish(new FullStateRequest());
        }

        private void UpdateHUD(FullStateResponse resp)
        {
            UpdateGameBalance(resp.Money);
            UpdateDonateBalance(resp.Donate);
        }

        private void UpdateGameBalance(int money)
        {
            gameBalance.text = money.GetShort();
        }

        private void UpdateDonateBalance(int donate)
        {
            donateBalance.text = donate.GetDisplay();
        }

        protected override void AfterHide()
        {
            _disposable.Clear();
        }
    }
}