using System;
using System.Collections.ObjectModel;

namespace Calendar.Scripts
{
    /// <summary>
    /// ViewModel reprezentující jeden den v kalendáři.
    /// </summary>
    public class DayViewModel
    {
        /// <summary>
        /// Určuje, zda den patří do aktuálně zobrazeného měsíce.
        /// </summary>
        public bool IsCurrentMonth { get; set; }

        /// <summary>
        /// Určuje, zda je tento den vybraný (označený).
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Datum tohoto dne.
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Kolekce událostí, které se v tento den konají.
        /// </summary>
        public ObservableCollection<Event> Events { get; }

        /// <summary>
        /// Inicializuje nový denní ViewModel s daným datem.
        /// </summary>
        /// <param name="date">Datum dne</param>
        public DayViewModel(DateTime date)
        {
            Date = date;
            Events = new ObservableCollection<Event>();
        }
    }
}
