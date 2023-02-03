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
    public class EntityTableAttribute : Attribute
    {
        public string TableName { get; set; }
        public string[] PrimaryFields { get; private set; }
        public bool AutoID { get; private set; }
        public EntityTableAttribute(bool autoID, params string[] primaryFields)
        {
            PrimaryFields = primaryFields;
            AutoID = autoID;
        }
        public EntityTableAttribute(params string[] primaryFields) : this(false, primaryFields) { }
    }
    public class IgnorePropertyAttribute : Attribute
    {
    }
    public abstract class BaseEntity
    {
        protected class ConfigValues
        {
            public string TableName { get; private set; }
            public string[] PrimaryFields { get; private set; }
            public bool AutoID { get; private set; }
            public ConfigValues(string name, string[] fields, bool autoID)
            {
                TableName = name;
                PrimaryFields = fields;
                AutoID = autoID;
            }
        }
        protected ConfigValues p_configValues = null;
        public BaseEntity()
            : this(SqlProvider.Default)
        {
        }
        public BaseEntity(SqlProvider sqlProvider)
        {
            Db = sqlProvider;
            Type type = GetType();
            EntityTableAttribute att = type.GetCustomAttribute<EntityTableAttribute>();
            if (att == null) throw new Exception("Must have EntityTableAttribute");
            p_configValues = new ConfigValues(att.TableName ?? type.Name, att.PrimaryFields, att.AutoID);
        }
        protected SqlProvider Db { get; private set; }
    }
    public abstract class BaseEntity<T>: BaseEntity
    {
        
        
        public void Copy(T source)
        {
            PropertyInfo[] props = Properties;

            foreach (var p in props)
            {
                if (p.CanRead && p.CanWrite)
                {
                    p.SetValue(this, p.GetValue(source));
                }

            }

        }
        PropertyInfo[] _properties = null;
        protected PropertyInfo[] Properties
        {
            get
            {
                if (_properties == null)
                {
                    Type type = typeof(T);

                    PropertyInfo[] props = type.GetProperties();
                    List<PropertyInfo> result = new List<PropertyInfo>();
                    foreach (var p in props)
                    {
                        IgnorePropertyAttribute att = p.GetCustomAttribute<IgnorePropertyAttribute>();
                        if (att == null) result.Add(p);
                    }
                    _properties = result.ToArray();
                }
                return _properties;
            }
        }
        [IgnoreProperty]
        public int TotalRows { get; protected set; }//for paging only
        public bool Get()
        {
            PropertyInfo[] props = Properties;
            
            
            string whereQuery = "1 = 1";
            foreach (var p in props)
            {
                if (p_configValues.PrimaryFields.Contains(p.Name))
                {
                    whereQuery += string.Format(" AND [{0}] = @{0}", p.Name);
                }

            }
            string query = "";
            query = string.Format("SELECT * FROM [{0}] (nolock) WHERE {1}", p_configValues.TableName, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (p_configValues.PrimaryFields.Contains(p.Name))
                {
                    parameters.Add(p.Name, p.GetValue(this));
                }
            }
            List<T> result = Db.ExecuteQueryCmd<T>(query, parameters);
            if (result == null) return false;
            Copy(result[0]);
            return true;
        }
        public bool Get(string queries, string join = null, string joinSelect = null)
        {
            var list = GetAll(null, queries, join, joinSelect);
            if (list != null && list.Count > 0) { Copy(list[0]); return true; }
            else return false;
        }
        public List<T> GetAll(string orderby = null, string queries = null, string join = null, string joinSelect = null)
        {
            return Get(0, 0, orderby, queries, join, joinSelect);
        }
        public List<T> Get(int pageIndex, int pageSize = 10, string orderby = null, string queries = null, string join = null, string joinSelect = null)
        {

            
            if (string.IsNullOrEmpty(orderby))
            {
                orderby = string.Join("],[", p_configValues.PrimaryFields);
                orderby = "[" + orderby + "]";
            }
            string query = "";
            ObjectParameter parameters = new ObjectParameter();
            if (pageSize > 0)
            {
                query = @"
;WITH s AS
(  
	SELECT COUNT(1) OVER() AS [TotalRows], ROW_NUMBER() OVER(ORDER BY {1}) AS [RowNum], [{0}].*{4}
	FROM [{0}] (NOLOCK)
    {3}
    {2}
    
)
SELECT * FROM s 
WHERE [RowNum] BETWEEN 
	(@PageIndex - 1) * @PageSize + 1 AND @PageIndex * @PageSize
ORDER BY [RowNum]
";
                parameters.Add("PageIndex", pageIndex);
                parameters.Add("PageSize", pageSize);
            }
            else
            {
                query = @"
SELECT [{0}].*{4}
FROM [{0}] (NOLOCK)
{3}
{2}
ORDER BY {1}
";
            }
            
            query = string.Format(query, p_configValues.TableName, orderby,
                string.IsNullOrEmpty(queries) ? "" : "WHERE " + queries,
                string.IsNullOrEmpty(join) ? "" : join,
                string.IsNullOrEmpty(joinSelect) ? "" : "," + joinSelect);
            
            
            return Db.ExecuteQueryCmd<T>(query, parameters);
        }
        public bool Delete()
        {
            PropertyInfo[] props = Properties;

            string whereQuery = "1 = 1";
            foreach (var p in props)
            {
                if (p_configValues.PrimaryFields.Contains(p.Name))
                {
                    whereQuery += string.Format(" AND [{0}] = @{0}", p.Name);
                }

            }
            string query = "";
            query = string.Format("DELETE FROM [{0}] WHERE {1}", p_configValues.TableName, whereQuery);
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (p_configValues.PrimaryFields.Contains(p.Name))
                {
                    parameters.Add(p.Name, p.GetValue(this));
                }
            }
            int result = Db.ExecuteNonQueryCmd(query, parameters);
            return result > 0;
        }

        public bool Update()
        {
            return Save(false);
        }
        public bool Insert()
        {
            //return Save(false);
            return Save(true);
        }
        protected virtual bool Save(bool insert)
        {
            PropertyInfo[] props = Properties;
            string valueQuery = "", fieldQuery = "";
            if (insert)
            {
                foreach (var p in props)
                {
                    if (p_configValues.AutoID && p_configValues.PrimaryFields.Contains(p.Name)) continue;
                    valueQuery += string.Format("@{0},", p.Name);
                    fieldQuery += string.Format("[{0}],", p.Name);
                }
                fieldQuery = fieldQuery.Trim(',');
            }
            else
            {
                foreach (var p in props)
                {
                    if (p_configValues.PrimaryFields.Contains(p.Name)) continue;
                    valueQuery += string.Format("[{0}] = @{0},", p.Name);
                }
            }
            valueQuery = valueQuery.Trim(',');
            string whereQuery = "1 = 1";
            if (!insert)
            {
                foreach (var p in props)
                {
                    if (p_configValues.PrimaryFields.Contains(p.Name))
                    {
                        whereQuery += string.Format(" AND [{0}] = @{0}", p.Name);
                    }

                }
            }
            string query = "";
            if (insert)
            {
                query = string.Format("INSERT INTO [{0}]({1}) VALUES ({2})", p_configValues.TableName, fieldQuery, valueQuery);
            }
            else
            {
                query = string.Format("UPDATE [{0}] SET {1} WHERE {2}", p_configValues.TableName, valueQuery, whereQuery);
            }
            ObjectParameter parameters = new ObjectParameter();
            foreach (var p in props)
            {
                if (insert && p_configValues.AutoID && p_configValues.PrimaryFields.Contains(p.Name)) continue;
                var val = p.GetValue(this);
                if (val == null) val = DBNull.Value;
                parameters.Add(p.Name, val);
            }
            int result = 0;
            if (p_configValues.AutoID && insert)
            {
                query += ";select @@IDENTITY";
                List<int> newID = Db.ExecuteQueryCmd<int>(query, parameters);

                foreach (var p in props)
                {
                    if (p_configValues.PrimaryFields.Contains(p.Name))
                    {
                        p.SetValue(this, newID[0]);
                    }
                }
                result = newID[0];
            }
            else
            {
                result = Db.ExecuteNonQueryCmd(query, parameters);
            }
            return result > 0;
        }

    }
}
