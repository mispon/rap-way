using Core.Settings;
using Messages.State;
using Models.Player;
using UniRx;

namespace Game
{
    public static class GameManagerEvents
    {
        private static IMessageBroker _messageBroker;
        
        private static PlayerData _playerData;
        private static GameSettings _settings;
        
        public static void Init()
        {
            _messageBroker = GameManager.Instance.MessageBroker;
            _playerData = GameManager.Instance.PlayerData;
            _settings = GameManager.Instance.Settings;

            HandleFullState();
            HandleAddMoney();
            HandleSpendMoney();
        }

        private static void HandleFullState()
        {
            _messageBroker
                .Receive<FullStateRequest>()
                .Subscribe(_ =>
                {
                    _messageBroker.Publish(new FullStateResponse
                    {
                        NickName = _playerData.Info.NickName,
                        Gender = _playerData.Info.Gender,
                        Money = _playerData.Money,
                        Fans = _playerData.Fans,
                        Hype = _playerData.Hype,
                        Exp = _playerData.Exp
                    });
                });
        }
        
        private static void HandleAddMoney()
        {
            _messageBroker
                .Receive<AddMoneyRequest>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Money;
                    int newVal = SafetyAdd(_playerData.Money, e.Amount, _settings.MaxMoney);

                    _playerData.Money = newVal;
                    _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                });
        }
        
        private static void HandleSpendMoney()
        {
            _messageBroker
                .Receive<SpendMoneyRequest>()
                .Subscribe(e =>
                {
                    int oldVal = _playerData.Money;
                    
                    bool ok = false;
                    if (_playerData.Money >= e.Amount)
                    {
                        int newVal = _playerData.Money - e.Amount;

                        _playerData.Money = newVal;
                        _messageBroker.Publish(new MoneyChangedEvent {OldVal = oldVal, NewVal = newVal});
                        
                        ok = true;
                    } 
                    
                    _messageBroker.Publish(new SpendMoneyResponse {OK = ok});
                });
        }
        
        private static int SafetyAdd(int current, int increment, int maxValue)
        {
            return maxValue - current > increment
                ? current + increment
                : maxValue;
        }
    }
}