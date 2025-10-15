using System;
using System.Windows;
using Calendar.Scripts;

namespace Calendar
{
    /// <summary>
    /// Hlavní okno aplikace Calendar.
    /// Zajišťuje inicializaci UI komponent.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Konstruktor okna.
        /// Inicializuje komponenty a události zavření okna.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        /// <summary>
        /// Obsluha události zavření hlavního okna.
        /// Volá metodu CleanUp ViewModelu pro uvolnění zdrojů.
        /// </summary>
        /// <param name="sender">Odesílatel události.</param>
        /// <param name="e">Argumenty události.</param>
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.CleanUp();
            }
        }
    }
}
