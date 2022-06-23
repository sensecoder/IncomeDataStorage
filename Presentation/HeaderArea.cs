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
    /// которая является возможным заголовком в наборе ячеек из таблицы екселя.
    /// </summary>
    public class HeaderArea
    {
        public event ScenarioStep GoNextStep;

        private bool isOnceAccepted = false; // Флаг, который указывает что один раз уже было подтверждение выбора.
        private StackPanel viewPanel;
        private KeyValuePair<Cell, Mask> pair;
        private Grid headerArea;
        private Grid captionArea;
        
        public HeaderArea() {}
        public HeaderArea(StackPanel Panel, KeyValuePair<Cell, Mask> Pair)
        {
            viewPanel = Panel;
            pair = Pair;
        }

        public void Show()
        {
            headerArea = new Grid();
            headerArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            headerArea.ColumnDefinitions.Add(new ColumnDefinition());
            headerArea.RowDefinitions.Add(new RowDefinition());
            headerArea.RowDefinitions.Add(new RowDefinition());

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

            TextBlock HeaderCaption = new TextBlock() { Text = "Возможно найден заголовок:", FontSize = 24 };
            TextBlock HeaderCellData = new TextBlock() { Text = "Ячейка: " + pair.Key.Name + " '" + pair.Key.Value + "'", FontSize = 24 };

            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("да, это так!");
            selectionPicker.Items.Add("нет, в игнор!");
            selectionPicker.SetValue(Grid.ColumnProperty, 0);
            selectionPicker.SelectionChanged += selectionPicker_SelectionChanged;

            Button HeaderAceptionBtn = new Button() { Content = "подтвердить" };
            HeaderAceptionBtn.SetValue(Grid.ColumnProperty, 1);
            HeaderAceptionBtn.Tap += HeaderAceptionBtn_Tap;

            panel.Children.Add(HeaderCaption);
            panel.Children.Add(HeaderCellData);

            selectionGrid.Children.Add(selectionPicker);
            selectionGrid.Children.Add(HeaderAceptionBtn);

            headerArea.Children.Add(Sign);
            headerArea.Children.Add(panel);
            headerArea.Children.Add(selectionGrid);
            //HeaderPanel.Children.Add(HeaderArea);
            //HeaderArea.Children.Add(HeaderPanel);
            viewPanel.Children.Add(headerArea);
        }

       //public SelectionChangedEventHandler selectionPicker_SelectionChanged { get; set; }

        private void HeaderAceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            headerArea.Visibility = Visibility.Collapsed;
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
            headerArea.Visibility = Visibility.Visible;
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa;
            if (pair.Value.IsHeader) capa = "Определен заголовок данных.";
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
            if (picker.SelectedIndex == 0) SelectHeader();
            if (picker.SelectedIndex == 1) SelectIgnore();
        }

        private void SelectIgnore()
        {
            var mask = pair.Value;
            mask.HasValue = false;
            mask.IsHeader = false;
            mask.MaskSyntax = "<SKIP>";
        }

        private void SelectHeader()
        {
            var mask = pair.Value;
            mask.HasValue = false;
            mask.IsHeader = true;
        }
    }
}
