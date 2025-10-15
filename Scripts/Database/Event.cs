using System;

namespace Calendar.Scripts
{
    /// <summary>
    /// Reprezentuje událost v kalendáři.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Jedinečný identifikátor události.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Datum události.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Název události.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Kategorie nebo štítek události.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Čas události (hodiny a minuty).
        /// </summary>
        public TimeSpan Time { get; set; }
    }
}
