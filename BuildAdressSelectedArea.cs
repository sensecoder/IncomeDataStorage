using System;
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
    public class BuildAdressSelectedArea
    {
        public event SecondaryKeyDataProcessing GoNext;

        private enum SelectionMethod
        {
            FromExcelTable, SkipSelection, AllTheSame
        }

        private SelectionMethod method = SelectionMethod.AllTheSame;
        private StackPanel viewPanel;
        private StackPanel areaPanel;
        private Grid captionArea;
        private Grid adressInputArea;
        private TextBox adressInputBox;

        // Конструкторы:
        public BuildAdressSelectedArea() { }
        public BuildAdressSelectedArea(StackPanel panel)
        {
            viewPanel = panel;
            ShowFillMethodSelectionArea();
        }

        public void ShowFillMethodSelectionArea()
        {
            if (areaPanel == null)
                areaPanel = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Адрес дома может быть один для всех квартир. " +
                        "Или можно выбрать список из таблицы экселя. " +
                        "Также возможно пропустить заполнения этого поля. " + "Сделайте выбор:";
            text.FontSize = 24;
            text.TextWrapping = TextWrapping.Wrap;
            areaPanel.Children.Add(text);

            Grid selectionArea = new Grid();
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());

            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("один для всех");
            selectionPicker.Items.Add("из таблицы");
            selectionPicker.Items.Add("пропустить");
            selectionPicker.SetValue(Grid.ColumnProperty, 0);
            selectionPicker.SelectionChanged += selectionPicker_SelectionChanged;

            Button AceptionBtn = new Button() { Content = "подтвердить" };
            AceptionBtn.SetValue(Grid.ColumnProperty, 1);
            AceptionBtn.Tap += MethodAceptionBtn_Tap;

            selectionArea.Children.Add(selectionPicker);
            selectionArea.Children.Add(AceptionBtn);

            areaPanel.Children.Add(selectionArea);
            viewPanel.Children.Add(areaPanel);
        }

        public void selectionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker picker = sender as ListPicker;
            if (picker.SelectedIndex == 0) method = SelectionMethod.AllTheSame;
            if (picker.SelectedIndex == 1) method = SelectionMethod.FromExcelTable;
            if (picker.SelectedIndex == 2) method = SelectionMethod.SkipSelection;
        }

        private void MethodAceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (method == SelectionMethod.AllTheSame)
            {
                ShowInputAdressArea();
                return;
            }
            areaPanel.Visibility = Visibility.Collapsed;
            if (adressInputArea != null) adressInputArea.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
            else captionArea.Visibility = Visibility.Visible;
               // GoNext(new SecondaryKeyDataParam() { FieldName = "BuildAdress", Method = ProcessingMethod.byAllTheSame });
            if (method == SelectionMethod.FromExcelTable) 
                GoNext(new SecondaryKeyDataParam() { FieldName = "BuildAdress", Method = ProcessingMethod.byExcelSet });
            if (method == SelectionMethod.SkipSelection) 
                GoNext(null);
        }

        private void ShowInputAdressArea()
        {
 	        if (adressInputArea != null) 
                adressInputArea.Visibility = Visibility.Visible;
            else
            {
                adressInputArea = new Grid();
                adressInputArea.RowDefinitions.Add(new RowDefinition());
                adressInputArea.RowDefinitions.Add(new RowDefinition());

                TextBlock tbl = new TextBlock() 
                {
                    Text = "Введите адрес:",
                    FontSize = 24
                };
                tbl.SetValue(Grid.RowProperty, 0);
                adressInputArea.Children.Add(tbl);

                Grid inputField = new Grid();
                inputField.ColumnDefinitions.Add(new ColumnDefinition());
                inputField.ColumnDefinitions.Add(new ColumnDefinition());
                inputField.SetValue(Grid.RowProperty, 1);
                adressInputArea.Children.Add(inputField);

                adressInputBox = new TextBox();
                adressInputBox.SetValue(Grid.ColumnProperty, 0);
                inputField.Children.Add(adressInputBox);

                Button adressAcceptionBtn = new Button() { Content = "подтвердить" };
                adressAcceptionBtn.SetValue(Grid.ColumnProperty, 1);
                adressAcceptionBtn.Tap += AdressAcception_Tap;
                inputField.Children.Add(adressAcceptionBtn);

                areaPanel.Children.Add(adressInputArea);
            }
        }
        
        private void AdressAcception_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (adressInputBox != null)
            {
                if (adressInputBox.Text.Trim() == "")
                {
                    MessageBox.Show("Введите адрес!");
                    return;
                }

                areaPanel.Visibility = Visibility.Collapsed;
                if (adressInputArea != null) adressInputArea.Visibility = Visibility.Collapsed;
                if (captionArea == null) ShowCaption();
                else captionArea.Visibility = Visibility.Visible;

                GoNext(new SecondaryKeyDataParam() 
                { 
                    FieldName = "BuildAdress", 
                    Method = ProcessingMethod.byAllTheSame,
                    Addition = adressInputBox.Text
                });
            }
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa = "";
            if (method == SelectionMethod.AllTheSame) capa = "Выбран один адрес дома для всех.";
            if (method == SelectionMethod.SkipSelection) capa = "Заполнение адреса дома пропускается.";
            if (method == SelectionMethod.FromExcelTable) capa = "Адреса домомв определяются из таблицы.";
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
