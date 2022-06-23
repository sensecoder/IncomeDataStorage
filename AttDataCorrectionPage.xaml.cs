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
using System.Windows.Navigation;

namespace IncomeDataStorage
{
    public partial class AttDataCorrectionPage : PhoneApplicationPage
    {
        private AttDataCorrection adkorr;
        
        public AttDataCorrectionPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("ID"))
            {
                int id = int.Parse(NavigationContext.QueryString["ID"]);
                adkorr = new AttDataCorrection(id);
            }
            else
            {
                adkorr = new AttDataCorrection();
            }
            LayoutRoot.DataContext = adkorr;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (adkorr.SaveData()) App.ViewModel.LoadData();
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void btn_DeleteRecord_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var diagRes = MessageBox.Show("Вы действительно хотите удалить запись?", "...ээх, Руслан...", MessageBoxButton.OKCancel);
            if (diagRes == MessageBoxResult.OK)
            {
                adkorr.DeleteRecord();
                App.ViewModel.LoadData();
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
                return;
        }
    }
}