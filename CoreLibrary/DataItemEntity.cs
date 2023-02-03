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
    public enum DataType
    {
        String,
        Int,
        DateTime,
        Bool,
        Decimal,
        Text,
        File
    }
    public class FileDataInfo
    {
        public static FileDataInfo LoadInfo(Guid fileId)
        {
            using (StreamReader rd = new StreamReader(BlueMoon.MVC.Controller.UploadFolder + fileId + ".inf"))
            {
                string fileName = rd.ReadLine();
                rd.Close();
                return new FileDataInfo(fileId, fileName);
            }
        }
        
        public Guid ID { get; private set; }
        public FileDataInfo(Guid id, string fileName)
        {
            ID = id;
            FileName = fileName;
        }
        public string FileName { get; private set; }
        public void Save()
        {
            using (StreamWriter wr = new StreamWriter(BlueMoon.MVC.Controller.UploadFolder + ID + ".inf",false))
            {
                wr.WriteLine(FileName); 
                wr.Close();
            }
        }
        public static void Delete(Guid fileId)
        {
            try
            {
                File.Delete(BlueMoon.MVC.Controller.UploadFolder + fileId + ".dat");
                File.Delete(BlueMoon.MVC.Controller.UploadFolder + fileId + ".inf");
            }
            catch { }
        }
    }
    public class DataItemEntity : DataItem
    {

        public DataItemEntity(ModelDefinition defi)
            : this(defi, SqlProvider.Default)
        { }
        public DataItemEntity(ModelDefinition defi, SqlProvider sqlProvider)
        {
            Properties = defi;
            Db = sqlProvider;
        }
        string ToSqlDataTypeString(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Bool:
                    return "bit";
                case DataType.Int:
                    return "int";
                case DataType.DateTime:
                    return "datetime";
                case DataType.Decimal:
                    return "decimal(18,2)";
                case DataType.Text:
                    return "nvarchar(max)";
                case DataType.File:
                    return "uniqueidentifier";
            }
            return "nvarchar(512)";
        }
        protected string TableName
        {
            get
            {
                return Properties.Name.Replace(" ", "");
            }
        }
        public void CreateTable()
        {
            string query = "SELECT top 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'" + TableName + "'";

            if (Db.ExecuteQueryCmd<int>(query) == null)
            {
                //add table
                string cols = "";
                foreach (var col in Properties)
                {
                    cols += " [" + col.Name + "] " + ToSqlDataTypeString(col.DataType) + ",";
                }
                cols = cols.Trim(',');
                query = string.Format("CREATE TABLE [dbo].[{0}]({1})", TableName, cols);
                Db.ExecuteNonQueryCmd(query);
            }
            else
            {
                //get current all columns
                query = string.Format("SELECT [COLUMN_NAME] FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'{0}'", TableName);
                List<DataItem> allColNames = Db.ExecuteQueryCmd(query);

                foreach (var col in Properties)
                {
                    bool isColExist = false;
                    foreach (DataItem row in allColNames)
                    {
                        string colName = row["COLUMN_NAME"].ToString();
                        if (colName == col.Name)
                        {
                            isColExist = true;
                            break;
                        }
                    }
                    if (isColExist)
                    {
                        query += string.Format("alter table [{0}] alter column [{1}] {2}\r\n", TableName, col.Name, ToSqlDataTypeString(col.DataType));
                    }
                    else
                    {
                        query += string.Format("alter table [{0}] add [{1}] {2}\r\n", TableName, col.Name, ToSqlDataTypeString(col.DataType));
                    }
                }
                Db.ExecuteNonQueryCmd(query);
            }
        }
        protected SqlProvider Db { get; private set; }
        public ModelDefinition Properties { get; private set; }
        public DataItemEntity Clone()
        {
            return new DataItemEntity(Properties);
        }
        public bool Get()
        {
            string fieldQuery = "";
            foreach (var p in Properties)
            {
                fieldQuery += string.Format("[{0}],", p.Name);
            }
            fieldQuery = fieldQuery.Trim(',');

            string whereQuery = string.Format("[{0}] = @{0}", Properties.KeyField);
            string query = "";
            query = string.Format("SELECT {1} FROM [{0}] (nolock) WHERE {2}", TableName, fieldQuery, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            parameters.Add(Properties.KeyField, this[Properties.KeyField]);
            List<DataItem> result = Db.ExecuteQueryCmd(query, parameters);
            if (result == null) return false;
            Copy(result[0]);
            return true;
        }

        public List<DataItemEntity> GetList(int[] ids = null)
        {
            string fieldQuery = "";
            foreach (var p in Properties)
            {
                fieldQuery += string.Format("[{0}],", p.Name);
            }
            fieldQuery = fieldQuery.Trim(',');

            //string whereQuery = string.Format("[{0}] = @{0}", Properties.KeyField);
            string query = "";
            if (ids == null)
            {
                query = string.Format("SELECT {1} FROM [{0}] (nolock)", TableName, fieldQuery);
            }
            else
            {
                string idStr = "";
                foreach (var id in ids) idStr += id + ",";
                string whereQuery = string.Format("[{0}] in ({1})", Properties.KeyField, idStr.Trim(','));
                query = string.Format("SELECT {1} FROM [{0}] (nolock) WHERE {2}", TableName, fieldQuery, whereQuery);
            }
            ObjectParameter parameters = new ObjectParameter();
            List<DataItem> result = Db.ExecuteQueryCmd(query, parameters);

            if (result != null)
            {
                List<DataItemEntity> list = new List<DataItemEntity>();
                DataItemEntity item = null;
                foreach (var o in result)
                {
                    item = Clone();
                    item.Copy(o);
                    list.Add(item);
                }
                return list;
            }
            return null;
        }
        public bool Delete()
        {
            DataItemEntity current = Clone();
            current[Properties.KeyField] = this[Properties.KeyField];
            current.Get();
            foreach (var p in Properties)
            {
                if (p.DataType == DataType.File)
                {
                    //delete
                    if (!IsNull(current[p.Name]))
                    {
                        FileDataInfo.Delete((Guid)current[p.Name]);
                    }
                }
            }
            string whereQuery = string.Format("[{0}] = @{0}", Properties.KeyField);
            string query = "";
            query = string.Format("DELETE FROM [{0}] WHERE {1}", TableName, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            parameters.Add(Properties.KeyField, this[Properties.KeyField]);
            int result = Db.ExecuteNonQueryCmd(query, parameters);
            return result > 0;
        }
        bool IsNull(object v)
        {
            return v == null || v is DBNull;
        }
        public bool Update()
        {
            DataItemEntity current = Clone();
            current.Copy(this);
            current.Get();
            foreach (var p in Properties)
            {
                if (p.DataType == DataType.File)
                {
                    //delete
                    if (!IsNull(current[p.Name]))
                    {
                        if (IsNull(this[p.Name]) || (!IsNull(this[p.Name]) && current[p.Name].ToString() != this[p.Name].ToString()))
                        {
                            FileDataInfo.Delete((Guid)current[p.Name]);
                        }
                    }

                }
            }
            return Save(false);
        }
        public bool Insert()
        {
            return Save(true);
        }
        protected virtual bool Save(bool insert)
        {
            string valueQuery = "", fieldQuery = "";
            if (insert)
            {
                valueQuery = "";
                fieldQuery = "";
                foreach (var p in Properties)
                {
                    valueQuery += string.Format("@{0},", p.Name);
                    fieldQuery += string.Format("[{0}],", p.Name);
                }
                fieldQuery = fieldQuery.Trim(',');
            }
            else
            {
                foreach (var p in Properties)
                {
                    if (p.Name != "ID") valueQuery += string.Format("[{0}] = @{0},", p.Name);
                }
            }
            valueQuery = valueQuery.Trim(',');
            string whereQuery = "1 = 1";
            if (!insert)
            {

                whereQuery += string.Format(" AND [{0}] = @{0}", Properties.KeyField);
            }
            string query = "";
            if (insert)
            {
                query = string.Format("INSERT INTO [{0}]({1}) VALUES ({2})", TableName, fieldQuery, valueQuery);
            }
            else
            {
                query = string.Format("UPDATE [{0}] SET {1} WHERE {2}", TableName, valueQuery, whereQuery);
            }
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in Properties)
            {
                parameters.Add(p.Name, this[p.Name] ?? DBNull.Value);
            }

            int result = Db.ExecuteNonQueryCmd(query, parameters);
            return result > 0;
        }

    }
}
