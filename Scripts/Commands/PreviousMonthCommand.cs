using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro přechod na předchozí měsíc v kalendáři.
    /// Aktualizuje vlastnost <see cref="MainViewModel.SelectedDate"/> o jeden měsíc zpět.
    /// </summary>
    public class PreviousMonthCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="PreviousMonthCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního ViewModelu, která obsahuje vybrané datum.</param>
        /// <exception cref="ArgumentNullException">Vyvoláno, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public PreviousMonthCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda lze příkaz vykonat. Vždy vrací <c>true</c>.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        /// <returns>Vždy <c>true</c>.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Provede přechod na předchozí měsíc tím, že odečte jeden měsíc od aktuálně vybraného data ve ViewModelu.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        public void Execute(object parameter)
        {
            _viewModel.SelectedDate = _viewModel.SelectedDate.AddMonths(-1);
        }

        /// <summary>
        /// Událost signalizující změnu schopnosti vykonání příkazu.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
