using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectAssistant
{
    internal class cSQLBases
    {
        //общедоступная строка подключения к базам SQL
        public static string conString { get; set; }

        //для отсчета прогресса выполнения
        private static int _pr;

        /// <summary>
        /// Cхема данных сервера БД
        /// </summary>
        public static DataTable dbSchema
        {
            get
            {
                SqlConnection connection = new SqlConnection(conString);
                try
                {
                    connection.Open();
                    DataTable dt = connection.GetSchema("DataBases");
                    connection.Close();
                    return dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка подключения к БД");
                    return null;
                }
            }
        }

        /// <summary>
        /// Обновляет выбранные базы данных
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="udmBaseName">имя базы UDM</param>
        /// <param name="awxBaseName">имя базы AlarmWorx</param>
        /// <param name="hhBaseName">имя базы HyperHistorian</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        public static void Update(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt, string udmBaseName, string awxBaseName, string hhBaseName, bool discrete)
        {
            if (dt == null || string.IsNullOrEmpty(dt.Rows[0]["TagName"].ToString()))
            {
                MessageBox.Show("Добавьте в таблицу теги, которые хотите внести в базу", "Нет данных в таблице",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (string.IsNullOrEmpty(udmBaseName) && string.IsNullOrEmpty(awxBaseName) && string.IsNullOrEmpty(hhBaseName))
            {
                MessageBox.Show("Выберите базу, которую нужно обновить", "Не выбрана база данных", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            _pr = 0;

            if (!string.IsNullOrEmpty(udmBaseName))
            {
                UpdateUDMItems(worker, workerEventArgs, udmBaseName, dt, discrete);
            }
            if (!string.IsNullOrEmpty(awxBaseName))
            {
                UpdateAWXItems(worker, workerEventArgs, awxBaseName, dt, discrete);
            }
            if (!string.IsNullOrEmpty(hhBaseName))
            {
                if (discrete)
                    worker.ReportProgress(_pr, "Внесение дискретных тегов в базу HyperHistorian программно не предусмотрено.");
                else
                {
                    UpdateHHItems(worker, workerEventArgs, hhBaseName, dt);
                }
            }
        }

        /// <summary>
        /// Прописывает все теги в базу HyperHistorian
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dbName">имя базы данных HyperHistorian</param>
        /// <param name="dt">таблица с данными</param>
        private static void UpdateHHItems(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, string dbName, DataTable dt)
        {
            worker.ReportProgress(_pr, "---->Вносим изменения в базу " + dbName + "...");
            int rowsAffected = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(conString + ";Initial Catalog=" + dbName))
                {
                    con.Open();

                    string loggerId = null;
                    bool newLogger = false;

                    #region получение ID (создание) логера с именем FileBasedLogger
                getLoggerId:
                    //если существует логгер с именем 'File Based Logger', берем его Id
                    using (DataTable loggersData = new DataTable())
                    {
                        SqlDataAdapter adapter =
                            new SqlDataAdapter("SELECT [ID],[Name] FROM [dbo].[HH_Loggers] WHERE [Name]='FileBasedLogger'", con);
                        adapter.Fill(loggersData);
                        loggerId = loggersData.Rows.Count > 0 ? loggersData.Rows[0]["ID"].ToString() : null;
                    }
                    //иначе создаем
                    if (string.IsNullOrEmpty(loggerId))
                    {
                        const string comText =
                            "INSERT INTO [dbo].[HH_Loggers] (Name,Description,Type) VALUES ('FileBasedLogger','file based hyper historian logger',4)";
                        SqlCommand command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();
                        newLogger = true;
                        goto getLoggerId;
                    }
                    #endregion

                    if (newLogger)
                    {
                        #region создание настроек для логера
                        string comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",12,0)";
                        SqlCommand command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,StringValue) " +
                             "VALUES (" + loggerId + ",13,'')";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",14,10485760)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",15,10080)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",16,1)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",17,1)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",18,2)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",19,60)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",20,3)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,StringValue) " +
                             "VALUES (" + loggerId + ",21,'')";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,StringValue) " +
                             "VALUES (" + loggerId + ",22,'')";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",23,1)";
                        command = new SqlCommand(comText, con);

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",24,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",25,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",26,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",27,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",28,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",29,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",30,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",31,1)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",32,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",33,1)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",34,86400)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",35,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,BitValue) " +
                             "VALUES (" + loggerId + ",36,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID) " +
                             "VALUES (" + loggerId + ",37)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",38,0)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",39,60)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,IntValue) " +
                             "VALUES (" + loggerId + ",40,102400)";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();

                        comText = "INSERT INTO [dbo].[HH_LoggerPropertiesValues] (ParentID,PropertyID,StringValue) " +
                             "VALUES (" + loggerId + ",41,'')";
                        command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();
                        #endregion
                    }

                    #region получение ID (создание) LoggerGroup для FileBasedLogger
                    string logGroupId = null;
                getLogGroupId:
                    //если группа существует, берем ее Id
                    using (DataTable loggersData = new DataTable())
                    {
                        SqlDataAdapter adapter =
                            new SqlDataAdapter("SELECT [ID],[LoggerID] FROM [dbo].[HH_LoggingGroups] WHERE [LoggerID]=" + loggerId, con);
                        adapter.Fill(loggersData);
                        logGroupId = loggersData.Rows.Count > 0 ? loggersData.Rows[0]["ID"].ToString() : null;
                    }
                    //иначе создаем
                    if (string.IsNullOrEmpty(logGroupId))
                    {
                        string comText =
                            "INSERT INTO [dbo].[HH_LoggingGroups] (Name,Enabled,LoggerID) " +
                            "VALUES ('LoggerGroup',1," + loggerId + ")";
                        SqlCommand command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();
                        goto getLogGroupId;
                    }
                    #endregion

                    #region получение ID (создание) CollectorGroup в нашей LoggerGroup
                    string colGroupId = null;
                getColGroupId:
                    //если группа существует, берем ее Id
                    using (DataTable loggersData = new DataTable())
                    {
                        SqlDataAdapter adapter =
                            new SqlDataAdapter("SELECT [ID],[ParentID] FROM [dbo].[HH_CollectorGroups] WHERE [ParentID]=" + logGroupId, con);
                        adapter.Fill(loggersData);
                        colGroupId = loggersData.Rows.Count > 0 ? loggersData.Rows[0]["ID"].ToString() : null;
                    }
                    //иначе создаем
                    if (string.IsNullOrEmpty(colGroupId))
                    {
                        string comText =
                            "INSERT INTO [dbo].[HH_CollectorGroups] (ParentID,Name,CollectorPairID,DataCollectionRate" +
                            ",CalculationPeriod,UseLocalTimeStamps,ForceRefreshRate,LogOnCondition)" +
                            " VALUES (" + logGroupId + ",'CollectorGroup',1,1000,10000,0,0,0)";
                        SqlCommand command = new SqlCommand(comText, con);
                        command.ExecuteNonQuery();
                        goto getColGroupId;
                    }
                    #endregion

                    const string hhComText =
                        "INSERT INTO [dbo].[HH_Tags]" +
                              "(ParentID,CollectorGroupID,Name,Description,IsCollected,SupportOperatorComments,DataType,FilterType,FilterTreshold,FilterTresholdInterpretation" +
                              ",FilterMinPeriod,FilterMaxPeriod,SteppedInterpretation,DataSource,EngineeringUnits,HighRange,LowRange,AggregateType,DeadbandValue,DeadbandType" +
                              ",TotalizerUnits,Enabled,UseUniversalTime,PercentGood,TreatUncertainAsBad,MovAvgWeight,UseCollectionPeriod,CollectionPeriodInMs,UseAlternateCalculationPeriod" +
                              ",AlternateCalculationPeriodInMs,UseShutdownMarker,MROT_WeeksOfMonth,MROT_DaysOfMonth,MROT_DaysOfWeek,MROT_Months" +
                              ",MROT_Hours,MROT_Minutes,MROT_Seconds,MROT_Period,MROT_PeriodMultiplier,MROT_SimplePeriod,MROT_RecurrenceType" +
                              ",MROT_UseUniversalTime,MROT_UseAdvancedTime,MROT_ActiveDaySpecification,SourceType)" +
                        "VALUES " +
                              "(@ParentID,@CollectorGroupID,@Name,@Description,1,0,14,0,1,0" +
                              ",500,60000,0,@DataSource,@Units,@HighRange,@LowRange,0,0,0" +
                              ",0,1,0,90,1,0.5,0,@CollectionPeriodInMs,0" +
                              ",30000,1,0,0,0,0" +
                              ",0,0,0,0,1,10,1,0,0,0,0)";

                    SqlDbType[] hhParTypes =
                    {
                        SqlDbType.Int, SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, 
                        SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Float, SqlDbType.Float,
                        SqlDbType.Int
                    };

                    foreach (DataRow row in dt.Rows)
                    {
                        //проверяем на отмену операции
                        if (worker.CancellationPending)
                        {
                            workerEventArgs.Cancel = true;
                            return;
                        }

                        _pr++;
                        worker.ReportProgress(_pr, null);

                        string tagName = row["TagName"].ToString();
                        string path = row["Path"].ToString();
                        string endAddress = row["EndAddress"].ToString();

                        //до первой пустой строки tagName
                        if (string.IsNullOrEmpty(tagName))
                        {
                            worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                            worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                            return;
                        }

                        ////@ParentID (пока решено все теги создавать в корне)
                        string parId = null;
                        ////@CollectorGroupID
                        //colGroupId 
                        ////@Name
                        string name = tagName;
                        if (string.IsNullOrEmpty(path))
                            name = tagName;
                        ////@Description
                        string description = row["Description"].ToString();
                        ////@DataSource
                        string dataSource = endAddress;
                        ////@Units
                        string units = row["Units"].ToString();
                        ////@HighRange
                        string hiRange = "1000";
                        ////@LowRange
                        string loRange = "0";
                        //@CollectionPeriodInMs
                        string collectionRate = "5000"; //странно, к нему нет доступа в WorkBench, оставим по умолчанию


                        //делаем такую структуру ROSL.DNS.AI
                        //                        0    1  2
                        //                      null  DNS.AI
                        //                        0    1  2
                        //                      null null AI
                        //                        0    1  2
                        string[] pathSplited = { null, null, null };
                        int i = 0;
                        foreach (string s1 in path.Split('.'))
                        {
                            if (path.Split('.').Length == 3)
                            {
                                pathSplited[i] = s1;
                                i++;
                            }
                            if (path.Split('.').Length == 2)
                            {
                                i++;
                                pathSplited[i] = s1;
                            }
                            if (path.Split('.').Length == 1)
                            {
                                pathSplited[2] = s1;
                            }
                        }

                        //если не вписан адрес проходим мимо
                        if (string.IsNullOrEmpty(endAddress))
                        {
                            worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, т.к. в столбце \"EndAddress\" нет значения", name));
                            continue;
                        }

                        #region создание структуры (папки по пути Path)
                        ////ЭТАП 1. Если не существует папок по пути path, то создаем их (при условии, что мы будем в них писать что-то).
                        if (!string.IsNullOrEmpty(pathSplited[0]))
                        {
                            if (string.IsNullOrEmpty(GetHHParId(con, null, pathSplited[0])))
                                WriteHHFolder(con, null, pathSplited[0]);
                        }
                        if (!string.IsNullOrEmpty(pathSplited[1]))
                        {
                            string recParID = GetHHParId(con, null, pathSplited[0]);
                            if (string.IsNullOrEmpty(GetHHParId(con, recParID, pathSplited[1])))
                                WriteHHFolder(con, recParID, pathSplited[1]);
                        }
                        if (!string.IsNullOrEmpty(pathSplited[2]))
                        {
                            string recParID = GetHHParId(con, GetHHParId(con, null, pathSplited[0]), pathSplited[1]);
                            if (string.IsNullOrEmpty(GetHHParId(con, recParID, pathSplited[2])))
                                WriteHHFolder(con, recParID, pathSplited[2]);
                        }
                        #endregion

                        parId = GetHHParId(con, GetHHParId(con, GetHHParId(con, null, pathSplited[0]), pathSplited[1]), pathSplited[2]);

                        //если такой тег уже сущ-ет, идем дальше
                        if (GetHHItemId(con, parId, name) != null)
                            continue;

                        //пишем тег
                        string[] hhParValues = { parId, colGroupId, name, description, dataSource, units, hiRange, loRange, collectionRate };
                        rowsAffected += CreateInsertCommand(con, hhComText, hhParTypes, hhParValues) != null
                            ? CreateInsertCommand(con, hhComText, hhParTypes, hhParValues).ExecuteNonQuery()
                            : 0;
                    }
                    worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                    worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить базу " + dbName, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                worker.ReportProgress(_pr, "---->Не удалось обновить базу " + dbName + ": " + ex.Message);
                return;
            }
        }


        /// <summary>
        /// Прописывает все теги в базу AlarmWorX
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dbName">имя базы данных AlarmWorX</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void UpdateAWXItems(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, string dbName, DataTable dt, bool discrete)
        {
            worker.ReportProgress(_pr, "---->Вносим изменения в базу " + dbName + "...");
            int rowsAffected = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(conString + ";Initial Catalog=" + dbName))
                {
                    con.Open();

                    string configId = null;
                    //если уже есть активная база, берем ее Id и пишем туда
                    using (DataTable defConfData = new DataTable())
                    {
                        SqlDataAdapter adapter =
                            new SqlDataAdapter("SELECT [DefaultConfigID]  FROM [dbo].[AWX_ConfigsRoot]", con);
                        adapter.Fill(defConfData);
                        configId = defConfData.Rows[0]["DefaultConfigID"].ToString();
                    }
                    //иначе создаем новую с ID=1 и делаем ее активной
                    //тут возможен конфликт, но хрен с ним, в сообщении об ошибке будет конкретно все сказано
                    if (string.IsNullOrEmpty(configId))
                    {
                        const string configComText =
                            "INSERT INTO [dbo].[AWX_Config] (ConfigID,Name,UseDATimestamp,ScanPeriod,ConfigsRootID,ScanDisabledTags) VALUES (1,'Configuration',0,1000,1,1)";
                        SqlCommand command = new SqlCommand(configComText, con);
                        command.ExecuteNonQuery();
                        command.CommandText = @"UPDATE [dbo].[AWX_ConfigsRoot] SET DefaultConfigID=1";
                        command.ExecuteNonQuery();
                        configId = "1";
                    }

                    const string srcComText =
                        "INSERT INTO [dbo].[AWX_Source] (" +
                              "ConfigID,Name,Input1,BaseText,Enabled,LIM_RTNText,LIM_Deadband" +
                              ",LIM_HIHI_RequiresAck,LIM_HIHI_Severity,LIM_HIHI_Limit,LIM_HIHI_MsgText " +
                              ",LIM_HI_RequiresAck,LIM_HI_Severity,LIM_HI_Limit,LIM_HI_MsgText" +
                              ",LIM_LO_RequiresAck,LIM_LO_Severity,LIM_LO_Limit,LIM_LO_MsgText" +
                              ",LIM_LOLO_RequiresAck,LIM_LOLO_Severity,LIM_LOLO_Limit,LIM_LOLO_MsgText" +
                              ",DEV_Deadband,DEV_HIHI_RequiresAck,DEV_HIHI_Severity,DEV_HI_RequiresAck,DEV_HI_Severity" +
                              ",DEV_LO_RequiresAck,DEV_LO_Severity,DEV_LOLO_RequiresAck,DEV_LOLO_Severity" +
                              ",DIG_RTNText,DIG_Deadband,DIG_RequiresAck,DIG_Severity,DIG_Limit,DIG_MsgText" +
                              ",ROC_Deadband,ROC_RequiresAck,ROC_Severity" +
                              ",Description,RelatedValue1,RelatedValue2,RelatedValue3,RelatedValue4,RelatedValue5" +
                              ",RelatedValue6,ROC_AckOnRTN,DIG_AckOnRTN,LIM_AckOnRTN,DEV_AckOnRTN" +
                              ",RLM_Deadband,RLM_HIHI_RequiresAck,RLM_HIHI_Severity,RLM_HI_RequiresAck,RLM_HI_Severity" +
                              ",RLM_LO_RequiresAck,RLM_LO_Severity,RLM_LOLO_RequiresAck,RLM_LOLO_Severity,RLM_AckOnRTN" +
                              ",TLA_Deadband,TLA_RequiresAck,TLA_Severity,TLA_AckOnRTN)" +
                        "VALUES (" +
                              "@ConfigID,@Name,@Input1,@BaseText,@Enabled,@LIM_RTNText,0" +
                              ",1,999,@LIM_HIHI_Limit,@LIM_HIHI_MsgText,1,500,@LIM_HI_Limit,@LIM_HI_MsgText" +
                              ",1,500,@LIM_LO_Limit,@LIM_LO_MsgText,1,999,@LIM_LOLO_Limit,@LIM_LOLO_MsgText" +
                              ",0,0,500,0,500,0,500,0,500" +
                              ",@DIG_RTNText,0,1,500,@DIG_Limit,@DIG_MsgText" +
                              ",0,0,500,@Description,'0','0','0','0','0','0',0,0,0,0,0,0,500,0,500,0,500,0,500,0,0,0,500,0)";

                    SqlDbType[] srcParTypes =
                    {
                        SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NText, SqlDbType.NVarChar, SqlDbType.NVarChar,
                        SqlDbType.NVarChar
                        , SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar
                        , SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar
                        , SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.NVarChar
                        , SqlDbType.NVarChar
                    };

                    foreach (DataRow row in dt.Rows)
                    {
                        //проверяем на отмену операции
                        if (worker.CancellationPending)
                        {
                            workerEventArgs.Cancel = true;
                            return;
                        }

                        _pr++;
                        worker.ReportProgress(_pr, null);

                        string tagName = row["TagName"].ToString();
                        string path = row["Path"].ToString();
                        string endAddress = row["EndAddress"].ToString();

                        //до первой пустой строки tagName
                        if (string.IsNullOrEmpty(tagName))
                        {
                            worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                            worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                            return;
                        }

                        ////@ConfigID
                        //configId
                        ////@Name
                        string name = path.Replace(".", "_") + "_" + tagName;
                        if (string.IsNullOrEmpty(path))
                            name = tagName;
                        ////@Input1
                        string input1 = endAddress;
                        ////@BaseText
                        string baseText = null;
                        ////@Enabled
                        string enabled;
                        ////@LIM_RTNText
                        string limRTNText = null;
                        ////@LIM_HIHI_Limit
                        string hihiLimit = null;
                        ////@LIM_HIHI_MsgText
                        string hihiText = null;
                        ////@LIM_HI_Limit
                        string hiLimit = null;
                        ////@LIM_HI_MsgText
                        string hiText = null;
                        ////@LIM_LO_Limit
                        string loLimit = null;
                        ////@LIM_LO_MsgText
                        string loText = null;
                        ////@LIM_LOLO_Limit
                        string loloLimit = null;
                        ////@LIM_LOLO_MsgText
                        string loloText = null;
                        ////@DIG_RTNText
                        string digRTNText = null;
                        ////@DIG_Limit
                        string digLimit = null;
                        ////@DIG_MsgText
                        string digText = null;
                        ////@Description
                        string description = row["Description"].ToString();

                        //если такой тег уже сущ-ет, идем дальше
                        if (GetAWXItemId(con, configId, name) != null)
                            continue;
                        //если не вписан адрес, тоже проходим мимо
                        if (string.IsNullOrEmpty(endAddress))
                        {
                            worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, т.к. в столбце \"EndAddress\" нет значения", name));
                            continue;
                        }

                        if (!discrete)
                        {
                            string gender = row["Gender"].ToString();
                            switch (gender)
                            {
                                case "M":
                                    loloText = " аварийный, низкий";
                                    loText = " низкий";
                                    hiText = " высокий";
                                    hihiText = " аварийный, высокий";
                                    break;
                                case "F":
                                    loloText = " аварийная, низкая";
                                    loText = " низкая";
                                    hiText = " высокая";
                                    hihiText = " аварийная, высокая";
                                    break;
                                case "N":
                                    loloText = " аварийное, низкое";
                                    loText = " низкое";
                                    hiText = " высокое";
                                    hihiText = " аварийное, высокое";
                                    break;
                                default:
                                    worker.ReportProgress(_pr, string.Format(
                                        "Тег {0} пропущен, не удалось определить текст сигнализации. В поле \"Gender\" должно быть значение M,F или N)", name));
                                    continue;
                            }
                            //пишем параметр
                            baseText = description;
                            enabled = "0"; //по умолч. выключаем аналог-е сигнализации
                            limRTNText = " в норме";
                            hihiLimit = row["HiHi"].ToString();
                            hiLimit = row["Hi"].ToString();
                            loLimit = row["Lo"].ToString();
                            loloLimit = row["LoLo"].ToString();

                            if (string.IsNullOrEmpty(hihiLimit))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, не указано значение \"HiHi\"", name));
                                continue;
                            }
                            if (string.IsNullOrEmpty(hiLimit))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, не указано значение \"Hi\"", name));
                                continue;
                            }
                            if (string.IsNullOrEmpty(loLimit))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, не указано значение \"Lo\"", name));
                                continue;
                            }
                            if (string.IsNullOrEmpty(loloLimit))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, не указано значение \"LoLo\"", name));
                                continue;
                            }

                            string[] srcParValues =
                            {
                                configId, name, input1, baseText, enabled, limRTNText,
                                hihiLimit, hihiText, hiLimit, hiText, loLimit, loText, loloLimit, loloText,
                                digRTNText, digLimit, digText, description
                            };
                            rowsAffected += CreateInsertCommand(con, srcComText, srcParTypes, srcParValues) != null
                                ? CreateInsertCommand(con, srcComText, srcParTypes, srcParValues).ExecuteNonQuery()
                                : 0;
                        }
                        else
                        {
                            enabled = "1"; //по умолч. дискретные обязательно включены
                            digRTNText = row["RTNText"].ToString();
                            baseText = row["BaseText"].ToString();
                            string split = row["Split"].ToString();
                            //нужно ли разбить на 2 сиг-и?
                            switch (split)
                            {
                                //нет
                                case "0":
                                    digLimit = "1";
                                    digText = row["Alm1MsgText"].ToString();
                                    string[] srcParValues =
                                        {
                                            configId, name, input1, baseText, enabled, limRTNText,
                                            hihiLimit, hihiText, hiLimit, hiText, loLimit, loText, loloLimit, loloText,
                                            digRTNText, digLimit, digText, description
                                        };
                                    rowsAffected += CreateInsertCommand(con, srcComText, srcParTypes, srcParValues) != null
                                        ? CreateInsertCommand(con, srcComText, srcParTypes, srcParValues).ExecuteNonQuery()
                                        : 0;
                                    break;
                                //да
                                case "1":
                                    //сиг-я при 1
                                    //если такой тег уже сущ-ет, идем дальше
                                    if (GetAWXItemId(con, configId, name + "_on") != null)
                                        continue;
                                    digLimit = "1";
                                    digText = row["Alm1MsgText"].ToString();
                                    srcParValues = new string[]
                                        {
                                            configId, name + "_on", input1, baseText, enabled, limRTNText,
                                            hihiLimit, hihiText, hiLimit, hiText, loLimit, loText, loloLimit, loloText,
                                            digRTNText, digLimit, digText, description
                                        };
                                    rowsAffected += CreateInsertCommand(con, srcComText, srcParTypes, srcParValues) != null
                                        ? CreateInsertCommand(con, srcComText, srcParTypes, srcParValues).ExecuteNonQuery()
                                        : 0;
                                    //сиг-я при 0
                                    //если такой тег уже сущ-ет, идем дальше
                                    if (GetAWXItemId(con, configId, name + "_off") != null)
                                        continue;
                                    digLimit = "0";
                                    digText = row["Alm2MsgText"].ToString();
                                    srcParValues = new string[]
                                        {
                                            configId, name + "_off", input1, baseText, enabled, limRTNText,
                                            hihiLimit, hihiText, hiLimit, hiText, loLimit, loText, loloLimit, loloText,
                                            digRTNText, digLimit, digText, description
                                        };
                                    rowsAffected += CreateInsertCommand(con, srcComText, srcParTypes, srcParValues) != null
                                        ? CreateInsertCommand(con, srcComText, srcParTypes, srcParValues).ExecuteNonQuery()
                                        : 0;
                                    break;
                                default:
                                    worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, в столбце \"Split\" должна быть 1 или 0", name));
                                    continue;
                            }
                        }
                    }
                    worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                    worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить базу " + dbName, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                worker.ReportProgress(_pr, "---->Не удалось обновить базу " + dbName + ": " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Прописывает все теги в базу UDM
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dbName">имя базы данных UDM</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void UpdateUDMItems(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, string dbName, DataTable dt, bool discrete)
        {
            worker.ReportProgress(_pr, "---->Вносим изменения в базу " + dbName + "...");
            int rowsAffected = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(conString + ";Initial Catalog=" + dbName))
                {
                    con.Open();

                    //для улучшения быстродействия и уменьшения кол-ва обращений в БД
                    //пишем во временный контейнер пройденные пути "path" и лезем в базу только если текущего пути
                    //нет в нашем контейнере.
                    //...вышло на 18% быстрее при заполнении пустой табл, и на 24% при добавлении в сущ-ю при кол-ве тегов = 390
                    List<string> passedPaths = new List<string>(1000);

                    const string regComText =
                        "INSERT INTO [dbo].[DMG_RegisterItems] " +
                              "(ParentID,Name,Description,Type,InputPattern,InputTag,InputDataType,ScanRate,UseInitialValue,InitialValue," +
                              "OutputTag,RefreshOutput,RefreshRate,OutputEnabled,ReleaseTagsNotInUse,Writable,PropagationStyle,Delay)" +
                        "VALUES( @ParentID,@Name,@Description,@Type,@InputPattern," +
                              "@InputTag,@Type,@ScanRate,@UseInitialValue,@InitialValue,@OutputTag,0,5,@OutputEnabled,1,@Writable,0,0)";
                    SqlDbType[] regParTypes =
                        {
                            SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Int, SqlDbType.SmallInt,
                            SqlDbType.NText, SqlDbType.Int, SqlDbType.Bit, SqlDbType.NVarChar, SqlDbType.NText,
                            SqlDbType.Bit, SqlDbType.Bit
                        };
                    const string expComText =
                        "INSERT INTO [dbo].[DMG_ExpressionItems] " +
                              "(ParentID,Name,Description,Type,DefaultScanRate,ReadExpression,UseTriggerTags,SendInitialUpdate,SendAllUpdates,EnableWriteExpression)" +
                        "VALUES( @ParentID,@Name,@Description,@Type,@DefaultScanRate,@ReadExpression,0,0,0,0)";
                    SqlDbType[] expParTypes =
                            {
                                SqlDbType.Int, SqlDbType.NVarChar, SqlDbType.NVarChar, SqlDbType.Int,
                                SqlDbType.Int, SqlDbType.NText
                            };
                    foreach (DataRow row in dt.Rows)
                    {
                        //проверяем на отмену операции
                        if (worker.CancellationPending)
                        {
                            workerEventArgs.Cancel = true;
                            return;
                        }

                        _pr++;
                        worker.ReportProgress(_pr, null);
                        //до первой пустой строки
                        if (string.IsNullOrEmpty(row["TagName"].ToString()))
                        {
                            worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                            worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                            return;
                        }
                        //@Type: 10 - String
                        //       1 - Float
                        //       3 - Boolean
                        string tagName = row["TagName"].ToString();
                        //// @ParentID (can be null)
                        string regParId = null;
                        //// @Name
                        string name;
                        //// @Description
                        string description;
                        //// @Type
                        string type;
                        //// @InputPattern
                        string inputPattern;
                        //// @InputTag (can be null)
                        string inputTag = null;
                        //// @ScanRate
                        string scanRate = row["ScanRate"].ToString();
                        //// @UseInitialValue
                        string useInitialValue;
                        //// @InitialValue (can be null)
                        string initialValue = null;
                        //// @OutputEnabled
                        string outputEnabled = "false";
                        //// @Writable
                        string writable;
                        //// @OutputTag (can be null)
                        string outputTag = null;

                        string path = row["Path"].ToString();
                        //если больше 3 уровней, пропускаем тег (пользователь предупрежден об этом еще при создании папок)
                        if (path.Split('.').Length > 3)
                        {
                            worker.ReportProgress(_pr, string.Format(
                                "Максимальное количество уровней - 3, тег {0}{1} пропущен", string.IsNullOrEmpty(path) ? path : path + "\\", tagName));
                            continue;
                        }

                        //пропускаем строку если не заполнено поле Address, Path или ScanRate
                        if (string.IsNullOrEmpty(row["Address"].ToString()) || string.IsNullOrEmpty(path) ||
                            string.IsNullOrEmpty(scanRate))
                        {
                            string emptyCol = string.IsNullOrEmpty(row["Address"].ToString())
                                ? "Address"
                                : string.IsNullOrEmpty(path)
                                    ? "Path"
                                    : string.IsNullOrEmpty(scanRate) ? "ScanRate"
                                        : null;
                            worker.ReportProgress(_pr, string.Format(
                                "Тег {0}{1} пропущен, т.к. в столбце \"{2}\" отсутствует информация", string.IsNullOrEmpty(path) ? path : path + "\\", tagName, emptyCol));
                            continue;
                        }
                        //делаем такую структуру ROSL.DNS.AI
                        //                        0    1  2
                        //                      null  DNS.AI
                        //                        0    1  2
                        //                      null null AI
                        //                        0    1  2
                        string[] pathSplited = { null, null, null };
                        int i = 0;
                        foreach (string s1 in path.Split('.'))
                        {
                            if (path.Split('.').Length == 3)
                            {
                                pathSplited[i] = s1;
                                i++;
                            }
                            if (path.Split('.').Length == 2)
                            {
                                i++;
                                pathSplited[i] = s1;
                            }
                            if (path.Split('.').Length == 1)
                            {
                                pathSplited[2] = s1;
                            }
                        }

                        #region создание структуры (папки по пути Path)

                        //проверяем нужно ли нам будет что то писать вообще в Registers и Expressions
                        bool wrReg = false;
                        bool wrExp = false;
                        bool EO = false;
                        if (!string.IsNullOrEmpty(row["Address"].ToString()))
                        {
                            if (!discrete)
                            {
                                EO = (row["EO"].ToString().Trim() == "1");
                                wrExp = true;
                                wrReg = !EO;
                            }
                            else
                            {
                                wrReg = true;
                            }
                        }
                        if (!discrete)
                        {
                            if (!string.IsNullOrEmpty(row["Description"].ToString())
                                || !string.IsNullOrEmpty(row["Units"].ToString())
                                || !string.IsNullOrEmpty(row["SclMin"].ToString())
                                || !string.IsNullOrEmpty(row["SclMax"].ToString())
                                || !string.IsNullOrEmpty(row["Broken"].ToString())
                                || !string.IsNullOrEmpty(row["State"].ToString()))
                                wrReg = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(row["Description"].ToString()))
                                wrReg = true;
                        }

                        ////ЭТАП 1. Если не существует папок по пути path, то создаем их (при условии, что мы будем в них писать что-то).
                        if (!string.IsNullOrEmpty(pathSplited[0]))
                        {
                            if (wrReg && string.IsNullOrEmpty(GetUDMParId(con, null, pathSplited[0], 0)))
                                WriteUDMFolder(con, null, pathSplited[0], 0);
                            if (wrExp && string.IsNullOrEmpty(GetUDMParId(con, null, pathSplited[0], 1)))
                                WriteUDMFolder(con, null, pathSplited[0], 1);
                        }
                        if (!string.IsNullOrEmpty(pathSplited[1]))
                        {
                            if (wrReg && string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0)))
                                WriteUDMFolder(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0);
                            if (wrExp && string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1)))
                                WriteUDMFolder(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1);
                        }
                        if (!string.IsNullOrEmpty(pathSplited[2]))
                        {
                            if (wrReg && string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0), pathSplited[2], 0)))
                                WriteUDMFolder(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0), pathSplited[2], 0);
                            if (wrExp && string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1), pathSplited[2], 1)))
                                WriteUDMFolder(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1), pathSplited[2], 1);
                        }
                        #endregion

                        ////ЭТАП 2. Пишем в папку DMG_RegisterItems параметры тега (Description, Units, LoRange, HiRange, Broken, State, Val).
                        regParId = GetUDMParId(con,
                            GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0),
                            pathSplited[2], 0);
                        if (wrReg &&
                            string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0), pathSplited[2], 0), tagName, 0)))
                            WriteUDMFolder(con, regParId, tagName, 0);

                        regParId = GetUDMParId(con,
                            GetUDMParId(con,
                                GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 0), pathSplited[1], 0),
                                pathSplited[2], 0), tagName, 0);
                        //пишем Description
                        name = "Description";
                        description = row["Description"].ToString();
                        type = "10";
                        inputPattern = "1";
                        useInitialValue = "true";
                        initialValue = row["Description"].ToString();
                        writable = "false";
                        string[] regParValues =
                        {
                            regParId, name, description, type, inputPattern, inputTag, scanRate,
                            useInitialValue, initialValue, outputTag, outputEnabled, writable
                        };

                        if (!string.IsNullOrEmpty(initialValue) &&
                            string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                            rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                : 0;
                        //только для аналоговых параметров
                        if (!discrete)
                        {
                            //пишем Units
                            name = "Units";
                            description = row["Description"].ToString() + " (единицы измерения)";
                            type = "10";
                            inputPattern = "1";
                            useInitialValue = "true";
                            initialValue = row["Units"].ToString();
                            writable = "false";
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(initialValue) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                            //пишем LoRange
                            name = "LoRange";
                            description = row["Description"].ToString() + " (нижняя граница (для графика))";
                            type = "1";
                            inputPattern = "1";
                            useInitialValue = "true";
                            initialValue = row["SclMin"].ToString();
                            writable = "true";
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(initialValue) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                            //пишем HiRange
                            name = "HiRange";
                            description = row["Description"].ToString() + " (верхняя граница (для графика))";
                            type = "1";
                            inputPattern = "1";
                            useInitialValue = "true";
                            initialValue = row["SclMax"].ToString();
                            writable = "true";
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(initialValue) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                            //пишем Broken
                            name = "Broken";
                            description = row["Description"].ToString() + " (отказ датчика)";
                            type = "3";
                            inputPattern = "0";
                            inputTag = string.IsNullOrEmpty(row["Broken"].ToString().Trim()) ? null : 
                                (row["Broken"].ToString().StartsWith("@") || row["Broken"].ToString().StartsWith("db")) ? row["Broken"].ToString() : "@" + row["Broken"].ToString(); 
                            useInitialValue = "false";
                            writable = "false";
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(inputTag) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                            //пишем State
                            name = "State";
                            description = row["Description"].ToString() + " (состояние датчика)";
                            type = "3";
                            inputPattern = "0";
                            inputTag = string.IsNullOrEmpty(row["State"].ToString().Trim()) ? null : 
                                (row["State"].ToString().StartsWith("@") || row["State"].ToString().StartsWith("db")) ? row["State"].ToString() : "@" + row["State"].ToString();
                            useInitialValue = "false";
                            initialValue = null;
                            writable = "false";
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(inputTag) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                        }
                        //пишем Val в Registers
                        if (wrReg)
                        {
                            name = "Val";
                            description = row["Description"].ToString() + " (значение параметра)";
                            type = discrete ? "3" : "0";
                            inputPattern = "0";
                            inputTag = string.IsNullOrEmpty(row["Address"].ToString().Trim())? null :
                                (row["Address"].ToString().StartsWith("@") || row["Address"].ToString().StartsWith("db")) ? row["Address"].ToString() : "@" + row["Address"].ToString();
                            useInitialValue = "false";
                            initialValue = null;
                            writable =
                                discrete ?
                                (row["RW"].ToString() == "RW" || row["RW"].ToString() == "R\\W" ||
                                       row["RW"].ToString() == "R/W") ?
                                       "true"
                                        : "false"
                                    : "false";
                            if (writable == "true")
                            {
                                outputTag = inputTag;
                                outputEnabled = "true";
                            }
                            regParValues = new string[]
                            {
                                regParId, name, description, type, inputPattern, inputTag, scanRate, useInitialValue,
                                initialValue, outputTag, outputEnabled, writable
                            };
                            if (!string.IsNullOrEmpty(inputTag) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, regParId, name, 0)))
                                rowsAffected += CreateInsertCommand(con, regComText, regParTypes, regParValues) != null
                                    ? CreateInsertCommand(con, regComText, regParTypes, regParValues).ExecuteNonQuery()
                                    : 0;
                        }
                        ////ЭТАП 3. Пишем в DMG_ExpressionItems, при условии, что это необходимо.
                        if (wrExp)
                        {
                            //// @ParentID (can be null)
                            string expParId = null;
                            /////@Name,@Description,@Type,@DefaultScanRate переменные остаются прежние
                            ////@ReadExpression (can be null)
                            string readExp = null;

                            expParId = GetUDMParId(con,
                                GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1),
                                pathSplited[2], 1);
                            if (wrExp &&
                                string.IsNullOrEmpty(GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1), pathSplited[2], 1), tagName, 1)))
                                WriteUDMFolder(con, expParId, tagName, 1);
                            expParId = GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, GetUDMParId(con, null, pathSplited[0], 1), pathSplited[1], 1), pathSplited[2], 1), tagName, 1);

                            name = "Val";
                            description = row["Description"].ToString();
                            type = "0";
                            readExp = EO ?
                                !string.IsNullOrEmpty(row["Address"].ToString())
                                ? row["Address"].ToString().Trim().StartsWith("@")
                                    ? "x={{" + row["Address"] + "}}"
                                    : row["Address"].ToString().Trim().StartsWith("{{")
                                        ? "x=" + row["Address"]
                                        : row["Address"].ToString()
                                : null
                                : "x={{@rgs64:" + row["Path"].ToString() + "." + row["TagName"] + ".Val.Value}}";

                            string[] expParValues = { expParId, name, description, type, scanRate, readExp };
                            if (!string.IsNullOrEmpty(readExp) &&
                                string.IsNullOrEmpty(GetUDMItemId(con, expParId, name, 1)))
                                rowsAffected += CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                    ? CreateInsertCommand(con, expComText, expParTypes, expParValues).ExecuteNonQuery()
                                    : 0;
                        }
                    }
                    worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                    worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось обновить базу " + dbName, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                worker.ReportProgress(_pr, "---->Не удалось обновить базу " + dbName + ": " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Пишет в таблицу DMG_RegisterFolders или DMG_ExpressionFolders указанную папку
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="recParId">id родительской папки (RecursiveParentID)</param>
        /// <param name="name">имя (Name)</param>
        /// <param name="table">0-DMG_RegisterFolders; 1-DMG_ExpressionFolders</param>
        private static void WriteUDMFolder(SqlConnection con, string recParId, string name, int table)
        {
            string comText =
                table == 0 ? @"INSERT INTO [dbo].[DMG_RegisterFolders] (RecursiveParentID,Name) VALUES (@RecursiveParentID,@Name)"
                : table == 1 ? @"INSERT INTO [dbo].[DMG_ExpressionFolders] (RecursiveParentID,Name) VALUES (@RecursiveParentID,@Name)"
                : null;
            if (string.IsNullOrEmpty(comText))
            {
                MessageBox.Show(string.Format("Метод WriteFolders(SqlConnection con, string recParId, string name, int table) " +
                                              "\n параметр table принимает значения только 0 или 1" +
                                              "\n папка с именем {0} parentId {1} не записана в базу", name, recParId));
                return;
            }
            SqlDbType[] parTypes = { SqlDbType.Int, SqlDbType.NVarChar };
            string[] parValues = { recParId, name };
            int rowsAffected = CreateInsertCommand(con, comText, parTypes, parValues) != null
                ? CreateInsertCommand(con, comText, parTypes, parValues).ExecuteNonQuery()
                : 0;
        }

        /// <summary>
        /// Пишет в таблицу HH_Folders указанную папку
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="recParId">id родительской папки (RecursiveParentID)</param>
        /// <param name="name">имя (Name)</param>
        private static void WriteHHFolder(SqlConnection con, string recParId, string name)
        {
            string comText = @"INSERT INTO [dbo].[HH_Folders] (RecursiveParentID,Name) VALUES (@RecursiveParentID,@Name)";
            SqlDbType[] parTypes = { SqlDbType.Int, SqlDbType.NVarChar };
            string[] parValues = { recParId, name };
            int rowsAffected = CreateInsertCommand(con, comText, parTypes, parValues) != null
                ? CreateInsertCommand(con, comText, parTypes, parValues).ExecuteNonQuery()
                : 0;
        }

        /// <summary>
        /// Создает объект SQLCommand
        /// </summary>
        /// <param name="connection">активное подключение SQLConnection</param>
        /// <param name="comText">текст комманды</param>
        /// <param name="types">типы данных параметров команды (ex.:SqlDbType.Int, SqlDbType.NVarChar)</param>
        /// <param name="values">значения параметров команды (ex.: null, "parName")</param>
        /// <returns>возвращает SqlCommand в случае усхеха, null в случае ошибки</returns>
        private static SqlCommand CreateInsertCommand(SqlConnection connection, string comText, SqlDbType[] types, string[] values)
        {
            try
            {
                //парсим текст комманды, вытаскиваем имена тегов
                string[] parameters =
                    comText.IndexOf("VALUES (", StringComparison.OrdinalIgnoreCase) != -1 ?
                        comText.Substring(comText.IndexOf("VALUES (", StringComparison.OrdinalIgnoreCase) + 8,
                        comText.Length -
                        comText.IndexOf("VALUES (", StringComparison.OrdinalIgnoreCase) - 9)
                        .Trim()
                        .Split(',')
                        .Where(s => s.IndexOf("@", StringComparison.OrdinalIgnoreCase) != -1)
                        .ToArray()
                    : comText.IndexOf("VALUES(", StringComparison.OrdinalIgnoreCase) != -1 ?
                         comText.Substring(comText.IndexOf("VALUES(", StringComparison.OrdinalIgnoreCase) + 8,
                         comText.Length -
                         comText.IndexOf("VALUES(", StringComparison.OrdinalIgnoreCase) - 9)
                         .Trim()
                         .Split(',')
                         .Where(s => s.IndexOf("@", StringComparison.OrdinalIgnoreCase) != -1)
                         .ToArray()
                   : null;
                if (parameters == null)
                {
                    MessageBox.Show(string.Format("Не верный текст InsertCommand. \n{0}", comText), "Невозможно извлечь параметры из InsertCommand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                string[] uniqueParameters = parameters.Distinct().ToArray();

                if (uniqueParameters.Length != types.Length || uniqueParameters.Length != values.Length)
                {
                    MessageBox.Show(
                        "Количество типов данных или значений, переданных методу CreateInsertCommand(string comText, SqlDbType[] types, string[] values), не совпадает с количеством самих параметров" +
                        "\n\nТекст SqlCommand - " + comText);
                    return null;
                }

                SqlCommand command = new SqlCommand(comText, connection);
                for (int i = 0; i < uniqueParameters.Length; i++)
                {
                    command.Parameters.Add(new SqlParameter(uniqueParameters[i], types[i]));
                    if (string.IsNullOrEmpty(values[i]))
                        command.Parameters[uniqueParameters[i]].Value = DBNull.Value;
                    else if (types[i] == SqlDbType.Bit)
                    {
                        bool b = Convert.ToBoolean(values[i]);
                        command.Parameters[uniqueParameters[i]].Value = b;
                    }
                    else
                        command.Parameters[uniqueParameters[i]].Value = values[i];
                }
                return command;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "Метод: CreateInsertCommand(string comText, SqlDbType[] types, string[] values) \n\nТекст ошибки: {0} \n\nТекст SqlCommand: {1}",
                    ex.Message, comText), "Не удалось создать SqlCommand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Получает значение ID заданной папки
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="recParId">значение столбца RecursiveParentID</param>
        /// <param name="name">значение столбца Name</param>
        /// <param name="table">0-DMG_RegisterFolders; 1-DMG_ExpressionFolders</param>
        /// <returns>ID в формате string, null если параметр не найден или в случае ошибки выполнения</returns>
        private static string GetUDMParId(SqlConnection con, string recParId, string name, int table)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand selectCommand =
                    table == 0
                        ? new SqlCommand("SELECT [ID],[RecursiveParentID],[Name] FROM [dbo].[DMG_RegisterFolders]", con)
                        : table == 1
                            ? new SqlCommand("SELECT [ID],[RecursiveParentID],[Name] FROM [dbo].[DMG_ExpressionFolders]",
                                con)
                            : null;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() != name) continue;
                    if (dt.Rows[i]["RecursiveParentID"].ToString() ==
                        (string.IsNullOrEmpty(recParId) ? string.Empty : recParId))
                        return dt.Rows[i]["ID"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetParId(SqlConnection con, string recParId, string name, int table) \n\nТекст ошибки: {0} \n\nИмя параметра: {1}",
                    ex.Message, name), "Не удалось создать SqlCommand");
                return null;
            }
        }

        /// <summary>
        /// Получает значение ID указанного параметра (из базы UDM)
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="parId">значение столбца ParentID</param>
        /// <param name="name">значение столбца Name</param>
        /// <param name="table">0-DMG_RegisterItems; 1-DMG_ExpressionItems</param>
        /// <returns>ID в формате string, null если параметр не найден и в случае ошибки выполнения</returns>
        private static string GetUDMItemId(SqlConnection con, string parId, string name, int table)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand selectCommand =
                    table == 0
                        ? new SqlCommand("SELECT [ID],[ParentID],[Name] FROM [dbo].[DMG_RegisterItems]", con)
                        : table == 1
                            ? new SqlCommand("SELECT [ID],[ParentID],[Name] FROM [dbo].[DMG_ExpressionItems]",
                                con)
                            : null;
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() != name) continue;
                    if (dt.Rows[i]["ParentID"].ToString() ==
                        (string.IsNullOrEmpty(parId) ? string.Empty : parId))
                        return dt.Rows[i]["ID"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetUDMItemId(SqlConnection con, string parId, string name, int table) \n\nТекст ошибки: {0} \n\nИмя параметра: {1}",
                    ex.Message, name), "Не удалось получить ID из таблицы dbo.DMG_RegisterItems\\dbo.DMG_ExpressionItems");
                return null;
            }
        }

        /// <summary>
        /// Получает значение ID указанного параметра (из базы AlarmWorX)
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="confId">значение столбца ConfigID</param>
        /// <param name="name">значение столбца Name</param>
        /// <returns>ID в формате string, null если параметр не найден и в случае ошибки выполнения</returns>
        private static string GetAWXItemId(SqlConnection con, string confId, string name)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand selectCommand = new SqlCommand("SELECT [SourceID],[ConfigID],[Name] FROM [dbo].[AWX_Source]", con);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() != name) continue;
                    if (dt.Rows[i]["ConfigID"].ToString() == confId)
                        return dt.Rows[i]["SourceID"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetAWXItemId(SqlConnection con, string confId, string name) \n\nТекст ошибки: {0} \n\nИмя параметра: {1}",
                    ex.Message, name), "Не удалось получить ID из таблицы dbo.AWX_Source");
                return null;
            }
        }

        /// <summary>
        /// Получает значение ID указанного параметра (из базы HyperHistorian)
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="parId">значение столбца ParentID</param>
        /// <param name="name">значение столбца Name</param>
        /// <returns>ID в формате string, null если параметр не найден и в случае ошибки выполнения</returns>
        private static string GetHHItemId(SqlConnection con, string parId, string name)
        {
            try
            {
                if (parId == null)
                    parId = string.Empty;
                DataTable dt = new DataTable();
                SqlCommand selectCommand = new SqlCommand("SELECT [ID],[ParentID],[Name] FROM [dbo].[HH_Tags]", con);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() != name) continue;
                    if (dt.Rows[i]["ParentID"].ToString() ==
                        (string.IsNullOrEmpty(parId) ? string.Empty : parId))
                        return dt.Rows[i]["ID"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetHHItemId(SqlConnection con, string parId, string name) \n\nТекст ошибки: {0} \n\nИмя параметра: {1}",
                    ex.Message, name), "Не удалось получить ID из таблицы dbo.HH_Tags");
                return null;
            }
        }

        /// <summary>
        /// Получает значение ID заданной папки
        /// </summary>
        /// <param name="con">активное подключение SQLConnection</param>
        /// <param name="recParId">значение столбца RecursiveParentID</param>
        /// <param name="name">значение столбца Name</param>
        /// <returns>ID в формате string, null если параметр не найден или в случае ошибки выполнения</returns>
        private static string GetHHParId(SqlConnection con, string recParId, string name)
        {
            try
            {
                DataTable dt = new DataTable();
                SqlCommand selectCommand = new SqlCommand("SELECT [ID],[RecursiveParentID],[Name] FROM [dbo].[HH_Folders]", con);
                SqlDataAdapter adapter = new SqlDataAdapter(selectCommand);
                adapter.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() != name) continue;
                    if (dt.Rows[i]["RecursiveParentID"].ToString() ==
                        (string.IsNullOrEmpty(recParId) ? string.Empty : recParId))
                        return dt.Rows[i]["ID"].ToString();
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetParId(SqlConnection con, string recParId, string name) \n\nТекст ошибки: {0} \n\nИмя параметра: {1}",
                    ex.Message, name), "Не удалось создать SqlCommand");
                return null;
            }
        }
    }
}
