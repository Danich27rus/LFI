using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace test_app
{
    //System Exception - 0
    //IO Exception - 1
    //Socket Exception - 2
    //InvalidOperationException - 3

    public enum errorCodes { SysExc, IOExc, ScktExc, InvalOpExc }

    internal class AdditionalFunctions
    {
        //public enum errorCodes { SysExc, IOExc, ScktExc }
        public static byte[] StringToByteArray(String hex)
        {
            string[] flex = hex.Split(' ');
            byte[] bytes = new byte[flex.Length];
            for (var i = 0; i < flex.Length; i++)
            {
                //flex = hex.Substring(i, 2);
                bytes[i] = Convert.ToByte(flex[i], 16);
            }
            return bytes;
        }

        //Обработчик ошибок
        public static byte ErrorExceptionHandler(errorCodes err, string expText)
        {
            switch (err)
            {
                case errorCodes.SysExc:
                    {
                        MessageBox.Show(expText);
                        return (byte)errorCodes.SysExc;
                    }
                case errorCodes.IOExc:
                    {
                        MessageBox.Show(expText);
                        return (byte)errorCodes.IOExc;
                    }
                case errorCodes.ScktExc:
                    {
                        MessageBox.Show(expText);
                        return (byte)errorCodes.ScktExc;
                    }
                case errorCodes.InvalOpExc:
                    {
                        MessageBox.Show(expText);
                        return (byte)errorCodes.InvalOpExc;
                    }
                default:
                    {
                        return 0xFF;
                    }
            }

            //return "0";
        }

        //Вывод в textBox всей информации
        public static string TextBoxPrint(string text, string cause, bool showTimeFlag)
        {
            string print = "";
            if (showTimeFlag)
            {
                print += ($"[{DateTime.Now}] {cause}: {text}\r\n");
                return print;
            }
            print += ($"{cause}: {text}\r\n");
            return print;
        }

        //Сдвиг массива влево на нужное кол-во шагов (offset)
        public static byte[] ArrayShift(byte[] array, int offset)
        {
            int n = array.Length;
            for (int i = 0; i < offset; ++i)
            {
                byte aLast = array[n - 1];
                for (int j = 0; j < n - 1; j++)
                    array[j] = array[j + 1];
                array[0] = aLast;
            }
            return array;
        }
    }
}
