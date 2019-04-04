using MapGIS.GeoDataBase;
using MapGIS.GeoMap;
using MapGIS.GeoObjects;
using MapGIS.GeoObjects.Att;
using MapGIS.GeoObjects.Geometry;
using MapGIS.GeoObjects.Info;
using MapGIS.UI.Controls;
using Panuon.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapGIS_WPF
{
    /// <summary>
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        ObservableCollection<string> xCollection = new ObservableCollection<string>();
        ObservableCollection<string> dbCollection = new ObservableCollection<string>();
        //自定义绘制对象
        Display disp = null;
        //图形参数信息
        PntInfo pntInfo = null;     //点图形参数信息
        UserDrawGeoInfo userDrawGeoInfo = null; //用户自定义图形信息


        public HomePage()
        {
            InitializeComponent();
            ShowAllClasses();
            ShowAllDatabases();
        }

        /// <summary>
        /// 交互绘制子图
        /// </summary>
        /// <param name="e">逻辑点坐标</param>
        public void DrawPoint(Dot dot)
        {
            //开始绘制
            disp.Begin();
            disp.SystemLib = SystemLibrarys.GetSystemLibrarys().GetSystemLibrary(1).Guid;
            bool draw = disp.DispPoint(dot, pntInfo);
            disp.End();
        }

        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] paths = null;
            var openFileDialog = new Microsoft.Win32.OpenFileDialog() { Filter = "Excel Files (*.csv)|*.csv", Multiselect = true };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                paths = openFileDialog.FileNames;
            }
            else
            {
                return;
            }

            for (int i = 0; i < paths.Length; i++)
            {
                DataTable csv = FileHelper.OpenCSV(paths[i], new string[] { "时间", "车牌", "x 坐标", "y 坐标" },0);
                if (csv == null)
                {
                    return;
                }
                //DataHelper.CreateXClass("BikePoint", GeomType.Pnt, "Templates", new string[] { "FieldID", "BikeID", "time" });
                bool done = DataHelper.ImportSFCLSData(csv, "BikePnts");
                csv.Dispose();
                csv = null;
                if (done)
                {
                    PUMessageBox.ShowDialog("导入文件 " + paths[i] + " 完成", "我好了");
                }
                else
                {
                    if (PUMessageBox.ShowConfirm("导入文件 " + paths[i] + " 失败，要继续吗？") == true)
                    {
                        continue;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            WuhanMapControl.Dock = DockStyle.Fill;
            WuhanMapControl.BackColor = System.Drawing.Color.White;

            //ShowMaps();
        }

        /// <summary>
        /// 查看所有要素类
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllClasses()
        {
            SFCListView.ItemsSource = null;
            xCollection.Clear();
            List<string> xlist = DataHelper.GetXClasses("GDBP://MapGISLocal/Templates");
            foreach (var item in xlist)
            {
                xCollection.Add(item);
            }
            SFCListView.ItemsSource = xCollection;
        }

        /// <summary>
        /// 查看所有数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowAllDatabases()
        {
            DatabasesListView.ItemsSource = null;
            dbCollection.Clear();
            List<string> dbList = DataHelper.GetDatabase();
            foreach (var item in dbList)
            {
                dbCollection.Add(item);
            }
            DatabasesListView.ItemsSource = dbCollection;
        }

        /// <summary>
        /// 导入数据后显示地图
        /// </summary>
        private void ShowMaps()
        {
            MapGrid.Visibility = Visibility.Visible;
            BackgroundGrid.Visibility = Visibility.Collapsed;
            MapGIS.UI.Controls.MapWorkSpaceTree _Tree = new MapGIS.UI.Controls.MapWorkSpaceTree();
            //地图文档
            Document doc = _Tree.Document;

            if (doc.Close(false))
            {
                OpenFileDialog mapxDialog = new OpenFileDialog();
                mapxDialog.Filter = ".mapx(地图文档)|*.mapx|.map(地图文档)|*.map|.mbag(地图包)|*.mbag";
                if (mapxDialog.ShowDialog() != DialogResult.OK)
                    return;
                string mapUrl = mapxDialog.FileName;
                //打开地图文档
                doc.Open(mapUrl);
            }

            Maps maps = doc.GetMaps();
            if (maps.Count > 0)
            {
                //获取当前第一个地图
                Map map = maps.GetMap(0);
                //设置地图的第一个图层为激活状态
                map.get_Layer(0).State = LayerState.Active;
                this.WuhanMapControl.ActiveMap = map;
                this.WuhanMapControl.Restore();
            }
            else
            {
                return;
            }

            disp = WuhanMapControl.Display;
            userDrawGeoInfo = disp.GetUserDrawGeoInfo();
            pntInfo = userDrawGeoInfo.GetPntInfo();

            EditGeomInfoForm pntInfoForm = new EditGeomInfoForm((VectorLayer)null, pntInfo);

            if (pntInfoForm.ShowDialog() == DialogResult.OK)
            {

            }
            pntInfoForm.Dispose();
            //画点
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
            SFCls.Open("BikePnts", 0);
            //获取属性结构
            Flds = SFCls.Fields;
            if (Flds == null)
            {
                SFCls.Close();
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
                    Dot dot = new Dot();
                    dot.X = Convert.ToDouble(listItem[6]);
                    dot.Y = Convert.ToDouble(listItem[7]);
                    DrawPoint(dot);
                }
                ID++;
            }
            SFCls.Close();
        }

    }
}
