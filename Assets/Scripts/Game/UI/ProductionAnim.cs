using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.UI
{
    public enum ProductionAnimType
    {
        Touchpad = 1,    
        Pathephone = 2,    
        Phone = 3    
    }
    
    [RequireComponent(typeof(Animator))]
    public class ProductionAnim : MonoBehaviour
    {
        private static readonly int _propHash = Animator.StringToHash("Type");
        
        [SerializeField] private ProductionAnimType Type = ProductionAnimType.Touchpad;
        [SerializeField] private bool randomAnim;

        public void Refresh()
        {
            var animator = GetComponent<Animator>();

            if (randomAnim)
            {
                var types = (ProductionAnimType[]) Enum.GetValues(typeof(ProductionAnimType));
                Type = types[Random.Range(0, types.Length)];
            }
            
            animator.SetInteger(_propHash, (int) Type);
        }
    }
}
