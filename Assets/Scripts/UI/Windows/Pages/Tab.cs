using UnityEngine;

namespace UI.Windows.Pages
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