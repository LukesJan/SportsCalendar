using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro smazání jedné konkrétní události z kalendáře.
    /// </summary>
    public class DeleteEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="DeleteEventCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance <see cref="MainViewModel"/>, která uchovává stav aplikace a datový repozitář.</param>
        /// <exception cref="ArgumentNullException">Vyvolána, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public DeleteEventCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda lze příkaz vykonat – tedy zda je parametr typu <see cref="Event"/>.
        /// </summary>
        /// <param name="parameter">Událost, která má být smazána.</param>
        /// <returns><c>true</c>, pokud je parametr typu <see cref="Event"/>; jinak <c>false</c>.</returns>
        public bool CanExecute(object parameter) => parameter is Event;

        /// <summary>
        /// Provede smazání události a obnoví zobrazení měsíce.
        /// </summary>
        /// <param name="parameter">Událost typu <see cref="Event"/>, která má být smazána.</param>
        public void Execute(object parameter)
        {
            if (parameter is Event eventToDelete)
            {
                _viewModel.Repository.DeleteEvent(eventToDelete);
                _viewModel.GenerateMonthDays();
            }
        }

        /// <summary>
        /// Událost signalizující změnu ve stavu možnosti vykonání příkazu.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
