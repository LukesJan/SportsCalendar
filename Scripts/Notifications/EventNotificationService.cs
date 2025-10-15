using System;
using System.Collections.Generic;
using System.Linq;

namespace Calendar.Scripts
{
    /// <summary>
    /// Service pro sledování nadcházejících událostí a vyvolání notifikací.
    /// </summary>
    public class EventNotificationService
    {
        private readonly IEventRepository _repository;
        private readonly Dictionary<int, bool> _notifiedEvents = new Dictionary<int, bool>();
        private const int NotificationThresholdMinutes = 10;
        public event Action<string> OnNotification;

        /// <summary>
        /// Vytvoří novou instanci služby s daným repository pro události.
        /// </summary>
        /// <param name="repository">Repository pro práci s událostmi.</param>
        /// <exception cref="ArgumentNullException">Pokud je repository null.</exception>
        public EventNotificationService(IEventRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Zkontroluje aktuální čas a vyvolá notifikace u událostí,
        /// které se blíží do nastaveného časového okna (např. 10 minut).
        /// </summary>
        public void CheckUpcomingEvents()
        {
            DateTime currentTime = DateTime.Now;
            DateTime notificationThreshold = currentTime.AddMinutes(NotificationThresholdMinutes);

            IEnumerable<Event> relevantEvents = GetRelevantEvents(currentTime, notificationThreshold);
            ProcessEventNotifications(relevantEvents, currentTime);
            CleanupExpiredNotifications(relevantEvents);
        }

        /// <summary>
        /// Vrátí seznam událostí, které jsou v notifikačním časovém okně.
        /// </summary>
        /// <param name="currentTime">Aktuální čas.</param>
        /// <param name="threshold">Časový limit pro notifikaci.</param>
        /// <returns>Seznam relevantních událostí.</returns>
        private IEnumerable<Event> GetRelevantEvents(DateTime currentTime, DateTime threshold)
        {
            return _repository.GetEvents(currentTime.Date)
                .Where(e => IsWithinNotificationWindow(e, currentTime, threshold));
        }

        /// <summary>
        /// Zkontroluje, jestli událost je v notifikačním časovém okně.
        /// Podporuje kontrolu i přes půlnoc (událost na další den).
        /// </summary>
        /// <param name="ev">Událost ke kontrole.</param>
        /// <param name="currentTime">Aktuální čas.</param>
        /// <param name="threshold">Časový limit notifikace.</param>
        /// <returns>True pokud je událost v okně, jinak false.</returns>
        private bool IsWithinNotificationWindow(Event ev, DateTime currentTime, DateTime threshold)
        {
            return (ev.Date.Date == currentTime.Date &&
                    ev.Time >= currentTime.TimeOfDay &&
                    ev.Time <= threshold.TimeOfDay) ||
                   (ev.Date.Date == currentTime.AddDays(1).Date &&
                    ev.Time <= threshold.TimeOfDay);
        }

        /// <summary>
        /// Projde seznam událostí a vyvolá notifikaci u těch,
        /// u kterých dosud nebyla vyvolána.
        /// </summary>
        /// <param name="events">Události k zpracování.</param>
        /// <param name="currentTime">Aktuální čas.</param>
        private void ProcessEventNotifications(IEnumerable<Event> events, DateTime currentTime)
        {
            foreach (var ev in events)
            {
                if (!_notifiedEvents.ContainsKey(ev.Id))
                {
                    TriggerNotification(ev, currentTime);
                    _notifiedEvents[ev.Id] = true;
                }
            }
        }

        /// <summary>
        /// Vytvoří text notifikace a vyvolá událost OnNotification.
        /// </summary>
        /// <param name="ev">Událost, kterou notifikujeme.</param>
        /// <param name="currentTime">Aktuální čas.</param>
        private void TriggerNotification(Event ev, DateTime currentTime)
        {
            double minutesUntilEvent = CalculateMinutesUntilEvent(ev, currentTime);
            string message = $"{ev.Title} ({ev.Tag})\nStarts in {minutesUntilEvent:0} minutes at {ev.Time:hh\\:mm}";
            OnNotification?.Invoke(message);
        }

        /// <summary>
        /// Spočítá počet minut do začátku události.
        /// </summary>
        /// <param name="ev">Událost.</param>
        /// <param name="currentTime">Aktuální čas.</param>
        /// <returns>Počet minut do začátku.</returns>
        private double CalculateMinutesUntilEvent(Event ev, DateTime currentTime)
        {
            return ev.Date.Date == currentTime.Date
                ? (ev.Time - currentTime.TimeOfDay).TotalMinutes
                : (TimeSpan.FromHours(24) - currentTime.TimeOfDay + ev.Time).TotalMinutes;
        }

        /// <summary>
        /// Vymaže záznamy o notifikacích u událostí,
        /// které už nejsou v aktuálním notifikačním okně.
        /// </summary>
        /// <param name="currentEvents">Aktuální relevantní události.</param>
        private void CleanupExpiredNotifications(IEnumerable<Event> currentEvents)
        {
            var currentEventIds = new HashSet<int>(currentEvents.Select(e => e.Id));
            var expiredIds = _notifiedEvents.Keys.Where(id => !currentEventIds.Contains(id)).ToList();

            foreach (var id in expiredIds)
            {
                _notifiedEvents.Remove(id);
            }
        }

        /// <summary>
        /// Resetuje interní stav oznámených událostí,
        /// aby bylo možné notifikovat znovu.
        /// </summary>
        public void ResetNotifiedEvents()
        {
            _notifiedEvents.Clear();
        }
    }
}
