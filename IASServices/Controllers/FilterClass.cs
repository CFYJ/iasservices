using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IASServices.Controllers
{
    public class FilterClass
    {
        string value;
        string field;

        Dictionary<string, string> freetextList = null;

        private FilterClass(string field, string value, Dictionary<string, string> lista = null, PropertyInfo[] obiekt = null)
        {
            this.value = value;
            this.field = obiekt != null ? getFieldName(field, obiekt) : field;
            this.freetextList = lista;
        }

        /// <summary>
        /// metoda generująca warunek do zapytania na bazę na podstawie filtrów generowanych z jqxgrid
        /// </summary>
        /// <param name="query">lista filtrów generowana z jqxgrid</param>
        /// <param name="lista">lista pól do fulltext search zmienianych na contains zamiast like</param>
        /// <param name="obiekt">property info obiektu, który w nazwach kolumn zawieraja podkreślnik</param>
        /// <returns></returns>
        public static string getFilters(Microsoft.AspNetCore.Http.IQueryCollection query, Dictionary<string,string> lista=null, PropertyInfo[] obiekt=null)
        {
            
            int count = int.TryParse(query["filterscount"], out count) ? count : 0;

            string rez = "";

            if (count > 0)
                for (int i = 0; i < count; i++)
                {
                    rez +=  " and ";
                    FilterClass fi = new FilterClass(query["filterGroups[" + i + "][filters][0][field]"], query["filterGroups[" + i + "][filters][0][value]"], lista, obiekt);

                    rez += fi.getFilter();
                }

            rez =  " where 1=1 " + rez + " ";


            return rez;
        }

       
        private string getFieldName(string field,PropertyInfo[] obiekt)
        {
            foreach (var prop in obiekt)
            {
                if(prop.Name.ToLower() == field.ToLower())
                    return (prop.GetCustomAttributes().First() as ColumnAttribute).Name;            
            }

            return field;
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
