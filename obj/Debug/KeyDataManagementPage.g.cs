#pragma checksum "C:\Users\админ\Documents\Visual Studio 2010\Projects\IncomeDataStorage\IncomeDataStorage\KeyDataManagementPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "48945B3BD719A1976A4301D1AAE91BA8"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18034
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Live.Controls;
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
    
    
    public partial class KeyDataManagementPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.StackPanel ContentPanel;
        
        internal System.Windows.Controls.Button SkyDriveLoad;
        
        internal System.Windows.Controls.Grid ConnectPanel;
        
        internal System.Windows.Controls.StackPanel ConnStat;
        
        internal System.Windows.Controls.TextBlock Client;
        
        internal System.Windows.Controls.TextBlock Session;
        
        internal Microsoft.Live.Controls.SignInButton btnLogin;
        
        internal System.Windows.Controls.Grid FileChooseArea;
        
        internal System.Windows.Controls.ListBox FileListBox;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/IncomeDataStorage;component/KeyDataManagementPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.StackPanel)(this.FindName("ContentPanel")));
            this.SkyDriveLoad = ((System.Windows.Controls.Button)(this.FindName("SkyDriveLoad")));
            this.ConnectPanel = ((System.Windows.Controls.Grid)(this.FindName("ConnectPanel")));
            this.ConnStat = ((System.Windows.Controls.StackPanel)(this.FindName("ConnStat")));
            this.Client = ((System.Windows.Controls.TextBlock)(this.FindName("Client")));
            this.Session = ((System.Windows.Controls.TextBlock)(this.FindName("Session")));
            this.btnLogin = ((Microsoft.Live.Controls.SignInButton)(this.FindName("btnLogin")));
            this.FileChooseArea = ((System.Windows.Controls.Grid)(this.FindName("FileChooseArea")));
            this.FileListBox = ((System.Windows.Controls.ListBox)(this.FindName("FileListBox")));
        }
    }
}

