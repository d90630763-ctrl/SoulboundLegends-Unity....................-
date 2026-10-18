using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulboundLegends.Lobby
{
    public class SL_ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        public float pressedScale = 0.96f;
        public float returnSpeed = 16f;
        public AudioClip clickSound;
        private Vector3 baseScale;
        private AudioSource audioSource;
        private bool down;

        private void Awake()
        {
            baseScale = transform.localScale;
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        private void Update()
        {
            Vector3 target = down ? baseScale * pressedScale : baseScale;
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.unscaledDeltaTime * returnSpeed);
        }

        public void OnPointerDown(PointerEventData eventData) => down = true;
        public void OnPointerUp(PointerEventData eventData) => down = false;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickSound != null) audioSource.PlayOneShot(clickSound);
        }
    }

    public class SL_PanelFade : MonoBehaviour
    {
        public float duration = 0.22f;
        private CanvasGroup group;
        private Coroutine routine;
        private void Awake() { group = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>(); }
        private void OnEnable() { if (routine != null) StopCoroutine(routine); routine = StartCoroutine(FadeIn()); }
        private IEnumerator FadeIn()
        {
            group.alpha = 0f;
            float t = 0f;
            while (t < duration) { t += Time.unscaledDeltaTime; group.alpha = Mathf.Clamp01(t / duration); yield return null; }
            group.alpha = 1f;
        }
    }
}
