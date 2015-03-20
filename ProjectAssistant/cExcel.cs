using System;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Windows.Forms;

namespace ProjectAssistant
{
    class cExcel
    {
        public static string sMessage { get; private set; }

        /// <summary>
        /// Преобразует Excel-файл в DataTable
        /// </summary>
        /// <param name="sFile">Полный путь к Excel-файлу</param>
        /// <param name="sRequest">SQL-запрос. Используйте $SHEETS$ для выбоки по всем листам</param>
        /// <returns>Возвращает DataTable с импортированными данными</returns>
        public static DataTable FileToDataTable(string sFile, string sRequest)
        {
            DataSet dsData = new DataSet();

            string sConnStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"{1};HDR=YES\";", sFile, sFile.EndsWith(".xls") ? "Excel 8.0" : "Excel 12.0 Xml");

            try
            {
                using (OleDbConnection odcConnection = new OleDbConnection(sConnStr))
                {
                    odcConnection.Open();
                    if (sRequest.IndexOf("$SHEETS$", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        using (
                            DataTable dtMetadata = odcConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables,
                                new object[4] {null, null, null, "TABLE"}))
                        {
                            for (int i = 0; i < dtMetadata.Rows.Count; i++)
                                if (
                                    dtMetadata.Rows[i]["TABLE_NAME"].ToString()
                                        .IndexOf("$", StringComparison.OrdinalIgnoreCase) == -1)
                                    dtMetadata.Rows.Remove(dtMetadata.Rows[i]);

                            foreach (DataRow drRow in dtMetadata.Rows)
                            {
                                string sLocalRequest = sRequest.Replace("$SHEETS$",
                                    String.Format("[{0}]", drRow["TABLE_NAME"]));
                                OleDbCommand odcCommand = new OleDbCommand(sLocalRequest, odcConnection);
                                using (OleDbDataAdapter oddaAdapter = new OleDbDataAdapter(odcCommand))
                                    oddaAdapter.Fill(dsData);
                            }
                        }
                    }
                    else
                    {
                        OleDbCommand odcCommand = new OleDbCommand(sRequest, odcConnection);
                        using (OleDbDataAdapter oddaAdapter = new OleDbDataAdapter(odcCommand))
                            oddaAdapter.Fill(dsData);
                    }
                    odcConnection.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("Недопустимое имя: \"" + Form1.AIListName + "$\"",
                        StringComparison.OrdinalIgnoreCase) == -1 &&
                    ex.Message.IndexOf("Недопустимое имя: \"" + Form1.DIListName + "$\"",
                        StringComparison.OrdinalIgnoreCase) == -1)
                    MessageBox.Show(ex.Message, "Ошибка импорта!", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                else
                    MessageBox.Show(
                        string.Format(
                            "Не найден лист в файле Excel. Листы Excel с аналоговыми и дискретными тегами должны называться {0} и {1} соответственно",
                            Form1.AIListName, Form1.DIListName), "Ошибка импорта!", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                return null;
            }
            return dsData.Tables[0];
        }
        
        /// <summary>
        /// Преобразует dataTable в Excel-файл
        /// </summary>
        /// <param name="dtData">Данные</param>
        /// <param name="sFilePath">Полный путь к файлу</param>
        /// <param name="b2007">false усли старый формат до Excel 2007</param>
        /// <returns>В случае успеха возвращает true</returns>
        public static bool DataTableToFile(DataTable dtData, string sFilePath, bool b2007)
        {
            try
            {
                // Исправляем имя таблицы
                if (dtData.TableName.Length != 0 || dtData.TableName.Equals("Table", StringComparison.OrdinalIgnoreCase) == true)
                    dtData.TableName = "NONAME";

                string sConnStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"{1};HDR=YES\";", sFilePath, b2007 ? "Excel 12.0 Xml" : "Excel 8.0");

                using (OleDbConnection odcConnection = new OleDbConnection(sConnStr))
                {
                    odcConnection.Open();
                    using (OleDbCommand odcCommand = new OleDbCommand { Connection = odcConnection })
                    {
                        // Создание таблицы
                        odcCommand.CommandText = GenerateSqlStatementCreateTable(dtData);
                        odcCommand.ExecuteNonQuery();

                        DataRow dr;
                        OleDbParameter odpParameter;

                        // Генерируем скрипт создания строк со значениями (в качестве параметров)
                        string sColumns, sParameters;
                        GenerateColumnsString(dtData, out sColumns, out sParameters);

                        for (int i = 0; i < dtData.Rows.Count; i++)
                        {
                            dr = dtData.Rows[i];
                            // Устанавливаем параметр для INSERT
                            odcCommand.Parameters.Clear();
                            for (int j = 0; j < dtData.Columns.Count; j++)
                            {
                                odpParameter = new OleDbParameter();
                                odpParameter.ParameterName = "@p" + j;

                                odpParameter.Value = dr.IsNull(j) ? DBNull.Value : dr[j];

                                odcCommand.Parameters.Add(odpParameter);
                            }
                            odcCommand.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", dtData.TableName, sColumns, sParameters);
                            odcCommand.ExecuteNonQuery();
                        }
                    }
                    odcConnection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Создает список столбцов ([columnname0],[columnname1],[columnname2])
        /// и соответствующих им параметров (@p0,@p1,@p2).
        /// В качестве разделителя используется запятая
        /// </summary>
        /// <param name="dtData">Данные</param>
        /// <param name="sColumns">Список столбцов</param>
        /// <param name="sParams">Список параметров</param>
        private static void GenerateColumnsString(DataTable dtData, out string sColumns, out string sParams)
        {
            StringBuilder sbColumns = new StringBuilder();
            StringBuilder sbParams = new StringBuilder();
            for (int i = 0; i < dtData.Columns.Count; i++)
            {
                if (i != 0)
                {
                    sbColumns.Append(',');
                    sbParams.Append(',');
                }
                sbColumns.AppendFormat("[{0}]", dtData.Columns[i].ColumnName);
                sbParams.AppendFormat("@p{0}", i);
            }

            sColumns = sbColumns.ToString();
            sParams = sbParams.ToString();
        }

        /// <summary>
        /// Создает SQL-скрипт для создания таблицы, в соответствии с DataTable
        /// </summary>
        /// <param name="dtData">Данные</param>
        /// <returns>Возвращает запрос 'CREATE TABLE...'</returns>
        private static string GenerateSqlStatementCreateTable(DataTable dtData)
        {
            StringBuilder sbCreateTable = new StringBuilder();

            DataColumn dc;

            sbCreateTable.AppendFormat("CREATE TABLE {0} (", dtData.TableName);
            for (int i = 0; i < dtData.Columns.Count; i++)
            {
                dc = dtData.Columns[i];

                if (i != 0) sbCreateTable.Append(",");

                string dataType = dc.DataType.Equals(typeof(double)) ? "DOUBLE" : "NVARCHAR";

                sbCreateTable.AppendFormat("[{0}] {1}", dc.ColumnName, dataType);
            }
            sbCreateTable.Append(")");

            return sbCreateTable.ToString();
        }
    }
}
