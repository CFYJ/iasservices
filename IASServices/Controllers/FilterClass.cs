﻿using System;
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

                #region stare
                //var z = query.GetEnumerator();

                //while (true)
                //{

                //    z.MoveNext();
                //    string value = "";
                //    string field = "";
                //    string condition="";


                //    if(!z.Current.Key.Contains("Group") && z.Current.Key.EndsWith("operator"))
                //    {
                //        while (z.MoveNext())
                //        {
                //            string key = z.Current.Key;
                //            if (key.Contains("value"))
                //                value = z.Current.Value;
                //            if (key.Contains("field"))
                //                field = z.Current.Value;
                //            if (key.Contains("condition"))
                //                condition= z.Current.Value;

                //            if (value != "" && field != "" && condition != "")
                //                break;

                //        }

                //        rez += " and " + generateCondition(field, value, condition, lista, obiekt);
                //        count--;

                //        if (count <= 0)
                //            break;
                //    }

                //}

                #endregion

                rez += " and (";

                string tmpdatafield = "";
                string tmpfilteroperator = "";
		
                for (int i=0; i < count; i++)
                {
                    string filtervalue = query["filtervalue" + i];
                    string filtercondition = query["filtercondition" + i];
                    string filterdatafield = getFieldName(query["filterdatafield" + i], obiekt);
                    string filteroperator = query["filteroperator" + i];
                  

                    if (tmpdatafield == "")
                        tmpdatafield = filterdatafield;
                    else if (tmpdatafield != filterdatafield)
                       rez += " ) AND (";
                    else if (tmpdatafield == filterdatafield)
                    {
                        if (tmpfilteroperator == "0")
                            rez += " AND ";
                        else
                            rez += " OR ";
                    }

                    string condition = "";
                    string value = "";
                    switch (filtercondition)
                    {
                        case "CONTAINS":
                            condition = " LIKE ";
                            value = "'%" + filtervalue + "%'";
                            break;
                        case "DOES_NOT_CONTAIN":
                            condition = " NOT LIKE ";
                            value = "'%" + filtervalue + "%'";
                            break;
                        case "EQUAL":
                            condition = " = ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "NOT_EQUAL":
                            condition = " <> ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "GREATER_THAN":
                            condition = " > ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "LESS_THAN":
                            condition = " < ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "GREATER_THAN_OR_EQUAL":
                            condition = " >= ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "LESS_THAN_OR_EQUAL":
                            condition = " <= ";
                            value = "'" + filtervalue + "'";
                            break;
                        case "STARTS_WITH":
                            condition = " LIKE ";
                            value = "'" + filtervalue + "%'";
                            break;
                        case "ENDS_WITH":
                            condition = " LIKE ";
                            value = "'%" + filtervalue + "'";
                            break;
                        case "NULL":
                            condition = " IS NULL ";
                            value = "";
                            break;
                        case "NOT_NULL":
                            condition = " IS NOT NULL ";
                            value = "";
                            break;
                    };

                    rez += " " + filterdatafield + " " + condition + " "+value+" ";

                    if (i == count - 1)
                        rez += " ) ";

                    tmpfilteroperator = filteroperator;
                    tmpdatafield = filterdatafield;

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
                //'equal', 'not equal', 'less than', 'less than or equal', 'greater than', 'greater than or equal', 'null', 'not null'
                switch (condition.ToLower())
                {
                    case "contains":
                        rez = " " + getFieldName(field, obiekt) + " like '%" + value + "%' ";
                        break;
                    case "equal":
                        rez = " " + getFieldName(field, obiekt) + " = '" + value + "' ";
                        break;
                    case "less_than":
                        rez = " " + getFieldName(field, obiekt) + " < '" + value + "' ";
                        break;
                     case "less_than_or_equal":
                        rez = " " + getFieldName(field, obiekt) + " <= '" + value + "' ";
                        break;
                    case "greater_than":
                        rez = " " + getFieldName(field, obiekt) + "  >'" + value + "' ";
                        break;
                    case "greater_than_or_equal":
                        rez = " " + getFieldName(field, obiekt) + "  >='" + value + "' ";
                        break;
                    case "null":
                        rez = " " + getFieldName(field, obiekt) + "  is null ";
                        break;
                    case "not_null":
                        rez = " " + getFieldName(field, obiekt) + "  is not null ";
                        break;
                    case "":
                        rez = " " + getFieldName(field, obiekt) + "  '" + value + "' ";
                        break;
                    default:
                        rez = "";
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
                if (prop.Name.ToString().Trim().ToUpper() == nazwa.ToString().Trim().ToUpper())
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
