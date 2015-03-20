using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProjectAssistant
{
    public partial class Form1 : Form
    {
        //измеряет время выполнения кода backgroundWorker1
        private Stopwatch sw;

        private static DataTable _contenTable { get; set; }
        //переменные для updateSqlBase
        private static string _udmBaseName { get; set; }
        private static string _awxBaseName { get; set; }
        private static string _hhBaseName { get; set; }
        //переменные для updateSoundBase
        private static string _udm32BaseName { get; set; }
        private static string _swx64BaseName { get; set; }
        private bool _discrete { get; set; }
        //переменные для createSoundFiles
        private static bool _crSoundTxt { get; set; }
        private static bool _crScriptTxt { get; set; }
        private static bool _crUdmBase { get; set; }
        private static bool _crScriptBase { get; set; }

        public static string excelFilePath { get; set; }
        public static string AIListName { get { return "ALL_AI"; } }
        public static string DIListName { get { return "ALL_DI"; } }

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            splitter1_SplitterMoved(sender, null);
        }

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            progressBar.Top = splitter1.Top - progressBar.Height;
        }

        //переключает на AI
        private void aiRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!aiRButton.Checked) return;
            //грузим AI из файла выбранного ранее
            if (excelFilePath != null)
                FromExcelToDataGrid();
            else
            {
                //или грузим шаблон таблицы тегов AI
                if (File.Exists(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx"))
                {
                    if (cExcel.FileToDataTable(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx", "SELECT * FROM [AITemplate$]") != null)
                        _contenTable = cExcel.FileToDataTable(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx", "SELECT * FROM [AITemplate$]");
                    dataGridView1.DataSource = _contenTable;
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
        }
        //переключает на DI
        private void diRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!diRButton.Checked) return;
            //грузим DI из файла выбранного ранее
            if (excelFilePath != null)
                FromExcelToDataGrid();
            else
            {
                //грузим шаблон таблицы тегов DI
                if (File.Exists(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx"))
                {
                    if (cExcel.FileToDataTable(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx", "SELECT * FROM [DITemplate$]") != null)
                        _contenTable = cExcel.FileToDataTable(Environment.CurrentDirectory + "\\TablesTemplate\\Template.xlsx", "SELECT * FROM [DITemplate$]");
                    dataGridView1.DataSource = _contenTable;
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                }
            }
        }
        //переключает radioButton'ы по умолчанию при загрузке
        private void Form1_Load(object sender, EventArgs e)
        {
            aiRButton.Checked = true;
            sqlRButton.Checked = true;
        }
        //получает схему БД
        private void connectButton_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            comboBox3.Items.Clear();
            groupBox1.Enabled = false;

            cSQLBases.conString = String.Format("Data Source={0};{1}", textBox1.Text, baseAuthTypeCBox.Checked ? "Integrated Security=SSPI" : "User Id=sa;Password=" + textBox2.Text);
            //cSQLBases.conString =
            //    "Driver={SQL Server};Server=.\\FTVIEWX64TAGDB;Uid=sa;Pwd=master";
            if (cSQLBases.dbSchema == null) return;
            foreach (DataRow row in cSQLBases.dbSchema.Rows.Cast<DataRow>().Where(row => row[0].ToString() != ""))
            {
                comboBox1.Items.Add(row[0].ToString());
                comboBox2.Items.Add(row[0].ToString());
                comboBox3.Items.Add(row[0].ToString());
            }
            groupBox1.Enabled = true;

        }
        //дает возможность залогиниться под sa
        private void baseAuthTypeCBox_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.Enabled = !baseAuthTypeCBox.Checked;
            textBox2.Focus();
            textBox2.SelectAll();
        }
        //нажатие кнопки импорт данных из Excel
        private void importExcelButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Файлы Excel |*.xls;*.xlsm;*.xlsx|Все файлы |*.*" };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                excelFilePath = fileDialog.FileName;
                FromExcelToDataGrid();
            }
        }
        //Excel файл в DataGrid
        private void FromExcelToDataGrid()
        {
            if (aiRButton.Checked)
            {
                if (cExcel.FileToDataTable(excelFilePath, "SELECT * FROM [" + AIListName + "$]") != null)
                    _contenTable = cExcel.FileToDataTable(excelFilePath, "SELECT * FROM [" + AIListName + "$]");
            }
            if (diRButton.Checked)
            {
                if (cExcel.FileToDataTable(excelFilePath, "SELECT * FROM [" + DIListName + "$]") != null)
                    _contenTable = cExcel.FileToDataTable(excelFilePath, "SELECT * FROM [" + DIListName + "$]");
            }
            dataGridView1.DataSource = _contenTable;
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }
        //делает прокрутку до нижней строки в logTBox
        private void logTBox_TextChanged(object sender, EventArgs e)
        {
            logTBox.Select(logTBox.Text.Length, 0);
            logTBox.ScrollToCaret();
        }
        //действия при закрытии формы
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //останавливаем поток при закрытии формы
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();
        }
        //переключение между режимами работы
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sqlRButton.Checked)
            {
                signalTypePanel.Enabled = true;

                soundPanel.Visible = false;
                dynamicPanel.Visible = false;
            }
            if (soundRButton.Checked)
            {
                signalTypePanel.Enabled = true;
                dynamicPanel.Visible = false;

                soundPanel.Visible = true;
                soundPanel.Bounds = sqlPanel.Bounds;
            }
            if (dynamicRButton.Checked)
            {
                signalTypePanel.Enabled = false;
                aiRButton.Checked = true;

                dynamicPanel.Visible = true;
                dynamicPanel.Bounds = sqlPanel.Bounds;
            }
        }

        #region задание имен БД для ScriptWorX и UDM RadioButton'ами
        private void crUdmRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!crUdmRButton.Checked)
                return;
            string defBase = Environment.CurrentDirectory + "\\BasesTemplate\\DataManager.mdb";
            string newBase = string.IsNullOrEmpty(excelFilePath) ? Environment.CurrentDirectory + "\\DataManager.mdb"
                : excelFilePath.Substring(0, excelFilePath.LastIndexOf("\\")) + "\\DataManager.mdb";
            if (!File.Exists(defBase))
            {
                MessageBox.Show("Не найдена база " + defBase);
                crUdmRButton.Checked = false;
                return;
            }
            if (File.Exists(newBase))
            {
                DialogResult askResult = MessageBox.Show("Перезаписать файл " + newBase, "Файл существует",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (askResult == DialogResult.No)
                {
                    crUdmRButton.Checked = false;
                    return;
                }
            }
            try
            {
                File.Copy(defBase, newBase, true);
                _udm32BaseName = newBase;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void updUdmRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!updUdmRButton.Checked)
                return;
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Файл базы UDM32 |DataManager.mdb;|Все файлы |*.*" };
            if (fileDialog.ShowDialog() == DialogResult.OK)
                _udm32BaseName = fileDialog.FileName;
            else
                updUdmRButton.Checked = false;
        }
        private void crSwxRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!crSwxRButton.Checked)
                return;
            string defBase = Environment.CurrentDirectory + "\\BasesTemplate\\ScriptWorX.mdb";
            string newBase = string.IsNullOrEmpty(excelFilePath) ? Environment.CurrentDirectory + "\\ScriptWorX.mdb"
                : excelFilePath.Substring(0, excelFilePath.LastIndexOf("\\")) + "\\ScriptWorX.mdb";
            if (!File.Exists(defBase))
            {
                MessageBox.Show("Не найдена база " + defBase);
                crSwxRButton.Checked = false;
                return;
            }
            if (File.Exists(newBase))
            {
                DialogResult askResult = MessageBox.Show("Перезаписать файл " + newBase, "Файл существует",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (askResult == DialogResult.No)
                {
                    crSwxRButton.Checked = false;
                    return;
                }
            }
            try
            {
                File.Copy(defBase, newBase, true);
                _swx64BaseName = newBase;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void updSwxRButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!updSwxRButton.Checked)
                return;
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Файл базы SWX64 |ScriptWorX.mdb;|Все файлы |*.*" };
            if (fileDialog.ShowDialog() == DialogResult.OK)
                _swx64BaseName = fileDialog.FileName;
            else
                updSwxRButton.Checked = false;
        }
        private void soundBaseCBox_CheckedChanged(object sender, EventArgs e)
        {
            crUdmRButton.Enabled = udmBaseCBox.Checked;
            updUdmRButton.Enabled = udmBaseCBox.Checked;
            crSwxRButton.Enabled = swxBaseCBox.Checked;
            updSwxRButton.Enabled = swxBaseCBox.Checked;
        }
        #endregion

        //обработчик нажатия кнопок выполнения
        private void doWork_Button_Click(object sender, EventArgs e)
        {
            try
            {
                int prMax = 0;
                _discrete = diRButton.Checked;
                //если работаем с базами SQL
                if (sqlRButton.Checked)
                {
                    prMax = 0;
                    _udmBaseName = comboBox1.Text;
                    _awxBaseName = comboBox2.Text;
                    _hhBaseName = comboBox3.Text;

                    if (!string.IsNullOrEmpty(_udmBaseName))
                        prMax += _contenTable.Rows.Count;
                    if (!string.IsNullOrEmpty(_awxBaseName))
                        prMax += _contenTable.Rows.Count;
                    if (!string.IsNullOrEmpty(_hhBaseName))
                        prMax += _contenTable.Rows.Count;
                }
                //если работаем со звуковыми файлами
                if (soundRButton.Checked)
                {
                    prMax = 0;
                    _crSoundTxt = txtFilesCBox.Checked;
                    _crScriptTxt = scriptCBox.Checked;
                    _crUdmBase = udmBaseCBox.Checked;
                    _crScriptBase = swxBaseCBox.Checked;

                    if (_crSoundTxt)
                        prMax += _contenTable.Rows.Count;
                    if (_crScriptTxt)
                        prMax += _contenTable.Rows.Count;
                    if (_crUdmBase)
                        prMax += _contenTable.Rows.Count;
                    if (_crScriptBase)
                        prMax += _contenTable.Rows.Count;
                }

                if (!backgroundWorker1.IsBusy)
                {
                    logTBox.Clear();
                    progressBar.Visible = true;
                    progressBar.Maximum = prMax;

                    //если работаем с базами SQL
                    if (sqlRButton.Checked)
                    {
                        //отключаем контролы на время выполнения задачи
                        sqlPanel.Enabled = false;
                        soundRButton.Enabled = false;
                        dynamicRButton.Enabled = false;
                    }
                    //если работаем со звуковыми файлами
                    if (soundRButton.Checked)
                    {
                        //отключем котролы на время выполнения задачи
                        soundPanel.Enabled = false;
                        sqlRButton.Enabled = false;
                        dynamicRButton.Enabled = false;
                    }
                    backgroundWorker1.RunWorkerAsync();
                }
                //просто так, на всякий случай:)
                else
                {
                    MessageBox.Show("Подождите, приложение занято другим процессом");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            sw = new Stopwatch();
            sw.Start();

            BackgroundWorker worker = sender as BackgroundWorker;
            if (sqlRButton.Checked)
                cSQLBases.Update(worker, e, _contenTable, _udmBaseName, _awxBaseName, _hhBaseName, _discrete);
            if (soundRButton.Checked)
                cSound.Create(worker, e, _contenTable, _crSoundTxt, _crScriptTxt, _crUdmBase, _crScriptBase, _udm32BaseName, _swx64BaseName, _discrete);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            if (e.UserState != null)
                logTBox.Text += Environment.NewLine + e.UserState.ToString();
        }
        //происходит при завершении работы backgroundWorker1
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                logTBox.Text += Environment.NewLine + "---->Задание отменено!";
            }
            else if (e.Error != null)
            {
                logTBox.Text += Environment.NewLine + "---->Работа прервана, возникло необработанное исключение: " + e.Error.Message;
            }
            else
            {
                progressBar.Value = progressBar.Maximum;
                progressBar.Visible = false;
            }
            //возвращаем контролы
            sqlRButton.Enabled = true;
            soundRButton.Enabled = true;

            sqlPanel.Enabled = true;

            if (!soundPanel.Enabled)
            {
                soundPanel.Enabled = true;
                crSwxRButton.Checked = false;
                updSwxRButton.Checked = false;
                updUdmRButton.Checked = false;
                crUdmRButton.Checked = false;
                crUdmRButton.Checked = false;
                udmBaseCBox.Checked = false;
                swxBaseCBox.Checked = false;
            }

            sw.Stop();
            logTBox.Text += Environment.NewLine + "На выполнение кода затрачено - " + sw.ElapsedMilliseconds + " мс.";
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            //try
            //{
                cBuildPP builder = new cBuildPP(elementHost1);
                builder.MakeProcessPoints(_contenTable, coordsCBox.Checked);
            //}
            //catch(Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }
    }
}
