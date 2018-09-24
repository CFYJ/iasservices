using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IASServices.Controllers
{
    public class FilterClass
    {
        string value;
        string field;
        string condition;

        Dictionary<string, string> freetextList = null;

        public static string getFilters(Microsoft.AspNetCore.Http.IQueryCollection query, Dictionary<string,string> lista=null)
        {
            
            int count = int.TryParse(query["filterscount"], out count) ? count : 0;

            string rez = "";

            if (count > 0)
                for (int i = 0; i < count; i++)
                {
                    rez += i > 0 ? " and " : "";
                    FilterClass fi = new FilterClass(query["filterGroups[" + i + "][filters][0][field]"], query["filterGroups[" + i + "][filters][0][value]"], "CONTAINS", lista);

                    rez += fi.getFilter();
                }

            rez = count > 0 ? " where " + rez + " " : "";


            return rez;
        }

        public FilterClass(string field, string value, string condition = "CONTAINS", Dictionary<string, string> lista =null)
        {
            this.value = value;
            this.field = field;
            this.condition = condition;
            this.freetextList = lista;
        }

        public string getFilter2(Type t)
        {
            Type myType = t;//t.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                //object propValue = prop.GetValue(t, null);
                if (prop.Name.ToLower() == this.field.ToLower())
                {
                    // prop.PropertyType
                }


            }

            //string result = "";

            //switch (typeof())
            //{
            //    case string.

            //}

            return "";
        }

        public string getFilter()
        {
            string rez = "";
            if (freetextList != null && freetextList.ContainsKey(field))
            {
                value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");
                rez = " contains (" + freetextList[field] + ",'" + value.Replace(" ", "%") + "') ";
            }
            else
            {
                rez = " " + field + " like '%" + value + "%' ";
            }
            
            return rez;
        }
    }
}
