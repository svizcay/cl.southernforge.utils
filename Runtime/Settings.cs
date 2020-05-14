using System;   // Boolean
using System.Collections;
using System.Collections.Generic;
using System.IO;    // Path
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Settings utility that loads settings from a txt file placed in the root directory of a built app
    ///  or from a TextAsset localted within the project if running the app from the editor.
    ///  It's expected to be placed in a gameObject in the first scene.
    /// </summary>
    public class Settings : MonoBehaviour {

        /// <summary>
        ///  Internal storage with all settings read from the txt file.
        /// </summary>
        private Dictionary<string, string> mSettings = new Dictionary<string, string>();

#if UNITY_EDITOR
        [Header("Settings")]
        [Tooltip("txt settings file within the asset folder to use when playing game from the editor")]
        public TextAsset localSettingFile;
#endif
        /// <summary>
        /// File within the root directory of the app containing all settings
        /// </summary>
        private string filename = "settings.txt";

        #pragma warning disable 0414
        private static string classTag = "Settings";
        #pragma warning restore 0414

        /// <summary>
        /// Used for parsing settings.
        /// settings are expected to be written one per line in the following format:
        /// <name>=<value>
        /// </summary>
        private char[] delimiters = new char[] { '=' };

        #region unity events
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            LoadFromFile();
        }
        #endregion

        #region public interface

        // TODO: write a single generic method TryGetValue<T> that retrieve the right type.

        public bool TryGetValue (string key, out bool result)
        {
            string strQueryResult;
            if (mSettings.TryGetValue(key, out strQueryResult))
            {
                bool innerResult;
                if (Boolean.TryParse(strQueryResult, out innerResult))
                {
                    result = innerResult;
                    return true;
                } else {
                    result = false;
                    return false;
                }

            } else
            {
                result = false;
                return false;
            }
        }

        public bool TryGetValue (string key, out int result)
        {
            string strQueryResult;
            if (mSettings.TryGetValue(key, out strQueryResult))
            {
                int innerResult;
                if (Int32.TryParse(strQueryResult, out innerResult))
                {
                    result = innerResult;
                    return true;
                } else {
                    result = -1;
                    return false;
                }

            } else
            {
                result = -1;
                return false;
            }
        }

        public bool TryGetValue (string key, out float result)
        {
            string strQueryResult;
            if (mSettings.TryGetValue(key, out strQueryResult))
            {
                float innerResult;
                if (float.TryParse(strQueryResult, out innerResult))
                {
                    result = innerResult;
                    return true;
                } else {
                    result = -1.0f;
                    return false;
                }

            } else
            {
                result = -1.0f;
                return false;
            }
        }

        public bool TryGetValue (string key, out string result)
        {
            if (mSettings.TryGetValue(key, out result))
            {
                return true;
            } else
            {
                return false;
            }
        }
        #endregion

        #region private methods
        /// <summary>
        /// read txt file containing application settings (one per line).
        /// </summary>
        private void LoadFromFile ()
        {
#if UNITY_EDITOR
            string pathToTxtFile = AssetDatabase.GetAssetPath(localSettingFile);
#else
            // NOTE: Application.dataPath points to <projetName>_Data folder that is one directory down the hierarchy of the application root folder.
            // settings.txt file is expected to be in the application root directory.

            System.IO.DirectoryInfo dirInfo = new DirectoryInfo(Application.dataPath);
            string executableRootDir = dirInfo.Parent.ToString();
            string pathToTxtFile = Path.Combine(executableRootDir, filename);
            Logger.Instance.LogToFile("[" + classTag + "] settings file = " + pathToTxtFile);
#endif
            if (System.IO.File.Exists(pathToTxtFile))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(pathToTxtFile);

                int lineNr = 1;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] tokens = line.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 2)
                    {
                        Logger.Instance.LogErrorToFile("[" + classTag + "] ERROR: file=" + pathToTxtFile + " has wrong setting format at line nr=" + lineNr + " value=" + line);
                    } else
                    {
                        mSettings.Add(tokens[0], tokens[1]);
                        Logger.Instance.LogMsgToFile("[" + classTag + "] using setting " + tokens[0] + " " + tokens[1]);
                    }
                    lineNr++;
                }

                sr.Close();
                Logger.Instance.LogMsgToFile("[" + classTag + "] done reading setting");
            } else
            {
                Logger.Instance.LogWarningToFile("[" + classTag + "] ERROR: file=" + pathToTxtFile + " couldn't be found");
            }

        }
        #endregion
    }
}

