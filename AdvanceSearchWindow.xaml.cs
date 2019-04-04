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
using MapGIS.GeoDataBase;
using MapGIS.GeoObjects.Att;
using Panuon.UI;

namespace MapGIS_WPF
{
    /// <summary>
    /// AdvanceSearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AdvanceSearchWindow : PUWindow
    {
        private List<string> FldsNames = null;

        public AdvanceSearchWindow()
        {
            InitializeComponent();
            ShowFlds("BikePnts");
        }

        /// <summary>
        /// 获取类的所有字段显示到列表中
        /// </summary>
        /// <param name="className"></param>
        private void ShowFlds(string className)
        {
            AttrListView.Items.Clear();
            FldsNames = new List<string>();

            //打开选中的简单要素类
            Server svr = new Server();
            svr.Connect("MapGISLocal", "", "");
            DataBase GDB = svr.OpenGDB("Templates");
            SFeatureCls SFCls = new SFeatureCls(GDB);
            SFCls.Open(className, 0);

            Fields Flds = null;   //获取属性结构
            Field Fld = null;   //获取属性字段信息

            Fld = new Field();
            Flds = new Fields();

            //直接取它的属性结构
            Flds = SFCls.Fields;
            if (Flds == null) return;

            //可以查看属性结构对象中的字段数目
            int cou = Flds.Count;

            //获取属性字段
            int i = 0;
            while (i < cou)
            {
                Fld = Flds.GetItem(i);
                AttrListView.Items.Add(Fld.FieldName + " (" + GetFieldTypeText(Fld.FieldType) + ")");
                FldsNames.Add(Fld.FieldName);
                i++;
            }
            SFCls.Close();
        }

        /// <summary>
        /// 获取属性字段类型的名称
        /// </summary>
        /// <param name="FldType">字段类型</param>
        /// <returns>名称</returns>
        public string GetFieldTypeText(FieldType FldType)
        {
            string fieldName = "";
            //设置属性字段             
            switch (FldType)
            {
                case FieldType.FldBinary:
                    fieldName = "定长二进制型";
                    break;
                case FieldType.FldBlob:
                    fieldName = "二进制大对象型";
                    break;
                case FieldType.FldBool:
                    fieldName = "布尔型";
                    break;
                case FieldType.FldByte:
                    fieldName = "字节型";
                    break;
                case FieldType.FldDate:
                    fieldName = "日期型";
                    break;
                case FieldType.FldDouble:
                    fieldName = "双精度型";
                    break;
                case FieldType.FldExt:
                    fieldName = "扩展型";
                    break;
                case FieldType.FldFloat:
                    fieldName = "浮点型";
                    break;
                case FieldType.FldIPAddress:
                    fieldName = "IP地址型";
                    break;
                case FieldType.FldInt64:
                    fieldName = "64位整型";
                    break;
                case FieldType.FldLong:
                    fieldName = "长整型";
                    break;
                case FieldType.FldMap:
                    fieldName = "地图型";
                    break;
                case FieldType.FldNumberic:
                    fieldName = "数值型";
                    break;
                case FieldType.FldPicture:
                    fieldName = "图片型";
                    break;
                case FieldType.FldShort:
                    fieldName = "短整型";
                    break;
                case FieldType.FldSound:
                    fieldName = "声音型";
                    break;
                case FieldType.FldString:
                    fieldName = "字符串型";
                    break;
                case FieldType.FldTable:
                    fieldName = "表格型";
                    break;
                case FieldType.FldText:
                    fieldName = "文本型";
                    break;
                case FieldType.FldTime:
                    fieldName = "时间型";
                    break;
                case FieldType.FldTimeStamp:
                    fieldName = "邮戳型";
                    break;
                case FieldType.FldUnknown:
                    fieldName = "未知型";
                    break;
                case FieldType.FldVideo:
                    fieldName = "视频型";
                    break;
                default:
                    break;
            }
            return fieldName;
        }

        #region 按键

        private void PUButton_Click(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "+");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_1(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "-");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_2(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "*");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_3(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "/");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_4(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "()");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_5(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "%");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_6(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "=");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_7(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "!=");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 2, 0);
        }

        private void PUButton_Click_8(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, ">");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_9(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "<");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 1, 0);
        }

        private void PUButton_Click_10(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, ">=");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 2, 0);
        }

        private void PUButton_Click_11(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, "<=");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 2, 0);
        }

        private void PUButton_Click_12(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " LIKE ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 6, 0);
        }

        private void PUButton_Click_13(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " AND ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 5, 0);
        }

        private void PUButton_Click_14(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " OR ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 4, 0);
        }

        private void PUButton_Click_15(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " NOT ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 5, 0);
        }

        private void PUButton_Click_16(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " IS ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 4, 0);
        }

        private void PUButton_Click_17(object sender, RoutedEventArgs e)
        {
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            s = s.Insert(i, " NULL ");
            SQLTextBox.Text = s;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + 6, 0);
        }

        #endregion

        /// <summary>
        /// 使用函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PUComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 0;
            string s = "";
            switch (FuncComboBox.SelectedIndex)
            {
                case -1:
                    break;
                case 0:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " abs() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 5, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 1:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Max() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 5, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 2:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Min() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 5, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 3:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Round() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 7, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 4:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Lower() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 7, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 5:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Upper() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 7, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                case 6:
                    i = SQLTextBox.SelectionStart;
                    s = SQLTextBox.Text;
                    s = s.Insert(i, " Substr() ");
                    SQLTextBox.Text = s;
                    SQLTextBox.Focus();
                    SQLTextBox.Select(i + 8, 0);
                    FuncComboBox.SelectedIndex = -1;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 选择字段名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttrListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AttrListView.SelectedIndex == -1)
            {
                return;
            }
            int i = SQLTextBox.SelectionStart;
            string s = SQLTextBox.Text;
            string name = FldsNames[AttrListView.SelectedIndex];
            s = s.Insert(i, name);
            SQLTextBox.Text = s;
            AttrListView.SelectedIndex = -1;
            SQLTextBox.Focus();
            SQLTextBox.Select(i + name.Length, 0);
        }

        /// <summary>
        /// SQL查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PUButton_Click_18(object sender, RoutedEventArgs e)
        {
            bool searchResult = DataHelper.SearchData("BikePnts", SQLTextBox.Text, "SQLSearchResult");
            SQLTextBox.Text = "";
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
                SFCls.Open("SQLSearchResult", 0);
                //获取属性结构
                Flds = SFCls.Fields;
                if (Flds == null)
                {
                    SFCls.Close();
                    SearchTabControl.Visibility = Visibility.Collapsed;
                    SearchResultGrid.Visibility = Visibility.Collapsed;
                    NoResultGrid.Visibility = Visibility.Visible;
                    return;
                }
                int num = Flds.Count;

                //目的类对象的个数
                int objnum = SFCls.Count;

                //获取所有对象的ID，思想是根据对象的个数进行循环，若OID不存在，则OID自加继续循环直到循环objnum次
                int n = 0;
                ID = 1;

                string[] listItem = new string[8];
                SearchListView.Items.Clear();

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
                        SearchListView.Items.Add(new { BikeID = listItem[3], Time = listItem[4].Substring(9), XAsis = listItem[6].Substring(0, 8), YAsis = listItem[7].Substring(0, 8) });
                    }
                    ID++;
                }
                SFCls.Close();
                SearchTabControl.Visibility = Visibility.Collapsed;
                SearchResultGrid.Visibility = Visibility.Visible;
                NoResultGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchTabControl.Visibility = Visibility.Collapsed;
                SearchListView.Visibility = Visibility.Collapsed;
                NoResultGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// 矩形查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PUButton_Click_19(object sender, RoutedEventArgs e)
        {
            if (!(Regex.IsMatch(XMaxTextBox.Text, "^[\\d.]+$") && Regex.IsMatch(XMinTextBox.Text, "^[\\d.]+$") && Regex.IsMatch(YMaxTextBox.Text, "^[\\d.]+$") && Regex.IsMatch(YMinTextBox.Text, "^[\\d.]+$")))
            {
                PUMessageBox.ShowDialog("输入的内容不符合规范");
                return;
            }
            string sql = "xAsis < " + XMaxTextBox.Text + " AND xAsis > " + XMinTextBox.Text + " AND yAsis < " + YMaxTextBox.Text + " AND yAsis > " + YMinTextBox.Text;
            bool searchResult = DataHelper.SearchData("BikePnts", sql, "RectangleSearchResult");
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
                SFCls.Open("RectangleSearchResult", 0);
                //获取属性结构
                Flds = SFCls.Fields;
                if (Flds == null)
                {
                    SFCls.Close();
                    SearchTabControl.Visibility = Visibility.Collapsed;
                    SearchResultGrid.Visibility = Visibility.Collapsed;
                    NoResultGrid.Visibility = Visibility.Visible;
                    return;
                }
                int num = Flds.Count;

                //目的类对象的个数
                int objnum = SFCls.Count;

                //获取所有对象的ID，思想是根据对象的个数进行循环，若OID不存在，则OID自加继续循环直到循环objnum次
                int n = 0;
                ID = 1;

                string[] listItem = new string[8];
                SearchListView.Items.Clear();
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
                        SearchListView.Items.Add(new { BikeID = listItem[3], Time = listItem[4].Substring(9), XAsis = listItem[6].Substring(0, 8), YAsis = listItem[7].Substring(0, 8) });
                    }
                    ID++;
                }
                SFCls.Close();
                SearchTabControl.Visibility = Visibility.Collapsed;
                SearchResultGrid.Visibility = Visibility.Visible;
                NoResultGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchTabControl.Visibility = Visibility.Collapsed;
                SearchResultGrid.Visibility = Visibility.Collapsed;
                NoResultGrid.Visibility = Visibility.Visible;
            }
        }

        private void PUButton_Click_20(object sender, RoutedEventArgs e)
        {
            NoResultGrid.Visibility = Visibility.Collapsed;
            SearchTabControl.Visibility = Visibility.Visible;
        }

        private void PUButton_Click_21(object sender, RoutedEventArgs e)
        {
            SearchResultGrid.Visibility = Visibility.Collapsed;
            SearchTabControl.Visibility = Visibility.Visible;
        }
    }
}
