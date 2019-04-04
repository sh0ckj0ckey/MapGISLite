using MapGIS.GeoDataBase;
using MapGIS.GeoObjects;
using MapGIS.GeoObjects.Att;
using Panuon.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGIS_WPF.Helpers
{
    public static class ChartHelper
    {
        /// <summary>
        /// 获取一天的骑行量
        /// </summary>
        /// <returns></returns>
        public static double[] GetOneDayData(string className)
        {
            //0:00 1:00 ... 23:00
            double[] results = new double[24] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            //定义变量
            IVectorCls VectorCls = new SFeatureCls();
            //打开简单要素类
            bool rtn = VectorCls.Open("GDBP://MapGisLocal/Templates/sfcls/" + className);
            if (!rtn)
            {
                PUMessageBox.ShowDialog("简单要素类 " + className + " 打开失败", "失败");
                return null;
            }

            QueryDef def = new QueryDef();
            RecordSet recordSet = null;

            for (int i = 0; i < 23; i++)
            {
                //设置属性查询条件
                def.Filter = "ParkTime > '" + i + ":00:00' AND ParkTime < '" + (i + 1).ToString() + ":00:00'";
                //查询要素
                recordSet = VectorCls.Select(def);
                if (recordSet != null)
                {
                    results[i] = recordSet.Count;
                }
            }

            def.Filter = "ParkTime > '23:00:00'";
            //查询要素
            recordSet = VectorCls.Select(def);
            if (recordSet != null)
            {
                results[23] = recordSet.Count;
            }

            //关闭类
            VectorCls.Close();
            return results;

        }

        /// <summary>
        /// 获取单车的距离和时间统计
        /// </summary>
        /// <param name="bikes"></param>
        /// <returns></returns>
        public static List<BikeSearchViewModel> GetLengthNTimeData(string className, string bikeid)
        {
            List<BikeSearchViewModel> searchResult = new List<BikeSearchViewModel>();

            bool haveResult = DataHelper.SearchData("BikePnts", "BikeID = '" + bikeid + "'", bikeid + "Temp");
            if (haveResult == false)
            {
                return null;
            }
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
            SFeatureCls tmpSFCls = new SFeatureCls(GDB);

            tmpSFCls.Open(bikeid + "Temp", 0);
            //获取属性结构
            Flds = tmpSFCls.Fields;
            if (Flds == null)
            {
                tmpSFCls.Close();
                return null;
            }
            int num = Flds.Count;

            //目的类对象的个数
            int objnum = tmpSFCls.Count;

            //获取所有对象的ID，思想是根据对象的个数进行循环，若OID不存在，则OID自加继续循环直到循环objnum次
            int n = 0;
            ID = 1;

            string[] listItem = new string[8];
            while (n < objnum)
            {
                //取得ID=ID.Int的简单要素的属性  
                Rcd = tmpSFCls.GetAtt(ID);

                //取得属性结构对象中的字段数目
                if (Rcd != null)
                {
                    Flds = Rcd.Fields;

                    listItem[0] = ID.ToString();

                    //获取对应属性字段的值
                    for (int j = 0; j < num; j++)
                    {
                        object val = null;
                        Fld = Flds.GetItem(j);
                        string name = Fld.FieldName;
                        val = Rcd.get_FldVal(name);
                        listItem[j + 1] = ((val != null) ? val.ToString() : "");
                    }
                    n++;
                    BikeSearchViewModel bike = new BikeSearchViewModel
                    {
                        BikeID = listItem[3],
                        Time = Convert.ToDateTime(listItem[4].Substring(9)),
                        XAsis = Convert.ToDouble(listItem[6].Substring(0, 8)),
                        YAsis = Convert.ToDouble(listItem[7].Substring(0, 8))
                    };
                    searchResult.Add(bike);
                }
                ID++;
            }
            tmpSFCls.Close();
            return searchResult;
        }

        /// <summary>
        /// 获取热点数据
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static double[] GetGeoHeatMapData(string className)
        {
            double[] results = new double[81];
            for (int i = 0; i < 81; i++)
            {
                results[i] = 0;
            }

            //定义变量
            IVectorCls VectorCls = new SFeatureCls();
            //打开简单要素类
            bool rtn = VectorCls.Open("GDBP://MapGisLocal/Templates/sfcls/" + className);
            if (!rtn)
            {
                PUMessageBox.ShowDialog("简单要素类 " + className + " 打开失败", "失败");
                return null;
            }

            QueryDef def = new QueryDef();
            RecordSet recordSet = null;
            int index = 0;
            double xStart = 113.750;
            double yStart = 31.000;
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {

                    def.Filter = "yAsis > " + (xStart + 0.125 * x).ToString() + " AND yAsis < " + (xStart + 0.125 * x + 0.125).ToString() + " AND xAsis < " + (yStart - 0.125 * y).ToString() + " AND xAsis > " + (yStart - 0.125 * y - 0.125) + "";
                    recordSet = VectorCls.Select(def);
                    if (recordSet != null)
                    {
                        results[index++] = recordSet.Count;
                    }
                }
            }

            //关闭类
            VectorCls.Close();
            return results;
        }

    }
}
