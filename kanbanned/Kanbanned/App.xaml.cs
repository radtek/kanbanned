using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

// kad smo pravili prve prozore aplikacije, imenovali smo projekat LoginDemo i posto smo ga posle samo preimenovali
// ovde je ostao stari namespace i sada dolazi do nekih gresaka u asemblijima ako se promeni
// tako da bolje neka ostane ovako
namespace LoginDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void SelectCulture(string culture)
        {
            if (String.IsNullOrEmpty(culture))
                return;

            //Copy all MergedDictionarys into a auxiliar list.
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();

            //Search for the specified culture.     
            string requestedCulture = string.Format("Resources/Jezik.{0}.xaml", culture);
            var resourceDictionary = dictionaryList.
                FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (resourceDictionary == null)
            {
                //If not found, select our default language.             
                requestedCulture = "Resources/Jezik.xaml";
                resourceDictionary = dictionaryList.
                    FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            //If we have the requested resource, remove it from the list and place at the end.     
            //Then this language will be our string table to use.      
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }

            //Inform the threads of the new culture.     
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        // zatvori konekciju
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Kanbanned.DBConnection.Close();
        }

        // otvori konekciju
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Kanbanned.DBConnection.Open();
        }
    }
}
