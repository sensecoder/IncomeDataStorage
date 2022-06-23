using System;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Microsoft.Phone.Controls;
using IncomeDataStorage.Data;

namespace IncomeDataStorage
{
    public class SummaryKeyDataSelectionArea
    {
        public event ScenarioStep GoNext;

        private StackPanel viewPanel;
        private StackPanel areaPanel;
        private Grid btnArea;
        private DoingSelection doingSelection;

        // Конструкторы:
        public SummaryKeyDataSelectionArea() { }
        public SummaryKeyDataSelectionArea(StackPanel panel)
        {
            viewPanel = panel;
            // ShowArea();
        }

        public void ShowStatusArea(ImporterStatus status)
        {
            if (areaPanel == null)
                areaPanel = new StackPanel();

            string concurText = "";

            if (status.ConcurrencedDataCount > 0)
            {
                concurText = "Из них ";
                if (status.ConcurrencedDataCount == status.SelectedKeyDataCount)
                    if (status.ConcurrencedDataCount == status.FullConcurrenced)
                    {
                        concurText += "все полностью совпадает с тем что уже есть в БД. Каких либо действий по обновлению БД не требуется.";
                        doingSelection = DoingSelection.nothing;
                    }
                    else
                    {
                        concurText += "все частично совпадают с тем что уже есть в БД. Нужно ли редактировать записи в БД?.";
                        doingSelection = DoingSelection.select;
                    }
                else
                {
                    if (status.PartialConcurrenced > 0)
                        concurText += status.PartialConcurrenced + " частично совпадают с записями в БД. ";
                    if (status.FullConcurrenced > 0)
                        concurText += status.FullConcurrenced + " полностью совпадают с записями в БД. ";
                    if (status.ConcurrencedDataCount == status.FullConcurrenced)
                    {
                        concurText += "Возможно только добавить новые записи.";
                        doingSelection = DoingSelection.addnew;
                    }
                    else
                    {
                        //concurText += "Нужно ли редактировать записи в БД?";
                        doingSelection = DoingSelection.select;
                    }
                }
            }
            else
            {
                concurText = "Добавить все эти данные в БД?";
                doingSelection = DoingSelection.addnew;
            }

            TextBlock tbl = new TextBlock()
            {
                Text = "Итак, выбрано " + status.SelectedKeyDataCount + " ключевых наборов данных для занесения в БД. " + concurText,
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            };

            areaPanel.Children.Add(tbl);

            if (doingSelection == DoingSelection.select) ShowBtnArea();

            //else tbl.Text += concurText;

            viewPanel.Children.Add(areaPanel);     
        }

        private void ShowBtnArea()
        {
            btnArea = new Grid();
            btnArea.RowDefinitions.Add(new RowDefinition());
            btnArea.RowDefinitions.Add(new RowDefinition());
            btnArea.ColumnDefinitions.Add(new ColumnDefinition());
            btnArea.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock tbl = new TextBlock()
            {
                Text = "Нужно ли редактировать записи в БД?",
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            };
            tbl.SetValue(Grid.RowProperty, 0);
            tbl.SetValue(Grid.ColumnSpanProperty, 2);

            Button btnOK = new Button() { Content = "Да" };
            btnOK.SetValue(Grid.RowProperty, 1);
            btnOK.SetValue(Grid.ColumnProperty, 0);
            btnOK.Tap += BtnOK_Tap;

            Button btnCancel = new Button() { Content = "Нет" };
            btnCancel.SetValue(Grid.RowProperty, 1);
            btnCancel.SetValue(Grid.ColumnProperty, 1);
            btnCancel.Tap += BtnCancel_Tap;

            btnArea.Children.Add(tbl);
            btnArea.Children.Add(btnOK);
            btnArea.Children.Add(btnCancel);

            areaPanel.Children.Add(btnArea);
        }

        private void ShowLastQuestion()
        {
            if (btnArea != null) btnArea.Visibility = Visibility.Collapsed;

            var tbl = new TextBlock()
            {
                FontSize = 24,
                TextWrapping = TextWrapping.Wrap
            };

            switch (doingSelection)
            {
                case DoingSelection.select:
                    tbl.Text = "Итак, существующие в БД записи будут отредактированы, а новые (если есть) будут добавлены. Сейчас будет начат импорт ключевых данных в БД.";
                    break;
                case DoingSelection.addnew:
                    tbl.Text = "Итак, в БД будут добавлены только новые записи. Сейчас будет начат импорт ключевых данных в БД.";
                    break;
                case DoingSelection.nothing:
                    tbl.Text = "Итак, оказалось что все ключевые данные уже есть в БД. Так что ничего импортировать не нужно.";
                    break;
            }

            var lastBtnArea = new Grid();
            lastBtnArea.RowDefinitions.Add(new RowDefinition());
            lastBtnArea.RowDefinitions.Add(new RowDefinition());
            lastBtnArea.ColumnDefinitions.Add(new ColumnDefinition());
            lastBtnArea.ColumnDefinitions.Add(new ColumnDefinition());

            tbl.SetValue(Grid.RowProperty, 0);
            tbl.SetValue(Grid.ColumnSpanProperty, 2);

            var btnOK = new Button() { Content = "Продолжить" };
            btnOK.SetValue(Grid.RowProperty, 1);
            btnOK.SetValue(Grid.ColumnProperty, 0);
            btnOK.Tap += BtnContinue_Tap;

            var btnCancel = new Button() { Content = "Отменить" };
            btnCancel.SetValue(Grid.RowProperty, 1);
            btnCancel.SetValue(Grid.ColumnProperty, 1);
            btnCancel.Tap += BtnCancelImport_Tap;

            lastBtnArea.Children.Add(tbl);
            lastBtnArea.Children.Add(btnOK);
            lastBtnArea.Children.Add(btnCancel);

            areaPanel.Children.Add(lastBtnArea);
        }

        private void BtnOK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            doingSelection = DoingSelection.select;
            ShowLastQuestion();
        }

        private void BtnCancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            doingSelection = DoingSelection.addnew;
            ShowLastQuestion();
        }

        private void BtnContinue_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
          /*  switch (doingSelection)
            {
                case DoingSelection.select:
                    tbl.Text = "Итак, существующие в БД записи будут отредактированы, а новые (если есть) будут добавлены. Сейчас будет начат импорт ключевых данных в БД.";
                    break;
                case DoingSelection.addnew:
                    tbl.Text = "Итак, в БД будут добавлены только новые записи. Сейчас будет начат импорт ключевых данных в БД.";
                    break;
                case DoingSelection.nothing:
                    tbl.Text = "Итак, оказалось что все ключевые данные уже есть в БД. Так что ничего импортировать не нужно.";
                    break;
            }
           */
        }

        private void BtnCancelImport_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }
    }


    public enum DoingSelection
    {
        nothing, select, addnew
    }
}
