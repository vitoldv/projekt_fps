using UnityEngine;

namespace _Core.UI
{
    public class Screen : MonoBehaviour
    {
        public virtual bool IsShown()
        {
            return gameObject.activeSelf;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
