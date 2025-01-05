using System.Linq;
using Game.Player.Inventory.Desc;
using MessageBroker;
using MessageBroker.Interfaces;
using MessageBroker.Messages.Player;
using MessageBroker.Messages.Player.State;
using UniRx;

namespace Game.Player.Inventory
{
    public class InventoryEventsHandler : IMessagesHandler
    {
        public void RegisterHandlers(CompositeDisposable disposable)
        {
            HandleAddNewItem(disposable);
            HandleItemExistsRequest(disposable);
        }

        private static void HandleAddNewItem(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<AddInventoryItemMessage>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;

                    var item = new InventoryItem
                    {
                        Name     = e.Name,
                        Type     = e.Type,
                        Equipped = true
                    };
                    item.SetValue(e.Raw);

                    playerData.Inventory.Add(item);
                    MsgBroker.Instance.Publish(new ChangeHypeMessage());
                })
                .AddTo(disposable);
        }

        private static void HandleItemExistsRequest(CompositeDisposable disposable)
        {
            MsgBroker.Instance
                .Receive<InventoryItemExistsRequest>()
                .Subscribe(e =>
                {
                    var playerData = GameManager.Instance.PlayerData;

                    var exists = e.IsNoAds
                        ? GameManager.Instance.LoadNoAds()
                        : playerData.Inventory.Any(g => g.Type == e.Type && g.Name == e.Name);

                    MsgBroker.Instance.Publish(new InventoryItemExistsResponse {Status = exists});
                })
                .AddTo(disposable);
        }
    }
}