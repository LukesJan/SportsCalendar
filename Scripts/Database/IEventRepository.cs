using System;
using System.Collections.Generic;

namespace Calendar.Scripts
{
    /// <summary>
    /// Rozhraní pro správu událostí.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>Vrátí události pro dané datum.</summary>
        List<Event> GetEvents(DateTime date);

        /// <summary>Přidá novou událost.</summary>
        void AddEvent(Event footballEvent);

        /// <summary>Aktualizuje existující událost.</summary>
        void UpdateEvent(Event footballEvent);

        /// <summary>Smaže jednu událost.</summary>
        void DeleteEvent(Event footballEvent);

        /// <summary>Smaže více událostí najednou.</summary>
        void DeleteEvents(IEnumerable<Event> events);

        /// <summary>Vrátí všechny události.</summary>
        List<Event> GetAllEvents();
    }
}
