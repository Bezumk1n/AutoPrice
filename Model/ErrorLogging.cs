using System;
using System.IO;

namespace AutoPrice.Model
{
    public class ErrorLogging
    {
        public bool isErrorOccured;
        public string ErrorMessage
        {
            get { return null; }
            set
            {
                isErrorOccured = true;
                using (StreamWriter sw = new StreamWriter(@".\errorLog.txt", true))
                {
                    sw.Write(value + Environment.NewLine);
                    sw.Close();
                }
            }
        }
    }
}
