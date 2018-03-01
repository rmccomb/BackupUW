using BackupUW.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BackupUW
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            try
            {
                this.InitializeComponent();
                this.Suspending += OnSuspending;
                PopulateJumpListAsync();
                Model.SourcesDataSource.Instance.GetItems();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            EnsureWindowAsync(e);

            Debug.WriteLine(e.Arguments);
            // Handle JumpList
            if (e.Kind == ActivationKind.Launch && e.Arguments == "/jumplist:discover")
            {
                // TODO Call Discover Files
            }
            if (e.Kind == ActivationKind.Launch && e.Arguments == "/jumplist:backup")
            {
                // TODO Invoke Backup
            }
        }

        private async void EnsureWindowAsync(IActivatedEventArgs args)
        {
            await NavigationInfoDataSource.Instance.GetItemsAsync();

            Frame rootFrame = GetRootFrame();

            Type targetPageType = typeof(MainPage);
            string targetPageArguments = string.Empty;

            rootFrame.Navigate(targetPageType, targetPageArguments);

            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = false;

            // Darken the window title bar using a color value to match app theme
            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            if (titleBar != null)
            {
                Color titleBarColor = (Color)App.Current.Resources["SystemChromeMediumColor"];
                //var titleBarColor = Colors.Bisque;
                titleBar.BackgroundColor = titleBarColor;
                titleBar.ButtonBackgroundColor = titleBarColor;
            }

            //// Ensure the current window is active
            Window.Current.Activate();
        }

        private Frame GetRootFrame()
        {
            Frame rootFrame;
            NavigationRootPage rootPage = Window.Current.Content as NavigationRootPage;
            if (rootPage == null)
            {
                rootPage = new NavigationRootPage();
                rootFrame = (Frame)rootPage.FindName("rootFrame");
                if (rootFrame == null)
                {
                    throw new Exception("Root frame not found");
                }
                //SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];
                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootPage;
            }
            else
            {
                rootFrame = (Frame)rootPage.FindName("rootFrame");
            }

            return rootFrame;
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        async void PopulateJumpListAsync()
        {
            var jumpList = await JumpList.LoadCurrentAsync();
            jumpList.Items.Clear();

            {
                var discover = JumpListItem.CreateWithArguments("/jumplist:discover", "Discover Files");
                discover.Description = "Find changed files to backup";
                //item.GroupName = GroupName.Text;
                discover.Logo = new Uri("ms-appx:///Resources/Collection_16x.png");
                jumpList.Items.Add(discover);
            }
            {
                var backup = JumpListItem.CreateWithArguments("/jumplist:backup", "Backup");
                backup.Description = "Invoke the backup process";
                backup.Logo = new Uri("ms-appx:///Resources/Cloud_48x.png");
                jumpList.Items.Add(backup);
            }

            await jumpList.SaveAsync();
        }
    }
}
