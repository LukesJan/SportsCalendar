using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Calendar.Scripts.Commands
{
    /// <summary>
    /// Příkaz pro export všech událostí do CSV souboru.
    /// </summary>
    public class ExportEventsCommand : ICommand
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Inicializuje novou instanci třídy <see cref="ExportEventsCommand"/>.
        /// </summary>
        /// <param name="viewModel">Instance hlavního viewmodelu, ze kterého se získávají události.</param>
        /// <exception cref="ArgumentNullException">Vyvolána, pokud je <paramref name="viewModel"/> <c>null</c>.</exception>
        public ExportEventsCommand(MainViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        /// <summary>
        /// Určuje, zda lze příkaz provést. Tento příkaz je vždy dostupný.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        /// <returns>Vždy <c>true</c>.</returns>
        public bool CanExecute(object parameter) => true;

        /// <summary>
        /// Otevře dialog pro uložení souboru a exportuje události do CSV souboru.
        /// </summary>
        /// <param name="parameter">Nepoužívá se.</param>
        public void Execute(object parameter)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    DefaultExt = "csv",
                    FileName = $"Events_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var allEvents = _viewModel.Repository.GetAllEvents();
                    ExportToCsv(allEvents, saveFileDialog.FileName);

                    MessageBox.Show($"Successfully exported {allEvents.Count} events to:\n{saveFileDialog.FileName}",
                                    "Export Successful",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting events: {ex.Message}",
                                "Export Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Exportuje zadané události do CSV souboru na určené cestě.
        /// </summary>
        /// <param name="events">Seznam událostí k exportu.</param>
        /// <param name="filePath">Cesta k výstupnímu CSV souboru.</param>
        private void ExportToCsv(IEnumerable<Event> events, string filePath)
        {
            var csvContent = new StringBuilder();

            // Hlavička CSV
            csvContent.AppendLine("Date,Time,Title,Tag");

            foreach (var ev in events.OrderBy(e => e.Date).ThenBy(e => e.Time))
            {
                var date = ev.Date.ToString("yyyy-MM-dd");
                var time = ev.Time.ToString(@"hh\:mm");
                var title = EscapeCsvField(ev.Title);
                var tag = EscapeCsvField(ev.Tag);

                csvContent.AppendLine($"{date},{time},{title},{tag}");
            }

            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
        }

        /// <summary>
        /// Ošetří textová pole pro zápis do CSV – escapuje uvozovky a oddělovače.
        /// </summary>
        /// <param name="input">Vstupní textové pole.</param>
        /// <returns>Escapovaná hodnota vhodná pro CSV.</returns>
        private string EscapeCsvField(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            if (input.Contains(",") || input.Contains("\"") || input.Contains("\n") || input.Contains("\r"))
            {
                return $"\"{input.Replace("\"", "\"\"")}\"";
            }
            return input;
        }

        /// <summary>
        /// Událost signalizující, že se změnil stav, kdy lze příkaz vykonat.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
