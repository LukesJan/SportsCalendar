using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Calendar.Scripts
{    /// <summary>
     /// Služba pro aplikování motivů (téma) do WPF aplikace.
     /// </summary>
    public class ThemeService
    {
     /// <summary>
     /// Aplikuje motiv podle názvu.
     /// </summary>
     /// <param name="themeName">Název motivu (odpovídá názvu XAML souboru v Team/)</param>
        public void ApplyTheme(string themeName)
        {
            try
            {
                var app = Application.Current;
                var dictionariesToRemove = app.Resources.MergedDictionaries
                    .Where(dict => dict.Source?.OriginalString.Contains("Team/") == true)
                    .ToList();

                foreach (var dict in dictionariesToRemove)
                {
                    app.Resources.MergedDictionaries.Remove(dict);
                }

                
                
                    var newDict = new ResourceDictionary
                    {
                        Source = new Uri($"pack://application:,,,/Calendar;component/Team/{themeName}.xaml")
                    };
                    app.Resources.MergedDictionaries.Add(newDict);
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load team theme: {ex.Message}");
            }
        }
    }
}