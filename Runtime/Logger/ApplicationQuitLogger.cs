using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Logger utility that will log the event when the application stars (aprox) and when the application finishes.
    ///  Needs to be added in a gameobject in the first scene.
    ///  
    ///  Depends on: Logger
    ///  
    ///  TODO: decide later how useful this class is having to rely on Loggger.
    ///  
    /// </summary>
    public class ApplicationQuitLogger : MonoBehaviour {

        #pragma warning disable 0414
        private static string classTag = "ApplicationQuitLogger";
        #pragma warning restore 0414

        #region unity events
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // logger filename is set on Awake. Wait until on Start to write messages to it
            Logger.Instance.LogAppMsgToFile("[" + classTag + "] experience started");
        }

        private void OnApplicationQuit()
        {
            string elapsedTimeStr = (Time.realtimeSinceStartup > 60) ? (Time.realtimeSinceStartup / 60.0f).ToString() + "min" : Time.realtimeSinceStartup.ToString() + "seg";
            Logger.Instance.LogAppMsgToFile("[" + classTag + "]  about to finish experience after " + elapsedTimeStr + " experience started");
        }
        #endregion
    }

}

