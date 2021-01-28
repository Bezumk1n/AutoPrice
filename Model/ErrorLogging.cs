using System;
using System.IO;

namespace AutoPrice.Model
{
    public class ErrorLogging
    {
        public string ErrorMessage
        {
            get { return null; }
            set
            {
                //Windows TaskManager почему-то не запускает программу если указан такой путь:  @".\errorLog.log"
                using (StreamWriter sw = new StreamWriter(@"C:\Users\MEsales4\Desktop\Projects\AutoPrice\bin\Release\netcoreapp3.1\errorLog.log", true))
                {
                    sw.Write(value + Environment.NewLine);
                    sw.Close();
                }
            }
        }
    }
}
