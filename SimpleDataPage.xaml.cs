using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using MapGIS.GeoDataBase;
using MapGIS.GeoObjects.Att;
using MapGIS_WPF.Helpers;
using Panuon.UI;
using static MapGIS_WPF.Helpers.ChartHelper;

namespace MapGIS_WPF
{
    /// <summary>
    /// SimpleDataPage.xaml 的交互逻辑
    /// </summary>
    public partial class SimpleDataPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public SeriesCollection SeriesCollection2 { get; set; }
        public string[] Labels { get; set; }
        public string[] Labels2 { get; set; }
        ChartValues<double> lengthData = new ChartValues<double>();
        ChartValues<double> timeData = new ChartValues<double>();

        public Func<double, string> YFormatter { get; set; }
        public Func<double, string> LengthFormatter { get; set; }
        public Func<double, string> TimeFormatter { get; set; }

        public SimpleDataPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SearchListView.Visibility = Visibility.Collapsed;
            RideChart.Visibility = Visibility.Collapsed;
            OnedayChart.Visibility = Visibility.Collapsed;
            BackgroundStackPanel.Visibility = Visibility.Collapsed;
            NoResultStackPanel.Visibility = Visibility.Collapsed;
            SearchListView.Items.Clear();
            bool searchResult = DataHelper.SearchData("BikePnts", "BikeID = '" + SearchTextBox.Text + "'", "SearchTemp");
            if (searchResult == true)
            {
                Fields Flds = null;
                Field Fld = null;
                long ID = 0;
                Record Rcd = null;

                //变量初始化
                Rcd = new Record();
                Flds = new Fields();

                Server svr = new Server();
                //连接数据源
                svr.Connect("MapGISLocal", "", "");
                DataBase GDB = svr.OpenGDB("Templates");
                SFeatureCls SFCls = new SFeatureCls(GDB);
                SFCls.Open("SearchTemp", 0);
                //获取属性结构
                Flds = SFCls.Fields;
                if (Flds == null)
                {
                    SFCls.Close();
                    OnedayChart.Visibility = Visibility.Collapsed;
                    RideChart.Visibility = Visibility.Collapsed;
                    SearchListView.Visibility = Visibility.Collapsed;
                    BackgroundStackPanel.Visibility = Visibility.Collapsed;
                    NoResultStackPanel.Visibility = Visibility.Visible;
                    return;
                }
                int num = Flds.Count;

                //目的类对象的个数
                int objnum = SFCls.Count;

                //获取所有对象的ID，思想是根据对象的个数进行循环，若OID不存在，则OID自加继续循环直到循环objnum次
                int n = 0;
                ID = 1;

                string[] listItem = new string[8];
                while (n < objnum)
                {
                    //取得ID=ID.Int的简单要素的属性  
                    Rcd = SFCls.GetAtt(ID);

                    //取得属性结构对象中的字段数目
                    if (Rcd != null)
                    {
                        Flds = Rcd.Fields;

                        listItem[0] = ID.ToString();

                        //获取对应属性字段的值
                        for (int i = 0; i < num; i++)
                        {
                            object val = null;
                            Fld = Flds.GetItem(i);
                            string name = Fld.FieldName;
                            val = Rcd.get_FldVal(name);
                            listItem[i + 1] = ((val != null) ? val.ToString() : "");
                        }
                        n++;
                        SearchListView.Items.Add(new { OID = listItem[0], BikeID = listItem[3], Time = listItem[4].Substring(9), XAsis = listItem[6].Substring(0, 8), YAsis = listItem[7].Substring(0, 8) });
                    }
                    ID++;
                }
                SFCls.Close();
                SearchListView.Visibility = Visibility.Visible;
            }
            else
            {
                OnedayChart.Visibility = Visibility.Collapsed;
                RideChart.Visibility = Visibility.Collapsed;
                SearchListView.Visibility = Visibility.Collapsed;
                BackgroundStackPanel.Visibility = Visibility.Collapsed;
                NoResultStackPanel.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 一天骑行量统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnedayChart.Visibility = Visibility.Visible;
            RideChart.Visibility = Visibility.Collapsed;
            SearchListView.Visibility = Visibility.Collapsed;
            BackgroundStackPanel.Visibility = Visibility.Collapsed;
            NoResultStackPanel.Visibility = Visibility.Collapsed;

            Labels = new[] { "0:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" };
            YFormatter = value => value.ToString() + " 辆";

            double[] data = ChartHelper.GetOneDayData("BikePnts");
            ChartValues<double> chartData = new ChartValues<double>();
            foreach (double item in data)
            {
                chartData.Add(item);
            }
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "骑行量",
                    Values = chartData
                }
            };

            OnedayChart.DataContext = this;
        }

        /// <summary>
        /// 骑行距离/时间统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            RideChart.Visibility = Visibility.Visible;
            OnedayChart.Visibility = Visibility.Collapsed;
            SearchListView.Visibility = Visibility.Collapsed;
            BackgroundStackPanel.Visibility = Visibility.Collapsed;
            NoResultStackPanel.Visibility = Visibility.Collapsed;
            Labels2 = new string[] { };
            SeriesCollection2 = new SeriesCollection
            {
                new RowSeries
                {
                    Title = "骑行距离",
                    Values = lengthData
                },
                new RowSeries
                {
                    Title = "骑行时间",
                    Values = timeData
                }
            };
            LengthFormatter = value => value.ToString("0.000") + "米";
            TimeFormatter = value => value.ToString("0.000") + "min";
            RideChart.DataContext = this;
        }

        /// <summary>
        /// 添加分析的车牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PUButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(BikeIDTextBox.Text, "^[\\w]+$"))
            {
                PUMessageBox.ShowDialog("车牌格式错误，只能包含字母和数字", "错误");
                return;
            }
            List<BikeSearchViewModel> bikeSearchViewModels = ChartHelper.GetLengthNTimeData("BikePnts", BikeIDTextBox.Text);
            if (bikeSearchViewModels == null)
            {
                PUMessageBox.ShowDialog("没有找到车牌 " + BikeIDTextBox.Text + " 的记录", "无结果");
                BikeIDTextBox.Text = "";
                return;
            }
            //获得到了一辆车的信息，接下来遍历这个集合，统计使用时间和距离，然后添加到图表的横纵坐标里面
            double length = 0;
            double time = 0;
            for (int i = 0; i < bikeSearchViewModels.Count - 1; i++)
            {
                double deltaX = bikeSearchViewModels[i + 1].XAsis - bikeSearchViewModels[i].XAsis;
                double deltaY = bikeSearchViewModels[i + 1].YAsis - bikeSearchViewModels[i].YAsis;
                if (deltaX > 0.0001 || deltaX < -0.0001 || deltaY > 0.0001 || deltaY < -0.0001)
                {
                    length += DataHelper.GetDistance(bikeSearchViewModels[i].XAsis, bikeSearchViewModels[i].YAsis, bikeSearchViewModels[i + 1].XAsis, bikeSearchViewModels[i + 1].YAsis);
                    time += Math.Abs((bikeSearchViewModels[i + 1].Time - bikeSearchViewModels[i].Time).TotalMinutes);
                }
            }

            AnalyzListView.Items.Add(BikeIDTextBox.Text);

            lengthData.Add(length);
            timeData.Add(time);
            List<string> tmpList = RideAxis.Labels.ToList();
            tmpList.Add(BikeIDTextBox.Text);
            RideAxis.Labels = tmpList;
            BikeIDTextBox.Text = "";
        }

        /// <summary>
        /// 高级搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PUButton_Click_1(object sender, RoutedEventArgs e)
        {
            var window1 = new AdvanceSearchWindow();
            window1.ShowDialog();
        }
    }
}
