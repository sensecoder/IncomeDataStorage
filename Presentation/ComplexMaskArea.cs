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
    /// которая является возможной сложной маской в наборе ячеек из таблицы екселя.
    /// </summary>
    public class ComplexMaskArea
    {
        public event ScenarioStep GoNextStep;
        
        private bool isOnceAccepted = false; // Флаг, который указывает что один раз уже было подтверждение выбора.
        private StackPanel viewPanel;
        private KeyValuePair<Cell, Mask>[] sameValArr;
        private Grid complexMaskArea;
        private Grid captionArea;
        private int allAssIndexCount;
        private PrimaryKeyDataSet primaryDataSet;
        
        public ComplexMaskArea() {}
        public ComplexMaskArea(StackPanel Panel, KeyValuePair<Cell, Mask>[] SameValArr, PrimaryKeyDataSet PrimarySet)
        {
            viewPanel = Panel;
            sameValArr = SameValArr;
            primaryDataSet = PrimarySet;
            allAssIndexCount = PrimarySet.AllAssIndexCount;
        }

        public void Show()
        {
            complexMaskArea = new Grid();
            complexMaskArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            complexMaskArea.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            complexMaskArea.RowDefinitions.Add(new RowDefinition());
            complexMaskArea.RowDefinitions.Add(new RowDefinition());
            complexMaskArea.RowDefinitions.Add(new RowDefinition());

            StackPanel panel = new StackPanel();
            panel.SetValue(Grid.ColumnProperty, 1);

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

            TextBlock HeaderCaption = new TextBlock() 
            { 
                Text = "Найдено " + sameValArr.Length + " ячейки, возможно объединённых одним значением.", 
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(HeaderCaption);

            foreach (var pair in sameValArr)
            {
                TextBlock HeaderCellData = new TextBlock() { Text = "Ячейка: " + pair.Key.Name + " '" + pair.Key.Value + "'", FontSize = 24 };
                panel.Children.Add(HeaderCellData);
            }

            Grid selectionGrid = new Grid();
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.ColumnDefinitions.Add(new ColumnDefinition());
            selectionGrid.SetValue(Grid.RowProperty, 1);
            selectionGrid.SetValue(Grid.ColumnProperty, 1);
            
            ListPicker selectionPicker = new ListPicker() { Margin = new Thickness(0, 3, 0, 0) };
            selectionPicker.Items.Add("да, объединить!");
            selectionPicker.Items.Add("нет, в игнор их!");
            selectionPicker.SetValue(Grid.ColumnProperty, 0);
            selectionPicker.SelectionChanged += selectionPicker_SelectionChanged;

            Button ConcationAceptionBtn = new Button() { Content = "подтвердить" };
            ConcationAceptionBtn.SetValue(Grid.ColumnProperty, 1);
            ConcationAceptionBtn.Tap += ConcationAceptionBtn_Tap;

            Grid  subCaptionArea = new Grid()
            {
                Background = new SolidColorBrush(Colors.Yellow),
                Height = 30,
                Visibility = Visibility.Collapsed
            };
            subCaptionArea.SetValue(Grid.ColumnProperty, 0);
            subCaptionArea.SetValue(Grid.ColumnSpanProperty, 2);
            string capa = "Считаем что ячейки объединены.";
            TextBlock NameCaption = new TextBlock()
            {
                Text = capa,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(10, 5, 0, 0)
            };
            TextBlock reSelect = new TextBlock()
            {
                Text = "  ...  ",
                TextAlignment = TextAlignment.Right,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(0, -20, 0, 0),
            };
            reSelect.Tap += reSelect_Tap;
            subCaptionArea.Children.Add(NameCaption);
            subCaptionArea.Children.Add(reSelect);

            selectionGrid.Children.Add(selectionPicker);
            selectionGrid.Children.Add(ConcationAceptionBtn);
            selectionGrid.Children.Add(subCaptionArea);

            complexMaskArea.Children.Add(Sign);
            complexMaskArea.Children.Add(panel);
            complexMaskArea.Children.Add(selectionGrid);
            //HeaderPanel.Children.Add(HeaderArea);
            //HeaderArea.Children.Add(HeaderPanel);
            viewPanel.Children.Add(complexMaskArea);
        }
  
        private void ConcationAceptionBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Button btn = sender as Button;
            Grid grd = btn.Parent as Grid;
            foreach (var child in grd.Children)
            {
                if (child.Visibility == Visibility.Visible)
                    child.Visibility = Visibility.Collapsed;
                else
                    child.Visibility = Visibility.Visible;
            }
            ShowMustGoOn();
            /* complexMaskArea.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
            else captionArea.Visibility = Visibility.Visible;
            GoNextStep(); */
        }

        private void ShowMustGoOn()
        { // Сие "тровуальное" название означает лишь продолжение утверждений результата анализа
            // и составление нехитрых правил, которые можно распространить на все остальные ячейки.
            StackPanel panel = new StackPanel();
            panel.SetValue(Grid.ColumnProperty, 1);
            panel.SetValue(Grid.RowProperty, 2);

            TextBlock SummaryText = new TextBlock()
            {
                Text = "Теперь для каждой маски, которая закрывает значение, необходимо выбрать ассоциативный индекс:",
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(SummaryText);

            var m = 0;
            foreach (var pair in sameValArr)
            {
                Grid MaskGrd = new Grid();
                MaskGrd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                MaskGrd.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                TextBlock MaskTxt = new TextBlock() 
                { 
                    Text = '"'+pair.Value.MaskSyntax+'"'+" = " ,
                    FontSize = 26,
                    Margin = new Thickness(5, 12, 5, 5)
                };
                MaskTxt.SetValue(Grid.ColumnProperty, 0);
                MaskGrd.Children.Add(MaskTxt);

                ListPicker IndexSelector = new ListPicker() { Margin = new Thickness(0, 0, 0, 0) };
                IndexSelector.DataContext = pair;
                for (int n = 0; n < (allAssIndexCount + sameValArr.Length); n++)
                {
                    IndexSelector.Items.Add("Index " + n.ToString());
                }
                IndexSelector.SetValue(Grid.ColumnProperty, 1);
                IndexSelector.SelectedIndex = m;
                IndexSelector.DataContext = pair;
                IndexSelector.SelectionChanged += IndexSelector_SelectionChanged;
                pair.Value.HasValue = true;
                pair.Value.AssIndex = m;
                pair.Value.АssIndexCount = sameValArr.Length;
                MaskGrd.Children.Add(IndexSelector);

                panel.Children.Add(MaskGrd);
                m++;
            }

            Button AcceptBtn = new Button()
            {
                Content = "подтвердить",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };
            AcceptBtn.Tap += AcceptBtn_Tap;
            panel.Children.Add(AcceptBtn);

            complexMaskArea.Children.Add(panel);
        }

        private void AcceptBtn_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplyRule();
            complexMaskArea.Visibility = Visibility.Collapsed;
            if (captionArea == null) ShowCaption();
            else captionArea.Visibility = Visibility.Visible;
            if (!isOnceAccepted)
            {
                isOnceAccepted = true;
                GoNextStep();
            }
        }

        private void ApplyRule()
        { // применить правило масок ко всем ячейкам с таким же синтаксисом маски.
            if (HasCoincidenceIndexes())
            {
                MessageBox.Show("Имеются совпадающие индексы.");
                return;
            }
            foreach (var lookPair in this.primaryDataSet.KeyMaskDic)
                foreach (var basePair in this.sameValArr)
                {
                    if (lookPair.Value != basePair.Value)
                    {
                        if (lookPair.Value.MaskSyntax == basePair.Value.MaskSyntax)
                        {
                            lookPair.Value.AssIndex = basePair.Value.AssIndex;
                            lookPair.Value.АssIndexCount = basePair.Value.АssIndexCount;
                        }
                    }
                }
        }

        private bool HasCoincidenceIndexes()
        { // проверяем на совпадающие индексы для набора ячеек с одним значением.
            foreach (var basePair in this.sameValArr)
            {
                foreach (var lookPair in sameValArr)
                    if (basePair.Value != lookPair.Value)
                        if (basePair.Value.AssIndex == lookPair.Value.AssIndex)
                            return true;
            }
            return false;
        }

        public void IndexSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListPicker picker = sender as ListPicker;
            KeyValuePair<Cell, Mask> pair = (KeyValuePair<Cell, Mask>)picker.DataContext;
            pair.Value.AssIndex = picker.SelectedIndex;
        }
       
        private void reSelect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock selTxtbl = sender as TextBlock;
            Grid selGrd = selTxtbl.Parent as Grid;
            Grid grd = selGrd.Parent as Grid;
            foreach (var child in grd.Children)
            {
                if (child.Visibility == Visibility.Visible)
                    child.Visibility = Visibility.Collapsed;
                else
                    child.Visibility = Visibility.Visible;
            }
        }

        private void reAction_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            captionArea.Visibility = Visibility.Collapsed;
            complexMaskArea.Visibility = Visibility.Visible;
        }

        private void ShowCaption()
        {
            captionArea = new Grid()
            {
                Background = new SolidColorBrush(new Color() { A = 255, R = 60, G = 179, B = 113 }),
                Height = 30
            };
            string capa = "Найдено объединение ячеек.";
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
            if (picker.SelectedIndex == 0) SelectConcat();
            if (picker.SelectedIndex == 1)
            {
                picker.SelectedIndex = 0;
                SelectIgnore();
            }
        }

        private void SelectConcat()
        {
            // а тут ниче делать не надо, просто будет продолжение по ходу пьессы.
        }

        private void SelectIgnore()
        {
            MessageBox.Show("Да пошел ты нахуй! Не бывать этому!");
            /* // Каждую ячейку, с предположительно объединенным значением, пометить
            // как не имеющую значения.
            foreach (var pair in sameValArr)
            {
                var mask = pair.Value;
                mask.HasValue = false;
            }  */
        }
    }
}
