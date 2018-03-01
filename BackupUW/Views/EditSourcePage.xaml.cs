using Backup.Logic;
using BackupUW.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BackupUW.Views
{
    public sealed partial class EditSourcePage : Page
    {
        public bool IsNewItem { get; set; }
        private Source EditItem { get; set; }

        public EditSourcePage()
        {
            this.InitializeComponent();
            Message.Text = "";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("EditSourcePage.OnNavigatedTo");

            if (e.Parameter == null)
            {
                // Adding new item
                IsNewItem = true;
                EditItem = null;
            }

            if (e.Parameter is Source source)
            {
                Debug.WriteLine("Clicked on {0}", ((Source)e.Parameter).Directory);
                IsNewItem = false;
                EditItem = source;
                Directory.Text = source.Directory;
                Pattern.Text = source.Pattern;
                ModifiedOnly.IsChecked = source.ModifiedOnly == "Yes";
            }

            base.OnNavigatedTo(e);
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
                // Accept the edit and navigate back to Sources
                #region Validation
                IStorageItem folder = await StorageFolder.GetFolderFromPathAsync(Directory.Text);
                if (String.IsNullOrWhiteSpace(Directory.Text))
                {
                    Message.Text = "Select a valid directory";
                    return;
                }
                //if (!StorageApplicationPermissions.FutureAccessList.CheckAccess(folder))
                //{
                //    return;
                //}
                if (String.IsNullOrWhiteSpace(Pattern.Text)
                    || !Pattern.Text.StartsWith("*."))
                {
                    Message.Text = "Enter a file extension, e.g. *.*";
                    return;
                }
                #endregion

                bool modifiedOnly = ModifiedOnly.IsChecked ?? false;

                if (IsNewItem)
                {
                    EditItem = new Source(
                        Directory.Text,
                        Pattern.Text,
                        modifiedOnly ? "Yes" : "No",
                        Source.NeverText);

                    SourcesDataSource.Instance.Add(EditItem);
                }
                else
                {
                    EditItem.Directory = Directory.Text;
                    EditItem.Pattern = Pattern.Text;
                    EditItem.ModifiedOnly = modifiedOnly ? "Yes" : "No";
                }

                SourcesDataSource.Instance.SaveItems();

                //StorageApplicationPermissions.FutureAccessList.Entries

                this.Frame.Navigate(
                    typeof(SourcesPage),
                    null,
                    new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
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

        //StorageFolder picturesFolder = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);

        //IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();
        //IReadOnlyList<StorageFolder> folderList = await picturesFolder.GetFoldersAsync();



    }
}
