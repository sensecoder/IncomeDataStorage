using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using IncomeDataStorage.Data;
using IncomeDataStorage.Presentation;

namespace IncomeDataStorage
{
    /// <summary>
    /// Формирует область отображающую значения в ячейке, 
    /// которая является возможной ошибкой в наборе ячеек из таблицы екселя.
    /// </summary>
    public class ErrorArea
    {
        public event ScenarioStep GoNextStep;

        private bool isOnceAccepted = false; // Флаг, который указывает что один раз уже было подтверждение выбора.
        private StackPanel viewPanel;
        private KeyValuePair<Cell, Mask> pair;
        private Grid errorArea;
        private Grid captionArea;
        
        public ErrorArea() {}
        public ErrorArea(StackPanel Panel, KeyValuePair<Cell, Mask> Pair)
        {
            viewPanel = Panel;
            pair = Pair;
        }

        public void Show()
        {
            errorArea = new Grid();
            errorArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            errorArea.ColumnDefinitions.Add(new ColumnDefinition());
            errorArea.RowDefinitions.Add(new RowDefinition());
            errorArea.RowDefinitions.Add(new RowDefinition());

            StackPanel panel = new StackPanel();
            panel.SetValue(Grid.ColumnProperty, 1);

            Grid selectionGrid = new Grid();
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.SetValue(Grid.RowProperty, 1);
            selectionGrid.SetValue(Grid.ColumnProperty, 1);

            TextBlock Sign = new TextBlock()
            {
                Text = "? ",
                Foreground = new SolidColorBrush(Colors.Yellow),
                FontSize = 36,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5, 5, 5, 5)
            };
            Sign.SetValue(Grid.ColumnProperty, 0);
            Sign.SetValue(Grid.RowProperty, 0);

            TextBlock HeaderCaption = new TextBlock() { Text = "Возможно закралась ошибка:", FontSize = 24 };
            TextBlock HeaderCellData = new TextBlock() { Text = "Ячейка: " + pair.Key.Name + " '" + pair.Key.Value + "'", FontSize = 24 };

            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("да, в игнор её!");
            selectionPicker.Items.Add("это значение!");
            selectionPicker.SetValue(Grid.ColumnProperty, 0);
            selectionPicker.SelectionChanged += selectionPicker_SelectionChanged;

            Button HeaderAceptionBtn = new Button() { Content = "подтвердить" };
            HeaderAceptionBtn.SetValue(Grid.ColumnProperty, 1);
            HeaderAceptionBtn.Tap += HeaderAceptionBtn_Tap;

            panel.Children.Add(HeaderCaption);
            panel.Children.Add(HeaderCellData);

            selectionGrid.Children.Add(selectionPicker);
            selectionGrid.Children.Add(HeaderAceptionBtn);

            errorArea.Children.Add(Sign);
            errorArea.Children.Add(panel);
            errorArea.Children.Add(selectionGrid);
            //HeaderPanel.Children.Add(HeaderArea);
            //HeaderArea.Children.Add(HeaderPanel);
            viewPanel.Children.Add(errorArea);
        }

       //public SelectionChangedEventHandler selectionPicker_SelectionChanged { get; set; }

        private void HeaderAceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            errorArea.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
            else captionArea.Visibility = Visibility.Visible;
            if (!isOnceAccepted)
            {
                isOnceAccepted = true;
                GoNextStep();
            }
        }

        private void reAction_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            captionArea.Visibility = Visibility.Collapsed;
            errorArea.Visibility = Visibility.Visible;
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa;
            if (pair.Value.HasValue) capa = "Исправлена ошибка в данных.";
            else capa = "Обнаружены ошибочные данные.";
            TextBlock NameCaption = new TextBlock()
            {
                Text = capa,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(10, 5, 0, 0)
            };
            TextBlock reAction = new TextBlock()
            {
                Text = "  ...  ",
                TextAlignment = TextAlignment.Right,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, -20, 0, 0),
            };
            reAction.IsHitTestVisible = false;
            Grid reActionArea = new Grid()
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Height = 30,
                Width = 70,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            reActionArea.Tap += reAction_Tap;

            captionArea.Children.Add(NameCaption);
            captionArea.Children.Add(reAction);
            captionArea.Children.Add(reActionArea);
            viewPanel.Children.Add(captionArea);
        }

        public void selectionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker picker = sender as ListPicker;
            if (picker.SelectedIndex == 0) SelectError();
            if (picker.SelectedIndex == 1) SelectValue();
        }

        private void SelectValue()
        {
            var mask = pair.Value;
            mask.HasValue = true;
            mask.IsHeader = false;
            mask.MaskSyntax = "<VALUE>";
            mask.AssIndex = 0;
            mask.АssIndexCount = 1;
        }

        private void SelectError()
        {
            var mask = pair.Value;
            mask.HasValue = false;
            mask.MaskSyntax = "<ERROR>";
            mask.AssIndex = -1;
            mask.АssIndexCount = -1;
        }
    }
}
