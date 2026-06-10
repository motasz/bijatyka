using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ButtonNavigationPlayer: MonoBehaviour, ISelectHandler
    {
        public void OnSelect(BaseEventData eventData)
        {
            UISoundPlayer.Instance.PlayNavigation();
        }
    }
}