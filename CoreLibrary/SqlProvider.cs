using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;
using System.Configuration;
using BlueMoon.MVC.Controls;
using System.IO;

namespace BlueMoon.Business
{
    public class DataItem : Dictionary<string, object>
    {
        public new object this[string key]
        {
            get
            {
                if (base.ContainsKey(key)) return base[key];
                else return null;
            }
            set
            {
                if (base.ContainsKey(key)) base[key] = value;
                else base.Add(key, value);
            }
        }
        public void Copy(DataItem source)
        {
            foreach (var k in source.Keys)
            {
                this[k] = source[k];
            }

        }
    }
    public class ObjectParameter : Dictionary<string, object>
    {
        public ObjectParameter()
        {

        }
    }
    public class SqlProvider
    {
        static SqlProvider s_Default = new SqlProvider();

        public static SqlProvider Default
        {
            get
            {
                return s_Default;
            }
        }

        string _conStr = null;
        protected SqlProvider()
            : this(ConfigurationManager.ConnectionStrings[0].ConnectionString)
        {

        }
        public SqlProvider(string conStr)
        {
            _conStr = conStr;
        }
        object BaseExecute(CommandType commandType, string command, ObjectParameter parameters, Func<IDataReader, object> buildResult)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = command;
            cmd.CommandType = commandType;
            cmd.Connection = new SqlConnection(_conStr);
            cmd.Connection.Open();
            if (parameters != null && parameters.Count > 0)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    SqlCommandBuilder.DeriveParameters(cmd);
                    foreach (var p in parameters)
                    {
                        cmd.Parameters["@" + p.Key].Value = p.Value ?? DBNull.Value;
                    }
                }
                else
                {
                    foreach (var p in parameters)
                    {
                        cmd.Parameters.Add(new SqlParameter("@" + p.Key, p.Value));
                    }
                }
            }
            object result = null;
            if (buildResult == null)
            {
                result = cmd.ExecuteNonQuery();
            }
            else
            {
                IDataReader reader = cmd.ExecuteReader();
                result = buildResult.Invoke(reader);
            }
            
            cmd.Connection.Close();
            if (parameters != null)
            {
                foreach (var p in parameters.ToArray())
                {
                    switch (cmd.Parameters["@" + p.Key].Direction)
                    {
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.Output: parameters[p.Key] = cmd.Parameters["@" + p.Key].Value;
                            break;
                    }
                }
            }
            return result;
        }
        List<T> Execute<T>(CommandType commandType, string command, ObjectParameter parameters)
        {
            return (List<T>)BaseExecute(commandType, command, parameters, reader =>
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                return To<T>(dt);
            });
        }
        List<DataItem> Execute(CommandType commandType, string command, ObjectParameter parameters)
        {
            return (List<DataItem>)BaseExecute(commandType, command, parameters, reader =>
            {
                List<DataItem> list = new List<DataItem>();
                string fieldName = "";
                DataItem item = null;
                while (reader.Read())
                {
                    item = new DataItem();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        fieldName = reader.GetName(i);
                        item[fieldName] = reader[i];
                    }
                    list.Add(item);
                }
                if (list.Count > 0) return list;
                else return null;
            });

        }
        public int ExecuteNonQueryCmd(string query, ObjectParameter parameters = null)
        {
            return (int)BaseExecute(CommandType.Text, query, parameters, null);
        }
        public List<T> ExecuteQueryCmd<T>(string query, ObjectParameter parameters = null)
        {
            return Execute<T>(CommandType.Text, query, parameters);
        }
        public List<DataItem> ExecuteQueryCmd(string query, ObjectParameter parameters = null)
        {
            return Execute(CommandType.Text, query, parameters);
        }
        public int ExecuteNonQuerySpa(string storeName, ObjectParameter parameters = null)
        {
            return (int)BaseExecute(CommandType.StoredProcedure, storeName, parameters, null);
        }
        public List<T> ExecuteSpa<T>(string storeName, ObjectParameter parameters = null)
        {
            return Execute<T>(CommandType.StoredProcedure, storeName, parameters);
        }
        
        public List<DataItem> ExecuteSpa(string storeName, ObjectParameter parameters = null)
        {
            return Execute(CommandType.StoredProcedure, storeName, parameters);
        }
        
        

        static T To<T>(DataRow row)
        {
            Type type = typeof(T);
            var obj = (T)Activator.CreateInstance(type);
            foreach (PropertyInfo p in type.GetProperties())
            {
                if (p.CanWrite)
                {
                    if (row.Table.Columns.Contains(p.Name) && row[p.Name] != null && row[p.Name] != DBNull.Value)
                    {
                        p.SetValue(obj, row[p.Name]);
                    }
                    else
                    {
                        p.SetValue(obj, null);
                    }
                }
            }

            return obj;
        }
        static List<T> To<T>(DataTable table)
        {
            List<T> ret = new List<T>();
            if (table.Columns.Count > 1)
            {
                foreach (DataRow row in table.Rows)
                {
                    T t = To<T>(row);
                    ret.Add(t);
                }
            }
            else
            {
                Type type = typeof(T);
                foreach (DataRow row in table.Rows)
                {
                    T t = default(T);
                    if (row[0] != null && row[0] != DBNull.Value) t = (T)Convert.ChangeType(row[0], type);
                    ret.Add(t);
                }
            }
            if (ret.Count > 0) return ret;
            else return null;
        }
    }
}
