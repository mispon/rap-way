using UnityEngine;

namespace UI.Windows.GameScreen
{
    public class Tab : MonoBehaviour
    {
        public virtual void Open()
        {
            BeforeOpen();
            gameObject.SetActive(true);
            AfterOpen();
        }
        
        public virtual void Close()
        {
            BeforeClose();
            gameObject.SetActive(false);
            AfterClose();
        }
        
        protected virtual void BeforeOpen() {}
        protected virtual void AfterOpen() {}
        protected virtual void BeforeClose() {}
        protected virtual void AfterClose() {}
    }
}