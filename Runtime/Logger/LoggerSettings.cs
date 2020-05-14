using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SouthernForge.Utils
{
    /// <summary>
    ///  By adding it to a gameObject at the top of the first scene, allow the user to specify the filename of the main log file.
    ///  Make sure to set the filename of the Logger utility before logging any message, otherwise the default filename is going to be used.
    ///  It also allows setting the desired log level (either by reading it from the settings file or by the mask specify in the editor).
    /// </summary>
    public class LoggerSettings : MonoBehaviour {

        /// <summary>
        ///  filename to use by Logger utility.
        /// </summary>
        [Header("Settings")]
        [SerializeField]
        [Tooltip("name to use for the log file")]
        private string filename = "log.txt";

        [SerializeField]
        [Tooltip("specify what messages to log in editor mode")]
		[EnumFlags]
        private Logger.LogType logMask = Logger.LogType.ERROR | Logger.LogType.WARNING | Logger.LogType.MSG | Logger.LogType.APPLICATION;
        private Logger.LogType defaultLogMask = Logger.LogType.ERROR | Logger.LogType.WARNING | Logger.LogType.MSG | Logger.LogType.APPLICATION;

        // external components
        private Settings settings;

        // private data

        /// <summary>
        ///  name of the settings that specifies the log level
        /// </summary>
        private static string logLevelkey = "logLevel";


        #region unity events
        private void Awake()
        {
            Logger.Instance.SetFileName(filename);
            settings = FindObjectOfType<Settings>();
        }

        private void Start()
        {
            // settings are read on Awake. Wait until on Start to query it.
#if UNITY_STANDALONE
            TryToSetLogLevelFromSettings();
#endif
            Logger.Instance.SetLogLevel(logMask);
        }
        #endregion

        #region public interface
        public void Test ()
        {
            print(logMask);
            if ((logMask & Logger.LogType.ERROR) != 0) print(Logger.LogType.ERROR);
            if ((logMask & Logger.LogType.WARNING) != 0) print(Logger.LogType.WARNING);
            if ((logMask & Logger.LogType.MSG) != 0) print(Logger.LogType.MSG);
            if ((logMask & Logger.LogType.APPLICATION) != 0) print(Logger.LogType.APPLICATION);
        }
        #endregion

        #region private methods
        private void TryToSetLogLevelFromSettings ()
        {
            if (settings != null)
            {
                string logLevelStr;
                if (settings.TryGetValue(logLevelkey, out logLevelStr))
                {
                    switch (logLevelStr)
                    {
                        case "all":
                            logMask = Logger.LogType.ERROR | Logger.LogType.WARNING | Logger.LogType.MSG | Logger.LogType.APPLICATION;
                            break;
                        case "application":
                            logMask = Logger.LogType.APPLICATION;
                            break;
                        default:
                            logMask = defaultLogMask;
                            break;
                    }
                }
            }
        }

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LoggerSettings))]
    public class LoggerSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LoggerSettings script = target as LoggerSettings;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test"))
            {
                script.Test();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif

}

