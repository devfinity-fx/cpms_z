using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        protected static bool All()
        {

            return true;
        }


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
            Console.WriteLine("SELECT * FROM {0} WHERE {1} = '{2}'", newInstance.TableName, newInstance.PrimaryKey, ID);
            object[] data = new object[] { };
            object[] cols = new object[] { };
            using (MySqlDataReader reader = ConnectionManager.ExecuteQuery("SELECT * FROM {0} Where {1} = '{2}'", newInstance.TableName, newInstance.PrimaryKey, ID))
            {
                if(reader.Read())
                {

                }
                else
                {
                    throw new NullReferenceException("No matching record(s) found");
                }
            }

            foreach (FieldInfo f in newInstance.GetType().GetFields())
            {
                //f.SetValue(null, )
            }


            //newInstance.GetType().GetProperty("").SetValue(newInstance, id);
            return newInstance;
        }


        // Where
        // Summary:
        //     Finds record from the table using where conditions.
        //
        //
        // Parameters:
        //   ID:
        //     Pair containing the column and the value to be find.
        //
        //
        // Exceptions:
        //   T:System.NullReferenceException
        //     Column not found
        public static QueryResult Where(KeyValuePair<String, object> pair, params KeyValuePair<String, object>[] pairx)
        {
            return null;
        }


        /*
        All - ok
        Find - ok
        // FindOrFail
        Where - ok (queryresult)
        OrderBy - ok (queryresult)
        Take - Limit - 
        First - Order Desc Limit 1
        Last - Order Asc Limit 1
        Update - update database
        Save
        Delete
        */



        public void Save()
        {
            Console.WriteLine("INSERT INTO {0}({1}) VALUES({2});", TableName, FillableFields, Values);
        }

        public void Update()
        {
            Console.WriteLine("UPDATE SET {0} WHERE {1};", UpdateValues, "ID=Shit");
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
                    if(fields[i].GetValue(this)!=null)
                    {
                        value = fields[i].GetValue(this);
                        if (Dates.Length > 0 && Dates.Contains(fields[i].Name))
                        {
                            if (((DateTime)fields[i].GetValue(this)) != DateTime.MinValue)
                            {
                                value = ((DateTime)value).ToString("yyyy-MM-dd");
                            }
                            else continue;
                        }
                        valuePlaceholder = Utils.NumericTypes.Contains(value.GetType()) ? "{0}" : "'{0}'"; 
                        placeholder += String.Format("{0}={1}", fields[i].Name, String.Format(valuePlaceholder, value));
                        placeholder += ", ";
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
                object[] values = new object[Fillable.Length];

                for (int i = 0; i < Fillable.Length; i++)
                {
                    FieldInfo info = GetType().GetField(Fillable[i]);
                    if(Fillable.Contains(info.Name))
                    {
                        if (Dates.Length > 0 && Dates.Contains(info.Name))
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


        }
        //=============================================================================
    }


    /*
    public static class ModelExtensions
    {
        public static Object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this Model obj, String name)
        {
            Object retval = GetPropValue(obj, name);
            if (retval == null) { return default(T); }

            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }


        public static bool GetValue(this Model currentObject, string propName, out object value)
        {
            // call helper function that keeps track of which objects we've seen before
            return GetValue(currentObject, propName, out value, new HashSet<object>());
        }

        public static bool GetValue(object currentObject, string propName, out object value,
                             HashSet<object> searchedObjects)
        {
            PropertyInfo propInfo = currentObject.GetType().GetProperty(propName);
            if (propInfo != null)
            {
                value = propInfo.GetValue(currentObject, null);
                return true;
            }
            // search child properties
            foreach (PropertyInfo propInfo2 in currentObject.GetType().GetProperties())
            {   // ignore indexed properties
                if (propInfo2.GetIndexParameters().Length == 0)
                {
                    object newObject = propInfo2.GetValue(currentObject, null);
                    if (newObject != null && searchedObjects.Add(newObject) &&
                        GetValue(newObject, propName, out value, searchedObjects))
                        return true;
                }
            }
            // property not found here
            value = null;
            return false;
        }
    }
    */

    public class QueryResult
    {
        private List<object> current;

        private QueryResult() { current = new List<object>(); }

        public static QueryResult Where(params KeyValuePair<String, object>[] pair)
        {
            return null;
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
