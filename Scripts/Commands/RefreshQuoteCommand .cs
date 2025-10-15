using System;
using System.Windows.Input;

namespace Calendar.Scripts.Commands
{
    /// <summary>
    /// Příkaz pro obnovení náhodného citátu v hlavním ViewModelu.
    /// Vyvolá metodu <see cref="MainViewModel.SelectRandomQuote"/>.
    /// </summary>
    internal class RefreshQuoteCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="RefreshQuoteCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního ViewModelu.</param>
        /// <exception cref="ArgumentNullException">Vyvoláno, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public RefreshQuoteCommand(MainViewModel viewModel)
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
        /// Provede výběr nového náhodného citátu voláním <see cref="MainViewModel.SelectRandomQuote"/>.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        public void Execute(object parameter)
        {
            _viewModel.SelectRandomQuote();
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
