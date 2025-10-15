using Calendar.Scripts;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Calendar.Scripts
{
    /// <summary>
    /// Singleton repozitář pro práci s událostmi v SQLite databázi.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private static readonly Lazy<EventRepository> _instance =
            new Lazy<EventRepository>(() => new EventRepository());
        public static EventRepository Instance => _instance.Value;

        private const string ConnectionString = "Data Source=events.db";
        private const string CreateTableQuery =
            "CREATE TABLE IF NOT EXISTS Events (Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "Date TEXT, Time TEXT, Title TEXT, Tag TEXT)";

        private EventRepository()
        {
            CreateEventsTableIfNotExists();
        }

        private void CreateEventsTableIfNotExists()
        {
            ExecuteNonQuery(CreateTableQuery);
        }

        public List<Event> GetEvents(DateTime date)
        {
            const string selectQuery = "SELECT * FROM Events WHERE Date = @date";
            var parameters = new Dictionary<string, object>
            {
                { "@date", date.ToShortDateString() }
            };

            return ExecuteQuery(selectQuery, parameters);
        }

        public void AddEvent(Event footballEvent)
        {
            const string insertQuery =
                "INSERT INTO Events (Date, Time, Title, Tag) " +
                "VALUES (@date, @time, @title, @tag)";

            ExecuteNonQuery(insertQuery, GetEventParameters(footballEvent));
        }

        public void UpdateEvent(Event footballEvent)
        {
            const string updateQuery =
                "UPDATE Events SET Time = @time, Title = @title, Tag = @tag " +
                "WHERE Id = @id";

            var parameters = GetEventParameters(footballEvent);
            parameters.Add("@id", footballEvent.Id);

            ExecuteNonQuery(updateQuery, parameters);
        }

        public void DeleteEvent(Event footballEvent)
        {
            const string deleteQuery = "DELETE FROM Events WHERE Id = @id";
            var parameters = new Dictionary<string, object>
            {
                { "@id", footballEvent.Id }
            };

            ExecuteNonQuery(deleteQuery, parameters);
        }

        /// <summary>
        /// Smaže více událostí v jedné transakci.
        /// </summary>
        public void DeleteEvents(IEnumerable<Event> events)
        {
            var eventList = events.ToList();
            if (eventList.Count == 0) return;

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        
                        var idList = new List<string>();
                        var parameters = new Dictionary<string, object>();

                        for (int i = 0; i < eventList.Count; i++)
                        {
                            string paramName = $"@id{i}";
                            idList.Add(paramName);
                            parameters.Add(paramName, eventList[i].Id);
                        }

                        string sql = $"DELETE FROM Events WHERE Id IN ({string.Join(",", idList)})";

                        using (var cmd = new SQLiteCommand(sql, connection, transaction))
                        {
                            foreach (var param in parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public List<Event> GetAllEvents()
        {
            const string selectQuery = "SELECT * FROM Events";
            return ExecuteQuery(selectQuery);
        }
        private List<Event> ExecuteQuery(string sql, Dictionary<string, object> parameters = null)
        {
            var events = new List<Event>();

            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    AddParameters(cmd, parameters);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            events.Add(MapReaderToEvent(reader));
                        }
                    }
                }
            }

            return events;
        }

        private void ExecuteNonQuery(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SQLiteCommand(sql, connection))
                {
                    AddParameters(cmd, parameters);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Event MapReaderToEvent(SQLiteDataReader reader)
        {
            return new Event
            {
                Id = Convert.ToInt32(reader["Id"]),
                Date = DateTime.Parse(reader["Date"].ToString()),
                Time = TimeSpan.Parse(reader["Time"].ToString()),
                Title = reader["Title"].ToString(),
                Tag = reader["Tag"].ToString()
            };
        }

        private Dictionary<string, object> GetEventParameters(Event footballEvent)
        {
            return new Dictionary<string, object>
            {
                { "@date", footballEvent.Date.ToShortDateString() },
                { "@time", footballEvent.Time.ToString() },
                { "@title", footballEvent.Title },
                { "@tag", footballEvent.Tag }
            };
        }

        private void AddParameters(SQLiteCommand cmd, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);
            }
        }
    }
}