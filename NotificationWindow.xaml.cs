using System;
using System.Windows;
using System.Windows.Threading;

namespace Calendar
{
    /// <summary>
    /// Okno pro zobrazení notifikace s automatickým zavřením po určité době.
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private static double _verticalOffset = 0;
        private const double NotificationHeight = 120;
        private const double MarginBetween = 10;

        /// <summary>
        /// Zpráva, která se zobrazí v notifikaci.
        /// </summary>
        public string NotificationMessage
        {
            get { return (string)GetValue(NotificationMessageProperty); }
            set { SetValue(NotificationMessageProperty, value); }
        }

        /// <summary>
        /// DependencyProperty pro vlastnost NotificationMessage.
        /// </summary>
        public static readonly DependencyProperty NotificationMessageProperty =
           DependencyProperty.Register("NotificationMessage", typeof(string), typeof(NotificationWindow));

        /// <summary>
        /// Konstruktor notifikačního okna.
        /// Nastaví zprávu, pozici okna a spustí časovač pro automatické zavření.
        /// </summary>
        /// <param name="message">Text notifikace, který se zobrazí uživateli.</param>
        public NotificationWindow(string message)
        {
            InitializeComponent();
            NotificationMessage = message;
            DataContext = this;

            // Nastavení pozice okna v pravém dolním rohu s odsazením podle počtu otevřených notifikací
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = SystemParameters.WorkArea.Height - Height - 10 - _verticalOffset;

            _verticalOffset += NotificationHeight + MarginBetween;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            timer.Tick += (s, e) => { timer.Stop(); CloseNotification(); };
            timer.Start();
        }

        /// <summary>
        /// Obsluha kliknutí na tlačítko pro ruční zavření notifikace.
        /// </summary>
        /// <param name="sender">Odesílatel události.</param>
        /// <param name="e">Argumenty události kliknutí.</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseNotification();
        }

        /// <summary>
        /// Zavře notifikaci a upraví vertikální odsazení pro další notifikace.
        /// </summary>
        private void CloseNotification()
        {
            _verticalOffset -= NotificationHeight + MarginBetween;
            Close();
        }
    }
}
