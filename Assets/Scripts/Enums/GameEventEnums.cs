using System.ComponentModel;

namespace Enums
{
    /// <summary>
    /// Тип события
    /// </summary>
    public enum GameEventType
    {
        Track,
        Album,
        Clip,
        Concert
    }
    
    /// <summary>
    /// Реакция на событие
    /// </summary>
    public enum GameEventDecisionType
    {
        [Description("gameevent_decision_peacefully")]
        Peacefully,
        [Description("gameevent_decision_aggressively")]
        Aggressively,
        [Description("gameevent_decision_indifferently")]
        Indifferently,
        [Description("gameevent_decision_peerassist")]
        PeerAssist
    }
}