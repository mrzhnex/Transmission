﻿#pragma checksum "..\..\..\SettingsWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "45BC8308010530229EE7B650B9CCBF019AFEA002"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Core.Application;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Client {
    
    
    /// <summary>
    /// SettingsWindow
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 48 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Minimize;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Deploy;
        
        #line default
        #line hidden
        
        
        #line 54 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseButton;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock RecordSaveFolder;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock PlayAudioFile;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Languages;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Themes;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox FontStyles;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ShouldLog;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBoxItem @true;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\SettingsWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBoxItem @false;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Client;V1.0.0.0;component/settingswindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SettingsWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "6.0.6.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 10 "..\..\..\SettingsWindow.xaml"
            ((Client.SettingsWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 40 "..\..\..\SettingsWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Grid_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Minimize = ((System.Windows.Controls.Button)(target));
            
            #line 48 "..\..\..\SettingsWindow.xaml"
            this.Minimize.Click += new System.Windows.RoutedEventHandler(this.Minimize_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Deploy = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\..\SettingsWindow.xaml"
            this.Deploy.Click += new System.Windows.RoutedEventHandler(this.Deploy_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.CloseButton = ((System.Windows.Controls.Button)(target));
            
            #line 54 "..\..\..\SettingsWindow.xaml"
            this.CloseButton.Click += new System.Windows.RoutedEventHandler(this.CloseButton_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.RecordSaveFolder = ((System.Windows.Controls.TextBlock)(target));
            
            #line 60 "..\..\..\SettingsWindow.xaml"
            this.RecordSaveFolder.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.RecordSaveFolder_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.PlayAudioFile = ((System.Windows.Controls.TextBlock)(target));
            
            #line 63 "..\..\..\SettingsWindow.xaml"
            this.PlayAudioFile.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.PlayAudioFile_MouseLeftButtonDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Languages = ((System.Windows.Controls.ComboBox)(target));
            
            #line 66 "..\..\..\SettingsWindow.xaml"
            this.Languages.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Languages_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Themes = ((System.Windows.Controls.ComboBox)(target));
            
            #line 69 "..\..\..\SettingsWindow.xaml"
            this.Themes.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Themes_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            this.FontStyles = ((System.Windows.Controls.ComboBox)(target));
            
            #line 72 "..\..\..\SettingsWindow.xaml"
            this.FontStyles.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.FontStyles_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 11:
            this.ShouldLog = ((System.Windows.Controls.ComboBox)(target));
            
            #line 81 "..\..\..\SettingsWindow.xaml"
            this.ShouldLog.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ShouldLog_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 12:
            this.@true = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            case 13:
            this.@false = ((System.Windows.Controls.ComboBoxItem)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

