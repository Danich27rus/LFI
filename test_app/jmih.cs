﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

//***************   Автор: Даниил Григорьев (Captain_Kirk54)    *********************
//***   ПО преддназначено для чтения данных с базового блока ИКЗ и записи на       **
//***   него своих значений для дальнейшей эксплуатации оборудования. Общение      **
//***   проходит по МЭК-104. Сначала идёт запрос по TCP на чтение данных.          **
//***   После этого в ответе приходят ДВА пакета - WRITE_RESPONSE и SHOW_          **
//***   S_FRAME. Запись происходит путём отправки запроса с значениями.            **
//***********************************************************************************


namespace test_app
{
    public partial class Jmih : Form
    {

        #region Переменные в среде жмыха 

        private Rectangle OriginalRectangle1;
        private Rectangle OriginalRectangle2;
        private Rectangle OriginalRectangle3;
        private Size OriginalFormSize;

        //TODO: Добавить комментарии к каждой переменной
        private static byte[] _dataResponse;
        private bool _showTime;
        private int _phase; //Фаза А - 1, Фаза Б - 2, Фаза С - 3


        public string[] str;
        public int sizeStr;

        protected CancellationTokenSource TokenSource = new CancellationTokenSource();
        protected TcpClient BaseBlockSender;
        protected IPAddress BaseBlockIp;
        protected NetworkStream BaseBlockStream;

        #endregion

        public Jmih()
        {
            InitializeComponent();
            SuspendLayout();

            ResumeLayout(false);
            PerformLayout();
        }

        /// Сводка:
        ///     Стартовая инициализация после запуска приложения
        ///     Настраивает таблицу с телеизмерениями и таблицу 
        ///     с вводными данными для подключения к серверу
        private void jmih_Load(object sender, EventArgs e)
        {
            connectionIndicator.BackColor = Color.White;
            _showTime = false;
            _phase = 0;
            //OriginalFormSize = this.Size;//new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height);
            //OriginalRectangle1 = new Rectangle(groupBox1.Location.X, groupBox1.Location.Y, groupBox1.Width, groupBox1.Height);
            //OriginalRectangle2 = new Rectangle(groupBox2.Location.X, groupBox2.Location.Y, groupBox2.Width, groupBox2.Height);
            //OriginalRectangle3 = new Rectangle(groupBox3.Location.X, groupBox3.Location.Y, groupBox3.Width, groupBox3.Height);

            inputDataGrid_Setup();
            telemetryDataGrid_Setup();
        }

        private void ResizeControl(Rectangle r, Control c)
        {
            float xRatio = (float)(this.Width) / (float)(OriginalFormSize.Width);
            float yRatio = (float)(this.Height) / (float)(OriginalFormSize.Height);

            int newX = (int)(r.X * xRatio);
            int newY = (int)(r.Y * yRatio);

            int newWidth = (int)(r.Width * xRatio);
            int newHeight = (int)(r.Height * yRatio);

            c.Location = new Point(newX, newY);
            c.Size = new Size(newWidth, newHeight);
        }

        private void Jmih_Resize(object sender, EventArgs e)
        {
            //resizeControl(OriginalRectangle1, groupBox1);
            //resizeControl(OriginalRectangle2, groupBox2);
            //resizeControl(OriginalRectangle3, groupBox3);
        }

        //---------------Таймер для телеизмерений-----------------------
        /// Сводка:
        ///     Таймер для телеизмерений
        ///     Необходим для остановки потока телеизмерений 
        ///     при первом считывании с базового блока
        /// 
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события
        private async void StopTimer(object sender, EventArgs e)
        {
            //if (_teleindicationFlag)
            //{
            //return;
            //}

            byte[] data = new byte[512];
            //byte[] stop = { 0x68, 0x04, 0x13, 0x00, 0x02, 0x00 };

            //await BaseBlockStream.WriteAsync(stop, 0, stop.Length);
            await BaseBlockStream.ReadAsync(data, 0, data.Length);

            //_teleindicationFlag = true;
            TeleindicationStopTimer.Stop();
        }

        //----------------Индикатор выполнения--------------------------
        /// Сводка:
        ///     Индикатор выполнения
        ///     Изменяет элемент формы ProgressBar, постепенно заполняя его
        /// 
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события
        private void IncreaseProgressBar(object sender, EventArgs e)
        {
            progressBarReceive.PerformStep();
            if (progressBarReceive.Value == progressBarReceive.Maximum)
            {
                ProgressBarTimer.Stop();
            }
        }

        //--------------Включение и отколючение кнопопк-----------
        /// Сводка:
        ///     Включение и отключение кнопок
        ///     Выключает кнопки при передаче данных через TCP,
        ///     чтобы пользователь не смог повредить информацию во время передачи/ <summary>
        ///     приёма
        /// 
        private void DisableButtons()
        {
            TCP_CONNECTION.Enabled = false;
            closeConnectionButton.Enabled = false;
            send_button.Enabled = false;
            readIndicatorParametersButton.Enabled = false;
            writeIndicatorParametersButton.Enabled = false;
            ReadSCADAParameterButton.Enabled = false;
            WriteSCADAParameterButton.Enabled = false;
        }

        private void EnableButtons()
        {
            TCP_CONNECTION.Enabled = true;
            closeConnectionButton.Enabled = true;
            send_button.Enabled = true;
            readIndicatorParametersButton.Enabled = true;
            writeIndicatorParametersButton.Enabled = true;
            ReadSCADAParameterButton.Enabled = true;
            WriteSCADAParameterButton.Enabled = true;
        }

        //-----------Настройка таблицы с IP/Port------------
        /// Сводка:
        ///     Задаёт некоторые параметры для таблицы
        ///     в группе "Соединение": устанавливает вид
        ///     требуемых данных в первйо колонке и делает их только для чтения
        /// 
        private void inputDataGrid_Setup()
        {
            baseBlockServerConstants.Rows.Add("IP Адрес");
            baseBlockServerConstants.Rows.Add("Порт");
            baseBlockServerConstants.AllowUserToAddRows = false;
            baseBlockServerConstants.AllowUserToOrderColumns = false;
            baseBlockServerConstants.RowHeadersVisible = false;
            baseBlockServerConstants.ClearSelection();

            baseBlockServerConstants.Rows[0].Cells[0].ReadOnly = true;
            baseBlockServerConstants.Rows[1].Cells[0].ReadOnly = true;
        }

        //---------Настройка таблицы с телеизмерениями----------
        /// Сводка:
        ///     Задаёт названия парметров в таблице
        ///     значений, получаемых от базового блока.
        ///     Все значения разделены на 3 фазы, поэтому в приложении
        ///     есть фаза A,B и C. Поля с названиями секций параметров
        ///     закрашиваются серым
        /// 
        private void telemetryDataGrid_Setup()
        {
            baseBlockTelemetryDataGrid.Rows.Add("Общие параметры индикатора");
            baseBlockTelemetryDataGrid.Rows[0].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[0].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Величина электрического поля (в В/м)");
            baseBlockTelemetryDataGrid.Rows.Add("Минимальное значение тока в линии (в А)");
            baseBlockTelemetryDataGrid.Rows.Add("Интервал отправки телеизмерений №1 (в с)");
            baseBlockTelemetryDataGrid.Rows.Add("Интервал отправки телеизмерений №2 (в с)");
            baseBlockTelemetryDataGrid.Rows.Add("Период отключения датчиков (в мин)");
            baseBlockTelemetryDataGrid.Rows.Add("Порог значения тока для отправки телеизмерений (в А)");
            baseBlockTelemetryDataGrid.Rows.Add("Относительное значение изменения тока (в %)");
            baseBlockTelemetryDataGrid.Rows.Add("Абсолютное значение изменения тока (в А)");

            baseBlockTelemetryDataGrid.Rows.Add("Параметры определения межфазного замыкания");
            baseBlockTelemetryDataGrid.Rows[9].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[9].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Время перезап. индик. после КЗ на линии (в с)");
            baseBlockTelemetryDataGrid.Rows.Add("Время перезап. индик. после КЗ на линии после перезап. (в с)");
            baseBlockTelemetryDataGrid.Rows.Add("Минимальное значение тока КЗ на линии (в А)");

            baseBlockTelemetryDataGrid.Rows.Add("Параметры определения замыкания на землю");
            baseBlockTelemetryDataGrid.Rows[13].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[13].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Падение напряжения в линии (в %)");
            baseBlockTelemetryDataGrid.Rows.Add("Время засечки падения напряжения (в с)");
            baseBlockTelemetryDataGrid.ClearSelection();

            baseBlockTelemetryDataGrid.AllowUserToAddRows = false;
            baseBlockTelemetryDataGrid.RowHeadersVisible = false;


            //Заголовки и секции readonly, остальное можно спокойно изменять
            for (var i = 0; i < 16; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[i].Cells[0].ReadOnly = true;
                if (i == 0 || i == 9 || i == 13)
                {
                    for (var j = 0; j < 4; ++j)
                    {
                        baseBlockTelemetryDataGrid.Rows[i].Cells[j].ReadOnly = true;
                    }
                }
            }
            //baseBlockTelemetryDataGrid.Rows[0].Cells[0].ReadOnly = true;
        }

        //---Добавление метки времени к сообщению в консоли--
        /// Сводка:
        ///     При включении этого чекбокса, у всех сообщений, полученных
        ///     после включения автоматически добавляется время отправки и
        ///     получения информации.
        /// 
        private void time_check_box_1_CheckedChanged(object sender, EventArgs e)
        {
            _showTime = false;
            if (time_check_box_1.Checked)
            {
                _showTime = true;
            }
        }

        //----------Кнопка "Отправка" для данных------------
        /// Сводка:
        ///     По нажатию кнопки данные, ввеёные в строке отправляются
        ///     по TCP на роутер и происходит проброс по RS-232.
        ///     Данные отправляются ТОЛЬКО В HEX И С ПРОБЕЛАМИ!
        ///     Например: 68 04 03 00 00 00
        ///     При любом другом формате отправки возникнет ошибка
        ///     ввода/вывода, из-за неправильного парсинга строки
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события      
        private async void send_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;
            if (BaseBlockStream == null) return;
            try
            {
                byte[] dataSend = AdditionalFunctions.StringToByteArray(textBox2.Text);
                await BaseBlockStream.WriteAsync(dataSend, 0, dataSend.Length);

                _dataResponse = new byte[256];
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }

            string[] str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');
            int sizeStr = Convert.ToInt16(str[1], 16);
            Array.Resize(ref _dataResponse, sizeStr + 2);

            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(
                string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Ответ", _showTime));
        }

        //-----------Кнопка "Установка соединения"---------------
        /// Сводка:
        ///     По нажатию этой кнопки и введённых значений в таблице
        ///     IP/Port, мы подключаемся к сокету
        ///     Если данная операция не была завершена, то приложение выдаст ошибку
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события         
        private async void TCP_CONNECTION_Click(object sender, EventArgs e)
        {
            if (baseBlockServerConstants.Rows[0].Cells[1].Value == null ||
                baseBlockServerConstants.Rows[1].Cells[1].Value == null)
            {
                MessageBox.Show("Какое-то из полей IP/Порт пустое, его необходимо заполнить", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(baseBlockServerConstants.Rows[0].Cells[1].Value.ToString(), "IP", _showTime));
            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(baseBlockServerConstants.Rows[1].Cells[1].Value.ToString(), "Порт", _showTime));

            BaseBlockSender = new TcpClient();
            //TODO: Сделать через using/await (красиво)
            /*using (var BaseBlockSender = new TcpClient())
            {

            }*/
            BaseBlockIp = IPAddress.Parse(baseBlockServerConstants.Rows[0].Cells[1].Value.ToString());
            try
            {
                DisableButtons();
                ProgressBarTimer.Start();
                await BaseBlockSender.ConnectAsync(BaseBlockIp, Convert.ToInt16(baseBlockServerConstants.Rows[1].Cells[1].Value.ToString()));
                ProgressBarTimer.Stop();
                BaseBlockStream = BaseBlockSender.GetStream();
                connectionIndicator.BackColor = Color.Lime;
                EnableButtons();

            }
            catch (SocketException ex)
            {
                progressBarReceive.Value = 1;
                ProgressBarTimer.Stop();
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.ScktExc, ex.ToString()).ToString(), "Код ошибки", _showTime));
                return;
            }
            byte[] BlockInit = { 0x68, 0x04, 0x07, 0x00, 0x02, 0x00 };
            _dataResponse = new byte[256];
            await BaseBlockStream.WriteAsync(BlockInit, 0, BlockInit.Length);
            await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
        }

        //--------------Кнопка "Очищение лога"-----------------------------
        /// Сводка:
        ///     По нажатию этой кнопки полностью удалится
        ///     весь текст из служебной консоли
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            connection_log.Clear();
        }

        //-------------Кнопка "Очищение все данные в таблице"-------------
        /// Сводка:
        ///     По нажатию этой кнопки полностью удалится
        ///     вся информация в таблице "Значения в памяти индикатора"
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void DataGridClearButton_Click(object sender, EventArgs e)
        {
            for (int i = 1; i < baseBlockTelemetryDataGrid.Columns.Count; i++)
            {
                for (int j = 1; j < baseBlockTelemetryDataGrid.Rows.Count; j++)
                {
                    baseBlockTelemetryDataGrid.Rows[j].Cells[i].Value = null;
                }
            }
            baseBlockTelemetryDataGrid.Refresh();
        }

        //--------------Кнопка "Завершение соедиения"----------------------
        /// Сводка:
        ///     По нажатию этой кнопки завершится соединение с роутером
        ///     Если соединения не было, то появится специально окно с надписью
        ///     "Соединение не установлено"
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void closeConnectionButton_Click(object sender, EventArgs e)
        {
            //TODO Вывести в отдельную ассинхронную задачу
            if (BaseBlockStream == null)
            {
                MessageBox.Show("Соединенине не установлено", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            try
            {
                BaseBlockSender.GetStream().Close();
                BaseBlockSender.Close();
                connectionIndicator.BackColor = Color.White;
            }
            catch (System.InvalidOperationException e1)
            {
                connectionIndicator.BackColor = Color.Yellow;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.InvalOpExc, e1.ToString()).ToString(), "Код ошибки", _showTime));
            }
            //System.InvalidOperationException
            //System.InvalidOperationException: "Операция не разрешается на неподключенных сокетах."
        }


        #region Функции, отвечающие за валидацию данных

        //----------------Валидация введёных данных в DataGrid-----------------------
        //----Данные: IP и порты-----------------------------------------------------
        /// Сводка:
        ///     По нажатию этой кнопки завершится соединение с роутером
        ///     Если соединения не было, то появится специально окно с надписью
        ///     "Соединение не установлено"
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void baseBlockServerConstants_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string strCurData;
            string strIpValidated = "";
            if (baseBlockServerConstants.CurrentCell.ColumnIndex == 1)//нужный столбец
            {
                if (baseBlockServerConstants.CurrentCell.RowIndex == 0)
                {
                    if (baseBlockServerConstants.CurrentCell.EditedFormattedValue.ToString() == "")
                    {
                        return;
                    }
                    try
                    {
                        strCurData = baseBlockServerConstants.CurrentCell.EditedFormattedValue.ToString();
                        string[] strIp = strCurData.Split('.');
                        if (strIp.Length != 4)
                        {
                            MessageBox.Show("IP адрес написан не по стандарту\n\rФормат записи: XXX.XXX.XXX.XXX или XXX.XXX.X.X", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                            baseBlockServerConstants.EndEdit();
                            baseBlockServerConstants.CurrentCell.Value = "";
                            return;
                        }
                        for (var i = 0; i <= 3; ++i)
                        {
                            strIpValidated += strCurData;
                            if (strIp[i] == "")
                            {
                                MessageBox.Show("IP адресс написан неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                baseBlockServerConstants.EndEdit();
                                baseBlockServerConstants.CurrentCell.Value = "";
                                //dataGridView1.CurrentCell = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex];//неработаюшая попытка вернуть курсор при неправильном вводе
                                return;
                            }
                            if ((Convert.ToInt32(strIp[i]) > 255 || (Convert.ToInt32(strIp[i]) < 0)))
                            {
                                MessageBox.Show("Все числа должны быть в диапазоне от 0 до 255", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                baseBlockServerConstants.EndEdit();
                                baseBlockServerConstants.CurrentCell.Value = "";
                                //dataGridView1.CurrentCell = dataGridView1[dataGridView1.CurrentCell.ColumnIndex, dataGridView1.CurrentCell.RowIndex];//неработаюшая попытка вернуть курсор при неправильном вводе
                                return;
                            }
                        }
                    }
                    catch (System.Exception e1)
                    {
                        connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString()).ToString(), "Код ошибки", _showTime));
                    }
                }
                if (baseBlockServerConstants.CurrentCell.RowIndex == 1)
                {
                    if (baseBlockServerConstants.CurrentCell.EditedFormattedValue.ToString() == "")
                    {
                        return;
                    }
                    strCurData = baseBlockServerConstants.CurrentCell.EditedFormattedValue.ToString();
                    try
                    {
                        if (Convert.ToInt64(strCurData) > 65535)
                        {
                            MessageBox.Show("Порт макcимально может быть 65535");
                            baseBlockServerConstants.CommitEdit(DataGridViewDataErrorContexts.Commit);
                            baseBlockServerConstants.CurrentCell.Value = "";
                            baseBlockServerConstants.EditingControl.Text = "";
                            return;
                        }
                    }
                    catch (System.Exception e1)
                    {
                        connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString()).ToString(), "Код ошибки", _showTime));
                    }
                }
            }
        }

        //---------------Проверка на изменение ячейки в таблице телеизмерений------------
        /// Сводка:
        ///     В таблице телеизмерений происходит валидация данных, и сравнения со старыми данными
        ///     Если значение будет изменено пользователем
        ///     вне зависимости было ли в ячейке что-то или нет - шрифт станет жирным
        ///     Если значение перезаписывается компьютером - шрифт будет нормальным
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void baseBlockTelemetryDataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            string save = null;
            if (baseBlockTelemetryDataGrid.CurrentCell.ColumnIndex == 0)
            {
                return;
            }
            if (baseBlockTelemetryDataGrid.CurrentCell.EditedFormattedValue.ToString() == "")
            {
                return;
            }
            if (!(int.TryParse(baseBlockTelemetryDataGrid.CurrentCell.EditedFormattedValue.ToString(), out _)))
            {
                if (baseBlockTelemetryDataGrid.CurrentCell.Value == null)
                {
                    baseBlockTelemetryDataGrid.EndEdit();
                    baseBlockTelemetryDataGrid.CurrentCell.Value = null;
                }
                else
                {
                    save = baseBlockTelemetryDataGrid.CurrentCell.Value.ToString();
                    baseBlockTelemetryDataGrid.EndEdit();
                    baseBlockTelemetryDataGrid.CurrentCell.Value = save;
                }
                MessageBox.Show("Введёное значение не является числом, необходимо ввести число", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            try
            {
                if (baseBlockTelemetryDataGrid.CurrentCell.EditedFormattedValue.ToString() != save && string.IsNullOrEmpty(save))
                {
                    //colNum = baseBlockServerConstants.CurrentCell
                    baseBlockTelemetryDataGrid.Rows[baseBlockTelemetryDataGrid.CurrentCell.RowIndex].Cells[baseBlockTelemetryDataGrid.CurrentCell.ColumnIndex].Style.Font =
                        new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                }
            }
            catch (System.Exception e1)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString()).ToString(), "Код ошибки", _showTime));
            }

        }

        //---------------Поле около кнопки "Прочитать данные с базвого блока"------------
        /// Сводка:
        ///     Проверка ввода со стороны пользователя, чтобы нельзя было ввести 
        ///     буквы, спецсимволы и число с плавающей точкой
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void SCADA_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            SCADA_TextBox.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))// && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //e.Handled = true;
            //}
        }

        //---------------Поле около кнопки "Прочитать данные с базвого блока"------------
        /// Сводка:
        ///     Проверка введённого числа, чтобы было не больше 65535 (ограничение на ББ у блока памяти - 2 байта) 
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void SCADA_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SCADA_TextBox.Text))
            {
                return;
            }
            if (Convert.ToInt64(SCADA_TextBox.Text) > 65535)
            {
                MessageBox.Show("Значение не должно быть больше 65535", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                SCADA_TextBox.Text = "65535";
                //return;
                //e.Handled = true;
            }
        }
        #endregion

        //-------------------------Кнопка с вопросительным знаком------------------------
        /// Сводка:
        ///     Вывод небольшой справки в MessageBox 
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void serviceConsoleHelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данная сервисная консоль предназначена для вывода информации в raw-виде" +
                " и для отладки напрямую. Не реккомендуется использовать если вы не знаете как " +
                "работает протокол МЭК-104", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        //-----------------Обработка нажатий на разные checkBoxы с фазами----------------
        /// Сводка:
        ///     Выбор фазы (индикатора) в программе в блоке "значения в памяти идикатора"
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void phaseAcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //phase = 0;
            if (phaseAcheckBox.Checked)
            {
                _phase = 1;
                phaseBcheckBox.Checked = false;
                phaseCcheckBox.Checked = false;
            }
        }

        private void phaseBcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //phase = 0;
            if (phaseBcheckBox.Checked)
            {
                _phase = 2;
                phaseAcheckBox.Checked = false;
                phaseCcheckBox.Checked = false;
            }
        }

        private void phaseCcheckBox_CheckedChanged(object sender, EventArgs e)
        {
            //phase = 0;
            if (phaseCcheckBox.Checked)
            {
                _phase = 3;
                phaseAcheckBox.Checked = false;
                phaseBcheckBox.Checked = false;
            }
        }

        #region Функции, отвечающие за чтение данных c индикаторов и памяти ББ


        //----------Ответ CONFIRM от ББ при запросе на чтение параметров------------
        /// Сводка:
        ///     При каждом запрос информации с инидкаторов, будь то это телеизмерения
        ///     или настройки, всегда вместе с конечными данными приходит тестовый
        ///     кадр подтверждения с форматом
        ///     68 04 83 00 XX 00, где XX - шаг команды, передаваемый в запросе.
        ///     Данная функция обрабатывает это, и если CONFIRM пакет накладывается на
        ///     другое сообщение, то CONFIRM выводится отдельным сообщением, соединённым
        ///     из нескольких массивов.
        ///     
        /// Параметры: 
        ///     CheckLength:
        ///         Значение длины сообщения
        ///         
        ///     position:
        ///         Позиция в массиве
        ///         
        ///     OptionalSavePlace:
        ///         (Опционально) Место куда будет сохранятся массив с подтверждением,
        ///         если известно, что CONFIRM будет находится в двух разных массивах данных

        private void ReadCONFIRM(byte CheckLength, byte position, byte[] OptionalSavePlace = null)
        {
            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[position] == CheckLength)
                {
                    if (OptionalSavePlace != null && OptionalSavePlace.Length > 8)
                    {
                        Array.ConstrainedCopy(_dataResponse, 0, OptionalSavePlace, 10, 12);

                        str = BitConverter.ToString(OptionalSavePlace, 0, OptionalSavePlace.Length).Split('-');

                        sizeStr = Convert.ToInt16(str[1], 16);

                        Array.Resize(ref OptionalSavePlace, sizeStr + 2);

                        connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(OptionalSavePlace).Replace("-", " ")), "Подтверждение", _showTime));

                    }
                    else
                    {
                        str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                        sizeStr = Convert.ToInt16(str[1], 16);

                        Array.Resize(ref _dataResponse, sizeStr + 2);

                        connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Подтверждение", _showTime));
                    }
                }
            }
        }

        //----------------------Обработка телеизмерений-----------------------------
        /// Сводка:
        ///     Обработка получения телеизмерений после отправки первого запроса на ББ
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private void ReadTeleindication()
        {
            byte[] stop = { 0x68, 0x04, 0x13, 0x00, 0x02, 0x00 };
            byte[] test = { 0x68, 0x04, 0x43, 0x00, 0x00, 0x00 };

            byte[] TeleindicationData = new byte[8192];
            BaseBlockStream.Write(stop, 0, stop.Length);
            Thread.Sleep(1000);
            BaseBlockStream.Read(TeleindicationData, 0, TeleindicationData.Length);
            TeleindicationStopTimer.Start();
            //TeleindicationStopTimer.
        }

        //TODO: Разбить на отдельные методы
        //---------Чтение параметров индикатора RunParam, CurrentParam и GroundParam-------------
        /// Сводка:
        ///     Чтение данных с индикаторов. На ББ отправляется запрос в формате
        ///     68 11 1A 00 4C 00 7A 01 0D 00 00 00 00 00 00 2A 00 04 00
        ///     где: 
        ///     2A - тип параметров, который мы хотим прочитать
        ///     04 - фаза, с которой мы хотим прочитать нужные нам параметры
        ///     
        ///     Первым читается RunParam - общие параметры индикатора (2A)
        ///     Вторым читается CurrentParam - параметры определения межфазного замыкания (2B)
        ///     Третьим: GroundParam - параметры определения замыкания на землю (2C)
        ///     
        /// Параметры:
        ///     Стандартные параметры C# ввиде отправителя 
        ///     и аргументов события 
        private async void readIndicatorParametersButton_Click(object sender, EventArgs e)
        {
            string[] IndicatorStatus = new string[] { "Получение общих параметров индикатора.", "Получение настроек межфазного замыкания.", "Получение настроек замыкания на землю.", "Ожидание подтверждения." };

            if (BaseBlockStream == null)
            {
                MessageBox.Show("Соединенине не установлено", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (!phaseAcheckBox.Checked && !phaseBcheckBox.Checked && !phaseCcheckBox.Checked)
            {
                MessageBox.Show("Необходимо выбрать одну из трёх фаз для считывания данных", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            BaseBlockStream.ReadTimeout = 25000; //25 секунд тайм аут
            DisableButtons();
            //this.baseBlockTelemetryDataGrid.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            //--------------------RunParam--------------------------
            //Запрос на чтение данных, сформированный в масиве
            byte[] readGeneralParams = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2A, 0x00, 0x04, 0x00 };
            //Добавление фазы, с которой будут считываться данные 
            readGeneralParams[17] += (byte)_phase;
            //Массив для хранения данных
            _dataResponse = new byte[256];
            try
            {
                ProgressBarTimer.Start();
                await BaseBlockStream.WriteAsync(readGeneralParams, 0, readGeneralParams.Length);
                IndicatorStatusLabel.Text = IndicatorStatus[0];
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }

            //Конвертация полученных данных в битовый массив и разделение по ячейкам
            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt16(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ не смог получить данные с индикаторов", _showTime));
                EnableButtons();
                return;
            }

            //68 66 4C 00 1C 00 7D 01 0D 00 00 00 00 00 00 2A 00 05 55      - Начало пакета
            //00 30 00 02 50 00 | 01 30 00 01 05 | 02 30 00 02 2C 01 |      - Основные параметры
            //03 30 00 04 84 03 00 00 | 04 30 00 02 78 00 | 05 30 00 01 14
            //06 30 00 01 0A | 07 30 00 02 0A 00 | (08 30 00 02 F0 0A
            //09 30 00 02 F0 0A 0A 30 00 01 06 0B 30 00 02 F4 01 0C
            //30 00 01 05 0D 30 00 01 02 0E 30 00 01 02)                    - Доп параметры
            //То что в скобках - пока обрабатывать не нужно

            //TODO: Возможен баг если длина пакета с данными не постоянна, и часть массива "скушается" из-за того, что оборвалась запись где-то посередине массива

            //Чтение идёт с 0x3000 до 0x300E в RunParam, данные нужны с 0x3000 по 0x3007
            //            с 0x3100 до 0x3102 в CurrentParam

            if (_dataResponse[1] > 0x66)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }
            WriteInDataGrid(_dataResponse, 0x30, 0x08, 1);
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            //TODO: ББ вовзращает 5 строк при чтении в первый раз, зачем то ещё и RunParam и какието левые данные. Надо будет исправить потом + на WPF уже писать с учётом такого нюанса
            try
            {
                IndicatorStatusLabel.Text = IndicatorStatus[3];
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1000;
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            if (_dataResponse[1] == 0x04)
            {
                ReadCONFIRM(0x04, 1);
            }
            else
            {
                //ReadTeleindication();               //Костыль для фикса 5 строк
                ReadCONFIRM(0x12, 1);
                byte[] stop = { 0x68, 0x04, 0x13, 0x00, 0x02, 0x00 };
                byte[] TeleindicationData = new byte[8192];
                BaseBlockStream.Write(stop, 0, stop.Length);
                Thread.Sleep(1500);
                BaseBlockStream.Read(TeleindicationData, 0, TeleindicationData.Length);
                //Thread.Sleep(5000);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //ReadCONFIRM(0x04, 1);
            }
            //------------------------------------------------------

            //-------------------CurrentParam-----------------------
            byte[] readCurrentData = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x00, 0x04, 0x00 };
            //68 11 1A 00 4C 00 7A 01 0D 00 00 00 00 00 00 2B 00 05 00
            readCurrentData[17] += (byte)_phase;
            _dataResponse = new byte[256];
            try
            {
                ProgressBarTimer.Start();
                await BaseBlockStream.WriteAsync(readCurrentData, 0, readCurrentData.Length);
                IndicatorStatusLabel.Text = IndicatorStatus[1];
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }

            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt16(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 30)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ не смог получить данные с индикаторов", _showTime));
                EnableButtons();
                return;
            }

            //68 51 24 00 08 00 7D 01 0D 00 01 00 00 00 00 2B 00 05 40
            //00 31 00 04 78 00 00 00 | 01 31 00 02 1E 00 | 02 31 00 02 05 00   - основные параметры
            //(03 31 00 02 00 00 | 04 31 00 02 0A | 00 05 31 00 02 0A 00
            //06 31 00 04 30 75 00 00 | 07 31 00 02 B8 0B | 08 31 00 02 14 00
            //09 31 00 02 A0 00)                                                - доп параметры


            if (_dataResponse[1] > 0x66)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }

            WriteInDataGrid(_dataResponse, 0x31, 0x03, 10);
            //baseBlockTelemetryDataGrid.RowHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                IndicatorStatusLabel.Text = IndicatorStatus[3];
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1000;
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            if (_dataResponse[1] == 0x04)
            {
                ReadCONFIRM(0x04, 1);
            }
            else
            {
                byte[] stop = { 0x68, 0x04, 0x13, 0x00, 0x02, 0x00 };
                byte[] TeleindicationData = new byte[8192];
                BaseBlockStream.Write(stop, 0, stop.Length);
                Thread.Sleep(1500);
                BaseBlockStream.Read(TeleindicationData, 0, TeleindicationData.Length);
                //ReadTeleindication();               //Костыль для фикса 5 строк
                ReadCONFIRM(0x12, 1);
                //Thread.Sleep(5000);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                //ReadCONFIRM(0x04, 1);
            }
            //---------------------------------------------------------
            //---------------------GroundParam-------------------------
            byte[] readGroundData = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x00, 0x04, 0x00 };
            //68 11 1A 00 4C 00 7A 01 0D 00 00 00 00 00 00 2C 00 0X 00
            readGroundData[17] += (byte)_phase;
            _dataResponse = new byte[256];
            try
            {
                ProgressBarTimer.Start();
                await BaseBlockStream.WriteAsync(readGroundData, 0, readGroundData.Length);
                IndicatorStatusLabel.Text = IndicatorStatus[2];
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }

            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt16(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 30)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ не смог получить данные с индикаторов", _showTime));
                EnableButtons();
                return;
            }

            if (_dataResponse[1] > 0x66)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }
            WriteInDataGrid(_dataResponse, 0x32, 0x02, 14);
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                IndicatorStatusLabel.Text = IndicatorStatus[3];
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1000;
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                IndicatorStatusLabel.Text = "";
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            ReadCONFIRM(0x04, 1);
            IndicatorStatusLabel.Text = "";

            byte[] stop_1 = { 0x68, 0x04, 0x13, 0x00, 0x02, 0x00 };
            byte[] TeleindicationData_1 = new byte[8192];
            BaseBlockStream.Write(stop_1, 0, stop_1.Length);
            Thread.Sleep(1500);
            BaseBlockStream.Read(TeleindicationData_1, 0, TeleindicationData_1.Length);

            EnableButtons();
            //ReadTeleindication();
            //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            //ReadCONFIRM(0x04, 1);
        }

        //--------------------Запись параметров в DataGrid--------------------
        private void WriteInDataGrid(byte[] Response, byte DestinationAddress, byte Length, Int16 DataGridShift)
        {
            for (int i = 19; i < Response[18]; ++i)
            {
                if (Response[i] == DestinationAddress && Response[i - 1] < Length) //&& dataResponse[i + 2] > 0x00)
                                                                             //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                             //в первой половине 0x31
                {
                    if (Response[i + 2] == 0x01)
                    {
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Value = Convert.ToInt16(Response[i + 3]);
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (Response[i + 2] == 0x02)
                    {
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Value = Response[i + 4] << 8 | Response[i + 3];
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (Response[i + 2] == 0x04) //В проге используется всё равно FF FF, может китайцы опять переиграли
                    {
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Value = Response[i + 4] << 8 | Response[i + 3];
                        baseBlockTelemetryDataGrid.Rows[Response[i - 1] + DataGridShift].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                }
            }
        }

        //--------------------Чтение рабочих параметров ББ-------------------- 
        private async void ReadSCADAParameterButton_Click(object sender, EventArgs e)
        {
            TeleindicationStopTimer.Stop();
            if (BaseBlockStream == null)
            {
                MessageBox.Show("Соединенине не установлено", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }

            string[] str;
            byte[] ConfirmArray;
            int sizeStr;
            //bool firstTimeRead = true;

            byte[] readOperatingParams = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00 };

            DisableButtons();
            _dataResponse = new byte[256];
            ConfirmArray = new byte[32];
            try
            {
                await BaseBlockStream.WriteAsync(readOperatingParams, 0, readOperatingParams.Length);
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            Array.ConstrainedCopy(_dataResponse, _dataResponse.GetUpperBound(0) - 8, ConfirmArray, 0, 9);
            sizeStr = Convert.ToInt16(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные", _showTime));
                EnableButtons();
                return;
            }
            //TODO: Возвращаемый ответ больше 256, это нужно учитывать если потом придётся работать с областью 8B FF
            for (var i = 19; i < _dataResponse[18]; ++i)
            {
                if (_dataResponse[i] == 0x00 && _dataResponse[i - 1] == 0x15) //&& dataResponse[i + 2] > 0x00)
                                                                              //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                              //в первой половине 0x31
                {
                    //_isUserInput = false;
                    SCADA_TextBox.Text = Convert.ToInt16(_dataResponse[i + 4] << 8 | _dataResponse[i + 3]).ToString();
                    SCADA_TextBox.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
                    //_currentSCADAValue = SCADA_TextBox.Text;
                }
            }
            //connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ОК: ", _showTime));
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            ReadCONFIRM(0x01, 1, ConfirmArray);
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            if (_dataResponse[1] == 0x04)
            {
                ReadCONFIRM(0x04, 1);
            }
            else
            {
                byte[] ok = new byte[1024];
                ReadTeleindication();
                TeleindicationStopTimer.Start();
                //await BaseBlockStream.ReadAsync(ok, 0, ok.Length);
                //await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }

            //---------------------------------------------------------
            BaseBlockStream.Flush();
            EnableButtons();
        }

        #endregion

        #region Функции, отвечающие за запись данных на индикаторы и память ББ
        //--------------------Запись данных на индикаторы------------------
        //В момент перезаписи все жирные подписи становятся снова обычными
        private async void writeIndicatorParametersButton_Click(object sender, EventArgs e)
        {
            if (BaseBlockStream == null)
            {
                MessageBox.Show("Соединенине не установлено", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            DisableButtons();
            //68 40 1C 00 4E 00 7D 01 0D 00 00 00 00 00 00 2A 00 05 2F
            //00 30 00 02 50 00 | 01 30 00 01 06 | 02 30 00 02 2C 01
            //03 30 00 04 84 03 00 00 | 04 30 00 02 78 00 | 05 30 00 01 14
            //06 30 00 01 0A | 07 30 00 02 0A 00

            byte[] defaultGeneralPackage =
              { 0x68, 0x40, 0x1C, 0x00, 0x4E, 0x00, 0x7D, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2A, 0x00, 0x04, 0x2F,
                0x00, 0x30, 0x00, 0x02, 0x00, 0x00,
                0x01, 0x30, 0x00, 0x01, 0x00,
                0x02, 0x30, 0x00, 0x02, 0x00, 0x00,
                0x03, 0x30, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
                0x04, 0x30, 0x00, 0x02, 0x00, 0x00,
                0x05, 0x30, 0x00, 0x01, 0x00,
                0x06, 0x30, 0x00, 0x01, 0x00,
                0x07, 0x30, 0x00, 0x02, 0x00, 0x00 };

            byte[] defaultCurrentPackage =
              { 0x68, 0x25, 0x02, 0x00, 0x00, 0x00, 0x7D, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x00, 0x04, 0x14,
                0x00, 0x31, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
                0x01, 0x31, 0x00, 0x02, 0x00, 0x00,
                0x02, 0x31, 0x00, 0x02, 0x00, 0x00 };

            byte[] defaultGroundPackage =
              { 0x68, 0x1C, 0x02, 0x00, 0x00, 0x00, 0x7D, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x00, 0x04, 0x0B,
                0x00, 0x32, 0x00, 0x01, 0x1E,
                0x01, 0x32, 0x00, 0x02, 0x0A, 0x00 };
            ProgressBarTimer.Start();
            defaultGeneralPackage[17] += (byte)_phase;
            defaultCurrentPackage[17] += (byte)_phase;
            defaultGroundPackage[17] += (byte)_phase;
            //--------------------GeneralPackage--------------------------
            DataGridToDefaultPackage(ref defaultGeneralPackage, 0x30, 1, _phase);
            if (defaultGeneralPackage == null)
            {
                return;
            }
            /*for (var i = 19; i < defaultGeneralPackage.Length; ++i)
            {
                if (defaultGeneralPackage[i] == 0x30 && defaultGeneralPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                             //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                             //в первой половине 0x30
                {
                    if (defaultGeneralPackage[i + 2] == 0x01)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        defaultGeneralPackage[i + 3] = Convert.ToByte(baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value);
                    }
                    if (defaultGeneralPackage[i + 2] == 0x02)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value);
                        defaultGeneralPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultGeneralPackage[i + 4] = (byte)(storage >> 8);
                    }
                    if (defaultGeneralPackage[i + 2] == 0x04)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultGeneralPackage[i - 1] + 1].Cells[_phase].Value);
                        defaultGeneralPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultGeneralPackage[i + 4] = (byte)(storage >> 8);
                        //defaultRunPackage[i + 5] = 0xFF; UNUSED
                        //defaultRunPackage[i + 6] = 0xFF; UNSUED
                        //TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535
                        //что является по факту диапазоном в 2 байта - 0xFF. Надо уточнять у китайцев
                    }
                }
            }*/

            await BaseBlockStream.WriteAsync(defaultGeneralPackage, 0, defaultGeneralPackage.Length);
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }
            ReadCONFIRM(0x04, 1);
            //---------------------------------------------------------
            //----------------CurrentPackage--------------------------
            ProgressBarTimer.Start();
            DataGridToDefaultPackage(ref defaultCurrentPackage, 0x31, 10, _phase);
            if (defaultCurrentPackage == null)
            {
                return;
            }
            /*for (int i = 19; i < defaultCurrentPackage.Length; ++i)
            {
                if (defaultCurrentPackage[i] == 0x31 && defaultCurrentPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                             //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                             //в первой половине 0x31
                {
                    if (defaultCurrentPackage[i + 2] == 0x01)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        defaultCurrentPackage[i + 3] = Convert.ToByte(baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value);
                    }
                    if (defaultCurrentPackage[i + 2] == 0x02)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value);
                        defaultCurrentPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultCurrentPackage[i + 4] = (byte)(storage >> 8);
                    }
                    if (defaultCurrentPackage[i + 2] == 0x04)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultCurrentPackage[i - 1] + 10].Cells[_phase].Value);
                        defaultCurrentPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultCurrentPackage[i + 4] = (byte)(storage >> 8);
                        //defaultRunPackage[i + 5] = 0xFF; UNUSED
                        //defaultRunPackage[i + 6] = 0xFF; UNSUED
                        //TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535
                        //что является по факту диапазоном в 2 байта - 0xFF. Надо уточнять у китайцев
                    }
                }
            }*/
            await BaseBlockStream.WriteAsync(defaultCurrentPackage, 0, defaultCurrentPackage.Length);

            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }
            ReadCONFIRM(0x04, 1);
            //---------------------------------------------------------
            //-----------------GroundPackage---------------------------
            ProgressBarTimer.Start();
            DataGridToDefaultPackage(ref defaultGroundPackage, 0x32, 14, _phase);
            if (defaultGroundPackage == null)
            {
                return;
            }
            /*for (int i = 19; i < defaultGroundPackage.Length; ++i)
            {
                if (defaultGroundPackage[i] == 0x32 && defaultGroundPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                           //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                           //в первой половине 0x32
                {
                    if (defaultGroundPackage[i + 2] == 0x01)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        defaultGroundPackage[i + 3] = Convert.ToByte(baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 10].Cells[_phase].Value);
                    }
                    if (defaultGroundPackage[i + 2] == 0x02)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Value);
                        defaultGroundPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultGroundPackage[i + 4] = (byte)(storage >> 8);
                    }
                    if (defaultGroundPackage[i + 2] == 0x04)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Value.ToString() == "")
                        {
                            return;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultGroundPackage[i - 1] + 14].Cells[_phase].Value);
                        defaultGroundPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultGroundPackage[i + 4] = (byte)(storage >> 8);
                        //defaultRunPackage[i + 5] = 0xFF; UNUSED
                        //defaultRunPackage[i + 6] = 0xFF; UNSUED
                        //TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535, что является по факту диапазоном в 2 байта - 0xFFFF. Надо уточнять у китайцев
                    }
                }
            }*/
            await BaseBlockStream.WriteAsync(defaultGroundPackage, 0, defaultGroundPackage.Length);

            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
            }
            catch (System.IO.IOException e2)
            {
                ProgressBarTimer.Stop();
                progressBarReceive.Value = 1;
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }
            ReadCONFIRM(0x04, 1);
            EnableButtons();
            //---------------------------------------------------------
        }

        /// <summary>
        /// Сводка:
        ///     Запись данных из таблицы в массив для передачи на ББ
        ///    
        /// Параметры:
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridToDefaultPackage(ref byte[] defaultPackage, byte DestinationAdress, int Index, int Phase)
        {
            short storage;
            for (int i = 19; i < defaultPackage.Length; ++i)
            {
                if (defaultPackage[i] == DestinationAdress && defaultPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                           //TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                           //в первой половине 0x30/0x31/0x32
                {
                    if (defaultPackage[i + 2] == 0x01)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value.ToString() == "")
                        {
                            defaultPackage = null;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        defaultPackage[i + 3] = Convert.ToByte(baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value);
                    }
                    if (defaultPackage[i + 2] == 0x02)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value.ToString() == "")
                        {
                            defaultPackage = null;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value);
                        defaultPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultPackage[i + 4] = (byte)(storage >> 8);
                    }
                    if (defaultPackage[i + 2] == 0x04)
                    {
                        if (baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase] == null ||
                            baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value.ToString() == "")
                        {
                            defaultPackage = null;
                        }
                        baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                        storage = Convert.ToInt16(baseBlockTelemetryDataGrid.Rows[defaultPackage[i - 1] + Index].Cells[Phase].Value);
                        defaultPackage[i + 3] = (byte)(storage & 0x00FF);
                        defaultPackage[i + 4] = (byte)(storage >> 8);
                        //defaultRunPackage[i + 5] = 0xFF; UNUSED
                        //defaultRunPackage[i + 6] = 0xFF; UNSUED
                        //TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535, что является по факту диапазоном в 2 байта - 0xFFFF. Надо уточнять у китайцев
                    }
                }
            }
            int k = 0;
            //return defaultPackage;
        }

        private async void WriteSCADAParameterButton_Click(object sender, EventArgs e)
        {
            if (BaseBlockStream == null)
            {
                MessageBox.Show("Соединенине не установлено", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            if (string.IsNullOrEmpty(SCADA_TextBox.Text))
            {
                MessageBox.Show("Поле 'Время отправки телеизмерений' не должно быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            DisableButtons();
            //_isUserInput = false;
            SCADA_TextBox.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);

            byte[] defaultWritePackage =
              { 0x68, 0xE8, 0x1C, 0x00, 0x4E, 0x00, 0x7D, 0x01, 0x0D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0xD7,
                0x0D, 0x00, 0x00, 0x02, 0x00, 0x00, // OHL RF
                0x0E, 0x00, 0x00, 0x02, 0x01, 0x00, // Режим работы - в реальном времени
                0x0F, 0x00, 0x00, 0x02, 0x01, 0x00, // Тип устройства - удалённое уст.
                0x10, 0x00, 0x00, 0x02, 0x00, 0x00, // Тип индикаторов - RF001
                0x11, 0x00, 0x00, 0x02, 0x60, 0x54, // Интервал блокировки при ошибке (?)
                0x12, 0x00, 0x00, 0x02, 0x78, 0x1E, // 
                0x13, 0x00, 0x00, 0x02, 0x78, 0x00, // AD Acquisition time
                0x14, 0x00, 0x00, 0x02, 0x3C, 0x00, // Heartbeat time
                0x15, 0x00, 0x00, 0x02, 0x84, 0x03, // Время отправки запроса на получение телеизмерений
                0x16, 0x00, 0x00, 0x02, 0x3C, 0x00, // 
                0x18, 0x00, 0x00, 0x02, 0x84, 0x03, // 
                0x1A, 0x00, 0x00, 0x02, 0x3C, 0x00, // Время которое фиксирует режим "power" если работа блока питания нестабильна 
                0x1F, 0x00, 0x00, 0x02, 0x01, 0x00, // Адрес подстанции
                0x21, 0x00, 0x00, 0x02, 0x06, 0x00, // Частота работы RF устройства
                0x22, 0x00, 0x00, 0x02, 0x64, 0x00, // 
                0x23, 0x00, 0x00, 0x02, 0x00, 0x00, // 
                0x24, 0x00, 0x00, 0x02, 0x0A, 0x00, // 
                0x8B, 0xFF, 0x00, 0x02, 0xD3, 0x04, // Значение защиты перенапряжения
                0x8C, 0xFF, 0x00, 0x02, 0x5B, 0x13, // Частота дикретизации АЦП батареи
                0x93, 0xFF, 0x00, 0x02, 0x00, 0x00, // 
                0x8D, 0xFF, 0x00, 0x02, 0x00, 0x00, // 
                0x8E, 0xFF, 0x00, 0x02, 0x00, 0x00, // 
                0x8F, 0xFF, 0x00, 0x02, 0x00, 0x00, // 
                0xA0, 0x00, 0x00, 0x01, 0x00,
                0xA1, 0x00, 0x00, 0x01, 0x00,
                0xA2, 0x00, 0x00, 0x01, 0x00,
                0xA3, 0x00, 0x00, 0x01, 0x00,
                0xA4, 0x00, 0x00, 0x01, 0x00,
                0xA5, 0x00, 0x00, 0x02, 0x00, 0x00,
                0xA6, 0x00, 0x00, 0x02, 0x00, 0x00,
                0x85, 0xFF, 0x00, 0x01, 0x00,
                0x1F, 0xFF, 0x00, 0x01, 0x00,
                0x86, 0xFF, 0x00, 0x02, 0x0A, 0x00,
                0x9D, 0xFF, 0x00, 0x02, 0x00, 0x00,
                0x9E, 0xFF, 0x00, 0x01, 0x00,
                0x20, 0xFF, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00,
                0x21, 0xFF, 0x00, 0x01, 0x00
            };

            //defaultWritePackage[3] += (byte)test;
            //defaultWritePackage[4] += (byte)test;
            defaultWritePackage[72] = (byte)(Convert.ToInt16(SCADA_TextBox.Text) >> 8);
            defaultWritePackage[71] = (byte)(Convert.ToInt16(SCADA_TextBox.Text) & 0xFF);

            _dataResponse = new byte[256];
            await BaseBlockStream.WriteAsync(defaultWritePackage, 0, defaultWritePackage.Length);
            await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);

            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString()).ToString(), "Код ошибки", _showTime));
            }
            ReadCONFIRM(0x04, 1);
            //---------------------------------------------------------
            BaseBlockStream.Flush();
            EnableButtons();
        }

        #endregion
    }
}
