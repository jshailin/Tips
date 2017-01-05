﻿using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Tips.Model;
using Tips.UI_Resources;

namespace Tips
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : UI_Resources.GWindow
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(
            IntPtr hwnd,
            ref MARGINS pMarInset);
        //Global Elements
        TaskPlan taskplan;

        public MainWindow()
        {
            InitializeComponent();
            
            FrmMainInit();
            
        }

        private void ExtendAeroGlass(Window window)
        {
            try
            {
                // 为WPF程序获取窗口句柄
                IntPtr mainWindowPtr = new WindowInteropHelper(window).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;

                // 设置Margins
                MARGINS margins = new MARGINS();

                // 扩展Aero Glass
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                int hr = DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                if (hr < 0)
                {
                    MessageBox.Show("DwmExtendFrameIntoClientArea Failed");
                }
            }
            catch (DllNotFoundException)
            {
                Application.Current.MainWindow.Background = Brushes.White;
            }
        }

        private void FrmMain_Loaded(object sender, RoutedEventArgs e)
        {
            this.Background = Brushes.Transparent;
            ExtendAeroGlass(this);
        }

        private void FrmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void FrmMainInit()
        {
            taskplan = new TaskPlan();
            RefreshTaskListBox();

            //TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter adapter = new TipsDBDataSetTableAdapters.ViewTaskSortTableAdapter();
            //TipsDBDataSet.ViewTaskSortDataTable table = new TipsDBDataSet.ViewTaskSortDataTable();
            //adapter.Fill(table);
            //Testbox.DataContext = table;
            //Testbox.DisplayMemberPath = "TaskName";
        }



        private void FrmMain_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.AreaMenuItems[0].Checked = false;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowTipsItem_Click(object sender, EventArgs e)
        {
            //显示主窗口
            if (this.Visibility == Visibility.Hidden)
            {
                this.Show();
                ((System.Windows.Forms.MenuItem)sender).Checked = true;
            }
            else if (this.Visibility == Visibility.Visible)
            {
                this.Hide();
                ((System.Windows.Forms.MenuItem)sender).Checked = false;
            }
        }

        private void DelayTaskItem_Click(object sender, EventArgs e)
        {
            //打开延迟任务列表
        }

        private void ShowHistoryItem_Click(object sender, EventArgs e)
        {
            //打开历史记录窗口
        }

        private void ShowOptionItem_Click(object sender, EventArgs e)
        {
            //打开配置窗口
        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshTaskListBox()
        {
            foreach (TaskItem newItem in taskplan.ProcessTaskList)
            {
                newItem.Template = TaskListBox.Resources["ListItemTemplate"] as ControlTemplate;
                newItem.ContextMenu = TaskListBox.Resources["TaskContexMenu"] as ContextMenu;
                TaskListBox.Items.Add(newItem);
            }
        }

    }
}
