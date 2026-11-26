// ArchNet · Notification Service — notification area
// Holds the stack of notifications (a corner of the screen) and the hidden template copied for
// each one, with a color per kind.
// Docs: https://archnet-plateform.fr/#ServiceNotification

using System;
using UnityEngine;

namespace ArchNet.Service
{
    public class NotificationView : MonoBehaviour
    {
        [SerializeField] RectTransform _stack;
        [Tooltip("Hidden template copied for each notification.")]
        [SerializeField] NotificationToast _template;
        [SerializeField] Color _info = new Color32(0x8b, 0x5c, 0xf6, 0xff);
        [SerializeField] Color _success = new Color32(0x22, 0xc5, 0x5e, 0xff);
        [SerializeField] Color _warning = new Color32(0xf5, 0x9e, 0x0b, 0xff);
        [SerializeField] Color _error = new Color32(0xef, 0x44, 0x44, 0xff);

        void Awake()
        {
            if (_template != null) _template.gameObject.SetActive(false);
        }

        public NotificationToast Spawn(NotificationKind kind, string title, string message, Action onClick)
        {
            if (_template == null) throw new InvalidOperationException("The notification view has no template.");
            NotificationToast toast = Instantiate(_template, _stack != null ? _stack : (RectTransform)transform);
            toast.gameObject.SetActive(true);
            toast.Set(ColorOf(kind), title, message, onClick);
            return toast;
        }

        public Color ColorOf(NotificationKind kind) => kind switch
        {
            NotificationKind.Success => _success,
            NotificationKind.Warning => _warning,
            NotificationKind.Error => _error,
            _ => _info,
        };
    }
}
