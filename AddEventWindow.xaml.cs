using Calendar.Scripts;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Calendar
{
    /// <summary>
    /// Okno pro přidání nebo úpravu události v kalendáři.
    /// </summary>
    public partial class AddEventWindow : Window
    {
      

        /// <summary>
        /// Reference na událost, která se případně upravuje (edituje).
        /// Pokud je null, znamená to přidání nové události.
        /// </summary>
        private readonly Event _editingEvent;

       

        /// <summary>
        /// Text z TextBoxu pro název události.
        /// </summary>
        public string TitleText => TitleBox.Text;

        /// <summary>
        /// Text z TextBoxu pro čas události.
        /// </summary>
        public string TimeText => TimeBox.Text;

        /// <summary>
        /// Text z ComboBoxu pro výběr tagu události.
        /// </summary>
        public string TagText
        {
            get
            {
                var selectedItem = TagComboBox.SelectedItem as ComboBoxItem;
                return selectedItem?.Content.ToString();
            }
        }

        /// <summary>
        /// Výsledná událost po potvrzení okna (přidání / editace).
        /// </summary>
        public Event ResultEvent { get; private set; }

        

        /// <summary>
        /// Konstruktor okna. Pokud je předána existující událost, naplní pole pro editaci.
        /// </summary>
        /// <param name="eventToModify">Událost k úpravě (volitelné)</param>
        public AddEventWindow(Event eventToModify = null)
        {
            InitializeComponent();

            if (eventToModify != null)
            {
                _editingEvent = eventToModify;

                TitleBox.Text = _editingEvent.Title;
                TimeBox.Text = _editingEvent.Time.ToString(@"hh\:mm");

               
                foreach (ComboBoxItem item in TagComboBox.Items)
                {
                    if ((string)item.Content == _editingEvent.Tag)
                    {
                        TagComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

      

        /// <summary>
        /// Handler kliknutí na tlačítko Přidat / Uložit.
        /// Validuje vstupy a vytváří nový nebo upravený objekt Event.
        /// </summary>
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleText))
            {
                MessageBox.Show("Please enter a title for the event.");
                return;
            }

            if (TagComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a tag.");
                return;
            }

            if (!IsValidTime(TimeBox.Text))
            {
                MessageBox.Show("Please enter a valid time in HH:mm format.");
                return;
            }

            var parsedTime = TimeSpan.Parse(TimeBox.Text);

            ResultEvent = new Event
            {
                Id = _editingEvent?.Id ?? 0,  
                Title = TitleText,
                Time = parsedTime,
                Tag = TagText,
                Date = _editingEvent?.Date ?? DateTime.Today
            };

            DialogResult = true; 
            Close();
        }

        /// <summary>
        /// Validuje časový formát HH:mm pomocí regulárního výrazu.
        /// </summary>
        /// <param name="time">Časový řetězec</param>
        /// <returns>True, pokud je formát správný</returns>
        private bool IsValidTime(string time)
        {
            string pattern = @"^([0-1][0-9]|2[0-3]):([0-5][0-9])$";
            return Regex.IsMatch(time, pattern);
        }

        /// <summary>
        /// Handler kliknutí na tlačítko Zrušit - pouze zavře okno bez uložení.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handler pro změnu textu v TextBoxu času (momentálně prázdný).
        /// Můžeš zde přidat live validaci nebo jiné reakce na změnu.
        /// </summary>
        private void TimeBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }
    }
}
