// ArchNet · Notification Service
// In-game notifications (toasts) in a corner of the screen: a few at a time, newest at the
// bottom, each closing after a few seconds or on click; the others wait their turn. The Hub
// creates GameNotifications (your class):
//
//   Services.Get<GameNotifications>().Success("Game saved");
//   Services.Get<GameNotifications>().Error("No connection", "Check your network.");
//
// Timing uses unscaled time, so notifications also close while the game is paused.
// Docs: https://archnet-plateform.fr/#ServiceNotification

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ArchNet.Core;
using UnityEngine;

[assembly: InternalsVisibleTo("ArchNet.Service.Notification.Tests")]

namespace ArchNet.Service
{
    public enum NotificationKind { Info, Success, Warning, Error }

    public class NotificationService : GameService
    {
        class Active
        {
            public NotificationToast Toast;
            public float Remaining;
        }

        readonly struct Request
        {
            public readonly NotificationKind Kind;
            public readonly string Title;
            public readonly string Message;
            public readonly float Seconds;

            public Request(NotificationKind kind, string title, string message, float seconds)
            {
                Kind = kind;
                Title = title;
                Message = message;
                Seconds = seconds;
            }
        }

        [SerializeField] NotificationView _view;
        [Tooltip("Notifications on screen at once; the next ones wait.")]
        [SerializeField, Min(1)] int _maxOnScreen = 3;
        [Tooltip("Seconds a notification stays when no duration is given.")]
        [SerializeField, Min(0.5f)] float _duration = 3.5f;

        readonly List<Active> _active = new List<Active>();
        readonly Queue<Request> _waiting = new Queue<Request>();

        public int OnScreen => _active.Count;
        public int Waiting => _waiting.Count;

        internal void Configure(NotificationView view, int maxOnScreen, float duration)
        {
            _view = view;
            _maxOnScreen = maxOnScreen;
            _duration = duration;
        }

        public void Info(string message, string title = null) => Show(NotificationKind.Info, message, title);
        public void Success(string message, string title = null) => Show(NotificationKind.Success, message, title);
        public void Warning(string message, string title = null) => Show(NotificationKind.Warning, message, title);
        public void Error(string message, string title = null) => Show(NotificationKind.Error, message, title);

        /// <summary>Shows a notification now, or as soon as there is room. seconds: how long it stays (default on the component).</summary>
        public void Show(NotificationKind kind, string message, string title = null, float seconds = 0f)
        {
            _waiting.Enqueue(new Request(kind, title, message, seconds > 0f ? seconds : _duration));
            ShowWaiting();
        }

        /// <summary>Closes every notification and forgets the waiting ones.</summary>
        public void Clear()
        {
            _waiting.Clear();
            foreach (Active active in _active.ToArray()) Close(active);
        }

        void Update() => Tick(Time.unscaledDeltaTime);

        internal void Tick(float seconds)
        {
            foreach (Active active in _active.ToArray())
            {
                active.Remaining -= seconds;
                if (active.Remaining <= 0f) Close(active);
            }
        }

        void ShowWaiting()
        {
            if (_view == null)
            {
                Debug.LogError("[ArchNet] The notification service has no NotificationView assigned.", this);
                _waiting.Clear();
                return;
            }
            while (_active.Count < _maxOnScreen && _waiting.Count > 0)
            {
                Request request = _waiting.Dequeue();
                var active = new Active { Remaining = request.Seconds };
                active.Toast = _view.Spawn(request.Kind, request.Title, request.Message, () => Close(active));
                _active.Add(active);
            }
        }

        void Close(Active active)
        {
            if (!_active.Remove(active)) return;
            if (active.Toast != null) active.Toast.Close();
            ShowWaiting();
        }
    }
}
