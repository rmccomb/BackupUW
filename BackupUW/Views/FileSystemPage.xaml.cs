using Autofac;
using Backup.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BackupUW.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FileSystemPage : Page
    {
        public FileSystemPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PopulateControls();

            base.OnNavigatedTo(e);
        }

        private void PopulateControls()
        {
            try
            {
                var sm = App.DIScope.Resolve<ISettingsManager>();
                var settings = sm.ReadSettings2();
                if (!String.IsNullOrEmpty(settings.FileSystemDirectory))
                    Directory.Text = settings.FileSystemDirectory;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void PickDirectory_ClickAsync(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add(".docx");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder 
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                Directory.Text = folder.Path;
                Message.Text = "";
            }
        }


        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Accept the edit and navigate back 
                #region Validation
                IStorageItem folder = await StorageFolder.GetFolderFromPathAsync(Directory.Text);
                if (String.IsNullOrWhiteSpace(Directory.Text))
                {
                    Message.Text = "Select a valid directory";
                    return;
                }
                #endregion

                // Save the settings
                var sm = App.DIScope.Resolve<ISettingsManager>();
                var settings = sm.ReadSettings2();
                settings.FileSystemDirectory = Directory.Text;
                sm.WriteSettings2(settings);
                Message.Text = "Settings saved";

                //StorageApplicationPermissions.FutureAccessList.Entries

                //this.Frame.Navigate(
                //    typeof(SourcesPage),
                //    null,
                //    new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            }
            catch (UnauthorizedAccessException access)
            {
                Debug.WriteLine(access.Message);
                Message.Text = "Access has not been granted to the directory";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sources = SourcesManager.GetSources();
                var archiveDir = Directory.Text;

                //var builder = new ContainerBuilder();
                //builder.RegisterType<FileManager>().As<IFileManager>();
                //builder.RegisterType<SettingsManager>().As<ISettingsManager>();
                //var container = builder.Build();
                //using (var scope = container.BeginLifetimeScope())
                //{
                var fm = App.DIScope.Resolve<IFileManager>();
                fm.InvokeBackup();
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Message.Text = ex.Message;
            }
        }
    }
}
