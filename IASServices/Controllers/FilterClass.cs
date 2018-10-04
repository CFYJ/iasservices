using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace IASServices.Controllers
{
    public class FilterClass
    {
        string value;
        string field;

        Dictionary<string, string> freetextList = null;

        private FilterClass(string field, string value, Dictionary<string, string> lista = null)
        {
            this.value = value;
            this.field = field;
            this.freetextList = lista;
        }

        /// <summary>
        /// metoda generująca warunek do zapytania na bazę na podstawie filtrów generowanych z jqxgrid
        /// </summary>
        /// <param name="query">lista filtrów generowana z jqxgrid</param>
        /// <param name="lista">lista pól do fulltext search zmienianych na contains zamiast like</param>
        /// <param name="obiekt">property info obiektu, który w nazwach kolumn zawieraja podkreślnik</param>
        /// <returns></returns>
        public static string getFilters(IQueryCollection query, Dictionary<string,string> lista=null)
        {
            
            int count = int.TryParse(query["filterscount"], out count) ? count : 0;

            string rez = "";

            if (count > 0)
                for (int i = 0; i < count; i++)
                {
                    rez +=  " and ";
                    FilterClass fi = new FilterClass(query["filterGroups[" + i + "][filters][0][field]"], query["filterGroups[" + i + "][filters][0][value]"], lista);

                    rez += fi.getFilter();
                }

            rez =  " where 1=1 " + rez + " ";


            return rez;
        }


        public static string generateQueryCondition(IQueryCollection query, Dictionary<string, string> lista = null, Type obiekt = null)
        {

            int count = int.TryParse(query["filterscount"], out count) ? count : 0;

            string rez = " where 1=1 ";

            if (count > 0)
            {
                var z = query.GetEnumerator();

                for (int i = 0; i < count; i++)
                {

                    string filed = z.Current.Value;
                    z.MoveNext();
                   //tring 
                   //tring 

                    //while (z.MoveNext())
                    //{
                    //    string o = z.Current.Key;

                    //}

                    // rez  +=" and " + generateCondition(query["filterGroups[" + i + "][filters][0][field]"], query["filterGroups[" + i + "][filters][0][value]"], lista, obiekt); 
                }

            }

            return rez;
        }


        private static string generateCondition(string field, string value, Dictionary<string, string> lista = null, Type obiekt = null)
        {
            string rez = "";
            if (lista != null && lista.ContainsKey(field))
            {
                value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");
                rez = " contains (" + lista[field] + ",'" + value.Replace(" ", "%") + "') ";
            }
            else
            {
                rez = " " + field + " like '%" + value + "%' ";
            }

            return rez;
        }


        private string getFilter()
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

        /// <summary>
        /// funkcja generujaca warunek sortowania na podstawie jqxgrid
        /// </summary>
        /// <param name="query">argumenty sortowania z jqxgrid</param>
        /// <param name="obiekt">(typeof(Class)) - gdzie klasa jest obiekt reprezentujacy tabele</param>
        /// <returns></returns>
        public static string getSortCondition(IQueryCollection query, Type obiekt)
        {

            string rez = " ";

            if (query["sortorder"] != "")
                rez = " ORDER BY " + getFieldName(query["sortdatafield"], obiekt) + " " + query["sortorder"];

            return rez;
        }

        public static string getFieldName(string nazwa,Type obiekt)
        {
            string rez = nazwa;
            foreach (var prop in obiekt.GetProperties())
            {
                if (prop.Name.ToString().ToUpper() == nazwa.ToString().ToUpper())
                    if (prop.CustomAttributes.Count() > 0)
                    {
                        var t = (prop.GetCustomAttributes().First() as ColumnAttribute).TypeName;
                        rez = (prop.GetCustomAttributes().First() as ColumnAttribute).Name;

                      
                    }
            }

            return rez;
        }
    }
}
