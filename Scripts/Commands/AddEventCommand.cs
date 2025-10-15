using System;
using System.Windows;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro otevření okna pro přidání události a její přidání do kalendáře.
    /// </summary>
    public class AddEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="AddEventCommand"/>.
        /// </summary>
        /// <param name="viewModel">Hlavní ViewModel aplikace.</param>
        /// <exception cref="ArgumentNullException">Vyvolána, pokud je <paramref name="viewModel"/> null.</exception>
        public AddEventCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda může být příkaz vykonán.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        /// <returns>Vždy vrací <c>true</c>, protože příkaz může být vykonán kdykoli.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Provede příkaz: otevře okno pro přidání události a přidá ji, pokud jsou data validní.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        public void Execute(object parameter)
        {
            if (TryShowEventDialog(out string title, out string tag, out string time))
            {
                if (TimeSpan.TryParse(time, out TimeSpan parsedTime))
                {
                    CreateAndAddEvent(title, tag, parsedTime);
                }
                else
                {
                    ShowTimeFormatError();
                }
            }
        }

        /// <summary>
        /// Zobrazí dialogové okno pro přidání události a získá vstup od uživatele.
        /// </summary>
        /// <param name="title">Vrací zadaný název události.</param>
        /// <param name="tag">Vrací zvolený štítek události.</param>
        /// <param name="time">Vrací zadaný čas události ve formátu řetězce.</param>
        /// <returns><c>true</c>, pokud uživatel potvrdil dialog, jinak <c>false</c>.</returns>
        private bool TryShowEventDialog(out string title, out string tag, out string time)
        {
            var addEventWindow = new AddEventWindow();
            bool? dialogResult = addEventWindow.ShowDialog();

            title = addEventWindow.TitleText;
            tag = addEventWindow.TagText;
            time = addEventWindow.TimeText;

            return dialogResult == true;
        }

        /// <summary>
        /// Vytvoří novou událost a přidá ji do úložiště.
        /// </summary>
        /// <param name="title">Název události.</param>
        /// <param name="tag">Štítek události.</param>
        /// <param name="time">Čas události.</param>
        private void CreateAndAddEvent(string title, string tag, TimeSpan time)
        {
            var newEvent = new Event
            {
                Date = _viewModel.SelectedDate,
                Time = time,
                Title = title,
                Tag = tag
            };

            _viewModel.Repository.AddEvent(newEvent);
            _viewModel.GenerateMonthDays();
        }

        /// <summary>
        /// Zobrazí chybové hlášení v případě neplatného formátu času.
        /// </summary>
        private static void ShowTimeFormatError()
        {
            MessageBox.Show("Invalid time format. Please use the HH:mm format (e.g., 14:30).",
                            "Time Format Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
        }

        /// <summary>
        /// Událost vyvolaná při změně, zda příkaz může být proveden.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
