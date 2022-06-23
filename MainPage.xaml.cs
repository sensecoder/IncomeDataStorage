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
using Microsoft.Phone.Shell;
using System.Windows.Navigation;
using IncomeDataStorage.Presentation;
using System.Threading;
using Useful;
using System.ComponentModel;

namespace IncomeDataStorage
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        KeyDataEnterField kdenter; // класс слоя представления.
        private bool tBl_KeyDataFocused;
        //private Thickness layoutMargin;

       /* public Thickness LayoutMargin
        {
            get
            {
                return layoutMargin;
            }
            set
            {
                layoutMargin = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("LayoutMargin"));
                }
            }
        } */
        
        // Конструктор
        public MainPage()
        {
            InitializeComponent();

            kdenter = new KeyDataEnterField();

            KeyDataEnterArea.DataContext = kdenter;
            //LayoutRoot.DataContext = this;

            //this.GotFocus +=new RoutedEventHandler(MainPage_GotFocus);
            
            // Задайте для контекста данных элемента управления listbox пример данных
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void LayoutShift(int level)
        {
            Thickness margin = new Thickness(0);
            if (level == 0) margin = new Thickness(0, -170, 0, 0);
            if (level == 1) margin = new Thickness(0, -200, 0, 0);
            if (LayoutRoot.Margin == margin)
                LayoutRoot.Margin = new Thickness(0);
            else
                LayoutRoot.Margin = margin;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
           /* if (kdenter != null)
            {
                this.State.Add("kdenter", (object)kdenter);
            } */

            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            /* if (this.State.ContainsKey("kdenter"))
             {
                 kdenter = (KeyDataEnterField)this.State["kdenter"];
             }  

             this.State.Remove("kdenter"); */
            kdenter.Refresh();
        }
        

        // Загрузка данных для элементов ViewModel
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void tBx_KeyData_GotFocus(object sender, RoutedEventArgs e)
        {
            tBl_KeyDataFocused = true;
            if (!StringOperation.IsIntNumber(tBx_KeyData.Text))
            {
                tBx_KeyData.SelectAll();
            }
            LayoutShift(0);
        }
        
        private void tBx_KeyData_LostFocus(object sender, RoutedEventArgs e)
        {
            //tBl_KeyDataFocused = false;
            kdenter.CheckEnterKeyData(tBx_KeyData.Text);
            LayoutShift(0);
        }

        private void tBx_KeyData_TextChanged(object sender, TextChangedEventArgs e)
        {
            kdenter.FindKeyData(tBx_KeyData.Text);
        }

        private void tBl_SelNumbFloor_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputScope inputScope = new InputScope();
            InputScopeName inputScopeName = new InputScopeName();
            inputScopeName.NameValue = InputScopeNameValue.Number;
            inputScope.Names.Add(inputScopeName);
            tBx_KeyData.InputScope = inputScope;
            kdenter.EnterKeyData();
            tBx_KeyData.Focus();
        }

        private void tBl_SelName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputScope inputScope = new InputScope();
            InputScopeName inputScopeName = new InputScopeName();
            inputScopeName.NameValue = InputScopeNameValue.Default;
            inputScope.Names.Add(inputScopeName);
            tBx_KeyData.InputScope = inputScope;
            kdenter.EnterKeyData();
            tBx_KeyData.Focus();
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // тут были попытки сделать выделение выбранного элемента списка. неудачно... пока...
            // ! идея пришла опосля! нужно использовать кнопку, а не текспбокс)))
            //TextBlock tb = e.OriginalSource as TextBlock; 
            //Canvas cv = tb.Parent as Canvas;
            //cv.Background.SetValue(SolidColorBrush.ColorProperty, Colors.Cyan);
            //tb.Foreground.SetValue(SolidColorBrush.ColorProperty, Colors.Yellow);
            Thread.Sleep(500);

            kdenter.SelectFromCaseOfSelectItems(e.OriginalSource);

            kdenter.CoSVisible = System.Windows.Visibility.Collapsed;
            tBx_HotWaterMainEnterField.Focus();
        }

        private void Pivot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (tBl_KeyDataFocused)
            {
                tBl_KeyDataFocused = false;
                kdenter.CoSVisible = Visibility.Collapsed;
                return;
            }
            else kdenter.CoSVisible = Visibility.Collapsed;
        }

        private void btn_AddKeyData_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (kdenter.AddButtonSymb == "?")
            {   // это нужно переходить на страницу редактирования ключевых данных
                App.CorrectionID = kdenter.SelectedID;
                NavigationService.Navigate(new Uri("/KeyDataCorrectionPage.xaml", UriKind.Relative));
                return;
            }
            kdenter.AddNewKeyData(tBx_KeyData.Text);
            tBx_HotWaterMainEnterField.Focus();
        }

        private void tBx_HotWaterEnterField_TextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            //kdenter.AttDataChanged();
        }

       /* private void Pivot_GotFocus(object sender, RoutedEventArgs e)
        {
            kdenter.AttDataChanged();
        }

        private void Pivot_GotFocus_1(object sender, RoutedEventArgs e)
        {
            kdenter.AttDataChanged();
        } */

        private void btn_SecondaryHotWater_Click(object sender, RoutedEventArgs e)
        {
            kdenter.AttData.HotWaterDivideSwitch();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            kdenter.AttData.Save();
            App.ViewModel.LoadData();
            kdenter.ClearData();
        }

        private void Border_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Border brd = sender as Border;
            //brd.BorderThickness = new Thickness(5); 
           // MessageBox.Show("yf;fk");
        }

        private void ListElement_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // MessageBox.Show("yf;fk");
        }

        private void Border_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border brd = sender as Border;
            // brd.BorderThickness = new Thickness(5);
            ItemViewModel attmodel = brd.DataContext as ItemViewModel;
            //  MessageBox.Show(attmodel.AttDataID.ToString());
            NavigationService.Navigate(new Uri("/AttDataCorrectionPage.xaml?ID="
                                        + Uri.EscapeDataString(attmodel.AttDataID.ToString()), UriKind.Relative));
        }




        public event PropertyChangedEventHandler PropertyChanged;

        private void tBx_ChangeFocus(object sender, RoutedEventArgs e)
        {
            LayoutShift(1);
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ImportDataPage.xaml", UriKind.Relative));
        }

        private void ApplicationBarMenu_JournalFilter_Click(object sender, EventArgs e)
        {

        }

        private void TextBlock_FloorNo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.ViewParam.SortedBy = Sorting.ByFloorNo;
            App.ViewModel.LoadData();
            // MessageBox.Show("yf;fk");
        }

        private void TextBlock_Date_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            App.ViewModel.ViewParam.SortedBy = Sorting.ByDateOfIncome;
            App.ViewModel.LoadData();
        }
    }
}