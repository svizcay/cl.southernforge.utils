using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;    // StreamWriter
using System;       // DateTime

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Logger utility that automatize logging messages either to the consolo or to a txt file.
    ///  Timestamp and frame id can be added automatically to the message.
    /// </summary>
    public class Logger  {

        /// <summary>
        ///  Type of log (bit mask)
        /// </summary>
        [Flags]
        public enum LogType {
            ERROR       = 1 << 0,   // star with 1 and not 0. 0 will be automatically set to None and -1 to Everything
            WARNING     = 1 << 1,
            MSG         = 1 << 2,
            APPLICATION = 1 << 3
        };

        /// <summary>
        /// Filename of log file.
        /// If it needs to be set, add a gameObject at the very beginning that sets the filename on Awake by using its singleton.
        /// </summary>
        private string fileName = "log.txt";

        /// <summary>
        ///  Current logType. Logging ALL by default
        /// </summary>
        private LogType logType = LogType.ERROR | LogType.WARNING | LogType.MSG | LogType.APPLICATION;

        #region singleton
        /// <summary>
        /// Singleton. Since this is not a MonoBehaviour class, it can not be instantiated with a gameObject.
        /// Users of this class needs to use this instance instead.
        /// </summary>
        private static Logger mInstance;
        public static Logger Instance
        {
            get {
                if (mInstance == null) mInstance = new Logger();    // new operator can not be called on MonoBehaviours
                return mInstance;
            }
        }
        #endregion

        #region public interface

        #region log to file interface
        public void LogErrorToFile (string msg, bool addSystemTime = true)
        {
            LogToFileByCheckingFlag(LogType.ERROR, msg, addSystemTime);
        }

        public void LogWarningToFile (string msg, bool addSystemTime = true)
        {
            LogToFileByCheckingFlag(LogType.WARNING, msg, addSystemTime);
        }

        public void LogMsgToFile (string msg, bool addSystemTime = true)
        {
            LogToFileByCheckingFlag(LogType.MSG, msg, addSystemTime);
        }

        public void LogAppMsgToFile (string msg, bool addSystemTime = true)
        {
            LogToFileByCheckingFlag(LogType.APPLICATION, msg, addSystemTime);
        }

        /// <summary>
        ///  Default method that bypass any log type checking.
        ///  Make sure to set the right file name before logging anything.
        /// </summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="addSystemTime">Whether to add or not timestamp.</param>
        public void LogToFile (string msg, bool addSystemTime = true)
        {
            WriteToFile(msg, addSystemTime);
        }
        #endregion log to file

        #region log to console interface

        /// <summary>
        ///  Log message and add frame nr by default unless explicitly said otherwise
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="addFrameNr"></param>
        public void Log (string msg, bool addFrameNr = true)
        {
            Log(msg, null, addFrameNr);
        }

        /// <summary>
        ///  Final method that actually print the message based on how the function was called
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tag"></param>
        /// <param name="addFrameNr"></param>
        public void Log (string msg, string tag, bool addFrameNr = true)
        {
            if (string.IsNullOrEmpty(tag))
            {
                if (addFrameNr) Debug.Log ("[" + Time.frameCount + "] " + msg);
                else Debug.Log(msg);

            } else
            {
                // tag was provided
                if (addFrameNr) Debug.Log ("[" + Time.frameCount + "] <b>[" + tag + "]</b> " + msg);
                else Debug.Log("<b>[" + tag + "]</b> " + msg);
            }
        }

        /// <summary>
        ///  Log error message and add frame nr by default unless explicitly said otherwise
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="addFrameNr"></param>
        public void LogError (string msg, bool addFrameNr = true)
        {
            LogError(msg, null, addFrameNr);
        }

        /// <summary>
        ///  Final method that actually print the error message based on how the function was called
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="tag"></param>
        /// <param name="addFrameNr"></param>
        public void LogError (string msg, string tag, bool addFrameNr = true)
        {
            if (string.IsNullOrEmpty(tag))
            {
                if (addFrameNr) Debug.LogError ("[" + Time.frameCount + "] " + msg);
                else Debug.LogError(msg);

            } else
            {
                // tag was provided
                if (addFrameNr) Debug.LogError ("[" + Time.frameCount + "] <b>[" + tag + "]</b> " + msg);
                else Debug.LogError("<b>[" + tag + "]</b> " + msg);
            }
        }

        /// <summary>
        ///  Log that a key was pressed and there is a component reacting to that.
        ///  To disable these messages, set the logType mask to exclude LogTye.MSG
        ///  Message format: [frameId] [keyCode] [component] [addition msg/intention/action to perform]
        /// </summary>
        /// <param name="component"></param>
        /// <param name="keyCode"></param>
        /// <param name="action"></param>
        /// <param name="addFrameNr"></param>
        public void LogKeyPressed (string component, string keyCode, string action, bool addFrameNr = true)
        {
            if ((logType & LogType.MSG) != 0)
            {
                string msg = ((addFrameNr) ? ("[" + Time.frameCount + "]") : "") +
                    " <b>[KeyPress=" + keyCode + "]</b> " +
                    " <b>[" + component + "]</b> " +
                    " action: " + action;
                Debug.Log(msg);
            }
        }
        #endregion log to console interace

        #region set instance values
        /// <summary>
        ///  Set instance file name where things are going to be logged in.
        /// </summary>
        /// <param name="fileName">Name of the file. Needs to include file extension.</param>
        public void SetFileName (string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        ///  Set instance log level. By Setting a log level, certain messages can be filtered out.
        /// </summary>
        /// <param name="mask">Bit mask specifying messages that will be allowed.</param>
        public void SetLogLevel (LogType mask)
        {
            logType = mask;
        }
        #endregion set instance values

        #endregion public interface

        #region private methods
        private void LogToFileByCheckingFlag (LogType flagToCheck, string msg, bool addSystemTime)
        {
            if ((logType & flagToCheck) != 0) WriteToFile(msg, addSystemTime);
        }

        /// <summary>
        ///  Internal method that actually writes to the file.
        ///  It writes to the current file that has been set up. Make sure to set the filename before logging anything.
        /// </summary>
        private void WriteToFile (string msg, bool addSystemTime)
        {
            StreamWriter writer = new StreamWriter(fileName, true);
            DateTime currentDate = DateTime.Now;
            if (addSystemTime) msg = "[" + currentDate.ToString("dd/MM/yyyy HH:mm:ss") + "] " + msg;
            writer.WriteLine(msg);
            writer.Close();
        }
        #endregion
    }
}

