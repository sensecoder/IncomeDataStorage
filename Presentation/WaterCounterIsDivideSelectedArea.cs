using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace IncomeDataStorage.Presentation
{
    public class WaterCounterIsDivideSelectedArea
    {
        public event SecondaryKeyDataProcessing GoNext;

        private enum SelectionMethod
        {
            ByRule, FromExcelTable
        }

        private SelectionMethod method = SelectionMethod.ByRule;
        private StackPanel viewPanel;
        private StackPanel areaPanel;
        private StackPanel ruleArea;
        private Grid captionArea;
        //private Grid FillMetodSelectionArea;

        public WaterCounterIsDivideSelectedArea()
        { }
        public WaterCounterIsDivideSelectedArea(StackPanel panel)
        {
            viewPanel = panel;
            ShowFillMethodSelectionArea();
        }

        public void ShowFillMethodSelectionArea()
        {
            if (areaPanel == null)
                areaPanel = new StackPanel();
            TextBlock text = new TextBlock();
            text.Text = "Обязательно необходимо уточнить в какой квартире имеется разделение учетов. " + 
                        "Если это явно не указано в таблице с данными, то можно задать правило, " +
                        "построенное по маске первичных ключевых данных. Сделайте выбор:";
            text.FontSize = 24;
            text.TextWrapping = TextWrapping.Wrap;
            areaPanel.Children.Add(text);

            Grid selectionArea = new Grid();
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());
            selectionArea.ColumnDefinitions.Add(new ColumnDefinition());
            
            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("по правилу");
            selectionPicker.Items.Add("из таблицы");
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

        private void ShowRuleDefinintionArea()
        {
            if (ruleArea == null)
                ruleArea = new StackPanel();
            else
            {
                ruleArea.Visibility = Visibility.Visible;
                return;
            }

            TextBlock text = new TextBlock();
            text.Text = "Правило: Если маска первичного набора данных содержит " +
                        "два индекса ассоциаций, то считать что разделение учетов имеет место быть. " +
                        "Если один индекс, то разделения нет.";
            text.FontSize = 24;
            text.TextWrapping = TextWrapping.Wrap;
            areaPanel.Children.Add(text);

            Button RuleAceptionBtn = new Button() { Content = "принять правило", HorizontalAlignment = HorizontalAlignment.Center };
            RuleAceptionBtn.SetValue(Grid.ColumnProperty, 1);
            RuleAceptionBtn.Tap += RuleAceptionBtn_Tap;
            areaPanel.Children.Add(RuleAceptionBtn);
        }

        private void AceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (method == SelectionMethod.ByRule) ShowRuleDefinintionArea();
            if (method == SelectionMethod.FromExcelTable) MessageBox.Show("А это пока не предусмотрено %)");
        }

        private void RuleAceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            areaPanel.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
               else captionArea.Visibility = Visibility.Visible;
            GoNext(new SecondaryKeyDataParam() { FieldName = "WaterCounterIsDivide", Method = ProcessingMethod.byRule });
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa = "";
            if (method == SelectionMethod.ByRule) capa = "Раздел учетов определяется по правилу.";
            if (method == SelectionMethod.FromExcelTable) capa = "Раздел учетов определяется из таблицы.";
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

        public void selectionPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker picker = sender as ListPicker;
            if (picker.SelectedIndex == 0) method = SelectionMethod.ByRule;
            if (picker.SelectedIndex == 1) method = SelectionMethod.FromExcelTable;
        }

    }
}
