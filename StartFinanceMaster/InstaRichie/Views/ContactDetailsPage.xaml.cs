using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public ContactDetailsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }
        public void Results()
        {
            // Creating table
            conn.CreateTable<ContactDetails>();
            var query = conn.Table<ContactDetails>();
            ContactDetailsList.ItemsSource = query.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if account name is null
                if (ContactName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Contact Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ContactName.Text.ToString() == "ContactName" || ContactName.Text.ToString() == "TelNum")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                }
                else
                {   // Inserts the data
                    conn.Insert(new ContactDetails()
                    {
                        Name = ContactName.Text,
                        TelNum = TelNum.Text,
                        Address = Address.Text
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Contact or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Contact Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("This account will be permanently deleted.", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    string ContactNameLabel = ((ContactDetails)ContactDetailsList.SelectedItem).Name;
                    var querydel = conn.Query<Accounts>("DELETE FROM ContactDetails WHERE Name='" + ContactNameLabel + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
