using UnityEngine;
namespace SNG.UI
{ 
    public interface IPointerTouchHandler
    {
        public void OnPointerTouch();
        public RectTransform Rect { get; }
        public int InstanceID { get; }
    }
}