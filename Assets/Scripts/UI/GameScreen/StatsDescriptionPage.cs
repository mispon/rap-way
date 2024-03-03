using Core.Context;
using UI.Windows.GameScreen;
using UnityEngine;
using UnityEngine.UI;

namespace UI.GameScreen
{
    /// <summary>
    /// Describe main characteristic page
    /// </summary>
    public class StatsDescriptionPage : Page
    {
        [SerializeField] private Image icon;
        [SerializeField] private Text statName;
        [SerializeField] private Text statDesc;

        public override void Show(object ctx = null)
        {
            var sprite  = ctx.ValueByKey<Sprite>("icon");
            var nameKey = ctx.ValueByKey<string>("nameKey");
            var descKey = ctx.ValueByKey<string>("descKey");
            
            
            icon.sprite = sprite;
            statName.text = GetLocale(nameKey).ToUpper();
            statDesc.text = GetLocale(descKey);

            base.Show(ctx);
        }
    }
}