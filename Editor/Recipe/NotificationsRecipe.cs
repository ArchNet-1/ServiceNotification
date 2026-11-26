// ArchNet · Notification Service — Notifications recipe (ArchNet Hub > Services > Notifications)
// Creates, under Assets/ArchNet/Notifications/: Scripts/GameNotifications.cs (your class) and
// Prefabs/Notifications.prefab (top-right stack, above every screen), placed under the
// ArchNet Bootstrap so notifications show in any scene.
// This assembly only compiles when the ArchNet Hub (com.archnet.hub 2.0.0+) is installed.
// Docs: https://archnet-plateform.fr/#ServiceNotification

using System;
using ArchNet.Core;
using ArchNet.Core.Editor;
using ArchNet.Installer.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ArchNet.Service.Editor
{
    public sealed class NotificationsRecipe : ServiceRecipe
    {
        public const string RecipeId = "notifications";
        const string TemplatePath = "Packages/com.archnet.service.notification/Editor/Recipe/Templates/GameNotifications.cs.txt";
        const string ServiceScript = "Scripts/GameNotifications.cs";
        const string PrefabFile = "Prefabs/Notifications.prefab";
        const int SortingOrder = 1100; // above popups: a notification may arrive while one is open

        public override string Id => RecipeId;
        public override string DisplayName => "Notifications";
        public override string Description => "In-game notifications (info, success, warning, error) in a corner of the screen, a few at a time: Services.Get<GameNotifications>().Success(\"Saved\").";
        public override string Category => "UI";
        public override string FolderName => "Notifications";
        public override string[] RequiredServices => new[] { BootstrapRecipe.RecipeId };
        public override bool HasTool => true;

        public override void Create(RecipeContext context)
        {
            UIBuilder.EnsureTextMeshPro();
            context.WriteTemplate(ServiceScript, RecipeContext.LoadTemplate(TemplatePath));
        }

        public override void AfterScriptsCompiled(RecipeContext context)
        {
            Type serviceType = RecipeTools.GeneratedType<NotificationService>(context, "GameNotifications", ServiceScript);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(context.PathOf(PrefabFile))
                                ?? context.SavePrefab(PrefabFile, BuildScreen(serviceType));
            BootstrapRecipe.AddUnderBootstrap(prefab);
        }

        public override void OpenTool()
        {
            if (!EditorApplication.isPlaying)
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>($"{RecipeRunner.RootFolder}/Notifications/{PrefabFile}"));
                return;
            }
            var notifications = Services.Get<NotificationService>();
            notifications.Info("This is an information.", "Info");
            notifications.Success("Game saved.");
            notifications.Warning("Low battery.", "Warning");
            notifications.Error("No connection to the server.", "Error");
        }

        static GameObject BuildScreen(Type serviceType)
        {
            GameObject root = UIBuilder.ScreenCanvas("Notifications", SortingOrder, asPanel: false);

            RectTransform stack = UIBuilder.Rect("Stack", root.transform);
            UIBuilder.Anchor(stack, new Vector2(1f, 1f), new Vector2(620f, 0f), new Vector2(-32f, -32f));
            UIBuilder.Column(stack, 12, alignment: TextAnchor.UpperRight);
            stack.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Image card = UIBuilder.Image("Toast Template", stack, UIBuilder.Window);
            card.gameObject.AddComponent<CanvasGroup>();
            var row = card.gameObject.AddComponent<HorizontalLayoutGroup>();
            row.padding = new RectOffset(0, 24, 0, 0);
            row.spacing = 20;
            row.childControlWidth = row.childControlHeight = true;
            row.childForceExpandWidth = false;
            row.childForceExpandHeight = true;
            Image accent = UIBuilder.Image("Accent", card.transform, UIBuilder.Accent);
            accent.gameObject.AddComponent<LayoutElement>().preferredWidth = 10;
            RectTransform texts = UIBuilder.Rect("Texts", card.transform);
            var textColumn = UIBuilder.Column(texts, 4, 18, TextAnchor.MiddleLeft);
            textColumn.padding.left = 0;
            texts.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            TextMeshProUGUI title = UIBuilder.Text("Title", texts, "Title", 32, TextAlignmentOptions.Left, FontStyles.Bold);
            TextMeshProUGUI message = UIBuilder.Text("Message", texts, "Message", 30, TextAlignmentOptions.Left);
            message.color = UIBuilder.MutedText;
            var button = card.gameObject.AddComponent<Button>();
            button.transition = Selectable.Transition.None;

            var toast = card.gameObject.AddComponent<NotificationToast>();
            RecipeTools.Assign(toast, "_accent", accent);
            RecipeTools.Assign(toast, "_title", title);
            RecipeTools.Assign(toast, "_message", message);
            RecipeTools.Assign(toast, "_button", button);
            card.gameObject.SetActive(false);

            var view = root.AddComponent<NotificationView>();
            RecipeTools.Assign(view, "_stack", stack);
            RecipeTools.Assign(view, "_template", toast);
            Component service = root.AddComponent(serviceType);
            RecipeTools.Assign(service, "_view", view);
            return root;
        }
    }
}
