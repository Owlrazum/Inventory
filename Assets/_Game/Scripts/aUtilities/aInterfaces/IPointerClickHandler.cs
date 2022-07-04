using UnityEngine;
namespace SNG.UI
{ 
    public interface IPointerClickHandler
    {
        public void OnPointerClick(MouseButtonType pressedButton);
        public RectTransform Rect { get; }
    }
}