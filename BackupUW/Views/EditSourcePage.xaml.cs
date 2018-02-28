﻿using System;
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
    public sealed partial class EditSourcePage : Page
    {
        public EditSourcePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("EditSourcePage.OnNavigatedTo");

            if (e.Parameter == null)
            {
                // Adding new item

            }

            if (e.Parameter is string)
            {
                Debug.WriteLine("Clicked on {0}", e.Parameter);
            }

            base.OnNavigatedTo(e);
        }

        private async void PickDirectory_ClickAsync(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add(".docx");
            //folderPicker.FileTypeFilter.Add(".xlsx");
            //folderPicker.FileTypeFilter.Add(".pptx");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder 
                // (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                Directory.Text = folder.Path;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void ConfirmDelete_Click(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("ConfirmDelete_Click");
        //    ConfirmDeletePopup.IsOpen = false;
        //}

        //private void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("Cancel_Click");
        //    ConfirmDeletePopup.IsOpen = false;
        //}
        
        //private void DisplayConfirmation_Click(object sender, RoutedEventArgs e)
        //{
        //    Debug.WriteLine("DisplayConfirmation_Click");
        //    ConfirmDeletePopup.IsOpen = true;
        //}

        //StorageFolder picturesFolder = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);

        //IReadOnlyList<StorageFile> fileList = await picturesFolder.GetFilesAsync();
        //IReadOnlyList<StorageFolder> folderList = await picturesFolder.GetFoldersAsync();



    }
}
