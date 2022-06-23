using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.Phone.Controls;

namespace IncomeDataStorage
{
    public class OwnerNameSelectedArea
    {
        public event SecondaryKeyDataProcessing GoNext;

        private enum SelectionMethod
        {
            FromExcelTable, SkipSelection
        }

        private SelectionMethod method = SelectionMethod.FromExcelTable;
        private StackPanel viewPanel;
        private StackPanel areaPanel;
        private Grid captionArea;

        // Конструкторы:
        public OwnerNameSelectedArea()
        { }
        public OwnerNameSelectedArea(StackPanel panel)
        {
            viewPanel = panel;
            ShowFillMethodSelectionArea();
        }

        public void ShowFillMethodSelectionArea()
        {
            if (areaPanel == null)
                areaPanel = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Фамилии собственников можно выбрать из таблицы экселя. " +
                        "Или пропустить заполнения этого поля. " + "Сделайте выбор:";
            text.FontSize = 24;
            text.TextWrapping = TextWrapping.Wrap;
            areaPanel.Children.Add(text);

            Grid selectionArea = new Grid();
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());

            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("из таблицы");
            selectionPicker.Items.Add("пропустить");
            selectionPicker.SetValue(Grid.ColumnProperty, 0);
            selectionPicker.SelectionChanged += selectionPicker_SelectionChanged;

            Button AceptionBtn = new Button() { Content = "подтвердить" };
            AceptionBtn.SetValue(Grid.ColumnProperty, 1);
            AceptionBtn.Tap += AceptionBtn_Tap;

            selectionArea.Children.Add(selectionPicker);
            selectionArea.Children.Add(AceptionBtn);

            areaPanel.Children.Add(selectionArea);
            viewPanel.Children.Add(areaPanel);
        }

        public void selectionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker picker = sender as ListPicker;
            if (picker.SelectedIndex == 0) method = SelectionMethod.FromExcelTable;
            if (picker.SelectedIndex == 1) method = SelectionMethod.SkipSelection;
        }

        private void AceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            areaPanel.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
            else captionArea.Visibility = Visibility.Visible;

            if (method == SelectionMethod.FromExcelTable) 
                GoNext(new SecondaryKeyDataParam() { FieldName = "Name", Method = ProcessingMethod.byExcelSet });
            if (method == SelectionMethod.SkipSelection) 
                GoNext(null);
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa = "";
            if (method == SelectionMethod.SkipSelection) capa = "Заполнение имени собственника пропускается.";
            if (method == SelectionMethod.FromExcelTable) capa = "Имена собственников определяются из таблицы.";
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

        private void reAction_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            captionArea.Visibility = Visibility.Collapsed;
            areaPanel.Visibility = Visibility.Visible;
        }
    }
}
