using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace BackupUW.Views
{
    public sealed partial class SourcesPage : Page
    {
        IEnumerable<int> Series(int k = 0, int n = 1, int c = 1)
        {
            while (true)
            {
                yield return k;
                k = (c * k) + n;
            }
        }

        public SourcesPage()
        {
            this.Model = new DataModel();
            this.InitializeComponent();
            this.SourceCollection = new Model.SourcesViewModel();

            //var rnames = Series().Take(25);
            //this.MyListOfNames = new List<ThingWithName> (rnames.Select(n => new ThingWithName(n.ToString())));
        }

        public List<ThingWithName> MyListOfNames { get; set; } 

        public DataModel Model { get; set; }
        public Model.SourcesViewModel SourceCollection { get; private set; }

        //public List<IEmployee> Employees = new List<IEmployee>();
        //void InitializeValues()
        //{
        //    this.Model.InitializeValues();
        //    foreach (IEmployee e in Model.ManagerProp.ReportsList)
        //    {
        //        if (this.Employees != null)
        //        {
        //            this.Employees.Add(e);
        //        }
        //    }
        //}
        //private void UpdateValuesClick(object sender, RoutedEventArgs e)
        //{
        //    this.Model.UpdateValues();
        //    if (this.Employees != null)
        //    {
        //        IEmployee temp = Employees[0];
        //        Employees.RemoveAt(0);
        //        Employees.Add(temp);
        //    }
        //}
        //private void ResetValuesClick(object sender, RoutedEventArgs e)
        //{
        //    this.InitializeValues();
        //}
        //public SourcesViewModel SourceCollection { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SourcesPage.OnNavigatedTo");
            base.OnNavigatedTo(e);

            this.DataContext = this;
            //this.SourcesListView.ItemsSource = SourcesCollection;
        }

        private void Add_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(
                typeof(EditSourcePage),
                null,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(
            //      typeof(EditSourcePage),
            //      this.SourcesListView.SelectedItem,
            //      new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new MessageDialog("Confirm Remove");
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Debug.WriteLine("Confirmed");
            }
            else
            {
                Debug.WriteLine("Canceled");
            }
        }
    }
}
