using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.IO.Ports;

//***************   Автор: Даниил Григорьев (Captain_Kirk54)    *******************
//***   ПО преддназначено для чтения данных с базового блока ИКЗ и записи на       **
//***   него своих значений для дальнейшей эксплуатации оборудования. Общение      **
//***   проходит по МЭК-104. Сначала идёт запрос по TCP на чтение данных.          **
//***   После этого в ответе приходят ДВА пакета - WRITE_RESPONSE и SHOW_          **
//***   S_FRAME. Запись происходит путём отправки запроса с значениями.            **



namespace test_app
{
    public partial class Jmih : Form
    {
        private bool _showTime;
        private int _phase; //Фаза А - 1, Фаза Б - 2, Фаза С - 3
        //int prg = 0;
        private static byte[] _dataResponse;

        protected CancellationTokenSource TokenSource = new CancellationTokenSource();
        protected TcpClient BaseBlockSender;
        protected IPAddress BaseBlockIp;
        //protected IPEndPoint BaseBlockEnd;
        protected NetworkStream BaseBlockStream;
        public Jmih()
        {
            InitializeComponent();
        }

        //Стартовая инициализация после запуска приложения
        private void jmih_Load(object sender, EventArgs e)
        {
            connectionIndicator.BackColor = Color.White;
            _showTime = false;
            _phase = 0;
            //baseBlockIP = IPAddress.Parse("192.168.1.1");
            //baseBlockEnd = new IPEndPoint(baseBlockIP, 10010);
            inputDataGrid_Setup();
            telemetryDataGrid_Setup();
        }


        //
        private async void ProgressBarWork()
        {
            var token = TokenSource.Token;
            await Task.Factory.StartNew(() =>
            {
                void Inc()
                {
                    for (var i = 0; i < 20; ++i)
                    {
                        Task.Delay(200, token);
                        progressBarReceive.Increment(5);
                    }
                }

                if (TCP_CONNECTION.InvokeRequired)
                {
                    TCP_CONNECTION.BeginInvoke((System.Action)Inc);
                }
                else
                {
                    Inc();
                }
                if (TCP_CONNECTION.InvokeRequired)
                {
                    TCP_CONNECTION.BeginInvoke((System.Action)Inc);
                }
                else
                {
                    Inc();
                }
            }, token);
        }

        //--------------Включение и отколючение кнопопк-----------
        private void DisableButtons()
        {
            TCP_CONNECTION.Enabled = false;
            closeConnectionButton.Enabled = false;
            send_button.Enabled = false;
            readParametersButton.Enabled = false;
            writeParametersButton.Enabled = false;
        }

        private void EnableButtons()
        {
            TCP_CONNECTION.Enabled = true;
            closeConnectionButton.Enabled = true;
            send_button.Enabled = true;
            readParametersButton.Enabled = true;
            writeParametersButton.Enabled = true;
        }

        //-----------Настройка таблицы с IP/Port------------
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
        private void telemetryDataGrid_Setup()
        {
            baseBlockTelemetryDataGrid.Rows.Add("Общие параметры индикатора");
            baseBlockTelemetryDataGrid.Rows[0].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[0].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Величина электрического поля");
            baseBlockTelemetryDataGrid.Rows.Add("Минимальное значение тока в линии");
            baseBlockTelemetryDataGrid.Rows.Add("Интервал отправки телеизмерений №1");
            baseBlockTelemetryDataGrid.Rows.Add("Интервал отправки телеизмерений №2");
            baseBlockTelemetryDataGrid.Rows.Add("Период отключения датчиков");
            baseBlockTelemetryDataGrid.Rows.Add("Порог значения тока для отправки телеизмерений");
            baseBlockTelemetryDataGrid.Rows.Add("Абсолютное значение изменения тока");
            baseBlockTelemetryDataGrid.Rows.Add("относительное значение изменения тока (в %)");

            baseBlockTelemetryDataGrid.Rows.Add("Параметры определения межфазного замыкания");
            baseBlockTelemetryDataGrid.Rows[9].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[9].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Время перезап. индик. после КЗ на линии");
            baseBlockTelemetryDataGrid.Rows.Add("Время перезап. индик. после КЗ на линии после перезап.");
            baseBlockTelemetryDataGrid.Rows.Add("Минимальное значение тока КЗ на линии");
            baseBlockTelemetryDataGrid.Rows.Add("Параметры определения замыкания на землю");
            baseBlockTelemetryDataGrid.Rows[13].Cells[0].Style.BackColor = Color.Gray;
            for (var i = 0; i < 4; ++i)
            {
                baseBlockTelemetryDataGrid.Rows[13].Cells[i].Style.BackColor = ColorTranslator.FromHtml("#A0A0A0");
            }
            baseBlockTelemetryDataGrid.Rows.Add("Падение напряжения в линии (в %)");
            baseBlockTelemetryDataGrid.Rows.Add("Время засечки падения напряжения");
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

        //----------------Метка времени---------------------
        private void time_check_box_1_CheckedChanged(object sender, EventArgs e)
        {
            _showTime = false;
            if (time_check_box_1.Checked)
            {
                _showTime = true;
            }
        }

        //----------Кнопка "Отправка" для данных------------
        private async void send_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text)) return;
            if (BaseBlockStream == null)
            {
                return;
            }
            try
            {
                byte[] dataSend = AdditionalFunctions.StringToByteArray(textBox2.Text);

                //Byte[] dataSend = { 0x68, 0x04, 0x43, 0x00, 0x00, 0x00 };
                await BaseBlockStream.WriteAsync(dataSend, 0, dataSend.Length);

                _dataResponse = new byte[256];
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }
            string[] str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');
            int sizeStr = Convert.ToInt32(str[1], 16);
            Array.Resize(ref _dataResponse, sizeStr + 2);

            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(str.ToString(), "", _showTime));
        }

        //-----------Кнопка "Установка соединения"---------------
        private async void TCP_CONNECTION_Click(object sender, EventArgs e)
        {
            if (baseBlockServerConstants.Rows[0].Cells[1].Value == null ||
                baseBlockServerConstants.Rows[1].Cells[1].Value == null)
            {
                MessageBox.Show("Какое-то из полей IP/Порт пустое, его необходимо заполнить", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Question);
                return;
            }
            /*if (showTime)
            {
                connection_log.AppendText(String.Format("[{0}]IP: {1}\r\n", DateTime.Now, baseBlockServerConstants.Rows[0].Cells[1].Value.ToString()));
                connection_log.AppendText(String.Format("[{0}]Порт: {1}\r\n", DateTime.Now, baseBlockServerConstants.Rows[1].Cells[1].Value.ToString()));
            }
            else
            {
                connection_log.AppendText("IP:" + baseBlockServerConstants.Rows[0].Cells[1].Value.ToString() + "\r\n");
                connection_log.AppendText("Порт: " + baseBlockServerConstants.Rows[1].Cells[1].Value.ToString() + "\r\n");
            }*/
            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(baseBlockServerConstants.Rows[0].Cells[1].Value.ToString(), "IP", _showTime));
            connection_log.AppendText(AdditionalFunctions.TextBoxPrint(baseBlockServerConstants.Rows[1].Cells[1].Value.ToString(), "Порт", _showTime));

            BaseBlockSender = new TcpClient();
            BaseBlockIp = IPAddress.Parse(baseBlockServerConstants.Rows[0].Cells[1].Value.ToString());

            try
            {
                DisableButtons();
                ProgressBarWork();
                await BaseBlockSender.ConnectAsync(BaseBlockIp, Convert.ToInt32(baseBlockServerConstants.Rows[1].Cells[1].Value.ToString()));
                progressBarReceive.Value = 1;
                BaseBlockStream = BaseBlockSender.GetStream();
                connectionIndicator.BackColor = Color.Lime;
                EnableButtons();
               
            }
            catch (SocketException ex)
            {
                EnableButtons();
                connectionIndicator.BackColor = Color.Red;
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.ScktExc, ex.ToString());
            }
        }

        //--------------Кнопка "Завершение соедиения"----------------------
        private void closeConnectionButton_Click(object sender, EventArgs e)
        {
            //@TODO Вывести в отдельную ассинхронную задачу
            try
            {
                BaseBlockSender.GetStream().Close();
                BaseBlockSender.Close();
                connectionIndicator.BackColor = Color.White;
            }
            catch(System.InvalidOperationException e1)
            {
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.InvalOpExc, e1.ToString());
            }
            //System.InvalidOperationException
            //System.InvalidOperationException: "Операция не разрешается на неподключенных сокетах."
        }

        //----------------Валидация введёных данных в DataGrid-----------------------
        //----Данные: IP и порты-----------------------------------------------------
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
                        AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString());
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
                        AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString());
                    }
                }
            }
        }

        private void serviceConsoleHelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данная сервисная консоль предназначена для вывода информации в raw-виде" +
                " и для отладки напрямую. Не реккомендуется использовать если вы не знаете как " +
                "работает протокол МЭК-104", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        //------Обработка нажатий на разные checkBoxы с фазами
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

        //---------Чтение параметров индикатора RunParam и CurrentParam-------------
        //Первое читается - RunParam     
        //Второе - CurrentParam
        private async void readParametersButton_Click(object sender, EventArgs e)
        {
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

            string[] str;
            int sizeStr;

            BaseBlockStream.ReadTimeout = 25000; //10 секунд тайм аут
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
                await BaseBlockStream.WriteAsync(readGeneralParams, 0, readGeneralParams.Length);
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            //Конвертация полученных данных в битовый массив и разделение по ячейкам
            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');
            
            sizeStr = Convert.ToInt32(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
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

            //@TODO: Возможен баг если длина пакета с данными не постоянно, и часть массива
            //"скушается" из-за того, что оборвалась запись где-то посередине массива

            //Чтение идёт с 0x3000 до 0x300E в RunParam, данные нужны с 0x3000 по 0x3007
            //            с 0x3100 до 0x3102 в CurrentParam

            if (_dataResponse[1] > 0x66)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }

            for (var i = 19; i < _dataResponse[18]; ++i)
            {
                if (_dataResponse[i] == 0x30 && _dataResponse[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                           //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно                                                                               //в первой половине 0x30
                {
                    if (_dataResponse[i + 2] == 0x01)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Value = Convert.ToInt32(_dataResponse[i + 3]);
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x02)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x04) //В проге используется всё равно FF FF, может китайцы опять переиграли
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 1].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                }
            }


            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            if (_dataResponse != null || _dataResponse.Length != 0)
            {
               if (_dataResponse[1] == 0x04)
               {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
               }
            }
            //------------------------------------------------------

            //-------------------CurrentParam-----------------------
            byte[] readCurrentData = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2B, 0x00, 0x04, 0x00 };
            //68 11 1A 00 4C 00 7A 01 0D 00 00 00 00 00 00 2B 00 05 00
            readCurrentData[17] += (byte)_phase;
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.WriteAsync(readCurrentData, 0, readCurrentData.Length);
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt32(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
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
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }

            for (var i = 19; i < _dataResponse[18]; ++i)
            {
                if (_dataResponse[i] == 0x31 && _dataResponse[i - 1] < 0x03) //&& dataResponse[i + 2] > 0x00)
                                                                           //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                           //в первой половине 0x31
                {
                    if (_dataResponse[i + 2] == 0x01)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Value = Convert.ToInt32(_dataResponse[i + 3]);
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x02)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x04) //В проге используется всё равно FF FF, может китайцы опять переиграли
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 10].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                }
            }
            //baseBlockTelemetryDataGrid.RowHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Regular);
            _dataResponse = new byte[256];
            //CONFIRM--------------------------------------------------
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[1] == 0x04)
                {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
                }
            }
            //---------------------------------------------------------
            //---------------------GroundParam-------------------------
            byte[] readGroundData = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x00, 0x04, 0x00 };
            //68 11 1A 00 4C 00 7A 01 0D 00 00 00 00 00 00 2C 00 0X 00
            readGroundData[17] += (byte)_phase;
            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.WriteAsync(readGroundData, 0, readGroundData.Length);
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt32(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ не смог получить данные с индикаторов", _showTime));
                EnableButtons();
                return;
            }

            if (_dataResponse[1] > 0x66)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ вернул не те данные, попробуйте снова отправить запрос на чтение", _showTime));
                EnableButtons();
                return;
            }
            for (int i = 19; i < _dataResponse[18]; ++i)
            {
                if (_dataResponse[i] == 0x32 && _dataResponse[i - 1] < 0x02) //&& dataResponse[i + 2] > 0x00)
                                                                           //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                           //в первой половине 0x31
                {
                    if (_dataResponse[i + 2] == 0x01)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Value = Convert.ToInt32(_dataResponse[i + 3]);
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x02)
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                    if (_dataResponse[i + 2] == 0x04) //В проге используется всё равно FF FF, может китайцы опять переиграли
                    {
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Value = _dataResponse[i + 4] << 8 | _dataResponse[i + 3];
                        baseBlockTelemetryDataGrid.Rows[_dataResponse[i - 1] + 14].Cells[_phase].Style.Font = new Font("Microsoft Sans Serif", 8);
                    }
                }
            }
            _dataResponse = new byte[256];

            //CONFIRM--------------------------------------------------
            try
            {
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[1] == 0x04)
                {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
                }
            }
            EnableButtons();
        }

        //---------------Проверка на изменение ячейки в таблице телеизмерений------------
        //Если значение будет изменено - шрифт станет жирным
        //Если значение записывается в первый раз - шрифт будет нормальным
        //Если значение перезаписывается компьютером - шрифт будет нормальным
        private void baseBlockTelemetryDataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (baseBlockTelemetryDataGrid.CurrentCell.Value == null ||
                baseBlockTelemetryDataGrid.CurrentCell.Value.ToString() == "")
            {
                return;
            }
            if (baseBlockTelemetryDataGrid.CurrentCell.ColumnIndex == 0)
            {
                return;
            }
            string currentValue = baseBlockTelemetryDataGrid.CurrentCell.Value.ToString();
            try
            {
                if (baseBlockTelemetryDataGrid.CurrentCell.EditedFormattedValue.ToString() != currentValue)
                {
                    //colNum = baseBlockServerConstants.CurrentCell
                    baseBlockTelemetryDataGrid.Rows[baseBlockTelemetryDataGrid.CurrentCell.RowIndex].Cells[baseBlockTelemetryDataGrid.CurrentCell.ColumnIndex].Style.Font =
                        new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                }
            }
            catch (System.Exception e1)
            {
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.SysExc, e1.ToString());
            }

        }

        //--------------------Запись данных на индикаторы------------------
        //В момент перезаписи все жирные подписи становятся снова обычными
        private async void writeParametersButton_Click(object sender, EventArgs e)
        {
            string[] str;
            int sizeStr;

            //68 40 1C 00 4E 00 7D 01 0D 00 00 00 00 00 00 2A 00 05 2F
            //00 30 00 02 50 00 | 01 30 00 01 06 | 02 30 00 02 2C 01
            //03 30 00 04 84 03 00 00 | 04 30 00 02 78 00 | 05 30 00 01 14
            //06 30 00 01 0A | 07 30 00 02 0A 00
            short storage;

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

            defaultGeneralPackage[17] += (byte)_phase;
            defaultCurrentPackage[17] += (byte)_phase;
            defaultGroundPackage[17] += (byte)_phase;
            //--------------------GeneralPackage--------------------------
            for (var i = 19; i < defaultGeneralPackage.Length; ++i)
            {
                if (defaultGeneralPackage[i] == 0x30 && defaultGeneralPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                     //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
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
                        //@TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535
                        //что является по факту диапазоном в 2 байта - 0xFF. Надо уточнять у китайцев
                    }
                }
            }

            await BaseBlockStream.WriteAsync(defaultGeneralPackage, 0, defaultGeneralPackage.Length);
            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[1] == 0x04)
                {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
                }
            }
            //---------------------------------------------------------
            //----------------CurrentPackage--------------------------
            for (int i = 19; i < defaultCurrentPackage.Length; ++i)
            {
                if (defaultCurrentPackage[i] == 0x31 && defaultCurrentPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                             //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                             //в первой половине 0x30
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
                        //@TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535
                        //что является по факту диапазоном в 2 байта - 0xFF. Надо уточнять у китайцев
                    }
                }
            }
            await BaseBlockStream.WriteAsync(defaultCurrentPackage, 0, defaultCurrentPackage.Length);

            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            BaseBlockStream.Read(_dataResponse, 0, _dataResponse.Length);
            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[1] == 0x04)
                {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
                }
            }
            //---------------------------------------------------------
            //-----------------GroundPackage---------------------------
            for (int i = 19; i < defaultGroundPackage.Length; ++i)
            {
                if (defaultGroundPackage[i] == 0x31 && defaultGroundPackage[i - 1] < 0x08) //&& dataResponse[i + 2] > 0x00)
                                                                                           //@TODO: На данный момент здесь возможен баг, если значение в памяти будет равно
                                                                                           //в первой половине 0x30
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
                        //@TODO: В проге китайцев написано что диапазон у чисел в 4 байта с 0 до 65535
                        //что является по факту диапазоном в 2 байта - 0xFF. Надо уточнять у китайцев
                    }
                }
            }
            await BaseBlockStream.WriteAsync(defaultGroundPackage, 0, defaultGroundPackage.Length);

            //CONFIRM--------------------------------------------------
            _dataResponse = new byte[256];
            BaseBlockStream.Read(_dataResponse, 0, _dataResponse.Length);
            if (_dataResponse != null || _dataResponse.Length != 0)
            {
                if (_dataResponse[1] == 0x04)
                {
                    str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

                    sizeStr = Convert.ToInt32(str[1], 16);

                    Array.Resize(ref _dataResponse, sizeStr + 2);

                    connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "Confirm/Подтверждение", _showTime));
                }
            }
            //---------------------------------------------------------
        }

        //-------------Чтение рабочих параметров ББ---------------------------- 
        private async void ReadSCADAParameterButton_Click(object sender, EventArgs e)
        {
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

            string[] str;
            int sizeStr;

            byte[] readOperatingParams = { 0x68, 0x11, 0x1A, 0x00, 0x4C, 0x00, 0x7A, 0x01, 0x0D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x02, 0x00 };

            _dataResponse = new byte[256];
            try
            {
                await BaseBlockStream.WriteAsync(readOperatingParams, 0, readOperatingParams.Length);
                await BaseBlockStream.ReadAsync(_dataResponse, 0, _dataResponse.Length);
            }
            catch (System.IO.IOException e2)
            {
                EnableButtons();
                AdditionalFunctions.ErrorExceptionHandler(errorCodes.IOExc, e2.ToString());
            }

            //Конвертация полученных данных в битовый массив и разделение по ячейкам
            str = BitConverter.ToString(_dataResponse, 0, _dataResponse.Length).Split('-');

            sizeStr = Convert.ToInt32(str[1], 16);

            Array.Resize(ref _dataResponse, sizeStr + 2);

            if (sizeStr < 20)
            {
                connection_log.AppendText(AdditionalFunctions.TextBoxPrint(string.Join("", BitConverter.ToString(_dataResponse).Replace("-", " ")), "ББ не смог получить данные с индикаторов", _showTime));
                EnableButtons();
                return;
            }
        }
        /*
 Task t = Task.Factory.StartNew(() =>
   {
       System.Action disableButtons = () => { 
           TCP_CONNECTION.Enabled = false;
           closeConnectionButton.Enabled = false;
           send_button.Enabled = false;
           readParametersButton.Enabled = false;
           writeParametersButton.Enabled = false;
       };
       System.Action enableButtons = () =>
       {
           TCP_CONNECTION.Enabled = true;
           closeConnectionButton.Enabled = true;
           send_button.Enabled = true;
           readParametersButton.Enabled = true;
           writeParametersButton.Enabled = true;
       };

       if (TCP_CONNECTION.InvokeRequired)
       {
           TCP_CONNECTION.BeginInvoke(disableButtons);
       }
       else
       {
           disableButtons();
       }     

       try
       {
           await baseBlockSender.ConnectAsync(baseBlockIP, Convert.ToInt32(baseBlockServerConstants.Rows[1].Cells[1].Value.ToString()));
           baseBlockStream = baseBlockSender.GetStream();
           connectionIndicator.BackColor = Color.Lime;
       }
       catch (SocketException ex)
       {
           //connection_log.AppendText("Ошибка: " + ex.ToString() + "\r\n");
           connectionIndicator.BackColor = Color.Red;
           AdditionalFunctions.ErrorExceptionHandler(errorCodes.ScktExc, ex.ToString());
       }
       if (TCP_CONNECTION.InvokeRequired)
       {
           TCP_CONNECTION.BeginInvoke(enableButtons);
       }
       else
       {
           enableButtons();
       }
   }, token) ; */
    }
}
