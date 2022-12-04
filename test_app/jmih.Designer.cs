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
            this.progressBarReceive = new System.Windows.Forms.ProgressBar();
            this.writeParametersButton = new System.Windows.Forms.Button();
            this.readParametersButton = new System.Windows.Forms.Button();
            this.phaseCcheckBox = new System.Windows.Forms.CheckBox();
            this.phaseBcheckBox = new System.Windows.Forms.CheckBox();
            this.phaseAcheckBox = new System.Windows.Forms.CheckBox();
            this.baseBlockTelemetryDataGrid = new System.Windows.Forms.DataGridView();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ACurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CCurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockServerConstants)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baseBlockTelemetryDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // send_button
            // 
            this.send_button.Location = new System.Drawing.Point(16, 52);
            this.send_button.Name = "send_button";
            this.send_button.Size = new System.Drawing.Size(121, 28);
            this.send_button.TabIndex = 0;
            this.send_button.Text = "Отправить";
            this.send_button.UseVisualStyleBackColor = true;
            this.send_button.Click += new System.EventHandler(this.send_button_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(16, 24);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(446, 22);
            this.textBox2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(998, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Состояние соединения";
            // 
            // connection_log
            // 
            this.connection_log.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connection_log.Location = new System.Drawing.Point(16, 136);
            this.connection_log.Multiline = true;
            this.connection_log.Name = "connection_log";
            this.connection_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.connection_log.Size = new System.Drawing.Size(446, 569);
            this.connection_log.TabIndex = 4;
            // 
            // time_check_box_1
            // 
            this.time_check_box_1.AutoSize = true;
            this.time_check_box_1.Location = new System.Drawing.Point(332, 52);
            this.time_check_box_1.Name = "time_check_box_1";
            this.time_check_box_1.Size = new System.Drawing.Size(130, 20);
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
            this.connectionIndicator.Location = new System.Drawing.Point(1177, 109);
            this.connectionIndicator.Name = "connectionIndicator";
            this.connectionIndicator.Size = new System.Drawing.Size(18, 18);
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
            this.groupBox1.Location = new System.Drawing.Point(760, 171);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 721);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Сервисная консоль";
            // 
            // serviceConsoleHelpButton
            // 
            this.serviceConsoleHelpButton.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("serviceConsoleHelpButton.BackgroundImage")));
            this.serviceConsoleHelpButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.serviceConsoleHelpButton.Location = new System.Drawing.Point(143, 52);
            this.serviceConsoleHelpButton.Name = "serviceConsoleHelpButton";
            this.serviceConsoleHelpButton.Size = new System.Drawing.Size(29, 28);
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
            this.groupBox2.Location = new System.Drawing.Point(27, 26);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1211, 139);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Соединение";
            // 
            // closeConnectionButton
            // 
            this.closeConnectionButton.Location = new System.Drawing.Point(1001, 53);
            this.closeConnectionButton.Name = "closeConnectionButton";
            this.closeConnectionButton.Size = new System.Drawing.Size(194, 23);
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
            this.baseBlockServerConstants.Location = new System.Drawing.Point(21, 24);
            this.baseBlockServerConstants.Name = "baseBlockServerConstants";
            this.baseBlockServerConstants.RowHeadersVisible = false;
            this.baseBlockServerConstants.RowHeadersWidth = 51;
            this.baseBlockServerConstants.RowTemplate.Height = 24;
            this.baseBlockServerConstants.Size = new System.Drawing.Size(314, 103);
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
            this.TCP_CONNECTION.Location = new System.Drawing.Point(1001, 24);
            this.TCP_CONNECTION.Name = "TCP_CONNECTION";
            this.TCP_CONNECTION.Size = new System.Drawing.Size(194, 23);
            this.TCP_CONNECTION.TabIndex = 0;
            this.TCP_CONNECTION.Text = "Установка соединения";
            this.TCP_CONNECTION.UseVisualStyleBackColor = true;
            this.TCP_CONNECTION.Click += new System.EventHandler(this.TCP_CONNECTION_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.progressBarReceive);
            this.groupBox3.Controls.Add(this.writeParametersButton);
            this.groupBox3.Controls.Add(this.readParametersButton);
            this.groupBox3.Controls.Add(this.phaseCcheckBox);
            this.groupBox3.Controls.Add(this.phaseBcheckBox);
            this.groupBox3.Controls.Add(this.phaseAcheckBox);
            this.groupBox3.Controls.Add(this.baseBlockTelemetryDataGrid);
            this.groupBox3.Location = new System.Drawing.Point(27, 171);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(714, 721);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Значения в памяти индикатора";
            // 
            // progressBarReceive
            // 
            this.progressBarReceive.Location = new System.Drawing.Point(349, 680);
            this.progressBarReceive.Maximum = 200;
            this.progressBarReceive.Minimum = 1;
            this.progressBarReceive.Name = "progressBarReceive";
            this.progressBarReceive.Size = new System.Drawing.Size(305, 25);
            this.progressBarReceive.Step = 1;
            this.progressBarReceive.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarReceive.TabIndex = 6;
            this.progressBarReceive.Value = 1;
            // 
            // writeParametersButton
            // 
            this.writeParametersButton.Location = new System.Drawing.Point(21, 682);
            this.writeParametersButton.Name = "writeParametersButton";
            this.writeParametersButton.Size = new System.Drawing.Size(113, 23);
            this.writeParametersButton.TabIndex = 5;
            this.writeParametersButton.Text = "Записать";
            this.writeParametersButton.UseVisualStyleBackColor = true;
            this.writeParametersButton.Click += new System.EventHandler(this.writeParametersButton_Click);
            // 
            // readParametersButton
            // 
            this.readParametersButton.Location = new System.Drawing.Point(21, 653);
            this.readParametersButton.Name = "readParametersButton";
            this.readParametersButton.Size = new System.Drawing.Size(113, 23);
            this.readParametersButton.TabIndex = 4;
            this.readParametersButton.Text = "Прочитать";
            this.readParametersButton.UseVisualStyleBackColor = true;
            this.readParametersButton.Click += new System.EventHandler(this.readParametersButton_Click);
            // 
            // phaseCcheckBox
            // 
            this.phaseCcheckBox.AutoSize = true;
            this.phaseCcheckBox.Location = new System.Drawing.Point(578, 653);
            this.phaseCcheckBox.Name = "phaseCcheckBox";
            this.phaseCcheckBox.Size = new System.Drawing.Size(76, 20);
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
            this.phaseBcheckBox.Location = new System.Drawing.Point(466, 653);
            this.phaseBcheckBox.Name = "phaseBcheckBox";
            this.phaseBcheckBox.Size = new System.Drawing.Size(76, 20);
            this.phaseBcheckBox.TabIndex = 2;
            this.phaseBcheckBox.Text = "Фаза B";
            this.phaseBcheckBox.UseVisualStyleBackColor = false;
            this.phaseBcheckBox.CheckedChanged += new System.EventHandler(this.phaseBcheckBox_CheckedChanged);
            // 
            // phaseAcheckBox
            // 
            this.phaseAcheckBox.AutoSize = true;
            this.phaseAcheckBox.Location = new System.Drawing.Point(349, 653);
            this.phaseAcheckBox.Name = "phaseAcheckBox";
            this.phaseAcheckBox.Size = new System.Drawing.Size(76, 20);
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
            this.baseBlockTelemetryDataGrid.Location = new System.Drawing.Point(21, 24);
            this.baseBlockTelemetryDataGrid.Name = "baseBlockTelemetryDataGrid";
            this.baseBlockTelemetryDataGrid.RowHeadersVisible = false;
            this.baseBlockTelemetryDataGrid.RowHeadersWidth = 51;
            this.baseBlockTelemetryDataGrid.RowTemplate.Height = 24;
            this.baseBlockTelemetryDataGrid.Size = new System.Drawing.Size(664, 580);
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
            // Jmih
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 913);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
        private Button writeParametersButton;
        private Button readParametersButton;
        private CheckBox phaseCcheckBox;
        private CheckBox phaseBcheckBox;
        private CheckBox phaseAcheckBox;
        private Button serviceConsoleHelpButton;
        private Button closeConnectionButton;
        private DataGridViewTextBoxColumn Constants;
        private DataGridViewTextBoxColumn inputConstants;
        private ProgressBar progressBarReceive;
    }
}

