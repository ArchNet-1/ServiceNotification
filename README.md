<p align="center">
  <a href="https://archnet-plateform.fr"><img src="https://archnet-plateform.fr/ArchNetLogoWithBck.png" alt="ArchNet" width="240"></a>
</p>

# ArchNet · Notification Service

In-game notifications (toasts): info, success, warning and error messages in a corner of the screen, a few at a time, the others waiting their turn.

- Unity **6000.0** or newer · Needs `com.unity.ugui` and [ArchNet Core](https://github.com/ArchNet-1/ArchNetCore)
- Part of [ArchNet](https://archnet-plateform.fr), free and open-source Unity packages

## Add it to your game (one click)

*ArchNet > Hub > Services > Notifications > Add to game* creates `Assets/ArchNet/Notifications/Scripts/GameNotifications.cs` (your class) and `Prefabs/Notifications.prefab` (top-right stack, above every screen) under the ArchNet Bootstrap.

```csharp
var notifications = Services.Get<GameNotifications>();
notifications.Success("Game saved");
notifications.Error("Check your network.", "No connection");
notifications.Show(NotificationKind.Warning, "Low battery", seconds: 6f);
```

Three at a time by default (the others wait), 3.5 s each, a click closes one; set both on the component. Timing uses unscaled time, so they also close during a pause. Colors per kind are on the Notification View. **Open tool**: the prefab, or in Play Mode one notification of each kind.

## License

MIT, see [LICENSE.md](LICENSE.md).
