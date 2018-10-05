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
        [Obsolete("Metoda zostanie usunięta, należy używać generateQueryCondition")]
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

                //for (int i = 0; i < count; i++)
                while(true)
                {
                    z.MoveNext();

                    string value = "";
                    string field = "";
                    string condition="";
                    

                    if(!z.Current.Key.Contains("Group") && z.Current.Key.EndsWith("operator"))
                    {
                        while (true)
                        {
                            string key = z.Current.Key;
                            if (key.Contains("value"))
                                value = z.Current.Value;
                            if (key.Contains("field"))
                                field = z.Current.Value;
                            if (key.Contains("condition"))
                                condition= z.Current.Value;

                            if (value != "" && field != "" && condition != "")
                                break;
                            z.MoveNext();
                        }

                        rez += " and " + generateCondition(field, value, condition, lista, obiekt);
                        count--;

                        if (count == 0)
                            break;
                    }


                    //z.MoveNext();
                    //while (!z.Current.Key.Contains("Group") && z.Current.Key.EndsWith("operator"))
                    //    z.MoveNext();
                    //if (z.Current.Key.EndsWith("operator"))
                    //    z.MoveNext();

                    ////z.MoveNext();
                    //string value = z.Current.Value;
                    //z.MoveNext();
                    //string condition = z.Current.Value;
                    //z.MoveNext();
                    //z.MoveNext();
                    //string field = getFieldName(z.Current.Value, obiekt);


                    //rez += " and " + generateCondition(field, value);
                }

            }

            return rez;
        }


        private static string generateCondition(string field, string value, string condition, Dictionary<string, string> lista = null, Type obiekt = null)
        {
            string rez = "";
            if (lista != null && lista.ContainsKey(field))
            {
                value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ");
                rez = " contains (" + getFieldName(lista[field], obiekt) + ",'" + value.Replace(" ", "%") + "') ";
            }
            else
            {
                switch (condition.ToLower())
                {
                    case "contains":
                        rez = " " + getFieldName(field, obiekt) + " like '%" + value + "%' ";
                        break;
                    case "equal":
                        rez = " " + getFieldName(field, obiekt) + " = '" + value + "' ";
                        break;
                }
                //rez = " " + getFieldName(field, obiekt) + " like '%" + value + "%' ";
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
            string rez = "";

            if (query.Where(a => a.Key == "sortorder").Count() >0)
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
