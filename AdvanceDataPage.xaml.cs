using LiveCharts;
using LiveCharts.Defaults;
using MapGIS_WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MapGIS_WPF
{
    /// <summary>
    /// AdvanceDataPage.xaml 的交互逻辑
    /// </summary>
    public partial class AdvanceDataPage : Page
    {
        public Dictionary<string, double> Values { get; set; }
        public Dictionary<string, string> LanguagePack { get; set; }
        public ChartValues<HeatPoint> Data { get; set; }
        public string[] xAsis { get; set; }
        public string[] yAsis { get; set; }

        public AdvanceDataPage()
        {
            InitializeComponent();
            var r = new Random();
            double[] results = ChartHelper.GetGeoHeatMapData("BikePnts");
            Data = new ChartValues<HeatPoint>();
            int index = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Data.Add(new HeatPoint(i, j, results[index++]));
                }
            }

            xAsis = new[]
            {
                "113.750",
                "113.875",
                "114.000",
                "114.125",
                "114.250",
                "114.375",
                "114.500",
                "114.625",
                "114.750"
            };

            yAsis = new[]
            {
                "31.000",
                "30.875",
                "30.750",
                "30.625",
                "30.500",
                "30.375",
                "30.250",
                "30.125",
                "30.000"
            };

            DataContext = this;
        }
    }
}
