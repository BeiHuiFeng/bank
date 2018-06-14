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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace newBank
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            if(args.IsSettingsSelected) {
                // do nothing
            }
            else {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                switch(item.Tag.ToString()) {
                    case "Branch":
                        ContentFrame.Navigate(typeof(branch));
                        break;
                    case "Staff":
                        ContentFrame.Navigate(typeof(Staff));
                        break;
                    case "Client":
                        ContentFrame.Navigate(typeof(Client));
                        break;
                    case "SAccount":
                        ContentFrame.Navigate(typeof(SAccount));
                        break;
                    case "CAccount":
                        ContentFrame.Navigate(typeof(CAccount));
                        break;
                    case "Loan":
                        ContentFrame.Navigate(typeof(Loan));
                        break;
                    case "Business":
                        ContentFrame.Navigate(typeof(Staff));
                        break;

                }
            }
        }
        

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {

        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {

        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {

        }


        private void More_Click(object sender, RoutedEventArgs e) {

        }

    }
}
