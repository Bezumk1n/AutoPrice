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
                using (StreamWriter sw = new StreamWriter(@".\errorLog.log", true))
                {
                    sw.Write(value + Environment.NewLine);
                    sw.Close();
                }
            }
        }
    }
}
