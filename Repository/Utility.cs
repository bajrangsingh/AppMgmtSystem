
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ApprovalPortal.Models;

namespace ApprovalPortal.Repository
{
    public static class Utility
    {
        public static void GetResultForSuccess(Result rs, int noOfRowUpdated)
        {
            rs.ReturnVal = Convert.ToString(noOfRowUpdated);
            rs.RowsEffected = noOfRowUpdated;
            rs.Sucessful = 1;
        }

        public static void GetResultForException(Result rs, Exception ex)
        {
            rs.Sucessful = 0;
            rs.exception = ex;
            rs.RowsEffected = 0;
            rs.ReturnVal = ex.StackTrace; rs.ShortMsg = ex.Message;
        }

        //public static DataSet getDataFromDB(string SQLQuery, CommandType commandType)
        //{
        //    System.Configuration _configuration = new Configuration();;

        //    string connectionString = _configuration.GetConnectionString("PMSNew");

        //    DataSet ds = new DataSet();

        //    using (SqlConnection con = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand(SQLQuery, con))
        //        {
        //            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
        //            {
        //                cmd.CommandType = commandType;
        //                con.Open();
        //                sda.Fill(ds);
        //                con.Close();
        //            }
        //        }
        //    }

        //    return ds;
        //}
        public static DataTable ToDataTable<T>(IList<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            List<T> lstItems = new List<T>();
            if (dt != null && dt.Rows.Count > 0)
                foreach (DataRow row in dt.Rows)
                    lstItems.Add(ConvertDataRowToGenericType<T>(row));
            else
                lstItems = null;
            return lstItems;
        }

        private static T ConvertDataRowToGenericType<T>(DataRow row) where T : class,new()
        {
            Type entityType = typeof(T);
            T objEntity = new T();
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row[column.ColumnName];
                if (value == DBNull.Value) value = null;
                PropertyInfo property = entityType.GetProperty(column.ColumnName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                try
                {
                    if (property != null && property.CanWrite)
                        property.SetValue(objEntity, value, null);

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return objEntity;
        }
    }
}
