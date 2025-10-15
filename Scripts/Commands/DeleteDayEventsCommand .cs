using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro smazání všech událostí v daný den.
    /// </summary>
    public class DeleteDayEventsCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="DeleteDayEventsCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance <see cref="MainViewModel"/>, se kterou příkaz pracuje.</param>
        /// <exception cref="ArgumentNullException">Vyvolána, pokud je <paramref name="viewModel"/> null.</exception>
        public DeleteDayEventsCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda může být příkaz vykonán – tedy zda existují události ke smazání pro daný den.
        /// </summary>
        /// <param name="parameter">Očekává se <see cref="DateTime"/> reprezentující den.</param>
        /// <returns><c>true</c>, pokud existují události k danému dni; jinak <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return parameter is DateTime date &&
                   _viewModel.Repository.GetEvents(date).Any();
        }

        /// <summary>
        /// Provede příkaz – smaže všechny události pro daný den po potvrzení uživatelem.
        /// </summary>
        /// <param name="parameter">Očekává se <see cref="DateTime"/> představující den, jehož události se mají smazat.</param>
        public void Execute(object parameter)
        {
            if (parameter is DateTime date)
            {
                if (ShowConfirmationDialog(date))
                {
                    try
                    {
                        var eventsToDelete = _viewModel.Repository.GetEvents(date).ToList();
                        _viewModel.Repository.DeleteEvents(eventsToDelete);

                        MessageBox.Show(
                            $"All events for {date.ToShortDateString()} were deleted.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information
                        );

                        _viewModel.GenerateMonthDays();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error deleting events: {ex.Message}",
                            "Database Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Zobrazí dialogové okno pro potvrzení smazání všech událostí v daný den.
        /// </summary>
        /// <param name="date">Datum, pro které se mají události smazat.</param>
        /// <returns><c>true</c>, pokud uživatel smazání potvrdil; jinak <c>false</c>.</returns>
        private bool ShowConfirmationDialog(DateTime date)
        {
            return MessageBox.Show(
                $"Delete all events on {date.ToShortDateString()}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            ) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Událost signalizující, že se změnila možnost vykonat příkaz.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
