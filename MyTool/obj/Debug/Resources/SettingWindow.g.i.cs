﻿#pragma checksum "..\..\..\Resources\SettingWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "CCA9D1A8FF7EA3E5D3172529CC3898FC71690DE8"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using MyTool;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace MyTool {
    
    
    /// <summary>
    /// SettingWindow
    /// </summary>
    public partial class SettingWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 26 "..\..\..\Resources\SettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtCmd_Open;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Resources\SettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lbItem;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\..\Resources\SettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtMsg;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MyTool;component/resources/settingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Resources\SettingWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.txtCmd_Open = ((System.Windows.Controls.TextBox)(target));
            
            #line 26 "..\..\..\Resources\SettingWindow.xaml"
            this.txtCmd_Open.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(this.Control_PreviewMouseUp);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\Resources\SettingWindow.xaml"
            this.txtCmd_Open.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Control_MouseLeave);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 43 "..\..\..\Resources\SettingWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).Drop += new System.Windows.DragEventHandler(this.Grid_Drop);
            
            #line default
            #line hidden
            
            #line 43 "..\..\..\Resources\SettingWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).DragEnter += new System.Windows.DragEventHandler(this.Grid_DragEnter);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lbItem = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.txtMsg = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

