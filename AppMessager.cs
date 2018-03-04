using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace WindowsSendMessage
{
    class AppMessager
    {
        #region DLL Imports

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern string SendMessage(IntPtr hwnd, int msg, int wParam, ref CopyDataStruct lParam);

        #endregion

        #region Variables

        public const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public IntPtr ID;
            public int Length;
            public string Data;
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Sends the specified data to all the instances of the windows process with the specified name.
        /// </summary>
        /// <param name="receiverAppName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool SendToApp(string receiverAppName, string data)
        {
            bool status = false;

            /* Get all the processes that have the specified name */
            Process[] processes = Process.GetProcessesByName(receiverAppName);

            CopyDataStruct dataStruct = new CopyDataStruct();

            /* Make sure that the process/es exist before sending the data */
            if(processes.Length > 0)
            {
                status = true;

                /* Pack the data into the data structure */
                dataStruct.Length = data.Length * Marshal.SystemDefaultCharSize;
                dataStruct.Data = data;

                foreach(Process p in processes)
                {
                    SendMessage(p.MainWindowHandle, WM_COPYDATA, 0, ref dataStruct);
                }
            }

            return status;
        }

        /// <summary>
        /// Retrieves the data string from the given Windows Message
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string GetCopyDataString(Message message)
        {
            string data = "";

            try
            {
                CopyDataStruct dataStruct = (CopyDataStruct)message.GetLParam(typeof(CopyDataStruct));
                data = dataStruct.Data;
            }
            catch (Exception ex) { }

            return data;
        }

        #endregion

        #region Requirements for Receiver App

        /* Implement the commented out function below in the Main Form's class to receive messages from thsi class */

        //protected override void WndProc(ref Message message)
        //{
        //    /* AppMessager sends a WM_COPYDATA Message */
        //    if (message.Msg == AppMessager.WM_COPYDATA)
        //    {
        //        /* Extract the data string */
        //        string dataString = AppMessager.GetCopyDataString(message);

        //        /* Do whatever you need to do with that data here */

        //    }

        //    /* Pass along all messages to the base method */
        //    base.WndProc(ref message);
        //}

        #endregion

        #region References

        /* http://forums.codeguru.com/showthread.php?504067-RESOLVED-Sending-string-messages-between-applications */
        /* https://www.mail-archive.com/dotnet@discuss.develop.com/msg01837.html */

        #endregion
    }
}
