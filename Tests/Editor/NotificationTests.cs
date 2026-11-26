using NUnit.Framework;
using UnityEngine;

namespace ArchNet.Service.Tests
{
    public class NotificationServiceTests
    {
        GameObject _object;
        NotificationService _notifications;
        Transform _stack;

        [SetUp]
        public void SetUp()
        {
            _object = new GameObject("Notifications", typeof(RectTransform));
            var template = new GameObject("Template", typeof(RectTransform)).AddComponent<NotificationToast>();
            template.transform.SetParent(_object.transform);
            template.gameObject.SetActive(false);
            var view = _object.AddComponent<NotificationView>();
            var serialized = new UnityEditor.SerializedObject(view);
            serialized.FindProperty("_template").objectReferenceValue = template;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            _stack = _object.transform;
            _notifications = _object.AddComponent<NotificationService>();
            _notifications.Configure(view, 2, 3f);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_object);

        [Test]
        public void ExtraNotificationsWaitTheirTurn()
        {
            _notifications.Info("one");
            _notifications.Info("two");
            _notifications.Info("three");

            Assert.AreEqual(2, _notifications.OnScreen);
            Assert.AreEqual(1, _notifications.Waiting);
        }

        [Test]
        public void ANotificationClosesAfterItsTimeAndTheNextOneShows()
        {
            _notifications.Info("one", null);
            _notifications.Show(NotificationKind.Error, "two", seconds: 10f);
            _notifications.Info("three");

            _notifications.Tick(3.5f); // "one" is over, "two" stays

            Assert.AreEqual(2, _notifications.OnScreen, "\"three\" took the free place");
            Assert.AreEqual(0, _notifications.Waiting);
        }

        [Test]
        public void ClearRemovesEverything()
        {
            _notifications.Warning("one");
            _notifications.Warning("two");
            _notifications.Warning("three");
            _notifications.Clear();

            Assert.AreEqual(0, _notifications.OnScreen);
            Assert.AreEqual(0, _notifications.Waiting);
            Assert.AreEqual(1, _stack.childCount, "only the hidden template is left");
        }
    }
}
