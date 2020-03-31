// **************************************************************************
//Start Finance - An to manage your personal finances.

//Start Finance is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//Start Finance is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with Start Finance.If not, see<http://www.gnu.org/licenses/>.
// ***************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInformationPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        PersonalInformation selectedContact = null;

        public PersonalInformationPage()
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
            conn.CreateTable<PersonalInformation>();
            var query = conn.Table<PersonalInformation>();
            TransactionList.ItemsSource = query.ToList();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if account name is null
                if (PIName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (PIName.Text.ToString() == "AccountName" || PIName.Text.ToString() == "InitialAmount")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInformation()
                    {
                        Name = PIName.Text,
                        Age = Convert.ToDouble(PIAge.Text),
                        Gender = PIGender.Text
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Account Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }
        // update data 
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if selectedContact is null                
                if (selectedContact == null)
                {
                    MessageDialog dialog = new MessageDialog("Contact is not selected to update", "Oops..!");
                    await dialog.ShowAsync();
                }
                string selectedName = selectedContact.Name;
                if ((PIName.Text.ToString() == "") || (PIAge.Text.ToString() == "") || (PIGender.Text.ToString() == ""))
                {
                    MessageDialog dialog = new MessageDialog("Contact Name, Age or Gender", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (PIName.Text.ToString() == "Name" || PIName.Text.ToString() == "Age" || PIName.Text.ToString() == "Gender")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                    await variableerror.ShowAsync();
                }
                else
                {   // Update the data
                    string sql = "UPDATE PersonalInformation SET Name = ?, Age = ?, Gender = ? Where Name = ?";
                    conn.Execute(sql, PIName.Text, PIAge.Text, PIGender.Text, selectedName);
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Format Exception", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        // Displays the data when navigation between pages
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure you want to delete your personal information?", "Important");
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
                    string PersonalInformationLabel = ((PersonalInformation)TransactionList.SelectedItem).Name;
                    var querydel = conn.Query<PersonalInformation>("DELETE FROM PersonalInformation WHERE Name='" + PersonalInformationLabel + "'");
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

        private void TransactionList_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            selectedContact = (PersonalInformation)TransactionList.SelectedItem;
            if (selectedContact != null)
            {
                PIName.Text = selectedContact.Name;
                PIAge.Text = selectedContact.Age.ToString();
                PIGender.Text = selectedContact.Gender;
            }
        }
    }
}
