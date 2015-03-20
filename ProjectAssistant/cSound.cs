using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProjectAssistant
{
    class cSound
    {
        //строка подключения к базам Access (без указания БД)
        private static string _conString { get { return "provider=Microsoft.ACE.OLEDB.12.0;data source="; } }
        private static int _pr;

        public static void Create(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt,
            bool crSoundTxt, bool crScriptTxt, bool crUdmBase, bool crScriptBase, string udm32BaseName, string swx64BaseName, bool discrete)
        {
            if (dt == null || string.IsNullOrEmpty(dt.Rows[0]["TagName"].ToString()))
            {
                MessageBox.Show("Добавьте в таблицу теги, которые хотите внести в базу", "Нет данных в таблице",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (!crSoundTxt && !crScriptTxt && !crUdmBase && !crScriptBase)
            {
                MessageBox.Show("Выберите необходимый пункт.", "Нет задачи",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            _pr = 0;

            if (crSoundTxt) { CreateSoundText(worker, workerEventArgs, dt, discrete); }
            if (crScriptTxt) { CreateVbaScript(worker, workerEventArgs, dt, discrete); }
            if (crUdmBase) { UpdateUdmBase(worker, workerEventArgs, dt, udm32BaseName, discrete); }
            if (crScriptBase) { UpdateSwxBase(worker, workerEventArgs, dt, swx64BaseName, discrete); }
        }

        /// <summary>
        /// Обновляет базу данных ScriptWorX (ScriptWorX.mdb)
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="dbName">путь к БД</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void UpdateSwxBase(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt, string dbName, bool discrete)
        {
            worker.ReportProgress(_pr, "---->Приступаем к обновлению базы " + dbName + "...");

            int rowsAffected = 0;
            if (!File.Exists(dbName))
            {
                MessageBox.Show(string.Format("Не найдена база {0}", dbName), "Не найдена база данных UDM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, string.Format("Не найдена база \"{0}\"", dbName));
                return;
            }
            try
            {
                using (OleDbConnection con = new OleDbConnection(_conString + dbName))
                {
                    con.Open();

                    const string swxTableName = "SWX_Scripts";
                    const string swxComText =
                        "INSERT INTO SWX_Scripts (" +
                        "  [ID], [ParentID], [Name], [Sequence], [Description], [Enabled], [VBAScript]" +
                        ", [OverrideWatchDog], [WatchDogTimeout], [TriggerName], [TriggerDescription]) " +
                        "VALUES (" +
                        "  @ID, @ParentID, @Name, @Sequence, @Description, @Enabled, " +
                        "@VBAScript, @OverrideWatchDog, @WatchDogTimeout, @TriggerName, @TriggerDescription)";

                    OleDbType[] swxParTypes =
                    {
                        OleDbType.Integer, OleDbType.Integer, OleDbType.BSTR, OleDbType.Integer, OleDbType.BSTR,
                        OleDbType.Boolean,
                        OleDbType.BSTR, OleDbType.Boolean, OleDbType.Integer, OleDbType.BSTR, OleDbType.BSTR
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
                        worker.ReportProgress(_pr);

                        string tagName = row["TagName"].ToString();
                        string path = row["Path"].ToString().Replace(".", "_");
                        string endAddress = row["EndAddress"].ToString();

                        //// переменные для SWX_Scripts
                        ////@ID, 
                        int swxId;
                        ////@ParentID
                        // "1"
                        //@Name
                        string name = path + "_" + tagName;
                        ////@Sequence
                        int swxSequence;
                        ////@Description
                        string description = row["Description"].ToString();
                        ////@Enabled
                        // "1";
                        ////@VBAScript
                        string vbaScript;
                        ////@OverrideWatchDog
                        // "false";
                        ////@WatchDogTimeout
                        // "30";
                        ////@TriggerName
                        string trName;
                        ////@TriggerDescription
                        // null;

                        //до первой пустой строки tagName
                        if (string.IsNullOrEmpty(tagName))
                        {
                            worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                            worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                            return;
                        }

                        swxId = GetId(con, swxTableName);
                        swxSequence = GetSeq(con, swxTableName);

                        //аналоговые
                        if (!discrete)
                        {
                            //чот называется, защита от дурака
                            string hihiLimit = row["HiHi"].ToString();
                            string hiLimit = row["Hi"].ToString();
                            string loLimit = row["Lo"].ToString();
                            string loloLimit = row["LoLo"].ToString();
                            string gender = row["Gender"].ToString();

                            if (string.IsNullOrEmpty(hihiLimit) || string.IsNullOrEmpty(hiLimit)
                                || string.IsNullOrEmpty(loLimit) || string.IsNullOrEmpty(loloLimit)
                                || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(endAddress))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} будет добавлен в базу UDM, но, " +
                                                                         "используя текущие данные его невозможно было добавить в базу AlarmWorx, убедитесь наличии Alarm'а!",
                                    name));
                            }

                            //LOLO
                            vbaScript = name + "_LOLO_FirstPlay";
                            trName = "trg_data:" + name + "_LOLO";
                            string[] swxParValues =
                            {
                                swxId.ToString(), "1", name + "_LOLO", swxSequence.ToString(),
                                description + " (аварийная, нижняя граница)", "true",
                                vbaScript, "false", "30", trName, null
                            };
                            if (!Exists(con, name + "_LOLO", swxTableName))
                                rowsAffected += CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                    ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues).ExecuteNonQuery()
                                    : 0;
                            //LO
                            swxId++;
                            swxSequence++;
                            vbaScript = name + "_LO_FirstPlay";
                            trName = "trg_data:" + name + "_LO";
                            swxParValues = new string[]
                            {
                                swxId.ToString(), "1", name + "_LO", swxSequence.ToString(),
                                description + " (нижняя граница)", "true",
                                vbaScript, "false", "30", trName, null
                            };
                            if (!Exists(con, name + "_LO", swxTableName))
                                rowsAffected += CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                    ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues).ExecuteNonQuery()
                                    : 0;
                            //HI
                            swxId++;
                            swxSequence++;
                            vbaScript = name + "_HI_FirstPlay";
                            trName = "trg_data:" + name + "_HI";
                            swxParValues = new string[]
                            {
                                swxId.ToString(), "1", name + "_HI", swxSequence.ToString(),
                                description + " (верхняя граница)", "true",
                                vbaScript, "false", "30", trName, null
                            };
                            if (!Exists(con, name + "_HI", swxTableName))
                                rowsAffected += CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                    ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues).ExecuteNonQuery()
                                    : 0;
                            //HIHI
                            swxId++;
                            swxSequence++;
                            vbaScript = name + "_HIHI_FirstPlay";
                            trName = "trg_data:" + name + "_HIHI";
                            swxParValues = new string[]
                            {
                                swxId.ToString(), "1", name + "_HIHI", swxSequence.ToString(),
                                description + " (аварийная, верхняя граница)", "true",
                                vbaScript, "false", "30", trName, null
                            };
                            if (!Exists(con, name + "_HIHI", swxTableName))
                                rowsAffected += CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                    ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues).ExecuteNonQuery()
                                    : 0;
                        }
                        //дискретные
                        else
                        {
                            string split = row["Split"].ToString();
                            description = row["BaseText"].ToString();
                            string alm1Text = row["Alm1MsgText"].ToString();
                            string alm2Text = row["Alm2MsgText"].ToString();
                            //нужно ли разбить на 2 сиг-и?
                            switch (split)
                            {
                                //нет
                                case "0":
                                    vbaScript = name + "_FirstPlay";
                                    trName = "trg_data:" + name;
                                    string[] swxParValues =
                                    {
                                        swxId.ToString(), "1", name + "_first", swxSequence.ToString(),
                                        description + " " + alm1Text, "true",
                                        vbaScript, "false", "30", trName, null
                                    };
                                    if (!Exists(con, name + "_first", swxTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                                ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues)
                                                    .ExecuteNonQuery()
                                                : 0;
                                    break;
                                //да
                                case "1":
                                    //on
                                    vbaScript = name + "_on_FirstPlay";
                                    trName = "trg_data:" + name + "_on";
                                    swxParValues = new string[]
                                    {
                                        swxId.ToString(), "1", name + "_on_first", swxSequence.ToString(),
                                        description + " " + alm1Text, "true",
                                        vbaScript, "false", "30", trName, null
                                    };
                                    if (!Exists(con, name + "_on_first", swxTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                                ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues)
                                                    .ExecuteNonQuery()
                                                : 0;
                                    //off
                                    swxId++;
                                    swxSequence++;
                                    vbaScript = name + "_off_FirstPlay";
                                    trName = "trg_data:" + name + "_off";
                                    swxParValues = new string[]
                                    {
                                        swxId.ToString(), "1", name + "_off_first", swxSequence.ToString(),
                                        description + " " + alm2Text, "true",
                                        vbaScript, "false", "30", trName, null
                                    };
                                    if (!Exists(con, name + "_off_first", swxTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, swxComText, swxParTypes, swxParValues) != null
                                                ? CreateInsertCommand(con, swxComText, swxParTypes, swxParValues)
                                                    .ExecuteNonQuery()
                                                : 0;
                                    break;
                                default:
                                    worker.ReportProgress(_pr,
                                        string.Format("Тег {0} пропущен, в столбце \"Split\" должна быть 1 или 0", name));
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
                MessageBox.Show("Не удалось обновить базу " + dbName, "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                worker.ReportProgress(_pr, "---->Не удалось обновить базу " + dbName + ": " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Обновляет базу данных UDM (DataManager.mdb)
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="dbName">путь к БД</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void UpdateUdmBase(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt, string dbName, bool discrete)
        {
            worker.ReportProgress(_pr, "---->Приступаем к обновлению базы " + dbName + "...");
            int rowsAffected = 0;
            if (!File.Exists(dbName))
            {
                MessageBox.Show(string.Format("Не найдена база {0}", dbName), "Не найдена база данных UDM", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, string.Format("Не найдена база \"{0}\"", dbName));
                return;
            }

            try
            {
                using (OleDbConnection con = new OleDbConnection(_conString + dbName))
                {
                    con.Open();

                    const string expTableName = "DMG_ExpressionItemsRoot";
                    const string expComText =
                        "INSERT INTO DMG_ExpressionItemsRoot (" +
                        "  [ID], [ParentID], [Name], [Sequence], [Description], [Type], [DefaultScanRate], [Parameters]" +
                        ", [ReadExpression], [UseTriggerTags], [TriggerTags], [SendInitialUpdate], [SendAllUpdates]" +
                        ", [EnableWriteExpression], [WriteExpression], [OutputTag]) " +
                        "VALUES (" +
                        "  @ID, @ParentID, @Name, @Sequence, @Description, @Type, @DefaultScanRate, @Parameters" +
                        ", @ReadExpression, @UseTriggerTags, @TriggerTags, @SendInitialUpdate, @SendAllUpdates" +
                        ", @EnableWriteExpression, @WriteExpression, @OutputTag)";

                    OleDbType[] expParTypes =
                    {
                        OleDbType.Integer, OleDbType.Integer, OleDbType.BSTR, OleDbType.Integer, OleDbType.BSTR,
                        OleDbType.Integer, OleDbType.Integer, OleDbType.BSTR,
                        OleDbType.BSTR, OleDbType.Boolean, OleDbType.BSTR, OleDbType.Boolean, OleDbType.Boolean,
                        OleDbType.Boolean, OleDbType.BSTR, OleDbType.BSTR
                    };

                    string trgTableName = "TRG_DataItemsRoot";
                    const string trgComText =
                        "INSERT INTO TRG_DataItemsRoot (" +
                        "  [ID], [ParentID], [Name], [Sequence], [Description]" +
                        ", [ExecuteCondition], [OPCTag], [UseEnableTag], [EnableTag]" +
                        ", [UseStart], [Start], [RelatedValues]) " +
                        "VALUES (" +
                        "  @ID, @ParentID, @Name, @Sequence, @Description" +
                        ", @ExecuteCondition, @OPCTag, @UseEnableTag, @EnableTag" +
                        ", @UseStart, @Start, @RelatedValues)";

                    OleDbType[] trgParTypes =
                    {
                        OleDbType.Integer, OleDbType.Integer, OleDbType.BSTR, OleDbType.Integer, OleDbType.BSTR,
                        OleDbType.Integer, OleDbType.BSTR, OleDbType.Boolean, OleDbType.BSTR,
                        OleDbType.Boolean, OleDbType.DBDate, OleDbType.BSTR
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
                        worker.ReportProgress(_pr);

                        string tagName = row["TagName"].ToString();
                        string path = row["Path"].ToString().Replace(".", "_");
                        string endAddress = row["EndAddress"].ToString();

                        //// переменные для DMG_ExpressionItemsRoot
                        ////@ID, 
                        int expId;
                        ////@ParentID
                        // "1"
                        //@Name
                        string name = path + "_" + tagName;
                        ////@Sequence
                        int expSequence;
                        ////@Description
                        string description = row["Description"].ToString();
                        ////@Type
                        // "0";
                        ////@DefaultScanRate
                        string scanRate = "1000";
                        ////@Parameters
                        // null;
                        //@ReadExpression
                        string readExp;

                        //// переменные для TRG_DataItemsRoot
                        ////@ID
                        int trgId;
                        ////@ParentID
                        // "1";
                        ////@Name 
                        //все тот же name;
                        ////@Sequence
                        int trgSequence;
                        ////@Description
                        //все тот же description;
                        ////@ExecuteCondition
                        // "1";
                        ////@OPCTag
                        string opcTag;
                        ////@UseEnableTag
                        // "false";
                        //@EnableTag
                        // null;
                        //@UseStart
                        // "false";
                        //@Start
                        // DateTime.Now();
                        //@RelatedValues
                        // null;

                        //до первой пустой строки tagName
                        if (string.IsNullOrEmpty(tagName))
                        {
                            worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                            worker.ReportProgress(_pr, "---->Обновление базы " + dbName + " завершено.");
                            return;
                        }

                        expId = GetId(con, expTableName);
                        expSequence = GetSeq(con, expTableName);
                        trgId = GetId(con, trgTableName);
                        trgSequence = GetSeq(con, trgTableName);

                        //аналоговые
                        if (!discrete)
                        {
                            //чот называется, защита от дурака
                            string hihiLimit = row["HiHi"].ToString();
                            string hiLimit = row["Hi"].ToString();
                            string loLimit = row["Lo"].ToString();
                            string loloLimit = row["LoLo"].ToString();
                            string gender = row["Gender"].ToString();

                            if (string.IsNullOrEmpty(hihiLimit) || string.IsNullOrEmpty(hiLimit)
                                || string.IsNullOrEmpty(loLimit) || string.IsNullOrEmpty(loloLimit)
                                || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(endAddress))
                            {
                                worker.ReportProgress(_pr, string.Format("Тег {0} будет добавлен в базу UDM, но, " +
                                                                         "используя текущие данные его невозможно было добавить в базу AlarmWorx, убедитесь наличии Alarm'а!",
                                    name));
                            }

                            #region запись в DMG_ExpressionItemsRoot

                            //expressionRead
                            //x= {{ICONICS.AlarmSvr_.1\     AI      _    LEP80      .RELATED VALUE 03}} 
                            //RELATED VALUE - (вспомогательный) вкл/выкл звуковую сиг. 03,04,05 и 06 соот-но для сиг-й
                            //&& {{ICONICS.AlarmSvr_.1\     AI      _    LEP80      .LIM_LOLO_Active}} 
                            //&& !{{ICONICS.AlarmSvr_.1\    AI      _    LEP80      .LIM_Acked}}

                            //LOLO
                            readExp = "x= tonumberbase({{ICONICS.AlarmSvr_.1\\" + path + "_" + tagName +
                                      ".RELATED VALUE 03}}, 10) " +
                                      "&& {{ICONICS.AlarmSvr_.1\\" + path + "_" + tagName + ".LIM_LOLO_Active}} && " +
                                      "!{{ICONICS.AlarmSvr_.1\\" + path + "_" + tagName + ".LIM_Acked}}";
                            string[] expParValues =
                            {
                                expId.ToString(), "1", name + "_LOLO", expSequence.ToString(),
                                description + " (аварийная, нижняя граница)", "0", scanRate, null,
                                readExp, "false", null, "false", "false",
                                "false", null, null
                            };
                            if (!Exists(con, name + "_LOLO", expTableName))
                                rowsAffected += CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                    ? CreateInsertCommand(con, expComText, expParTypes, expParValues).ExecuteNonQuery()
                                    : 0;
                            //LO
                            expId++;
                            expSequence++;
                            readExp = readExp.Replace(".RELATED VALUE 03", ".RELATED VALUE 04")
                                .Replace("_LOLO_", "_LO_");
                            expParValues = new string[]
                            {
                                expId.ToString(), "1", name + "_LO", expSequence.ToString(),
                                description + " (нижняя граница)",
                                "0", scanRate, null,
                                readExp, "false", null, "false", "false",
                                "false", null, null
                            };
                            if (!Exists(con, name + "_LO", expTableName))
                                rowsAffected += CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                    ? CreateInsertCommand(con, expComText, expParTypes, expParValues).ExecuteNonQuery()
                                    : 0;
                            //HI
                            expId++;
                            expSequence++;
                            readExp = readExp.Replace(".RELATED VALUE 04", ".RELATED VALUE 05").Replace("_LO_", "_HI_");
                            expParValues = new string[]
                            {
                                expId.ToString(), "1", name + "_HI", expSequence.ToString(),
                                description + " (верхняя граница)",
                                "0", scanRate, null,
                                readExp, "false", null, "false", "false",
                                "false", null, null
                            };
                            if (!Exists(con, name + "_HI", expTableName))
                                rowsAffected += CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                    ? CreateInsertCommand(con, expComText, expParTypes, expParValues).ExecuteNonQuery()
                                    : 0;
                            //HIHI
                            expId++;
                            expSequence++;
                            readExp = readExp.Replace(".RELATED VALUE 05", ".RELATED VALUE 06")
                                .Replace("_HI_", "_HIHI_");
                            expParValues = new string[]
                            {
                                expId.ToString(), "1", name + "_HIHI", expSequence.ToString(),
                                description + " (аварийная, верхняя граница)", "0", scanRate, null,
                                readExp, "false", null, "false", "false",
                                "false", null, null
                            };
                            if (!Exists(con, name + "_HIHI", expTableName))
                                rowsAffected += CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                    ? CreateInsertCommand(con, expComText, expParTypes, expParValues).ExecuteNonQuery()
                                    : 0;

                            #endregion

                            #region запись в TRG_DataItemsRoot

                            //LOLO
                            opcTag = "exp:" + name + "_LOLO";
                            string[] trgParValues =
                            {
                                trgId.ToString(), "1", name + "_LOLO", trgSequence.ToString(),
                                description + " (аварийная, нижняя граница)",
                                "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                            };
                            if (!Exists(con, name + "_LOLO", trgTableName))
                                rowsAffected += CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                    ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues).ExecuteNonQuery()
                                    : 0;
                            //LO
                            trgId++;
                            trgSequence++;
                            opcTag = "exp:" + name + "_LO";
                            trgParValues = new string[]
                            {
                                trgId.ToString(), "1", name + "_LO", trgSequence.ToString(),
                                description + " (нижняя граница)",
                                "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                            };
                            if (!Exists(con, name + "_LO", trgTableName))
                                rowsAffected += CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                    ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues).ExecuteNonQuery()
                                    : 0;
                            //HI
                            trgId++;
                            trgSequence++;
                            opcTag = "exp:" + name + "_HI";
                            trgParValues = new string[]
                            {
                                trgId.ToString(), "1", name + "_HI", trgSequence.ToString(),
                                description + " (верхняя граница)",
                                "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                            };
                            if (!Exists(con, name + "_HI", trgTableName))
                                rowsAffected += CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                    ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues).ExecuteNonQuery()
                                    : 0;
                            //HIHI
                            trgId++;
                            trgSequence++;
                            opcTag = "exp:" + name + "_HIHI";
                            trgParValues = new string[]
                            {
                                trgId.ToString(), "1", name + "_HIHI", trgSequence.ToString(),
                                description + " (аварийная, верхняя граница)",
                                "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                            };
                            if (!Exists(con, name + "_HIHI", trgTableName))
                                rowsAffected += CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                    ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues).ExecuteNonQuery()
                                    : 0;

                            #endregion
                        }
                        //дискретные
                        else
                        {
                            string split = row["Split"].ToString();
                            description = row["BaseText"].ToString();
                            string alm1Text = row["Alm1MsgText"].ToString();
                            string alm2Text = row["Alm2MsgText"].ToString();
                            //нужно ли разбить на 2 сиг-и?
                            switch (split)
                            {
                                //нет
                                case "0":

                                    #region запись в DMG_ExpressionItemsRoot

                                    readExp = "x= {{ICONICS.AlarmSvr_.1\\" + name + ".DIG_Active}} " +
                                              "&& !{{ICONICS.AlarmSvr_.1\\" + name + ".DIG_Acked}}";
                                    string[] expParValues =
                                    {
                                        expId.ToString(), "1", name, expSequence.ToString(),
                                        description, "0", scanRate, null,
                                        readExp, "false", null, "false", "false",
                                        "false", null, null
                                    };
                                    if (!Exists(con, name, expTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                                ? CreateInsertCommand(con, expComText, expParTypes, expParValues)
                                                    .ExecuteNonQuery()
                                                : 0;

                                    #endregion

                                    #region запись в TRG_DataItemsRoot

                                    opcTag = "exp:" + name;
                                    string[] trgParValues =
                                    {
                                        trgId.ToString(), "1", name, trgSequence.ToString(),
                                        description + " " + alm1Text,
                                        "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                                    };
                                    if (!Exists(con, name, trgTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                                ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues)
                                                    .ExecuteNonQuery()
                                                : 0;

                                    #endregion

                                    break;
                                //да
                                case "1":

                                    #region запись в DMG_ExpressionItemsRoot

                                    //_on
                                    readExp = "x= {{ICONICS.AlarmSvr_.1\\" + name + "_on.DIG_Active}} " +
                                              "&& !{{ICONICS.AlarmSvr_.1\\" + name + "_on.DIG_Acked}}";
                                    expParValues = new string[]
                                    {
                                        expId.ToString(), "1", name + "_on", expSequence.ToString(),
                                        description, "0", scanRate, null,
                                        readExp, "false", null, "false", "false",
                                        "false", null, null
                                    };
                                    if (!Exists(con, name + "_on", expTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                                ? CreateInsertCommand(con, expComText, expParTypes, expParValues)
                                                    .ExecuteNonQuery()
                                                : 0;
                                    //_off
                                    expId++;
                                    expSequence++;
                                    readExp = "x= {{ICONICS.AlarmSvr_.1\\" + name + "_off.DIG_Active}} " +
                                              "&& !{{ICONICS.AlarmSvr_.1\\" + name + "_off.DIG_Acked}}";
                                    expParValues = new string[]
                                    {
                                        expId.ToString(), "1", name + "_off", expSequence.ToString(),
                                        description, "0", scanRate, null,
                                        readExp, "false", null, "false", "false",
                                        "false", null, null
                                    };
                                    if (!Exists(con, name + "_off", expTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, expComText, expParTypes, expParValues) != null
                                                ? CreateInsertCommand(con, expComText, expParTypes, expParValues)
                                                    .ExecuteNonQuery()
                                                : 0;

                                    #endregion

                                    #region запись в TRG_DataItemsRoot

                                    //_on
                                    opcTag = "exp:" + name + "_on";
                                    trgParValues = new string[]
                                    {
                                        trgId.ToString(), "1", name + "_on", trgSequence.ToString(),
                                        description + " " + alm1Text,
                                        "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                                    };
                                    if (!Exists(con, name + "_on", trgTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                                ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues)
                                                    .ExecuteNonQuery()
                                                : 0;
                                    //_off
                                    trgId++;
                                    trgSequence++;
                                    opcTag = "exp:" + name + "_off";
                                    trgParValues = new string[]
                                    {
                                        trgId.ToString(), "1", name + "_off", trgSequence.ToString(),
                                        description + " " + alm2Text,
                                        "1", opcTag, "false", null, "false", DateTime.Now.ToLongDateString(), null
                                    };
                                    if (!Exists(con, name + "_off", trgTableName))
                                        rowsAffected +=
                                            CreateInsertCommand(con, trgComText, trgParTypes, trgParValues) != null
                                                ? CreateInsertCommand(con, trgComText, trgParTypes, trgParValues)
                                                    .ExecuteNonQuery()
                                                : 0;

                                    #endregion

                                    break;
                                default:
                                    worker.ReportProgress(_pr,
                                        string.Format("Тег {0} пропущен, в столбце \"Split\" должна быть 1 или 0", name));
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
                MessageBox.Show("Не удалось обновить базу " + dbName, "Ошибка", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "Внесено " + rowsAffected + " строк");
                worker.ReportProgress(_pr, "---->Не удалось обновить базу " + dbName + ": " + ex.Message);
                return;
            }
        }


        /// <summary>
        /// Создает txt-файлы для звуковой сигнализации
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void CreateSoundText(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt, bool discrete)
        {
            worker.ReportProgress(_pr, "---->Приступаем к созданию txt-файлов для сигнализаций...");
            try
            {
                string filePath = Form1.excelFilePath.Substring(0, Form1.excelFilePath.LastIndexOf("\\") + 1) +
                                  "SoundText";

                //создаем папку SoundText в папке где хранится файл Excel
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                worker.ReportProgress(_pr, "Рабочая директория - " + filePath);
                filePath += "\\";
                int filesCreated = 0;

                foreach (DataRow row in dt.Rows)
                {
                    //проверяем на отмену операции
                    if (worker.CancellationPending)
                    {
                        workerEventArgs.Cancel = true;
                        return;
                    }

                    _pr++;
                    worker.ReportProgress(_pr);

                    string tagName = row["tagName"].ToString();
                    string path = row["Path"].ToString().Replace(".", "_");
                    string baseText = row["SoundBaseText"].ToString();
                    string name = path.Replace(".", "_") + "_" + tagName;
                    if (string.IsNullOrEmpty(path))
                        name = tagName;
                    string fileName, alarmText;

                    //до первой пустой строки tagName
                    if (string.IsNullOrEmpty(tagName))
                    {
                        worker.ReportProgress(_pr, "Создан(о) " + filesCreated + " файл(ов).");
                        worker.ReportProgress(_pr, "---->Создание txt-файлов завершено...");
                        return;
                    }

                    if (string.IsNullOrEmpty(baseText))
                    {
                        worker.ReportProgress(_pr, string.Format("Тег {0} пропущен, не заполнено поле \"SoundBaseText\")", name));
                        continue;
                    }

                    //аналоговый параметр
                    if (!discrete)
                    {
                        string loloText, loText, hiText, hihiText;
                        string gender = row["Gender"].ToString();
                        switch (gender)
                        {
                            case "M":
                                loloText = ": аварийный, низкий";
                                loText = ": низкий";
                                hiText = ": высокий";
                                hihiText = ": аварийный, высокий";
                                break;
                            case "F":
                                loloText = ": аварийная, низкая";
                                loText = ": низкая";
                                hiText = ": высокая";
                                hihiText = ": аварийная, высокая";
                                break;
                            case "N":
                                loloText = ": аварийное, низкое";
                                loText = ": низкое";
                                hiText = ": высокое";
                                hihiText = ": аварийное, высокое";
                                break;
                            default:
                                worker.ReportProgress(_pr, string.Format(
                                    "Тег {0} пропущен, не удалось определить текст сигнализации. В поле \"Gender\" должно быть значение M,F или N)", name));
                                continue;
                        }
                        //создаем .txt файлы для аналоговой сигнализации
                        fileName = filePath + path + "_" + tagName + "_LOLO.txt";
                        alarmText = baseText + loloText;
                        if (CreateTxt(fileName, alarmText))
                            filesCreated++;

                        fileName = fileName.Replace("_LOLO", "_LO");
                        alarmText = baseText + loText;
                        if (CreateTxt(fileName, alarmText))
                            filesCreated++;

                        fileName = fileName.Replace("_LO", "_HI");
                        alarmText = baseText + hiText;
                        if (CreateTxt(fileName, alarmText))
                            filesCreated++;

                        fileName = fileName.Replace("_HI", "_HIHI");
                        alarmText = baseText + hihiText;
                        if (CreateTxt(fileName, alarmText))
                            filesCreated++;
                    }
                    //дискретный параметр
                    if (discrete)
                    {
                        string split = row["Split"].ToString();

                        switch (split)
                        {
                            case "0":
                                fileName = filePath + path + "_" + tagName + ".txt";
                                alarmText = baseText + ", " + row["Alm1MsgText"].ToString();
                                if (CreateTxt(fileName, alarmText))
                                    filesCreated++;
                                break;
                            case "1":
                                fileName = filePath + path + "_" + tagName + "_on.txt";
                                alarmText = baseText + ", " + row["Alm1MsgText"].ToString();
                                if (CreateTxt(fileName, alarmText))
                                    filesCreated++;

                                fileName = filePath + path + "_" + tagName + "_off.txt";
                                alarmText = baseText + ", " + row["Alm2MsgText"].ToString();
                                if (CreateTxt(fileName, alarmText))
                                    filesCreated++;
                                break;
                            default:
                                continue;
                        }
                    }
                }
                worker.ReportProgress(_pr, "Создан(о) " + filesCreated + " файл(ов).");
                worker.ReportProgress(_pr, "---->Создание txt-файлов завершено...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось создать все txt-файлы для звуковых сигнализаций \n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "---->Не удалось создать все txt-файлы для звуковых сигнализаций: " + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Создает файл VBAScript.txt
        /// </summary>
        /// <param name="worker">BackgroundWorker, в котором выполняется метод</param>
        /// <param name="workerEventArgs">DoWorkEventArgs</param>
        /// <param name="dt">таблица с данными</param>
        /// <param name="discrete">true - дискретные теги, false - аналоговые</param>
        private static void CreateVbaScript(BackgroundWorker worker, DoWorkEventArgs workerEventArgs, DataTable dt, bool discrete)
        {
            string fName = discrete ? "\\DI_VBAScript.txt" : "\\AI_VBAScript.txt";
            string filePath = string.IsNullOrEmpty(Form1.excelFilePath)? Environment.CurrentDirectory + fName
                : Form1.excelFilePath.Substring(0, Form1.excelFilePath.LastIndexOf("\\")) + fName;
            worker.ReportProgress(_pr, "---->Приступаем к созданию " + filePath + "...");

            StringBuilder vbaScript = new StringBuilder();

            try
            {
                if (File.Exists(filePath))
                {
                    DialogResult askResult =
                        MessageBox.Show("Перезаписать файл " + filePath + " ?",
                            "Файл существует",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (askResult == DialogResult.Yes)
                    {
                        File.Delete(filePath);
                    }
                    else
                    {
                        worker.ReportProgress(_pr, "---->Выполнение прервано. Удалите файл " + filePath + " и попробуйте снова");
                        return;
                    }
                }

                int procCreated = 0;

                vbaScript.AppendLine();
                vbaScript.AppendLine("Private Declare Function PlaySounds Lib \"winmm.dll\" Alias \"PlaySoundA\" _");
                vbaScript.AppendLine("(ByVal lpszName As String, ByVal hModule As Long, ByVal dwFlags As Long) _");
                vbaScript.AppendLine("As Long");
                vbaScript.AppendLine();
                vbaScript.AppendLine("Const SND_ALIAS = &H10000");
                vbaScript.AppendLine("Const SND_ALIAS_ID = &H110000");
                vbaScript.AppendLine("Const SND_ALIAS_START = 0");
                vbaScript.AppendLine("Const SND_APPLICATION = &H80");
                vbaScript.AppendLine("Const SND_ASYNC = &H1");
                vbaScript.AppendLine("Const SND_FILENAME = &H20000");
                vbaScript.AppendLine("Const SND_LOOP = &H8");
                vbaScript.AppendLine("Const SND_MEMORY = &H4");
                vbaScript.AppendLine("Const SND_NODEFAULT = &H2");
                vbaScript.AppendLine("Const SND_NOSTOP = &H10");
                vbaScript.AppendLine("Const SND_NOWAIT = &H2000");
                vbaScript.AppendLine("Const SND_PURGE = &H40");
                vbaScript.AppendLine("Const SND_RESERVED = &HFF000000");
                vbaScript.AppendLine("Const SND_RESOURCE = &H40004");
                vbaScript.AppendLine("Const SND_SYNC = &H0");
                vbaScript.AppendLine("Const SOUND_DIR = \"C:\\Sounds\\\"");
                vbaScript.AppendLine();
                vbaScript.AppendLine("Public Sub PlayEvent(iSoundType As Integer, strTextMessage As String)");
                vbaScript.AppendLine("Dim retval As Long  '' return value of the function");
                vbaScript.AppendLine("Dim strFileName As String");
                vbaScript.AppendLine();
                vbaScript.AppendLine("Select Case iSoundType");
                vbaScript.AppendLine("    Case 0");
                vbaScript.AppendLine("        strFileName = \"SIREN2.WAV\"");
                vbaScript.AppendLine("    Case 1");
                vbaScript.AppendLine("        strFileName = \"ALARM.WAV\"");
                vbaScript.AppendLine("    Case 2");
                vbaScript.AppendLine("        strFileName = \"BIKEBELL.WAV\"");
                vbaScript.AppendLine("    Case 3");
                vbaScript.AppendLine("        strFileName = \"BIKEBELL.WAV\"");
                vbaScript.AppendLine("    Case Else");
                vbaScript.AppendLine("        strFileName = \"BIKEBELL.WAV\"");
                vbaScript.AppendLine("End Select");
                vbaScript.AppendLine();
                vbaScript.AppendLine("retval = PlaySounds(SOUND_DIR + strFileName, 0&, SND_SYNC Or SND_FILENAME)");
                vbaScript.AppendLine("'retval = PlaySounds(SOUND_DIR + strTextMessage, 0&, SND_SYNC Or SND_FILENAME)");
                vbaScript.AppendLine("'retval = PlaySounds(\"\", 0&, SND_PURGE)");
                vbaScript.AppendLine();
                vbaScript.AppendLine("End Sub");

                foreach (DataRow row in dt.Rows)
                {
                    //проверяем на отмену операции
                    if (worker.CancellationPending)
                    {
                        workerEventArgs.Cancel = true;
                        return;
                    }

                    _pr++;
                    worker.ReportProgress(_pr);

                    string tagName = row["tagName"].ToString();
                    string path = row["Path"].ToString().Replace(".","_");
                    //до первой пустой строки tagName
                    if (string.IsNullOrEmpty(tagName))
                    {
                        //удаляем старый файл если пользователь выберет перезапись, иначе дописываем (Append) 
                        File.AppendAllText(filePath, vbaScript.ToString());
                        worker.ReportProgress(_pr, "Создано " + procCreated + " процедур.");
                        worker.ReportProgress(_pr, "---->Создание файла VBAScript завершено...");
                        return;
                    }

                    if (!discrete)
                    {
                        CreateSub(ref vbaScript, path + "_" + tagName + "_LOLO", 1);
                        procCreated++;
                        CreateSub(ref vbaScript, path + "_" + tagName + "_LO", 0);
                        procCreated++;
                        CreateSub(ref vbaScript, path + "_" + tagName + "_HI", 0);
                        procCreated++;
                        CreateSub(ref vbaScript, path + "_" + tagName + "_HIHI", 1);
                        procCreated++;
                    }
                    else
                    {
                        string split = row["Split"].ToString();

                        switch (split)
                        {
                            case "0":
                                CreateSub(ref vbaScript, path + "_" + tagName, 2);
                                procCreated++;
                                break;
                            case "1":
                                CreateSub(ref vbaScript, path + "_" + tagName + "_on", 2);
                                procCreated++;
                                CreateSub(ref vbaScript, path + "_" + tagName + "_off", 2);
                                procCreated++;
                                break;
                            default:
                                continue;
                        }
                    }
                }
                //выбираем метод Append, и просто удаляем старый файл если пользователь выберет перезапись
                File.AppendAllText(filePath, vbaScript.ToString());
                worker.ReportProgress(_pr, "Создано " + procCreated + " процедур.");
                worker.ReportProgress(_pr, "---->Создание файла VBAScript завершено...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось успешно записать в файл " + filePath, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                worker.ReportProgress(_pr, "---->Не удалось создать\\обновить файл VBAScript: " + ex.Message);
            }
        }

        /// <summary>
        /// Записывает заданную строку в указанный файл, используя StreamWriter
        /// </summary>
        /// <param name="filePath">полный путь к файлу (*.txt)</param>
        /// <param name="text">текст для записи</param>
        /// <returns>возвращает true в случае усхеха, null в случае ошибки</returns>
        private static bool CreateTxt(string filePath, string text)
        {
            try
            {
                StreamWriter stream = new StreamWriter(filePath, false, Encoding.GetEncoding(1251));
                stream.WriteLine(text);
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Создает объект OleDbCommand
        /// </summary>
        /// <param name="connection">активное подключение OleDbConnection</param>
        /// <param name="comText">текст комманды</param>
        /// <param name="types">типы данных параметров команды (ex.:SqlDbType.Int, SqlDbType.NVarChar)</param>
        /// <param name="values">значения параметров команды (ex.: null, "parName")</param>
        /// <returns>возвращает OleDbCommand в случае усхеха, null в случае ошибки</returns>
        private static OleDbCommand CreateInsertCommand(OleDbConnection connection, string comText, OleDbType[] types, string[] values)
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

                OleDbCommand command = new OleDbCommand(comText, connection);
                for (int i = 0; i < uniqueParameters.Length; i++)
                {
                    command.Parameters.Add(new OleDbParameter(uniqueParameters[i], types[i]));
                    if (string.IsNullOrEmpty(values[i]))
                        command.Parameters[uniqueParameters[i]].Value = DBNull.Value;
                    else if (types[i] == OleDbType.Boolean)
                    {
                        bool b = Convert.ToBoolean(values[i]);
                        command.Parameters[uniqueParameters[i]].Value = b;
                    }
                    else if (types[i] == OleDbType.DBDate)
                    {
                        DateTime date = Convert.ToDateTime(values[i]);
                        command.Parameters[uniqueParameters[i]].Value = date;
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
                        "Метод: CreateInsertCommand(string comText, OleDbDbType[] types, string[] values) \n\nТекст ошибки: {0} \n\nТекст SqlCommand: {1}",
                    ex.Message, comText), "Не удалось создать SqlCommand", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Вовзращает следующий свободный ID в таблице
        /// </summary>
        /// <param name="con">активное подключение OleDbConnection</param>
        /// <param name="tableName">имя таблицы</param>
        /// <returns></returns>
        private static int GetId(OleDbConnection con, string tableName)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbCommand selectCommand = new OleDbCommand("SELECT [ID] FROM " + tableName + " ORDER BY [ID]", con);
                OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand);
                adapter.Fill(dt);
                int id = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    id = Convert.ToInt32(dt.Rows[i]["ID"].ToString());
                }
                return id + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetId(OleDbConnection con, string tableName) \n\nТекст ошибки: {0} \n\nИмя таблицы: {1}",
                    ex.Message, tableName), "Не удалось получить ID из таблицы " + tableName);
                return 0;
            }
        }

        /// <summary>
        /// Вовзращает следующий свободный Sequence в таблице
        /// </summary>
        /// <param name="con">активное подключение OleDbConnection</param>
        /// <param name="tableName">имя таблицы</param>
        /// <returns></returns>
        private static int GetSeq(OleDbConnection con, string tableName)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbCommand selectCommand =
                    new OleDbCommand("SELECT [Sequence] FROM " + tableName + " ORDER BY [Sequence]", con);
                OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand);
                adapter.Fill(dt);
                int id = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    id = Convert.ToInt32(dt.Rows[i]["Sequence"].ToString());
                }
                return id + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: GetSeq(OleDbConnection con, string tableName) \n\nТекст ошибки: {0} \n\nИмя таблицы: {1}",
                    ex.Message, tableName), "Не удалось получить ID из таблицы " + tableName);
                return 0;
            }
        }

        /// <summary>
        /// Проверяет наличие тега в указанной таблице таблице
        /// </summary>
        /// <param name="con">активное подключение OleDbConnection</param>
        /// <param name="tagName">имя тега</param>
        /// <param name="tableName">имя таблицы</param>
        /// <returns></returns>
        private static bool Exists(OleDbConnection con, string tagName, string tableName)
        {
            try
            {
                DataTable dt = new DataTable();
                OleDbCommand selectCommand =
                    new OleDbCommand("SELECT [Name] FROM " + tableName, con);
                OleDbDataAdapter adapter = new OleDbDataAdapter(selectCommand);
                adapter.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["Name"].ToString() == tagName) return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(
                    "Метод: Exists(OleDbConnection con, string tagName, string tableName) \n\nТекст ошибки: {0} \n\nИмя тега: {1} \n\nИмя таблицы: {2}",
                    ex.Message, tagName, tableName), "Не удалось проверить наличие тега в таблице " + tableName);
                return false;
            }
        }

        /// <summary>
        /// Дописывает в StringBuilder текст процедуры для тега
        /// </summary>
        /// <param name="vbaScript">StringBuilder, в который производится запись</param>
        /// <param name="tagName">имя тега</param>
        /// <param name="almType">0 - предупредительная, 1 - аварийная, 2 - дискретная</param>
        private static void CreateSub(ref StringBuilder vbaScript, string tagName, int almType)
        {
            //vbaScript.AppendLine();
            //vbaScript.AppendLine("Public Sub " + tagName + "_InstantPlay(td As TriggerData)");
            //vbaScript.AppendLine("On Error GoTo ErrHandler");
            //vbaScript.AppendLine("    If " + tagName + "_firstTime = False Then");
            //vbaScript.AppendLine("        Call PlayEvent(" + alm +", td.name + \".WAV\")");
            //vbaScript.AppendLine("    End If");
            //vbaScript.AppendLine("    If " + tagName + "_firstTime = True Then");
            //vbaScript.AppendLine("    " + tagName + "_firstTime = False");
            //vbaScript.AppendLine("    End If");
            //vbaScript.AppendLine("    Exit Sub");
            //vbaScript.AppendLine("ErrHandler:");
            //vbaScript.AppendLine("    g.ConsoleMsg MSG_SEVERE_ERROR, \"Script\", \"'NewScript' failed\"");
            //vbaScript.AppendLine("End Sub");
            vbaScript.AppendLine();
            vbaScript.AppendLine("Public Sub " + tagName + "_FirstPlay(td As TriggerData)");
            vbaScript.AppendLine("    On Error GoTo ErrHandler");
            vbaScript.AppendLine("    Call PlayEvent(" + almType + ", td.name + \".WAV\")");
            //vbaScript.AppendLine("    " + tagName + "_firstTime = True");
            vbaScript.AppendLine("    Exit Sub");
            vbaScript.AppendLine("ErrHandler:");
            vbaScript.AppendLine("    g.ConsoleMsg MSG_SEVERE_ERROR, \"Script\", \"'" + tagName +
                                 "_InstantPlay' failed\"");
            vbaScript.AppendLine("End Sub");
        }
    }
}
