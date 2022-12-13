using System.Windows.Forms;

namespace test_app
{
    partial class Jmih
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Jmih));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.send_button = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connection_log = new System.Windows.Forms.TextBox();
            this.time_check_box_1 = new System.Windows.Forms.CheckBox();
            this.connectionIndicator = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.serviceConsoleHelpButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.closeConnectionButton = new System.Windows.Forms.Button();
            this.baseBlockServerConstants = new System.Windows.Forms.DataGridView();
            this.Constants = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputConstants = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TCP_CONNECTION = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.WriteSCADAParameterButton = new System.Windows.Forms.Button();
            this.SCADA_TextBox = new System.Windows.Forms.TextBox();
            this.ReadSCADAParameterButton = new System.Windows.Forms.Button();
            this.baseBlockUploadTimeLabel = new System.Windows.Forms.Label();
            this.progressBarReceive = new System.Windows.Forms.ProgressBar();
            this.writeIndicatorParametersButton = new System.Windows.Forms.Button();
            this.readIndicatorParametersButton = new System.Windows.Forms.Button();
            this.phaseCcheckBox = new System.Windows.Forms.CheckBox();
            this.phaseBcheckBox = new System.Windows.Forms.CheckBox();
            this.phaseAcheckBox = new System.Windows.Forms.CheckBox();
            this.baseBlockTelemetryDataGrid = new System.Windows.Forms.DataGridView();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ACurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProgressBarTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockServerConstants)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockTelemetryDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // send_button
            // 
            this.send_button.Location = new System.Drawing.Point(12, 42);
            this.send_button.Margin = new System.Windows.Forms.Padding(2);
            this.send_button.Name = "send_button";
            this.send_button.Size = new System.Drawing.Size(91, 23);
            this.send_button.TabIndex = 0;
            this.send_button.Text = "Отправить";
            this.send_button.UseVisualStyleBackColor = true;
            this.send_button.Click += new System.EventHandler(this.send_button_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 20);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(336, 20);
            this.textBox2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(748, 89);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Состояние соединения";
            // 
            // connection_log
            // 
            this.connection_log.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connection_log.Location = new System.Drawing.Point(12, 114);
            this.connection_log.Margin = new System.Windows.Forms.Padding(2);
            this.connection_log.Multiline = true;
            this.connection_log.Name = "connection_log";
            this.connection_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.connection_log.Size = new System.Drawing.Size(336, 473);
            this.connection_log.TabIndex = 4;
            // 
            // time_check_box_1
            // 
            this.time_check_box_1.AutoSize = true;
            this.time_check_box_1.Location = new System.Drawing.Point(249, 42);
            this.time_check_box_1.Margin = new System.Windows.Forms.Padding(2);
            this.time_check_box_1.Name = "time_check_box_1";
            this.time_check_box_1.Size = new System.Drawing.Size(105, 17);
            this.time_check_box_1.TabIndex = 5;
            this.time_check_box_1.Text = "Метка времени";
            this.time_check_box_1.UseVisualStyleBackColor = true;
            this.time_check_box_1.CheckedChanged += new System.EventHandler(this.time_check_box_1_CheckedChanged);
            // 
            // connectionIndicator
            // 
            this.connectionIndicator.AutoSize = true;
            this.connectionIndicator.BackColor = System.Drawing.Color.Lime;
            this.connectionIndicator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.connectionIndicator.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.connectionIndicator.Location = new System.Drawing.Point(883, 89);
            this.connectionIndicator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.connectionIndicator.Name = "connectionIndicator";
            this.connectionIndicator.Size = new System.Drawing.Size(18, 15);
            this.connectionIndicator.TabIndex = 6;
            this.connectionIndicator.Text = "   ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.serviceConsoleHelpButton);
            this.groupBox1.Controls.Add(this.time_check_box_1);
            this.groupBox1.Controls.Add(this.send_button);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.connection_log);
            this.groupBox1.Location = new System.Drawing.Point(570, 139);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(358, 604);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сервисная консоль";
            // 
            // serviceConsoleHelpButton
            // 
            this.serviceConsoleHelpButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("serviceConsoleHelpButton.BackgroundImage")));
            this.serviceConsoleHelpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.serviceConsoleHelpButton.Location = new System.Drawing.Point(107, 42);
            this.serviceConsoleHelpButton.Margin = new System.Windows.Forms.Padding(2);
            this.serviceConsoleHelpButton.Name = "serviceConsoleHelpButton";
            this.serviceConsoleHelpButton.Size = new System.Drawing.Size(22, 23);
            this.serviceConsoleHelpButton.TabIndex = 7;
            this.serviceConsoleHelpButton.UseVisualStyleBackColor = true;
            this.serviceConsoleHelpButton.Click += new System.EventHandler(this.serviceConsoleHelpButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.closeConnectionButton);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.baseBlockServerConstants);
            this.groupBox2.Controls.Add(this.TCP_CONNECTION);
            this.groupBox2.Controls.Add(this.connectionIndicator);
            this.groupBox2.Location = new System.Drawing.Point(20, 21);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(908, 113);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Соединение";
            // 
            // closeConnectionButton
            // 
            this.closeConnectionButton.Location = new System.Drawing.Point(751, 43);
            this.closeConnectionButton.Margin = new System.Windows.Forms.Padding(2);
            this.closeConnectionButton.Name = "closeConnectionButton";
            this.closeConnectionButton.Size = new System.Drawing.Size(146, 19);
            this.closeConnectionButton.TabIndex = 6;
            this.closeConnectionButton.Text = "Завершение соединения";
            this.closeConnectionButton.UseVisualStyleBackColor = true;
            this.closeConnectionButton.Click += new System.EventHandler(this.closeConnectionButton_Click);
            // 
            // baseBlockServerConstants
            // 
            this.baseBlockServerConstants.AllowUserToResizeColumns = false;
            this.baseBlockServerConstants.AllowUserToResizeRows = false;
            this.baseBlockServerConstants.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.baseBlockServerConstants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.baseBlockServerConstants.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Constants,
            this.inputConstants});
            this.baseBlockServerConstants.Location = new System.Drawing.Point(16, 20);
            this.baseBlockServerConstants.Margin = new System.Windows.Forms.Padding(2);
            this.baseBlockServerConstants.Name = "baseBlockServerConstants";
            this.baseBlockServerConstants.RowHeadersVisible = false;
            this.baseBlockServerConstants.RowHeadersWidth = 51;
            this.baseBlockServerConstants.RowTemplate.Height = 24;
            this.baseBlockServerConstants.Size = new System.Drawing.Size(236, 84);
            this.baseBlockServerConstants.TabIndex = 1;
            this.baseBlockServerConstants.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.baseBlockServerConstants_CellValidating);
            // 
            // Constants
            // 
            this.Constants.HeaderText = "Названия данных";
            this.Constants.MinimumWidth = 6;
            this.Constants.Name = "Constants";
            this.Constants.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // inputConstants
            // 
            this.inputConstants.HeaderText = "Ввод данных";
            this.inputConstants.MinimumWidth = 6;
            this.inputConstants.Name = "inputConstants";
            this.inputConstants.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // TCP_CONNECTION
            // 
            this.TCP_CONNECTION.Location = new System.Drawing.Point(751, 20);
            this.TCP_CONNECTION.Margin = new System.Windows.Forms.Padding(2);
            this.TCP_CONNECTION.Name = "TCP_CONNECTION";
            this.TCP_CONNECTION.Size = new System.Drawing.Size(146, 19);
            this.TCP_CONNECTION.TabIndex = 0;
            this.TCP_CONNECTION.Text = "Установка соединения";
            this.TCP_CONNECTION.UseVisualStyleBackColor = true;
            this.TCP_CONNECTION.Click += new System.EventHandler(this.TCP_CONNECTION_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.WriteSCADAParameterButton);
            this.groupBox3.Controls.Add(this.SCADA_TextBox);
            this.groupBox3.Controls.Add(this.ReadSCADAParameterButton);
            this.groupBox3.Controls.Add(this.baseBlockUploadTimeLabel);
            this.groupBox3.Controls.Add(this.progressBarReceive);
            this.groupBox3.Controls.Add(this.writeIndicatorParametersButton);
            this.groupBox3.Controls.Add(this.readIndicatorParametersButton);
            this.groupBox3.Controls.Add(this.phaseCcheckBox);
            this.groupBox3.Controls.Add(this.phaseBcheckBox);
            this.groupBox3.Controls.Add(this.phaseAcheckBox);
            this.groupBox3.Controls.Add(this.baseBlockTelemetryDataGrid);
            this.groupBox3.Location = new System.Drawing.Point(20, 139);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(536, 604);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Значения в памяти индикатора";
            // 
            // WriteSCADAParameterButton
            // 
            this.WriteSCADAParameterButton.Location = new System.Drawing.Point(262, 502);
            this.WriteSCADAParameterButton.Name = "WriteSCADAParameterButton";
            this.WriteSCADAParameterButton.Size = new System.Drawing.Size(238, 23);
            this.WriteSCADAParameterButton.TabIndex = 10;
            this.WriteSCADAParameterButton.Text = "Записать данные в базовый блок";
            this.WriteSCADAParameterButton.UseVisualStyleBackColor = true;
            this.WriteSCADAParameterButton.Click += new System.EventHandler(this.WriteSCADAParameterButton_Click);
            // 
            // SCADA_TextBox
            // 
            this.SCADA_TextBox.Location = new System.Drawing.Point(16, 476);
            this.SCADA_TextBox.Name = "SCADA_TextBox";
            this.SCADA_TextBox.Size = new System.Drawing.Size(169, 20);
            this.SCADA_TextBox.TabIndex = 9;
            this.SCADA_TextBox.TextChanged += new System.EventHandler(this.SCADA_TextBox_TextChanged);
            this.SCADA_TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SCADA_TextBox_KeyDown);
            // 
            // ReadSCADAParameterButton
            // 
            this.ReadSCADAParameterButton.Location = new System.Drawing.Point(262, 473);
            this.ReadSCADAParameterButton.Name = "ReadSCADAParameterButton";
            this.ReadSCADAParameterButton.Size = new System.Drawing.Size(238, 23);
            this.ReadSCADAParameterButton.TabIndex = 8;
            this.ReadSCADAParameterButton.Text = "Прочитать данные с базового блока";
            this.ReadSCADAParameterButton.UseVisualStyleBackColor = true;
            this.ReadSCADAParameterButton.Click += new System.EventHandler(this.ReadSCADAParameterButton_Click);
            // 
            // baseBlockUploadTimeLabel
            // 
            this.baseBlockUploadTimeLabel.AutoSize = true;
            this.baseBlockUploadTimeLabel.Location = new System.Drawing.Point(13, 460);
            this.baseBlockUploadTimeLabel.Name = "baseBlockUploadTimeLabel";
            this.baseBlockUploadTimeLabel.Size = new System.Drawing.Size(172, 13);
            this.baseBlockUploadTimeLabel.TabIndex = 7;
            this.baseBlockUploadTimeLabel.Text = "Время отправки телеизмерений";
            // 
            // progressBarReceive
            // 
            this.progressBarReceive.Location = new System.Drawing.Point(16, 567);
            this.progressBarReceive.Margin = new System.Windows.Forms.Padding(2);
            this.progressBarReceive.Maximum = 1000;
            this.progressBarReceive.Minimum = 1;
            this.progressBarReceive.Name = "progressBarReceive";
            this.progressBarReceive.Size = new System.Drawing.Size(229, 20);
            this.progressBarReceive.Step = 1;
            this.progressBarReceive.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarReceive.TabIndex = 6;
            this.progressBarReceive.Value = 1;
            // 
            // writeIndicatorParametersButton
            // 
            this.writeIndicatorParametersButton.Location = new System.Drawing.Point(262, 566);
            this.writeIndicatorParametersButton.Margin = new System.Windows.Forms.Padding(2);
            this.writeIndicatorParametersButton.Name = "writeIndicatorParametersButton";
            this.writeIndicatorParametersButton.Size = new System.Drawing.Size(238, 22);
            this.writeIndicatorParametersButton.TabIndex = 5;
            this.writeIndicatorParametersButton.Text = "Записать данные на индикатор";
            this.writeIndicatorParametersButton.UseVisualStyleBackColor = true;
            this.writeIndicatorParametersButton.Click += new System.EventHandler(this.writeIndicatorParametersButton_Click);
            // 
            // readIndicatorParametersButton
            // 
            this.readIndicatorParametersButton.Location = new System.Drawing.Point(262, 538);
            this.readIndicatorParametersButton.Margin = new System.Windows.Forms.Padding(2);
            this.readIndicatorParametersButton.Name = "readIndicatorParametersButton";
            this.readIndicatorParametersButton.Size = new System.Drawing.Size(238, 23);
            this.readIndicatorParametersButton.TabIndex = 4;
            this.readIndicatorParametersButton.Text = "Прочитать данные с индикатора";
            this.readIndicatorParametersButton.UseVisualStyleBackColor = true;
            this.readIndicatorParametersButton.Click += new System.EventHandler(this.readIndicatorParametersButton_Click);
            // 
            // phaseCcheckBox
            // 
            this.phaseCcheckBox.AutoSize = true;
            this.phaseCcheckBox.Location = new System.Drawing.Point(180, 544);
            this.phaseCcheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.phaseCcheckBox.Name = "phaseCcheckBox";
            this.phaseCcheckBox.Size = new System.Drawing.Size(65, 17);
            this.phaseCcheckBox.TabIndex = 3;
            this.phaseCcheckBox.Text = "Фаза С";
            this.phaseCcheckBox.UseVisualStyleBackColor = true;
            this.phaseCcheckBox.CheckedChanged += new System.EventHandler(this.phaseCcheckBox_CheckedChanged);
            // 
            // phaseBcheckBox
            // 
            this.phaseBcheckBox.AutoSize = true;
            this.phaseBcheckBox.BackColor = System.Drawing.SystemColors.Control;
            this.phaseBcheckBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.phaseBcheckBox.Location = new System.Drawing.Point(98, 544);
            this.phaseBcheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.phaseBcheckBox.Name = "phaseBcheckBox";
            this.phaseBcheckBox.Size = new System.Drawing.Size(65, 17);
            this.phaseBcheckBox.TabIndex = 2;
            this.phaseBcheckBox.Text = "Фаза B";
            this.phaseBcheckBox.UseVisualStyleBackColor = false;
            this.phaseBcheckBox.CheckedChanged += new System.EventHandler(this.phaseBcheckBox_CheckedChanged);
            // 
            // phaseAcheckBox
            // 
            this.phaseAcheckBox.AutoSize = true;
            this.phaseAcheckBox.Location = new System.Drawing.Point(16, 544);
            this.phaseAcheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.phaseAcheckBox.Name = "phaseAcheckBox";
            this.phaseAcheckBox.Size = new System.Drawing.Size(65, 17);
            this.phaseAcheckBox.TabIndex = 1;
            this.phaseAcheckBox.Text = "Фаза А";
            this.phaseAcheckBox.UseVisualStyleBackColor = true;
            this.phaseAcheckBox.CheckedChanged += new System.EventHandler(this.phaseAcheckBox_CheckedChanged);
            // 
            // baseBlockTelemetryDataGrid
            // 
            this.baseBlockTelemetryDataGrid.AllowUserToAddRows = false;
            this.baseBlockTelemetryDataGrid.AllowUserToDeleteRows = false;
            this.baseBlockTelemetryDataGrid.AllowUserToResizeColumns = false;
            this.baseBlockTelemetryDataGrid.AllowUserToResizeRows = false;
            this.baseBlockTelemetryDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.baseBlockTelemetryDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.baseBlockTelemetryDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Type,
            this.ACurrent,
            this.BCurrent,
            this.CCurrent});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.baseBlockTelemetryDataGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.baseBlockTelemetryDataGrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.baseBlockTelemetryDataGrid.Location = new System.Drawing.Point(16, 20);
            this.baseBlockTelemetryDataGrid.Margin = new System.Windows.Forms.Padding(2);
            this.baseBlockTelemetryDataGrid.Name = "baseBlockTelemetryDataGrid";
            this.baseBlockTelemetryDataGrid.RowHeadersVisible = false;
            this.baseBlockTelemetryDataGrid.RowHeadersWidth = 51;
            this.baseBlockTelemetryDataGrid.RowTemplate.Height = 24;
            this.baseBlockTelemetryDataGrid.Size = new System.Drawing.Size(498, 420);
            this.baseBlockTelemetryDataGrid.TabIndex = 0;
            this.baseBlockTelemetryDataGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.baseBlockTelemetryDataGrid_CellValidating);
            // 
            // Type
            // 
            this.Type.FillWeight = 235.2941F;
            this.Type.HeaderText = "Тип значения";
            this.Type.MinimumWidth = 6;
            this.Type.Name = "Type";
            // 
            // ACurrent
            // 
            this.ACurrent.FillWeight = 54.90196F;
            this.ACurrent.HeaderText = "Фаза А";
            this.ACurrent.MinimumWidth = 6;
            this.ACurrent.Name = "ACurrent";
            // 
            // BCurrent
            // 
            this.BCurrent.FillWeight = 54.90196F;
            this.BCurrent.HeaderText = "Фаза B";
            this.BCurrent.MinimumWidth = 6;
            this.BCurrent.Name = "BCurrent";
            // 
            // CCurrent
            // 
            this.CCurrent.FillWeight = 54.90196F;
            this.CCurrent.HeaderText = "Фаза С";
            this.CCurrent.MinimumWidth = 6;
            this.CCurrent.Name = "CCurrent";
            // 
            // ProgressBarTimer
            // 
            //this.ProgressBarTimer.Interval = 1000;
            //this.ProgressBarTimer.Tick += new System.EventHandler(this.ProgressBarTimer_Tick);
            // 
            // Jmih
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(946, 754);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Jmih";
            this.Text = "jmih";
            this.Load += new System.EventHandler(this.jmih_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockServerConstants)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockTelemetryDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button send_button;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox connection_log;
        private System.Windows.Forms.CheckBox time_check_box_1;
        private System.Windows.Forms.Label connectionIndicator;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView baseBlockServerConstants;
        private System.Windows.Forms.Button TCP_CONNECTION;
        private GroupBox groupBox3;
        private DataGridView baseBlockTelemetryDataGrid;
        private DataGridViewTextBoxColumn Type;
        private DataGridViewTextBoxColumn ACurrent;
        private DataGridViewTextBoxColumn BCurrent;
        private DataGridViewTextBoxColumn CCurrent;
        private Button writeIndicatorParametersButton;
        private Button readIndicatorParametersButton;
        private CheckBox phaseCcheckBox;
        private CheckBox phaseBcheckBox;
        private CheckBox phaseAcheckBox;
        private Button serviceConsoleHelpButton;
        private Button closeConnectionButton;
        private DataGridViewTextBoxColumn Constants;
        private DataGridViewTextBoxColumn inputConstants;
        private ProgressBar progressBarReceive;
        private Label baseBlockUploadTimeLabel;
        private TextBox SCADA_TextBox;
        private Button ReadSCADAParameterButton;
        private Button WriteSCADAParameterButton;
        private Timer ProgressBarTimer;
    }
}

