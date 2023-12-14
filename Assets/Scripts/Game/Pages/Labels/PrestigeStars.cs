using UnityEngine;
using UnityEngine.UI;

namespace Game.Pages.Labels
{
    public class PrestigeStars : MonoBehaviour
    {
        [SerializeField] private Image[] prestigeStars;
        [SerializeField] private Sprite fullStar;
        [SerializeField] private Sprite emptyStar;
        [SerializeField] private Sprite halfStar;
        
        public void Display(float prestige)
        {
            foreach (var star in prestigeStars)
            {
                star.sprite = prestige switch
                {
                    >= 1f => fullStar,
                    > 0 => halfStar,
                    _ => emptyStar
                };

                prestige -= 1f;
            }
        }
    }
}