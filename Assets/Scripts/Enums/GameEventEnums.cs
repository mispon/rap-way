using System.ComponentModel;

namespace Enums
{
    public enum GameEventType
    {
        Track,
        Album,
        Clip,
        Concert
    }
    
    public enum GameEventDecisionType
    {
        [Description("gameevent_decision_peacefully")]
        Peacefully,
        [Description("gameevent_decision_aggressively")]
        Aggressively,
        [Description("gameevent_decision_neutral")]
        Neutral
    }
}