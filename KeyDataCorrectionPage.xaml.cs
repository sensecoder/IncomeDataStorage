using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using IncomeDataStorage.Presentation;

namespace IncomeDataStorage
{
    public partial class KeyDataCorrectionPage : PhoneApplicationPage
    {
        private KeyDataCorrection kdcorr = new KeyDataCorrection(); // класс слоя представления.
        
        public KeyDataCorrectionPage()
        {
            InitializeComponent();
            LayoutRoot.DataContext = kdcorr;
        }

        private void tBx_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            kdcorr.Name.Text = tb.Text;
            kdcorr.CheckState();
        }

        private void tBx_FloorNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            kdcorr.FloorNo.Text = tb.Text;
            kdcorr.CheckState();
        }

        private void GoBack()
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            kdcorr.SaveData();
            GoBack();
        }

        private void tBx_Name_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
            }
        }
    }
}