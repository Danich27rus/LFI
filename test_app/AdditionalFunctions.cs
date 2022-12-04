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
        public static string ErrorExceptionHandler(errorCodes err, string expText)
        {
            switch (err)
            {
                case errorCodes.SysExc:
                    {
                        MessageBox.Show(expText);
                        return "sys ne ok";
                    }
                case errorCodes.IOExc:
                    {
                        MessageBox.Show(expText);
                        return "io ne ok";
                    }
                case errorCodes.ScktExc:
                    {
                        MessageBox.Show(expText);
                        return "socket ne ok";
                    }
                case errorCodes.InvalOpExc:
                    {
                        MessageBox.Show(expText);
                        return "opeartion ne ok";
                    }
                default:
                    {
                        return "ne ok";
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
    }
}
