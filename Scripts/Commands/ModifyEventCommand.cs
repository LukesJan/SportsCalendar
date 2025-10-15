using System;
using System.Windows.Input;
using Calendar.Scripts;

namespace Calendar.Commands
{
    /// <summary>
    /// Příkaz pro úpravu existující události v kalendáři.
    /// Otevře okno pro úpravu a po potvrzení aktualizuje událost v repozitáři.
    /// </summary>
    public class ModifyEventCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="ModifyEventCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního viewmodelu, který obsahuje repozitář a metodu pro generování dnů.</param>
        /// <exception cref="ArgumentNullException">Vyvoláno, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public ModifyEventCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda lze příkaz vykonat. Vykonání je možné pouze pokud je parametr typu <see cref="Event"/>.
        /// </summary>
        /// <param name="parameter">Událost, která má být upravena.</param>
        /// <returns><c>true</c>, pokud je parametr platný <see cref="Event"/>; jinak <c>false</c>.</returns>
        public bool CanExecute(object parameter) => parameter is Event;

        /// <summary>
        /// Spustí proces úpravy události – otevře okno pro úpravu a po potvrzení aktualizuje událost v repozitáři.
        /// </summary>
        /// <param name="parameter">Událost typu <see cref="Event"/>, která má být upravena.</param>
        public void Execute(object parameter)
        {
            if (parameter is Event eventToModify)
            {
                
                var editWindow = new AddEventWindow(eventToModify);

                
                if (editWindow.ShowDialog() == true && editWindow.ResultEvent != null)
                {
                    _viewModel.Repository.UpdateEvent(editWindow.ResultEvent);
                    _viewModel.GenerateMonthDays(); 
                }
            }
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
