using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGIS.GeoDataBase;
using MapGIS.GeoObjects;
using MapGIS.GeoObjects.Att;
using MapGIS.GeoObjects.Geometry;
using MapGIS.GeoObjects.Info;
using Panuon.UI;

namespace MapGIS_WPF
{
    public static class DataHelper
    {
        /// <summary>
        /// 获取简单要素类列表
        /// </summary>
        /// <param name="url">要打开的地理数据库的url</param>
        /// <returns></returns>
        public static List<string> GetXClasses(string url)
        {
            List<string> names = new List<string>();
            Server Svr = new Server();
            Svr.Connect("MapGISLocal", "", "");
            DataBase GDB = Svr.OpenGDB("Templates");

            int count;

            List<int> dsIDs = null; //要素数据集ID列表
            List<int> sfIDs = null; //要素数据集中矢量类的ID列表
            List<int> dbSFIDs = null; //数据库中矢量类的ID列表

            List<string> allnames = new List<string>();

            //先查找要素数据集中的矢量类信息
            if (true)
            {
                dsIDs = GDB.GetXclses(XClsType.Fds, 0);

                if (dsIDs != null)
                {
                    count = dsIDs.Count;

                    while (count > 0)
                    {
                        sfIDs = GDB.GetXclses(XClsType.SFCls, dsIDs[count - 1]);
                        if (sfIDs == null) break;

                        int n = sfIDs.Count;
                        while (n > 0)
                        {
                            string ClsName = GDB.GetXclsName(XClsType.SFCls, sfIDs[n - 1]);
                            string FdsName = GDB.GetXclsName(XClsType.Fds, dsIDs[count - 1]);
                            allnames.Add(sfIDs[n - 1].ToString() + ". " + ClsName);

                            allnames.Add(dsIDs[count - 1].ToString() + ". " + FdsName);
                            n--;
                        }
                        count--;
                    }
                }
            }

            //再查找数据库中的矢量类,如果查询的是要素数据集，则只需要执行下面语句就可以了
            dbSFIDs = GDB.GetXclses(XClsType.SFCls, 0);
            if (dbSFIDs != null)
            {
                count = dbSFIDs.Count;
                while (count > 0)
                {
                    string SFName = GDB.GetXclsName(XClsType.SFCls, dbSFIDs[count - 1]);
                    allnames.Add(dbSFIDs[count - 1].ToString() + " " + SFName);

                    //如果是简单要素类，则添加其几何类型
                    if (true)
                    {
                        SFeatureCls sfcls = new SFeatureCls(GDB);
                        sfcls.Open(SFName, 0);
                        sfcls.Close();
                    }
                    count--;
                }
            }
            return allnames;
        }

        /// <summary>
        /// 获取数据库列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDatabase()
        {
            //定义数据源、数据库
            Server Svr = new Server();
            DataBase GDB = new DataBase();

            List<string> names = new List<string>();
            List<int> dbIDs = new List<int>();

            //连接数据源
            Svr.Connect("MapGISLocal", "", "");

            dbIDs = Svr.GDBIDs;
            int count = dbIDs.Count;
            int i = 0;

            //获取数据源下的所有数据库的ID和NAME
            while (i < count)
            {
                int id = dbIDs[i];
                string name = Svr.GetDBName(id);
                names.Add(id.ToString() + ". " + name + ": " + Svr.SvrName);
                i++;
            }

            return names;
        }

        /// <summary>
        /// 创建地理数据库
        /// </summary>
        /// <param name="databasePath">存储路径</param>
        /// <param name="gdbName">数据库名</param>
        /// <param name="gdbUser">账号</param>
        /// <param name="gdbPassword">密码</param>
        /// <returns></returns>
        public static bool CreateDataBase(string databasePath, string gdbName, string gdbUser, string gdbPassword)
        {
            Server svr = new Server();  //地理数据库服务器
            bool rtn = svr.Connect("Mapgislocal", "", "");
            if (rtn == true)
            {
                //日志事件触发器
                LogEventReceiver LogER = new LogEventReceiver();

                //地理数据库创建信息对象
                GDBCreateParam createParam = new GDBCreateParam();
                //数据库文件信息对象
                DBFileInfo fileInfo = new DBFileInfo();
                //数据库文件信息列表对象
                List<DBFileInfo> ListDB = new List<DBFileInfo>();
                //数据库文件扩展信息对象
                FileExtendInfo extendInfo = new FileExtendInfo
                {
                    //设置数据库文件扩展信息
                    ExtendMode = FileExtendMode.Size, //数据增长类型
                    ExtendSize = 1,                   //数据文件增长大小
                    ExtendUnit = FileExtendUnit.Mbyte, //文件增长单位
                    IsExtendable = true, //是否实现自增长
                    MaxFileSize = 0   //数据库最大容量
                };

                //设置数据库文件信息
                fileInfo.ExtendInfo = extendInfo;  //数据库文件扩展信息
                fileInfo.FilePath = databasePath; //数据库路径  @"C:\Users\姚一鸣\Desktop\test.HDF"
                fileInfo.InitSize = 20; //数据库初始大小

                //数据库文件信息列表
                ListDB.Add(fileInfo);

                createParam.GDBName = gdbName; //地理数据库的名称  "test"
                createParam.GDBOwner = gdbUser;  //用户名称  ""
                createParam.OwnerPsw = gdbPassword;  //用户密码  ""
                createParam.DataFileInfos = ListDB; //数据库文件信息列表
                createParam.IndexFileInfo = fileInfo; //数据库文件信息

                //创建数据库
                int i = svr.CreateGDB(createParam, LogER);
                svr.DisConnect();
                if (i > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 创建简单要素类
        /// </summary>
        /// <param name="layerName">要素类名</param>
        /// <param name="geomType">几何类型，枚举</param>
        /// <param name="gdbName">打开的地理数据库名</param>
        public static bool CreateXClass(string layerName, GeomType geomType, string gdbName, string[] fields)
        {
            //方法：指定类型、数据集ID，空间参考系，创建类
            Server svr = new Server();
            SFeatureCls sfcls = null;
            //连接数据源，打开数据库
            bool rtn = svr.Connect("MapGISLocal", "", "");
            if (rtn == true)
            {
                DataBase gdb = svr.OpenGDB(gdbName);
                //打开简单要素类
                sfcls = gdb.GetXClass(XClsType.SFCls) as SFeatureCls;
                //创建区类型的简单要素类        
                int id = sfcls.Create(layerName, geomType, 0, 0, null);
                if (id <= 0)
                {
                    //关闭类、数据库、断开数据源
                    sfcls.Close();
                    gdb.Close();
                    svr.DisConnect();
                    return false;
                }

                sfcls.Open(layerName, 0);
                Fields temp = sfcls.Fields;
                if (temp == null)
                {
                    temp = new Fields();
                }
                //Field adding = null;

                #region 添加属性

                //1: successful  0: failed
                int result = temp.AppendField(new Field
                {
                    FieldName = "DataID",
                    FieldLength = 255,
                    FieldType = FieldType.FldString,
                    Editable = 1,
                    MskLength = 15
                });

                result = temp.AppendField(new Field
                {
                    FieldName = "BikeID",
                    FieldLength = 255,
                    FieldType = FieldType.FldString,
                    Editable = 1,
                    MskLength = 15
                });

                result = temp.AppendField(new Field
                {
                    FieldName = "ParkTime",
                    FieldType = FieldType.FldTime,
                    Editable = 1,
                    FieldLength = 10,
                    MskLength = 10
                });

                result = temp.AppendField(new Field
                {
                    FieldName = "Date",
                    FieldType = FieldType.FldTime,
                    Editable = 1,
                    FieldLength = 10,
                    MskLength = 10
                });

                result = temp.AppendField(new Field
                {
                    FieldName = "xAsis",
                    FieldLength = 255,
                    FieldType = FieldType.FldDouble,
                    Editable = 1,
                    MskLength = 15
                });

                result = temp.AppendField(new Field
                {
                    FieldName = "yAsis",
                    FieldLength = 255,
                    FieldType = FieldType.FldDouble,
                    Editable = 1,
                    MskLength = 15
                });

                #endregion

                //foreach (var item in fields)
                //{
                //    adding = new Field
                //    {
                //        FieldName = item,
                //        FieldLength = 255,
                //        FieldType = FieldType.FldString,
                //        Editable = 1,
                //        MskLength = 15
                //    };
                //    int result = temp.AppendField(adding);  //1: successful  0: failed
                //}
                sfcls.Fields = temp;

                //关闭类、数据库、断开数据源
                sfcls.Close();
                gdb.Close();
                svr.DisConnect();

                if (id > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 向数据库添加点数据
        /// </summary>
        public static bool ImportSFCLSData(DataTable dataTable, string className)
        {
            DataBase gdb = DataBase.OpenByURL("GDBP://MapGisLocal/Templates");
            //打开简单要素类
            SFeatureCls sfcls = gdb.GetXClass(XClsType.SFCls) as SFeatureCls;
            bool op = sfcls.Open(className, 0);

            GeoPoints pnts = null;
            Dot3D newdot = new Dot3D();
            //PntInfo pntInfo = new PntInfo();

            int i = 0;
            foreach (DataRow dr in dataTable.Rows)
            {
                double.TryParse(dr["x 坐标"].ToString(), out double x);
                double.TryParse(dr["y 坐标"].ToString(), out double y);
                //点要素坐标
                newdot.X = y;
                newdot.Y = x;
                newdot.Z = 0;
                pnts = new GeoPoints();
                pnts.Append(newdot);

                Record rcd = new Record();
                Fields Flds = sfcls.Fields;
                if (Flds == null)
                {
                    return false;
                }

                PntInfo pntInfo = new PntInfo();
                //点参数
                pntInfo.LibID = 0;
                pntInfo.SymID = 1;
                pntInfo.Width = 10;
                pntInfo.Height = 10;
        

                rcd.Fields = Flds;

                //设置属性字段的值
                rcd.set_FldVal(1, (i++).ToString());
                rcd.set_FldVal(2, dr["车牌"].ToString());
                string time = dr["时间"].ToString().Replace('T', ' ');
                rcd.set_FldVal(3, time);
                rcd.set_FldVal(4, time);
                rcd.set_FldVal(5, x);
                rcd.set_FldVal(6, y);

                //添加点要素
                long oid = sfcls.Append(pnts, rcd, pntInfo);
            }

            Debug.WriteLine("Done.");
            return true;
        }

        /// <summary>
        /// 查询数据，将查询到的结果放到新建的 temp 类中
        /// </summary>
        /// <param name="className">要查询的类的名字</param>
        /// <param name="search">查询语句</param>
        /// <param name="tempClass">要创建的临时类</param>
        /// <returns></returns>
        public static bool SearchData(string className, string search, string tempClass)
        {
            //定义变量
            IVectorCls VectorCls = new SFeatureCls();
            //打开简单要素类
            bool rtn = VectorCls.Open("GDBP://MapGisLocal/Templates/sfcls/" + className);
            if (!rtn)
            {
                PUMessageBox.ShowDialog("简单要素类 " + className + " 打开失败", "失败");
                return false;
            }

            QueryDef def = new QueryDef();
            //设置属性查询条件
            def.Filter = search;
            //查询要素
            RecordSet recordSet = VectorCls.Select(def);
            if (recordSet != null)
            {
                int num = recordSet.Count;
            }
            else
            {
                return false;
            }

            Server svr = new Server();
            //连接数据源
            svr.Connect("MapGISLocal", "", "");
            DataBase GDB = svr.OpenGDB("templates");
            SFeatureCls tmpSFCls = new SFeatureCls(GDB);
            int id = tmpSFCls.Create(tempClass, GeomType.Pnt, 0, 0, null);

            if (id == 0)
            {
                bool temp = SFeatureCls.Remove(GDB, tempClass);
                id = tmpSFCls.Create(tempClass, GeomType.Pnt, 0, 0, null);
                if (id == 0)
                {
                    PUMessageBox.ShowDialog("无法操作简单要素类，请检查是否有其他进程正在使用 " + tempClass, "失败");
                    return false;
                }
            }

            rtn = tmpSFCls.CopySet(recordSet);
            if (rtn == false)
            {
                tmpSFCls.Close();
                SFeatureCls.Remove(GDB, id);
            }
            //关闭类
            VectorCls.Close();
            return true;
        }


        //地球半径，单位米
        private const double EARTH_RADIUS = 6378137;
        /// <summary>
        /// 计算两点位置的距离，返回两点的距离，单位 米
        /// 该公式为GOOGLE提供，误差小于0.2米
        /// </summary>
        /// <param name="lat1">第一点纬度</param>
        /// <param name="lng1">第一点经度</param>
        /// <param name="lat2">第二点纬度</param>
        /// <param name="lng2">第二点经度</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = Rad(lat1);
            double radLng1 = Rad(lng1);
            double radLat2 = Rad(lat2);
            double radLng2 = Rad(lng2);
            double a = radLat1 - radLat2;
            double b = radLng1 - radLng2;
            double result = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))) * EARTH_RADIUS;
            return result;
        }

        /// <summary>
        /// 经纬度转化成弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double Rad(double d)
        {
            return (double)d * Math.PI / 180d;
        }

    }
}
