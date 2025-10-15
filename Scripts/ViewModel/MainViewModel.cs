using Calendar.Commands;
using Calendar.Scripts.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;

namespace Calendar.Scripts
{
    /// <summary>
    /// Hlavní ViewModel aplikace kalendáře, který spravuje data, stav a logiku uživatelského rozhraní.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // --- FIELDS ---

        private bool _notificationsEnabled = true;
        private readonly IEventRepository _repository;
        private readonly EventNotificationService _eventNotifier;
        private readonly ThemeService _themeService = new ThemeService();
        private DateTime _selectedDate = DateTime.Today;
        private string _selectedTagFilter = "All";
        private string _selectedTheme = "All";
        private DayViewModel _selectedDay;
        private DispatcherTimer _notificationTimer;
        private string _randomQuote;
        private readonly Random _rnd = new Random();

        /// <summary>
        /// Událost vyvolaná při změně vlastnosti pro aktualizaci UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Kolekce dní zobrazených v aktuálním měsíci.
        /// </summary>
        public ObservableCollection<DayViewModel> Days { get; private set; }

        /// <summary>
        /// Seznam filtrů podle štítků (tagů) událostí.
        /// </summary>
        public List<string> TagFilters { get; } = new List<string> { "All", "Match", "Training", "Gym", "Tactics", "Other" };

        /// <summary>
        /// Dostupné témata pro změnu vzhledu aplikace.
        /// </summary>
        public ObservableCollection<string> AvailableThemes { get; } = new ObservableCollection<string>
        {
            "Barca", "Real", "City", "Liverpool"
        };

        /// <summary>
        /// Repozitář pro získávání a správu událostí.
        /// </summary>
        public IEventRepository Repository => _repository;

        // --- COMMANDY ---

        public ICommand ChangeSelectedDateCommand { get; private set; }
        public ICommand AddEventCommand { get; private set; }
        public ICommand PreviousMonthCommand { get; private set; }
        public ICommand NextMonthCommand { get; private set; }
        public ICommand ModifyEventCommand { get; private set; }
        public ICommand DeleteEventCommand { get; private set; }
        public ICommand DeleteDayEventsCommand { get; private set; }
        public ICommand RefreshQuoteCommand { get; private set; }
        public ICommand ExportEventsCommand { get; private set; }

        // --- VLASTNOSTI ---

        /// <summary>
        /// Aktuálně vybrané téma (vzhled) aplikace.
        /// Při změně se téma aplikuje a uloží do nastavení.
        /// </summary>
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (_selectedTheme != value)
                {
                    _selectedTheme = value;
                    OnPropertyChanged();
                    _themeService.ApplyTheme(value);
                    Properties.Settings.Default.SelectedTeamTheme = value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// Zapnutí/vypnutí notifikací o nadcházejících událostech.
        /// </summary>
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                {
                    Properties.Settings.Default.NotificationsEnabled = value;
                    Properties.Settings.Default.Save();

                    if (value)
                    {
                        _eventNotifier.ResetNotifiedEvents();
                        _eventNotifier.CheckUpcomingEvents();
                    }
                }
            }
        }

        /// <summary>
        /// Vybrané datum v kalendáři.
        /// Při změně se generují dny měsíce.
        /// </summary>
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                    GenerateMonthDays();
                }
            }
        }

        /// <summary>
        /// Aktuálně vybraný filtr podle tagu události.
        /// Při změně se aktualizují zobrazené události.
        /// </summary>
        public string SelectedTagFilter
        {
            get => _selectedTagFilter;
            set
            {
                if (_selectedTagFilter != value)
                {
                    _selectedTagFilter = value;
                    OnPropertyChanged();
                    GenerateMonthDays();
                }
            }
        }

        /// <summary>
        /// Vybraný den (denní ViewModel).
        /// </summary>
        public DayViewModel SelectedDay
        {
            get => _selectedDay;
            set => SetProperty(ref _selectedDay, value);
        }

        /// <summary>
        /// Náhodný citát z fotbalové tematiky.
        /// </summary>
        public string RandomQuote
        {
            get => _randomQuote;
            set
            {
                _randomQuote = value;
                OnPropertyChanged();
            }
        }

        // --- KONSTRUKTOR ---

        /// <summary>
        /// Inicializuje hlavní ViewModel, načte nastavení, vytvoří příkazy, vygeneruje dny a spustí notifikace.
        /// </summary>
        public MainViewModel()
        {
            _repository = EventRepository.Instance;
            _eventNotifier = new EventNotificationService(_repository);
            Days = new ObservableCollection<DayViewModel>();

            InitializeCommands();
            LoadSettings();
            GenerateMonthDays();
            InitializeNotificationSystem();
            SelectRandomQuote();
        }

        // --- PRIVÁTNÍ METODY ---

        /// <summary>
        /// Inicializuje všechny ICommand příkazy používané v UI.
        /// </summary>
        private void InitializeCommands()
        {
            AddEventCommand = new AddEventCommand(this);
            ChangeSelectedDateCommand = new ChangeSelectedDateCommand(this);
            PreviousMonthCommand = new PreviousMonthCommand(this);
            NextMonthCommand = new NextMonthCommand(this);
            ModifyEventCommand = new ModifyEventCommand(this);
            DeleteEventCommand = new DeleteEventCommand(this);
            DeleteDayEventsCommand = new DeleteDayEventsCommand(this);
            RefreshQuoteCommand = new RefreshQuoteCommand(this);
            ExportEventsCommand = new ExportEventsCommand(this);
        }

        /// <summary>
        /// Nastaví a spustí systém notifikací s timerem kontrolujícím události každou minutu.
        /// </summary>
        private void InitializeNotificationSystem()
        {
            _eventNotifier.OnNotification += NotificationManager.ShowNotification;
            _notificationTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _notificationTimer.Tick += OnNotificationTimerTick;
            _notificationTimer.Start();
            _eventNotifier.CheckUpcomingEvents();
        }

        /// <summary>
        /// Událost volaná každou minutu, která kontroluje, zda se blíží nějaká událost, pokud jsou notifikace zapnuté.
        /// </summary>
        private void OnNotificationTimerTick(object sender, EventArgs e)
        {
            if (NotificationsEnabled)
            {
                _eventNotifier.CheckUpcomingEvents();
            }
        }

        /// <summary>
        /// Načte nastavení aplikace (téma a zapnutí notifikací).
        /// </summary>
        private void LoadSettings()
        {
            SelectedTheme = Properties.Settings.Default.SelectedTeamTheme ?? "Barca";
            NotificationsEnabled = Properties.Settings.Default.NotificationsEnabled;
            _themeService.ApplyTheme(SelectedTheme);
        }

        /// <summary>
        /// Vygeneruje seznam dní aktuálního měsíce a načte k nim odpovídající události s ohledem na vybraný filtr tagů.
        /// </summary>
        public void GenerateMonthDays()
        {
            Days.Clear();
            var firstDayOfMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(SelectedDate.Year, SelectedDate.Month, day);
                var dayViewModel = new DayViewModel(date) { IsCurrentMonth = true };

                PopulateDayEvents(dayViewModel);
                Days.Add(dayViewModel);
            }

            UpdateSelectedDay();
            UpdateDaySelection();
        }

        /// <summary>
        /// Naplní daný denní ViewModel událostmi z repozitáře podle filtru tagů.
        /// </summary>
        /// <param name="dayViewModel">Den, do kterého se přidávají události</param>
        private void PopulateDayEvents(DayViewModel dayViewModel)
        {
            var dayEvents = _repository.GetEvents(dayViewModel.Date);

            if (!string.Equals(_selectedTagFilter, "All", StringComparison.OrdinalIgnoreCase))
            {
                dayEvents = dayEvents
                    .Where(e => string.Equals(e.Tag, _selectedTagFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            dayViewModel.Events.Clear();
            foreach (var evt in dayEvents)
            {
                dayViewModel.Events.Add(evt);
            }
        }

        /// <summary>
        /// Aktualizuje vybraný den na základě aktuálního vybraného data.
        /// </summary>
        private void UpdateSelectedDay()
        {
            SelectedDay = Days.FirstOrDefault(d => d.Date == SelectedDate);
        }

        /// <summary>
        /// Označí v kolekci dní vybraný den.
        /// </summary>
        private void UpdateDaySelection()
        {
            foreach (var day in Days)
            {
                day.IsSelected = day == SelectedDay;
            }
        }

        /// <summary>
        /// Vybere náhodný citát z předdefinovaného seznamu.
        /// </summary>
        public void SelectRandomQuote()
        {
            var quotes = new List<string>
            {
"Fotbal je umění pohybu bez míče. - Karel Poborský",
"Každý gól začíná myšlenkou, ne náhodou. - Jan Nezmar",
"Nejlepší hráč je ten, kdo dělá spoluhráče lepšími. - Pavel Hapal",
"Bez týmové práce nemá hvězda šanci zářit. - Michal Bílek",
"Brankář je poslední obránce a první útočník. - Petr Čech",
"Vítězství chutná nejlépe po prohře. - Tomáš Sivok",
"Zápas trvá 90 minut, ale vzpomínky celý život. - Karel Jarolím",
"Každá přihrávka je šance na změnu hry. - Jaroslav Šilhavý",
"Nauč se prohrávat, abys mohl skutečně vyhrát. - David Lafata",
"Rychlost bez rozumu je jen chaos. - Vladimír Šmicer",
"A great pass speaks louder than a thousand dribbles. - Steven Gerrard",
"Champions train when no one is watching. - Cristiano Ronaldo",
"Defense is not about strength, but anticipation. - Paolo Maldini",
"The goal is not always to score, but to create magic. - Ronaldinho",
"Every touch on the ball is a decision. - Xavi Hernández",
"Pressure makes diamonds—or mistakes. - Pep Guardiola",
"Scoring is glory, but defending is pride. - Sergio Ramos",
"Your first touch tells your story. - Andrés Iniesta",
"You don't play football with your feet—you play with your brain. - Arsène Wenger",
"Let me talk, let me fucking talk. - Kevin De Bruyne",
"A good coach sees what others ignore. - Jürgen Klopp",
"Talent vyhrává zápasy, ale týmová práce a inteligence vyhrávají šampionáty. - Lionel Messi",
"Some people think football is a matter of life and death. I assure you, it's much more serious than that. - Bill Shankly",
"Believe in yourself. If you don't, why should anyone else? - Zlatan Ibrahimović",
"Success is no accident. It is hard work, perseverance, learning, sacrifice and most of all, love of what you are doing. - Pelé",
"Fotbal je jednoduchá hra. Dvaadvacet mužů běhá za míčem a nakonec vyhrají Němci. - Gary Lineker",
"In football, you’re never as good as they say you are, nor as bad as they say. - Sir Alex Ferguson",
"If you don’t believe you can win, you have no place on the pitch. - Patrick Vieira",
"My heroes? My teammates who push me every day. - Luka Modrić",
"It’s not just about scoring goals. It’s about the moment that changes the game. - Eden Hazard",
"Sometimes you need to fall to know how much you want to rise. - Frank Lampard",
"It’s not the strongest team that wins, but the one that wants it more. - Diego Simeone",
"Every match is a new story. - Andrés Guardado",
"Training makes you a player, but character makes you a legend. - Thierry Henry",
"The most important thing in football is the mind. - Roberto Mancini",
"On the pitch, there are no excuses. Only results. - Didier Drogba",
"Fotbal je vášeň, která nikdy neuhasne. - Francesco Totti",
"The greatest victory is overcoming yourself. - Clarence Seedorf",
"A goal is just the cherry. What matters is the cake. - Marco Reus",
"The greatest strength is not in the individual, but in the team. - Antonio Conte",
"A great player shines. A great captain leads. - Carles Puyol",
"Never underestimate the quiet player. - N’Golo Kanté",
"Fotbal je nejkrásnější, když je hraný srdcem. - Pavel Nedvěd",
"Injuries don’t stop you. They just show how badly you want it. - Marco van Basten",
"When you fight until the final whistle, you never lose. - Ole Gunnar Solskjær",
"Play like your idol is watching. - João Félix",
"You don’t need to be the fastest, just the smartest. - Andrea Pirlo",
"If you run without purpose, you’ll never get there. - Clarence Acuña",
"First touch matters more than the final step. - Juan Mata",
"Play simple, but think like a master. - Johan Cruyff",
"It doesn’t matter what they say. It matters what you show. - Neymar Jr.",
"Nie som natoľko kvalitatívne dobrý, aby som hral futbal za slovenskú reprezentáciu, preto som minister vnútra. - Matúš Šutaj Eštok"
            };

            RandomQuote = quotes[_rnd.Next(quotes.Count)];
        }
        public void CleanUp() => _notificationTimer.Stop();
        /// <summary>
        /// Pomocná metoda pro volání PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Název vlastnosti, která se změnila</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Pomocná metoda pro nastavení hodnoty vlastnosti a vyvolání PropertyChanged, pokud došlo ke změně.
        /// </summary>
        /// <typeparam name="T">Typ vlastnosti</typeparam>
        /// <param name="field">Referenční pole, kde je hodnota uložena</param>
        /// <param name="value">Nová hodnota</param>
        /// <param name="propertyName">Název vlastnosti (volitelné)</param>
        /// <returns>Vrací true, pokud hodnota byla změněna</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
