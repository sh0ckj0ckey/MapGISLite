using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapGIS_WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 标记当前显示的是第几个页面
        /// </summary>
        private int PageIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            //DataHelper.AddData("1", new MapGIS.GeoObjects.Geometry.Dot(1, 1), "JJKKLL", new DateTime(1999, 12, 25));
            //DataHelper.SearchData();
            MainFrame.NavigationService.Navigate(new Uri("HomePage.xaml", UriKind.Relative));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // This can't be done any earlier than the SourceInitialized event:
            //-1表示将整个窗体设置为透明
            GlassHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        /// <summary>
        /// 窗口拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #region 关闭窗口

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        bool _closinganimation = true;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = _closinganimation;
            _closinganimation = false;

            Storyboard sb = new Storyboard();
            DoubleAnimation dh = new DoubleAnimation();
            DoubleAnimation dw = new DoubleAnimation();
            DoubleAnimation dop = new DoubleAnimation();
            dop.Duration = dh.Duration = dw.Duration = sb.Duration = new Duration(TimeSpan.FromSeconds(0.4));
            dop.To = dh.To = dw.To = 0;
            Storyboard.SetTarget(dop, this);
            Storyboard.SetTarget(dh, this);
            Storyboard.SetTarget(dw, this);
            Storyboard.SetTargetProperty(dop, new PropertyPath("Opacity", new object[] { }));
            Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
            Storyboard.SetTargetProperty(dw, new PropertyPath("Width", new object[] { }));
            sb.Children.Add(dh);
            sb.Children.Add(dw);
            sb.Children.Add(dop);
            sb.Completed += new EventHandler(Sb_Completed); //(a, b) => { this.Close(); };
            sb.Begin();
        }

        void Sb_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region 导航

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 0;
            HomeButton.BorderThickness = new Thickness(0);
            HomeButton.Background = new SolidColorBrush(Color.FromArgb(255, 92, 29, 244));
            SimpleDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            AdvanceDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            HomeButton.Width = 64;
            SimpleDataButton.Width = 36;
            AdvanceDataButton.Width = 36;
            Button1TextBlock1.Visibility = Visibility.Collapsed;
            Button1TextBlock2.Visibility = Visibility.Visible;
            Button2TextBlock1.Visibility = Visibility.Visible;
            Button2TextBlock2.Visibility = Visibility.Collapsed;
            Button3TextBlock1.Visibility = Visibility.Visible;
            Button3TextBlock2.Visibility = Visibility.Collapsed;
            MainFrame.NavigationService.Navigate(new Uri("HomePage.xaml", UriKind.Relative));
        }

        private void SimpleDataButton_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 1;
            SimpleDataButton.BorderThickness = new Thickness(0);
            HomeButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            SimpleDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 92, 29, 244));
            AdvanceDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            HomeButton.Width = 36;
            SimpleDataButton.Width = 64;
            AdvanceDataButton.Width = 36;
            Button1TextBlock1.Visibility = Visibility.Visible;
            Button1TextBlock2.Visibility = Visibility.Collapsed;
            Button2TextBlock1.Visibility = Visibility.Collapsed;
            Button2TextBlock2.Visibility = Visibility.Visible;
            Button3TextBlock1.Visibility = Visibility.Visible;
            Button3TextBlock2.Visibility = Visibility.Collapsed;
            MainFrame.NavigationService.Navigate(new Uri("SimpleDataPage.xaml", UriKind.Relative));
        }

        private void AdvanceDataButton_Click(object sender, RoutedEventArgs e)
        {
            PageIndex = 2;
            AdvanceDataButton.BorderThickness = new Thickness(0);
            HomeButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            SimpleDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
            AdvanceDataButton.Background = new SolidColorBrush(Color.FromArgb(255, 92, 29, 244));
            HomeButton.Width = 36;
            SimpleDataButton.Width = 36;
            AdvanceDataButton.Width = 64;
            Button1TextBlock1.Visibility = Visibility.Visible;
            Button1TextBlock2.Visibility = Visibility.Collapsed;
            Button2TextBlock1.Visibility = Visibility.Visible;
            Button2TextBlock2.Visibility = Visibility.Collapsed;
            Button3TextBlock1.Visibility = Visibility.Collapsed;
            Button3TextBlock2.Visibility = Visibility.Visible;
            MainFrame.NavigationService.Navigate(new Uri("AdvanceDataPage.xaml", UriKind.Relative));
        }

        private void SimpleDataButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PageIndex != 1)
            {
                SimpleDataButton.BorderThickness = new Thickness(2);
            }
        }

        private void SimpleDataButton_MouseLeave(object sender, MouseEventArgs e)
        {
            SimpleDataButton.BorderThickness = new Thickness(0);
        }

        private void HomeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PageIndex != 0)
            {
                HomeButton.BorderThickness = new Thickness(2);
            }
        }

        private void HomeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            HomeButton.BorderThickness = new Thickness(0);
        }

        private void AdvanceDataButton_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PageIndex != 2)
            {
                AdvanceDataButton.BorderThickness = new Thickness(2);
            }
        }

        private void AdvanceDataButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AdvanceDataButton.BorderThickness = new Thickness(0);
        }

        #endregion


        public class GlassHelper
        {
            public static bool ExtendGlassFrame(Window window, Thickness margin)
            {
                if (!DwmIsCompositionEnabled())
                    return false;

                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero)
                    throw new InvalidOperationException("The Window must be shown before extending glass.");

                // Set the background to transparent from both the WPF and Win32 perspectives
                window.Background = Brushes.Transparent;
                HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

                MARGINS margins = new MARGINS(margin);
                DwmExtendFrameIntoClientArea(hwnd, ref margins);
                return true;
            }

            struct MARGINS
            {
                public MARGINS(Thickness t)
                {
                    Left = (int)t.Left;
                    Right = (int)t.Right;
                    Top = (int)t.Top;
                    Bottom = (int)t.Bottom;
                }
                public int Left;
                public int Right;
                public int Top;
                public int Bottom;
            }

            [DllImport("dwmapi.dll", PreserveSig = false)]
            static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);


            [DllImport("dwmapi.dll", PreserveSig = false)]
            static extern bool DwmIsCompositionEnabled();
        }

    }
}
