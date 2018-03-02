using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BackupUW.Views
{
    public sealed partial class MessageDialog : ContentDialog
    {
        public MessageDialog(string title)
        {
            this.InitializeComponent();
            this.Title = title;
            this.DefaultButton = ContentDialogButton.Primary;
        }

        public string MessageText
        {
            get { return this.Message.Text; }
            set { this.Message.Text = value; }
        }
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Debug.WriteLine("Primary");
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //Debug.WriteLine("Secondary");
        }
    }
}
