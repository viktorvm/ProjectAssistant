namespace ProjectAssistant
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.topPanel = new System.Windows.Forms.Panel();
            this.signalTypePanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.aiRButton = new System.Windows.Forms.RadioButton();
            this.diRButton = new System.Windows.Forms.RadioButton();
            this.dynamicRButton = new System.Windows.Forms.RadioButton();
            this.soundRButton = new System.Windows.Forms.RadioButton();
            this.sqlRButton = new System.Windows.Forms.RadioButton();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.dynamicPanel = new System.Windows.Forms.Panel();
            this.btnBuild = new System.Windows.Forms.Button();
            this.soundPanel = new System.Windows.Forms.Panel();
            this.createSoundButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.updSwxRButton = new System.Windows.Forms.RadioButton();
            this.crSwxRButton = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.updUdmRButton = new System.Windows.Forms.RadioButton();
            this.crUdmRButton = new System.Windows.Forms.RadioButton();
            this.swxBaseCBox = new System.Windows.Forms.CheckBox();
            this.udmBaseCBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFilesCBox = new System.Windows.Forms.CheckBox();
            this.scriptCBox = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.sqlPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.updateSqlButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.connectButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.baseAuthTypeCBox = new System.Windows.Forms.CheckBox();
            this.logPanel = new System.Windows.Forms.Panel();
            this.logTBox = new System.Windows.Forms.TextBox();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.dataPanel = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.importExcelButton = new System.Windows.Forms.Button();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.coordsCBox = new System.Windows.Forms.CheckBox();
            this.topPanel.SuspendLayout();
            this.signalTypePanel.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.dynamicPanel.SuspendLayout();
            this.soundPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.sqlPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.logPanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.dataPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.topPanel.Controls.Add(this.signalTypePanel);
            this.topPanel.Controls.Add(this.dynamicRButton);
            this.topPanel.Controls.Add(this.soundRButton);
            this.topPanel.Controls.Add(this.sqlRButton);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(1006, 30);
            this.topPanel.TabIndex = 0;
            // 
            // signalTypePanel
            // 
            this.signalTypePanel.Controls.Add(this.label5);
            this.signalTypePanel.Controls.Add(this.aiRButton);
            this.signalTypePanel.Controls.Add(this.diRButton);
            this.signalTypePanel.Location = new System.Drawing.Point(776, 4);
            this.signalTypePanel.Name = "signalTypePanel";
            this.signalTypePanel.Size = new System.Drawing.Size(209, 23);
            this.signalTypePanel.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Работаем с";
            // 
            // aiRButton
            // 
            this.aiRButton.AutoSize = true;
            this.aiRButton.Location = new System.Drawing.Point(93, 3);
            this.aiRButton.Name = "aiRButton";
            this.aiRButton.Size = new System.Drawing.Size(35, 17);
            this.aiRButton.TabIndex = 0;
            this.aiRButton.Text = "AI";
            this.aiRButton.UseVisualStyleBackColor = true;
            this.aiRButton.CheckedChanged += new System.EventHandler(this.aiRButton_CheckedChanged);
            // 
            // diRButton
            // 
            this.diRButton.AutoSize = true;
            this.diRButton.Location = new System.Drawing.Point(134, 3);
            this.diRButton.Name = "diRButton";
            this.diRButton.Size = new System.Drawing.Size(36, 17);
            this.diRButton.TabIndex = 0;
            this.diRButton.Text = "DI";
            this.diRButton.UseVisualStyleBackColor = true;
            this.diRButton.CheckedChanged += new System.EventHandler(this.diRButton_CheckedChanged);
            // 
            // dynamicRButton
            // 
            this.dynamicRButton.AutoSize = true;
            this.dynamicRButton.Location = new System.Drawing.Point(340, 5);
            this.dynamicRButton.Name = "dynamicRButton";
            this.dynamicRButton.Size = new System.Drawing.Size(126, 17);
            this.dynamicRButton.TabIndex = 0;
            this.dynamicRButton.Text = "Генерация динамик";
            this.dynamicRButton.UseVisualStyleBackColor = true;
            this.dynamicRButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // soundRButton
            // 
            this.soundRButton.AutoSize = true;
            this.soundRButton.Location = new System.Drawing.Point(170, 5);
            this.soundRButton.Name = "soundRButton";
            this.soundRButton.Size = new System.Drawing.Size(166, 17);
            this.soundRButton.TabIndex = 0;
            this.soundRButton.Text = "Создание звуковых файлов";
            this.soundRButton.UseVisualStyleBackColor = true;
            this.soundRButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // sqlRButton
            // 
            this.sqlRButton.AutoSize = true;
            this.sqlRButton.Location = new System.Drawing.Point(12, 5);
            this.sqlRButton.Name = "sqlRButton";
            this.sqlRButton.Size = new System.Drawing.Size(154, 17);
            this.sqlRButton.TabIndex = 0;
            this.sqlRButton.Text = "Редактирование баз SQL";
            this.sqlRButton.UseVisualStyleBackColor = true;
            this.sqlRButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.dynamicPanel);
            this.leftPanel.Controls.Add(this.soundPanel);
            this.leftPanel.Controls.Add(this.importExcelButton);
            this.leftPanel.Controls.Add(this.progressBar);
            this.leftPanel.Controls.Add(this.sqlPanel);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(146, 519);
            this.leftPanel.TabIndex = 4;
            // 
            // dynamicPanel
            // 
            this.dynamicPanel.Controls.Add(this.coordsCBox);
            this.dynamicPanel.Controls.Add(this.btnBuild);
            this.dynamicPanel.Location = new System.Drawing.Point(0, 224);
            this.dynamicPanel.Name = "dynamicPanel";
            this.dynamicPanel.Size = new System.Drawing.Size(142, 287);
            this.dynamicPanel.TabIndex = 4;
            // 
            // btnBuild
            // 
            this.btnBuild.Location = new System.Drawing.Point(6, 13);
            this.btnBuild.Name = "btnBuild";
            this.btnBuild.Size = new System.Drawing.Size(131, 23);
            this.btnBuild.TabIndex = 1;
            this.btnBuild.Text = "Построить";
            this.btnBuild.UseVisualStyleBackColor = true;
            this.btnBuild.Click += new System.EventHandler(this.btnBuild_Click);
            // 
            // soundPanel
            // 
            this.soundPanel.Controls.Add(this.createSoundButton);
            this.soundPanel.Controls.Add(this.groupBox3);
            this.soundPanel.Controls.Add(this.groupBox2);
            this.soundPanel.Location = new System.Drawing.Point(1, 102);
            this.soundPanel.Name = "soundPanel";
            this.soundPanel.Size = new System.Drawing.Size(142, 287);
            this.soundPanel.TabIndex = 1;
            // 
            // createSoundButton
            // 
            this.createSoundButton.Location = new System.Drawing.Point(33, 255);
            this.createSoundButton.Name = "createSoundButton";
            this.createSoundButton.Size = new System.Drawing.Size(75, 23);
            this.createSoundButton.TabIndex = 1;
            this.createSoundButton.Text = "Выполнить";
            this.createSoundButton.UseVisualStyleBackColor = true;
            this.createSoundButton.Click += new System.EventHandler(this.doWork_Button_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.panel2);
            this.groupBox3.Controls.Add(this.panel1);
            this.groupBox3.Controls.Add(this.swxBaseCBox);
            this.groupBox3.Controls.Add(this.udmBaseCBox);
            this.groupBox3.Location = new System.Drawing.Point(5, 86);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(134, 166);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Базы";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.updSwxRButton);
            this.panel2.Controls.Add(this.crSwxRButton);
            this.panel2.Location = new System.Drawing.Point(24, 108);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(86, 48);
            this.panel2.TabIndex = 2;
            // 
            // updSwxRButton
            // 
            this.updSwxRButton.AutoSize = true;
            this.updSwxRButton.Enabled = false;
            this.updSwxRButton.Location = new System.Drawing.Point(5, 25);
            this.updSwxRButton.Name = "updSwxRButton";
            this.updSwxRButton.Size = new System.Drawing.Size(74, 17);
            this.updSwxRButton.TabIndex = 2;
            this.updSwxRButton.Text = "Обновить";
            this.updSwxRButton.UseVisualStyleBackColor = true;
            this.updSwxRButton.CheckedChanged += new System.EventHandler(this.updSwxRButton_CheckedChanged);
            // 
            // crSwxRButton
            // 
            this.crSwxRButton.AutoSize = true;
            this.crSwxRButton.Enabled = false;
            this.crSwxRButton.Location = new System.Drawing.Point(5, 5);
            this.crSwxRButton.Name = "crSwxRButton";
            this.crSwxRButton.Size = new System.Drawing.Size(67, 17);
            this.crSwxRButton.TabIndex = 2;
            this.crSwxRButton.Text = "Создать";
            this.crSwxRButton.UseVisualStyleBackColor = true;
            this.crSwxRButton.CheckedChanged += new System.EventHandler(this.crSwxRButton_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.updUdmRButton);
            this.panel1.Controls.Add(this.crUdmRButton);
            this.panel1.Location = new System.Drawing.Point(24, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(86, 48);
            this.panel1.TabIndex = 2;
            // 
            // updUdmRButton
            // 
            this.updUdmRButton.AutoSize = true;
            this.updUdmRButton.Enabled = false;
            this.updUdmRButton.Location = new System.Drawing.Point(5, 25);
            this.updUdmRButton.Name = "updUdmRButton";
            this.updUdmRButton.Size = new System.Drawing.Size(74, 17);
            this.updUdmRButton.TabIndex = 2;
            this.updUdmRButton.Text = "Обновить";
            this.updUdmRButton.UseVisualStyleBackColor = true;
            this.updUdmRButton.CheckedChanged += new System.EventHandler(this.updUdmRButton_CheckedChanged);
            // 
            // crUdmRButton
            // 
            this.crUdmRButton.AutoSize = true;
            this.crUdmRButton.Enabled = false;
            this.crUdmRButton.Location = new System.Drawing.Point(5, 5);
            this.crUdmRButton.Name = "crUdmRButton";
            this.crUdmRButton.Size = new System.Drawing.Size(67, 17);
            this.crUdmRButton.TabIndex = 2;
            this.crUdmRButton.Text = "Создать";
            this.crUdmRButton.UseVisualStyleBackColor = true;
            this.crUdmRButton.CheckedChanged += new System.EventHandler(this.crUdmRButton_CheckedChanged);
            // 
            // swxBaseCBox
            // 
            this.swxBaseCBox.AutoSize = true;
            this.swxBaseCBox.Location = new System.Drawing.Point(9, 88);
            this.swxBaseCBox.Name = "swxBaseCBox";
            this.swxBaseCBox.Size = new System.Drawing.Size(120, 17);
            this.swxBaseCBox.TabIndex = 1;
            this.swxBaseCBox.Text = "База ScriptWorX64";
            this.swxBaseCBox.UseVisualStyleBackColor = true;
            this.swxBaseCBox.CheckedChanged += new System.EventHandler(this.soundBaseCBox_CheckedChanged);
            // 
            // udmBaseCBox
            // 
            this.udmBaseCBox.AutoSize = true;
            this.udmBaseCBox.Location = new System.Drawing.Point(9, 19);
            this.udmBaseCBox.Name = "udmBaseCBox";
            this.udmBaseCBox.Size = new System.Drawing.Size(91, 17);
            this.udmBaseCBox.TabIndex = 1;
            this.udmBaseCBox.Text = "База UDM32";
            this.udmBaseCBox.UseVisualStyleBackColor = true;
            this.udmBaseCBox.CheckedChanged += new System.EventHandler(this.soundBaseCBox_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtFilesCBox);
            this.groupBox2.Controls.Add(this.scriptCBox);
            this.groupBox2.Location = new System.Drawing.Point(5, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(134, 77);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Текст";
            // 
            // txtFilesCBox
            // 
            this.txtFilesCBox.AutoSize = true;
            this.txtFilesCBox.Location = new System.Drawing.Point(9, 19);
            this.txtFilesCBox.Name = "txtFilesCBox";
            this.txtFilesCBox.Size = new System.Drawing.Size(77, 17);
            this.txtFilesCBox.TabIndex = 1;
            this.txtFilesCBox.Text = "Файлы txt";
            this.txtFilesCBox.UseVisualStyleBackColor = true;
            // 
            // scriptCBox
            // 
            this.scriptCBox.AutoSize = true;
            this.scriptCBox.Location = new System.Drawing.Point(9, 48);
            this.scriptCBox.Name = "scriptCBox";
            this.scriptCBox.Size = new System.Drawing.Size(106, 17);
            this.scriptCBox.TabIndex = 1;
            this.scriptCBox.Text = "Файл VBAScript";
            this.scriptCBox.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(23, 490);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 23);
            this.progressBar.TabIndex = 4;
            // 
            // sqlPanel
            // 
            this.sqlPanel.Controls.Add(this.label4);
            this.sqlPanel.Controls.Add(this.textBox2);
            this.sqlPanel.Controls.Add(this.groupBox1);
            this.sqlPanel.Controls.Add(this.connectButton);
            this.sqlPanel.Controls.Add(this.textBox1);
            this.sqlPanel.Controls.Add(this.baseAuthTypeCBox);
            this.sqlPanel.Location = new System.Drawing.Point(3, 6);
            this.sqlPanel.Name = "sqlPanel";
            this.sqlPanel.Size = new System.Drawing.Size(142, 297);
            this.sqlPanel.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Сервер:";
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(8, 70);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(121, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "пароль для sa";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Controls.Add(this.updateSqlButton);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.comboBox3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(6, 125);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(129, 166);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Укажите базу UDM:";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(4, 68);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 5;
            // 
            // updateSqlButton
            // 
            this.updateSqlButton.Location = new System.Drawing.Point(6, 135);
            this.updateSqlButton.Name = "updateSqlButton";
            this.updateSqlButton.Size = new System.Drawing.Size(121, 23);
            this.updateSqlButton.TabIndex = 1;
            this.updateSqlButton.Text = "Обновить базы";
            this.updateSqlButton.UseVisualStyleBackColor = true;
            this.updateSqlButton.Click += new System.EventHandler(this.doWork_Button_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(4, 26);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(4, 108);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Укажите базу AWX:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Укажите базу HH:";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(21, 96);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(94, 23);
            this.connectButton.TabIndex = 1;
            this.connectButton.Text = "Подключиться";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(129, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = ".\\SQLEXPRESS2014";
            // 
            // baseAuthTypeCBox
            // 
            this.baseAuthTypeCBox.AutoSize = true;
            this.baseAuthTypeCBox.Checked = true;
            this.baseAuthTypeCBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.baseAuthTypeCBox.Location = new System.Drawing.Point(8, 47);
            this.baseAuthTypeCBox.Name = "baseAuthTypeCBox";
            this.baseAuthTypeCBox.Size = new System.Drawing.Size(115, 17);
            this.baseAuthTypeCBox.TabIndex = 1;
            this.baseAuthTypeCBox.Text = "Integrated Security";
            this.baseAuthTypeCBox.UseVisualStyleBackColor = true;
            this.baseAuthTypeCBox.CheckedChanged += new System.EventHandler(this.baseAuthTypeCBox_CheckedChanged);
            // 
            // logPanel
            // 
            this.logPanel.Controls.Add(this.logTBox);
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.logPanel.Location = new System.Drawing.Point(0, 519);
            this.logPanel.Name = "logPanel";
            this.logPanel.Size = new System.Drawing.Size(1006, 79);
            this.logPanel.TabIndex = 2;
            // 
            // logTBox
            // 
            this.logTBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTBox.Location = new System.Drawing.Point(0, 0);
            this.logTBox.Multiline = true;
            this.logTBox.Name = "logTBox";
            this.logTBox.ReadOnly = true;
            this.logTBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTBox.Size = new System.Drawing.Size(1006, 79);
            this.logTBox.TabIndex = 1;
            this.logTBox.TextChanged += new System.EventHandler(this.logTBox_TextChanged);
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.dataPanel);
            this.mainPanel.Controls.Add(this.splitter1);
            this.mainPanel.Controls.Add(this.leftPanel);
            this.mainPanel.Controls.Add(this.logPanel);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 30);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(1006, 598);
            this.mainPanel.TabIndex = 4;
            // 
            // dataPanel
            // 
            this.dataPanel.Controls.Add(this.dataGridView1);
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPanel.Location = new System.Drawing.Point(146, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(860, 514);
            this.dataPanel.TabIndex = 6;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidth = 10;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.Size = new System.Drawing.Size(860, 514);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(146, 514);
            this.splitter1.MinExtra = 400;
            this.splitter1.MinSize = 50;
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(860, 5);
            this.splitter1.TabIndex = 5;
            this.splitter1.TabStop = false;
            this.splitter1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitter1_SplitterMoved);
            // 
            // importExcelButton
            // 
            this.importExcelButton.Image = global::ProjectAssistant.Properties.Resources.Excel_icon;
            this.importExcelButton.Location = new System.Drawing.Point(82, 421);
            this.importExcelButton.Name = "importExcelButton";
            this.importExcelButton.Size = new System.Drawing.Size(58, 57);
            this.importExcelButton.TabIndex = 1;
            this.importExcelButton.UseVisualStyleBackColor = true;
            this.importExcelButton.Click += new System.EventHandler(this.importExcelButton_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // elementHost1
            // 
            this.elementHost1.Location = new System.Drawing.Point(0, 0);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(200, 100);
            this.elementHost1.TabIndex = 0;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = null;
            // 
            // coordsCBox
            // 
            this.coordsCBox.AutoSize = true;
            this.coordsCBox.Location = new System.Drawing.Point(7, 42);
            this.coordsCBox.Name = "coordsCBox";
            this.coordsCBox.Size = new System.Drawing.Size(108, 17);
            this.coordsCBox.TabIndex = 2;
            this.coordsCBox.Text = "по координатам";
            this.coordsCBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 628);
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.topPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ProjectAssistant";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.signalTypePanel.ResumeLayout(false);
            this.signalTypePanel.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.dynamicPanel.ResumeLayout(false);
            this.dynamicPanel.PerformLayout();
            this.soundPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.sqlPanel.ResumeLayout(false);
            this.sqlPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.logPanel.ResumeLayout(false);
            this.logPanel.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.dataPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.RadioButton soundRButton;
        private System.Windows.Forms.RadioButton sqlRButton;
        private System.Windows.Forms.RadioButton dynamicRButton;
        private System.Windows.Forms.RadioButton diRButton;
        private System.Windows.Forms.RadioButton aiRButton;
        private System.Windows.Forms.Panel signalTypePanel;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button importExcelButton;
        private System.Windows.Forms.CheckBox baseAuthTypeCBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button updateSqlButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.TextBox logTBox;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel soundPanel;
        private System.Windows.Forms.CheckBox swxBaseCBox;
        private System.Windows.Forms.CheckBox udmBaseCBox;
        private System.Windows.Forms.CheckBox scriptCBox;
        private System.Windows.Forms.CheckBox txtFilesCBox;
        private System.Windows.Forms.Button createSoundButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel dataPanel;
        private System.Windows.Forms.Panel sqlPanel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton updSwxRButton;
        private System.Windows.Forms.RadioButton crSwxRButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton updUdmRButton;
        private System.Windows.Forms.RadioButton crUdmRButton;
        private System.Windows.Forms.Panel dynamicPanel;
        private System.Windows.Forms.Button btnBuild;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private System.Windows.Forms.CheckBox coordsCBox;
    }
}

