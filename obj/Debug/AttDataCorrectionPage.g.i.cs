﻿#pragma checksum "C:\Users\админ\Documents\Visual Studio 2010\Projects\IncomeDataStorage\IncomeDataStorage\AttDataCorrectionPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0A25C49FAE8A77E04B91A6A54DD638CA"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18047
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace IncomeDataStorage {
    
    
    public partial class AttDataCorrectionPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Grid KeyDataArea;
        
        internal System.Windows.Controls.TextBlock textBlock1;
        
        internal System.Windows.Controls.TextBlock textBlock2;
        
        internal System.Windows.Controls.TextBlock textBlock3;
        
        internal System.Windows.Controls.Grid FloorDataArea;
        
        internal System.Windows.Controls.TextBlock textBlock4;
        
        internal System.Windows.Controls.TextBlock textBlock5;
        
        internal System.Windows.Controls.Button btn_DeleteRecord;
        
        internal System.Windows.Controls.Grid CounterDataArea;
        
        internal System.Windows.Controls.TextBlock textBlock6;
        
        internal System.Windows.Controls.Grid HotWaterCountersArea;
        
        internal System.Windows.Controls.TextBlock SecondCounter;
        
        internal System.Windows.Controls.Grid ColdWaterCountersArea;
        
        internal System.Windows.Controls.Button btn_Save;
        
        internal System.Windows.Controls.Button btn_Cancel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/IncomeDataStorage;component/AttDataCorrectionPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.KeyDataArea = ((System.Windows.Controls.Grid)(this.FindName("KeyDataArea")));
            this.textBlock1 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock1")));
            this.textBlock2 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock2")));
            this.textBlock3 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock3")));
            this.FloorDataArea = ((System.Windows.Controls.Grid)(this.FindName("FloorDataArea")));
            this.textBlock4 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock4")));
            this.textBlock5 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock5")));
            this.btn_DeleteRecord = ((System.Windows.Controls.Button)(this.FindName("btn_DeleteRecord")));
            this.CounterDataArea = ((System.Windows.Controls.Grid)(this.FindName("CounterDataArea")));
            this.textBlock6 = ((System.Windows.Controls.TextBlock)(this.FindName("textBlock6")));
            this.HotWaterCountersArea = ((System.Windows.Controls.Grid)(this.FindName("HotWaterCountersArea")));
            this.SecondCounter = ((System.Windows.Controls.TextBlock)(this.FindName("SecondCounter")));
            this.ColdWaterCountersArea = ((System.Windows.Controls.Grid)(this.FindName("ColdWaterCountersArea")));
            this.btn_Save = ((System.Windows.Controls.Button)(this.FindName("btn_Save")));
            this.btn_Cancel = ((System.Windows.Controls.Button)(this.FindName("btn_Cancel")));
        }
    }
}
