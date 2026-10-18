using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulboundLegends.Runtime
{
    /// <summary>Simple on-screen movement stick used by generated mobile HUDs.</summary>
    public sealed class SLVirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform background;
        public RectTransform knob;
        public float deadZone = .12f;
        public static Vector2 Movement { get; private set; }

        public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);
        public void OnDrag(PointerEventData eventData)
        {
            if (background == null || knob == null) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out var local)) return;
            Vector2 half = background.rect.size * .5f;
            if (half.x <= 0 || half.y <= 0) return;
            Vector2 normalized = new Vector2(local.x / half.x, local.y / half.y);
            normalized = Vector2.ClampMagnitude(normalized, 1f);
            knob.anchoredPosition = new Vector2(normalized.x * half.x * .58f, normalized.y * half.y * .58f);
            Movement = normalized.magnitude < deadZone ? Vector2.zero : normalized;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Movement = Vector2.zero;
            if (knob != null) knob.anchoredPosition = Vector2.zero;
        }
        void OnDisable() { Movement = Vector2.zero; }
    }
}
