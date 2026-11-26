// ArchNet · Notification Service — one notification
// A colored bar for its kind, an optional title and the message; fades in, and out when it
// closes. Clicking it closes it.
// Docs: https://archnet-plateform.fr/#ServiceNotification

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArchNet.Service
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NotificationToast : MonoBehaviour
    {
        const float FadeSeconds = 0.2f;

        [SerializeField] Graphic _accent;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _message;
        [SerializeField] Button _button;

        public void Set(Color color, string title, string message, Action onClick)
        {
            if (_accent != null) _accent.color = color;
            if (_title != null)
            {
                _title.text = title ?? "";
                _title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            }
            if (_message != null) _message.text = message ?? "";
            if (_button != null) _button.onClick.AddListener(() => onClick?.Invoke());
            if (Application.isPlaying) StartCoroutine(Fade(0f, 1f, destroy: false));
        }

        public void Close()
        {
            if (Application.isPlaying && isActiveAndEnabled) StartCoroutine(Fade(GetComponent<CanvasGroup>().alpha, 0f, destroy: true));
            else DestroyImmediate(gameObject);
        }

        IEnumerator Fade(float from, float to, bool destroy)
        {
            var group = GetComponent<CanvasGroup>();
            for (float time = 0f; time < FadeSeconds; time += Time.unscaledDeltaTime)
            {
                group.alpha = Mathf.Lerp(from, to, time / FadeSeconds);
                yield return null;
            }
            group.alpha = to;
            if (destroy) Destroy(gameObject);
        }
    }
}
