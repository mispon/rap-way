using UnityEngine;

namespace UI.Windows.GameScreen
{
    public class Tab : MonoBehaviour
    {
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
        
        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}