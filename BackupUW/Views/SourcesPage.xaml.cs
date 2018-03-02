using Backup.Logic;
using BackupUW.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
        //IEnumerable<int> Series(int k = 0, int n = 1, int c = 1)
        //{
        //    while (true)
        //    {
        //        yield return k;
        //        k = (c * k) + n;
        //    }
        //}

        public SourcesPage()
        {
            this.InitializeComponent();

            //var rnames = Series().Take(25);
            //this.MyListOfNames = new List<ThingWithName> (rnames.Select(n => new ThingWithName(n.ToString())));
        }

        //public List<ThingWithName> MyListOfNames { get; set; } 

        //public DataModel Model { get; set; }
        public SourcesDataSource SourceCollection { get { return SourcesDataSource.Instance; } }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SourcesPage.OnNavigatedTo");
            base.OnNavigatedTo(e);

            //this.DataContext = this; // CHECK NEEDED?
        }

        private async void CheckSourcePermission()
        {
            StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            Debug.WriteLine(appFolder.Path);

            var failedAccess = new List<string>();
            foreach (var source in SourceCollection)
            {
                try
                {
                    IStorageItem folder = await StorageFolder.GetFolderFromPathAsync(source.Directory);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{ex.Message} {source.Directory}");
                    failedAccess.Add(source.Directory);
                }
            }

            if (failedAccess.Count > 0)
            {
                var dlg = new MessageDialog("No Access Error");
                dlg.MessageText = string.Join("\n", failedAccess.ToArray());
                dlg.IsSecondaryButtonEnabled = false;
                var result = await dlg.ShowAsync();
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e) => this.Frame.Navigate(
                typeof(EditSourcePage),
                null,
                new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if(this.SourcesListView.SelectedItem != null)
                this.Frame.Navigate(
                      typeof(EditSourcePage),
                      this.SourcesListView.SelectedItem,
                      new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new MessageDialog("Confirm Remove");
                var result = await dlg.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Debug.WriteLine("Confirmed");
                    SourceCollection.Remove((Source)SourcesListView.SelectedItem);
                    SourceCollection.SaveItems();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }           
        }

        private void SourcesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourcesListView.SelectedItem != null)
            {
                EditSource.IsEnabled = true;
                RemoveSource.IsEnabled = true;
            }
            else
            {
                EditSource.IsEnabled = false;
                RemoveSource.IsEnabled = false;
            }
        }

        private void CheckAccess_Click(object sender, RoutedEventArgs e) => CheckSourcePermission();
    }
}
