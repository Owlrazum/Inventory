using UnityEngine;
namespace SNG.UI
{
    public interface IPointerEnterExitHandler
    {
        public void OnPointerEnter();
        public void OnPointerExit();
        public RectTransform Rect { get; }
    }
}