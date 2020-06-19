using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
       
        
        
        


        #region Monobehavior
        private void Start()
        {
            deltaIndex = 1 / (float) BtnsCnt;
            deltaScroll = 1 / (float) (BtnsCnt-1);
        }
        #endregion


        #region Switching
        private byte activeIndex = 0;
        [SerializeField]private byte BtnsCnt = 3;
        private float deltaIndex;
        private float deltaScroll;
        
        [Header("Switching data")]
        [SerializeField] private Scrollbar horScroll;
        [SerializeField, Min(0.05f)] private float t_scrollingWithBtns = 1;
        private bool lockActiveIndexUpdate = false;
        
        public void OnLeftSwitch()
        {
            if(activeIndex ==0 || lockActiveIndexUpdate)
                return;

            --activeIndex;
            StartCoroutine(Switch());
        }
        public void OnRightSwitch()
        {
            if(activeIndex==BtnsCnt-1 || lockActiveIndexUpdate)
                return;

            ++activeIndex;
            StartCoroutine(Switch());
        }

        private IEnumerator Switch()
        {
            lockActiveIndexUpdate = true;
            float t_Start = Time.time, ct;
            float startValue = horScroll.value, endValue = activeIndex * deltaScroll;

            while ((ct = (Time.time - t_Start) / t_scrollingWithBtns) < 1)
            {
                horScroll.value = Mathf.Lerp(startValue, endValue, ct);
                yield return null;
            }
            horScroll.value = endValue;
            
            lockActiveIndexUpdate = false;
        }
         
        public void OnScrollValueChange()
        {
            if(lockActiveIndexUpdate)
                return;
            
            float curValue = horScroll.value < 0 ? 0 : horScroll.value > 1 ? 1 : horScroll.value;
            activeIndex = (byte) (curValue / deltaIndex);
        }
        #endregion
        


        public void OnClick(int index)
        {
            Debug.Log($"Click at {index}");
        }
    }    
}

