using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro přechod na následující měsíc v kalendáři.
    /// Aktualizuje vlastnost <see cref="MainViewModel.SelectedDate"/> o jeden měsíc dopředu.
    /// </summary>
    public class NextMonthCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="NextMonthCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního viewmodelu obsahující aktuálně vybraný datum.</param>
        /// <exception cref="ArgumentNullException">Vyvoláno, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public NextMonthCommand(MainViewModel viewModel)
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
        /// Provede přechod na další měsíc tím, že přičte jeden měsíc k aktuálně vybranému datu ve ViewModelu.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        public void Execute(object parameter)
        {
            _viewModel.SelectedDate = _viewModel.SelectedDate.AddMonths(1);
        }

        /// <summary>
        /// Událost oznamující změnu schopnosti vykonání příkazu.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
