﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Newtonsoft.Json;
using System.Collections;

using CoopManagement.Classes;
using CoopManagement.Core;

using MySql.Data.MySqlClient;

namespace CoopManagement.Core
{
    public abstract class Model : Object
    {
        // Fillable Fields
        // Summary:
        //      Gets a list of fillable fields via create.
        //
        //
        // Returns:
        //     The list of fillable fields.
        protected virtual String[] Fillable { get; }


        // Table Name
        // Summary:
        //      Gets or sets the name of the table of the model.
        //
        //
        // Returns:
        //     The table name relative to the model.
        protected virtual String TableName { get; }


        // Primary Key
        // Summary:
        //      Gets or sets the field name that will represent the table's primary key.
        //
        //
        // Returns:
        //     The primary key of the table.
        protected virtual String PrimaryKey { get; }

        // Date Fields
        // Summary:
        //     Initializes a new instance of the System.String class to the value indicated
        //     by an array of Unicode characters.
        //
        // Parameters:
        //   value:
        //     An array of Unicode characters.
        protected virtual String[] Dates { get; }


        // Fetch All
        // Summary:
        //      Queries all the data from the table and fills [JsonResult] field.
        //
        // Returns:
        //   value:
        //      A bool value indicating if the query succeeded or failed.
        public static QueryResult All<T>() where T : Model, new()
        {
            
            T newInstance = new T();

            StringBuilder qr = QueryResult.GetCurrentInstance();

            qr.AppendFormat("SELECT * FROM {0}", newInstance.TableName);
            return qr.ToString();
        }

        private static object[] valuesOnFind;
        // Find Record by ID
        // Summary:
        //     Finds record from the table using the primary key.
        //
        //
        // Parameters:
        //   ID:
        //     The primary key field value to find.
        //
        //
        // Exceptions:
        //   T:System.NullReferenceException
        //     The primary key value is not found
        public static T Find<T>(object ID) where T : Model, new()
        {
            T newInstance = new T();

            FieldInfo[] fields = newInstance.GetType().GetFields();
            valuesOnFind = new object[fields.Length];
            using (MySqlDataReader reader = ConnectionManager.ExecuteQuery("SELECT * FROM {0} WHERE {1} = '{2}'", newInstance.TableName, newInstance.PrimaryKey, ID))
            {
                if(reader.Read())
                {
                    for(int i=0;i<fields.Length;i++)
                    {
                        object value; // inner scope
                        if(fields[i].FieldType==typeof(DateTime))
                        {
                            value = reader.GetDateTime(fields[i].Name);
                            fields[i].SetValue(newInstance, DateTime.Parse(value.ToString()));
                            valuesOnFind[i] = DateTime.Parse(value.ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            value = reader.GetString(fields[i].Name);
                            fields[i].SetValue(newInstance, Convert.ChangeType(value, fields[i].FieldType));
                            valuesOnFind[i] = value;
                        }
                        
                        if(fields[i].Name==newInstance.PrimaryKey)
                        {
                            KeyValue = value;
                        }
                    }
                    IsUpdate = true;
                }
                else
                {
                    throw new NullReferenceException("No matching record(s) found");
                }
            }
            
            return newInstance;
        }


        // Where
        // Summary:
        //     Finds record from the table using where conditions.
        //
        //
        // Parameters:
        //   pair:
        //     Pair containing the column and the value to be find.
        //
        //
        // Exceptions:
        //   T:System.NullReferenceException
        //     Column not found
        public static QueryResult Where<T>(Tuple<String, object> pair) where T : Model, new()
        {
            T newInstance = new T();
            List<Tuple<String, object>> parameters = new List<Tuple<string, object>>();
            parameters.Add(pair);
            StringBuilder qr = QueryResult.GetCurrentInstance();

            if(qr.ToString().StartsWith("SELECT")==false)
            {
                qr.AppendFormat("SELECT * FROM {0}", newInstance.TableName);
            }

            qr.AppendFormat(" WHERE {0}='{1}'", pair.Item1, pair.Item2);
            return qr.ToString();
        }

        // Last
        // Summary:
        //     Fetch the last record in the table
        //
        //
        // Exceptions:
        //   T:System.NullReferenceException
        //     No Records found.
        public static QueryResult Last<T>() where T : Model, new()
        {
            T newInstance = new T();

            StringBuilder qr = QueryResult.GetCurrentInstance();

            if (qr.ToString().StartsWith("SELECT") == false)
            {
                qr.AppendFormat("SELECT * FROM {0} ORDER BY {1} DESC;", newInstance.TableName, newInstance.PrimaryKey);
            }
            
            return qr.ToString();
        }

        /*
        All - ok
        Find - ok
        // FindOrFail
        //Where - ok (queryresult)
        OrderBy - ok (queryresult)
        Take - Limit - 
        First - Order Desc Limit 1
        Last - Order Asc Limit 1
        Update - update database
        Save
        Delete
        */


        //=============================================================================

        public bool Save()
        {
            if(!IsUpdate)
            {
                return ConnectionManager.ExecuteCommand("INSERT INTO {0}({1}) VALUES({2});", TableName, FillableFields, Values);
            }
            return false;
        }

        public bool Update()
        {
            if(IsUpdate)
            {
                return ConnectionManager.ExecuteCommand("UPDATE {0} SET {1} WHERE {2}='{3}';", TableName, UpdateValues, PrimaryKey, KeyValue);
            }
            return false;
        }

        //=============================================================================
        private String UpdateValues
        {
            get
            {
                FieldInfo[] fields = GetType().GetFields();
                String placeholder = "";
                String valuePlaceholder;
                object value;

                for(int i=0;i<fields.Length;i++)
                {
                    value = fields[i].GetValue(this);
                    if (value != null)
                    {
                        if (Dates.Length > 0 && Dates.Contains(fields[i].Name))
                        {
                            if (((DateTime)fields[i].GetValue(this)) != DateTime.MinValue)
                            {
                                value = ((DateTime)value).ToString("yyyy-MM-dd");
                            }
                            else continue;
                        }
                        
                        if(valuesOnFind[i].ToString() != value.ToString())
                        {
                            valuePlaceholder = Utils.NumericTypes.Contains(value.GetType()) ? "{0}" : "'{0}'";
                            placeholder += String.Format("{0}={1}", fields[i].Name, String.Format(valuePlaceholder, value));
                            placeholder += ", ";
                        }
                    }
                }
                placeholder = placeholder.Remove(placeholder.Length - 2);
                return placeholder;
            }


        }

        private String Values
        {
            get
            {
                FieldInfo[] fields = GetType().GetFields();
                Console.WriteLine(fields.Length);
                object[] values = new object[Fillable.Length];

                for (int i = 0; i < Fillable.Length; i++)
                {
                    FieldInfo info = GetType().GetField(Fillable[i]);
                    if(Fillable.Contains(info.Name))
                    {
                        if (Dates!=null && Dates.Length > 0 && Dates.Contains(info.Name))
                        {
                            values[i] = ((DateTime)info.GetValue(this)).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            values[i] = ParseValue(info.GetValue(this), info.GetValue(this).GetType());
                        }
                    }
                }
                return Utils.Format(values);
            }
        }

        private String FillableFields
        {
            get
            {
                String placeholder = "";
                for (int i = 0; i < Fillable.Length; i++)
                {
                    placeholder += Fillable[i];
                    placeholder += i == Fillable.Length - 1 ? "" : ", ";
                }
                return placeholder;
            }
        }

        private object ParseValue(object obj, Type type)
        {
            return Convert.ChangeType(obj, type);
        }

        public static Tuple<string, object> Param(String key, object val)
        {
            return Tuple.Create<String, object>(key, val);
        }


        //=============================================================================

        protected static bool IsUpdate = false;
        protected static object KeyValue;
        protected static String Query;
    }
    
    public class QueryResult
    {
        private List<object> current;
        private static StringBuilder CurrentInstance;


        private QueryResult() { current = new List<object>(); }
        private QueryResult(String q)
        {
            new QueryResult();
        }

        public static StringBuilder GetCurrentInstance()
        {
            if(CurrentInstance==null)
            {
                CurrentInstance = new StringBuilder();
            }
            return CurrentInstance;
        }

        // Where
        // Summary:
        //     Finds record from the table using where conditions.
        //
        //
        // Parameters:
        //   pair:
        //     Pair containing the column and the value to be find.
        //
        //
        // Exceptions:
        //   T:System.NullReferenceException
        //     Column not found
        public QueryResult Where<T>(Tuple<String, object> pair) where T : Model, new()
        {
            List<Tuple<String, object>> parameters = new List<Tuple<string, object>>();
            parameters.Add(pair);
            StringBuilder qr = QueryResult.GetCurrentInstance();
            qr.AppendFormat(" AND {0}='{1}'", pair.Item1, pair.Item2);
            return qr.ToString();
        }

        // Get
        // Summary:
        //     Builds the query.
        //
        public T[] Get<T>() where T: Model, new()
        {
            StringBuilder sb = CurrentInstance;
            sb.Append(";");

            List<T> list = new List<T>();
            T obj = new T();

            FieldInfo[] fields = obj.GetType().GetFields();
            object value;
            
            using (MySqlDataReader reader = ConnectionManager.ExecuteQuery(sb.ToString()))
            {
                while(reader.Read())
                {
                    obj = new T();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (fields[i].FieldType == typeof(DateTime))
                        {
                            value = reader.GetDateTime(fields[i].Name);
                            fields[i].SetValue(obj, DateTime.Parse(value.ToString()));
                        }
                        else
                        {
                            value = reader.GetString(fields[i].Name);
                            fields[i].SetValue(obj, Convert.ChangeType(value, fields[i].FieldType));
                        }
                    }

                    list.Add(obj);
                }
            }

            return list.ToArray<T>();
        }

        // Get
        // Summary:
        //     Builds the query.
        // Parameters:
        //     columns:
        //      Contains the list of fields to be fetched from the query
        public T[] Get<T>(params String[] columns) where T : Model, new()
        {
            StringBuilder sb = CurrentInstance;
            sb.Append(";");
            sb.Replace("*", columns.ToText());

            List<T> list = new List<T>();
            T obj = new T();

            FieldInfo[] fields = obj.GetType().GetFields();
            object value;

            using (MySqlDataReader reader = ConnectionManager.ExecuteQuery(sb.ToString()))
            {
                while (reader.Read())
                {
                    obj = new T();
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (columns.Contains(fields[i].Name))
                        {
                            if (fields[i].FieldType == typeof(DateTime))
                            {
                                value = reader.GetDateTime(fields[i].Name);
                                fields[i].SetValue(obj, DateTime.Parse(value.ToString()));
                            }
                            else
                            {
                                value = reader.GetString(fields[i].Name);
                                fields[i].SetValue(obj, value);
                            }
                        }
                    }

                    list.Add(obj);
                }
            }

            return list.ToArray<T>();
        }

        public QueryResult SortBy(params Tuple<String, SortType>[] order)
        {
            SortBy(Tuple.Create<String, SortType>("magic", SortType.ASC));
            return new QueryResult();
        }

        public QueryResult Take(int count)
        {
            return new QueryResult();
        }

        public QueryResult First()
        {
            return new QueryResult();
        }

        public QueryResult Last()
        {
            return new QueryResult();
        }

        public override String ToString()
        {
            // return json
            return "";
        }

        //

        public static implicit operator string (QueryResult q)
        {
            return q.ToString();
        }
        public static implicit operator QueryResult(string q)
        {
            return new QueryResult(q);
        }

        #region Interface Implementation
        public object this[int index]
        {
            get
            {
                return current[index];
            }

            set
            {
                current[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return current.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(object item)
        {
            current.Add(item);
        }

        public void Clear()
        {
            current.Clear();
        }

        public bool Contains(object item)
        {
            return current.Contains(item);
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            current.CopyTo(array, arrayIndex);
        }

        public IEnumerator<object> GetEnumerator()
        {
            return current.GetEnumerator();
        }

        public int IndexOf(object item)
        {
            return current.IndexOf(item);
        }

        public void Insert(int index, object item)
        {
            current.Insert(index, item);
        }

        public bool Remove(object item)
        {
            return current.Remove(item);
        }

        public void RemoveAt(int index)
        {
            current.RemoveAt(index);
        }
        
        #endregion
    }

    public enum SortType
    {
        ASC = 0,
        DESC = 1
    }
}
