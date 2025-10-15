using System;
using System.Linq;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro změnu aktuálně vybraného data v kalendáři.
    /// Aktualizuje vlastnosti <see cref="MainViewModel.SelectedDate"/> a <see cref="MainViewModel.SelectedDay"/>.
    /// </summary>
    public class ChangeSelectedDateCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="ChangeSelectedDateCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního viewmodelu, který obsahuje vybrané datum a dny.</param>
        /// <exception cref="ArgumentNullException">Vyvolána, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public ChangeSelectedDateCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda lze příkaz vykonat. Příkaz lze spustit pouze tehdy, pokud je parametr typu <see cref="DateTime"/>.
        /// </summary>
        /// <param name="parameter">Očekává se <see cref="DateTime"/> reprezentující nové vybrané datum.</param>
        /// <returns><c>true</c> pokud je parametr <see cref="DateTime"/>; jinak <c>false</c>.</returns>
        public bool CanExecute(object parameter) => parameter is DateTime;

        /// <summary>
        /// Nastaví nové vybrané datum a aktualizuje stav jednotlivých dnů v kalendáři.
        /// </summary>
        /// <param name="parameter">Nové datum typu <see cref="DateTime"/>, které má být vybráno.</param>
        public void Execute(object parameter)
        {
            if (parameter is DateTime date)
            {
                _viewModel.SelectedDate = date;

                /// Aktualizace výběru pro každý den
                foreach (var day in _viewModel.Days)
                {
                    day.IsSelected = day.Date.Date == date.Date;
                }

                /// Nastavení aktuálně vybraného dne
                _viewModel.SelectedDay = _viewModel.Days
                    .FirstOrDefault(d => d.Date.Date == date.Date);
            }
        }

        /// <summary>
        /// Událost signalizující změnu, kdy lze příkaz vykonat.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
