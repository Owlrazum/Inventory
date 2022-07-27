using UnityEngine;
namespace Orazum.UI
{
    public interface IPointerEnterExitHandler
    {
        public bool ShouldInvokePointerEnterExitEvents { get; }
        public void OnPointerEnter();
        public void OnPointerExit();
        public RectTransform InteractionRect { get; }
        public bool EnterState { get; set; }
        public int InstanceID { get; }
    }
}