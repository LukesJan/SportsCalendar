using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Calendar.Scripts
{
    /// <summary>
    /// Statická třída pro správu zobrazení notifikací jako samostatných oken.
    /// </summary>
    public static class NotificationManager
    {
        private static readonly List<NotificationWindow> _activeNotifications = new List<NotificationWindow>();
        private const double NotificationHeight = 120;
        private const double MarginBetween = 10;
        private const double PositionOffset = 10;

        /// <summary>
        /// Zobrazí novou notifikaci se zadanou zprávou,
        /// pokud jsou notifikace povolené v nastavení.
        /// </summary>
        /// <param name="message">Text notifikace.</param>
        public static void ShowNotification(string message)
        {
            if (!Properties.Settings.Default.NotificationsEnabled) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = new NotificationWindow(message);
                notification.Closed += (s, e) => RemoveNotification(notification);
                _activeNotifications.Add(notification);
                notification.Show();
                UpdateNotificationPositions();
            });
        }

        /// <summary>
        /// Odebere zavřené okno z aktivních notifikací a aktualizuje pozice zbylých.
        /// </summary>
        /// <param name="notification">Okno notifikace k odstranění.</param>
        private static void RemoveNotification(NotificationWindow notification)
        {
            _activeNotifications.Remove(notification);
            UpdateNotificationPositions();
        }

        /// <summary>
        /// Přepočítá a nastaví pozice všech aktivních notifikačních oken,
        /// aby byly hezky seskupené vpravo nahoře obrazovky.
        /// </summary>
        private static void UpdateNotificationPositions()
        {
            double currentOffset = 0;
            foreach (var notification in _activeNotifications)
            {
                notification.Top = PositionOffset + currentOffset;
                notification.Left = SystemParameters.WorkArea.Width - notification.Width - PositionOffset;
                currentOffset += NotificationHeight + MarginBetween;
            }
        }
    }
}
