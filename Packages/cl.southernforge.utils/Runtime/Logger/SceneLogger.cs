using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // UnityEvent
using UnityEngine.SceneManagement;  // SceneManager, Scene, LoadSceneMode

namespace SouthernForge.Utils
{
    /*
     * Unity order of events: Awake -> OnEnable -> Start
     * */
    /// <summary>
    ///  Utility class that logs whenever a scene is loaded/unloaded.
    ///  No events should be attached to react to a scene loading event ie, code shoud not be scene dependent.
    ///  GameObject is persistent along the experience.
    ///  Make sure Logger file has been correctly set it up (filename and loglevel).
    /// </summary>
    public class SceneLogger : MonoBehaviour {

        // NOTE: we were using this script to react to sceneLoad events
        // and allow clients to attach following events (by triggering things within the current scene).
        // we discarded that idea and now it's just a logger of scene load and unload events
        // that means, there will be only one instance of this component during the whole "experience".

        [Header("Settings")]
        [Tooltip("Whether to log scene unloads or not")]
        [SerializeField]
        private bool logAlsoSceneUnload = false;

        // [SerializeField]
        // private UnityEvent onSceneLoaded;

        // disable warnings:
        // 0168 declared but not used
        // 0219 assigned but not used
        // 0414 private assigned but not used
        #pragma warning disable 0414
        private static string classTag = "SceneLogger";
        #pragma warning restore 0414

        #region unity events
        private void Awake()
        {
            // Logger.Instance.Log("Awake", tag);
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            // Logger.Instance.Log("OnEnable", tag);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        #endregion

        #region private methods
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string elapsedTimeStr = (Time.realtimeSinceStartup > 60) ? (Time.realtimeSinceStartup / 60.0f).ToString() + "min" : Time.realtimeSinceStartup.ToString() + "seg";
            Logger.Instance.LogMsgToFile("[SceneLogger] sceneId=" + scene.buildIndex + " scene=" + scene.name + " loaded at " + elapsedTimeStr + " after experience started");
        }

        private void OnSceneUnloaded(Scene scene)
        {
            if (logAlsoSceneUnload)
            {
                string elapsedTimeStr = (Time.realtimeSinceStartup > 60) ? (Time.realtimeSinceStartup / 60.0f).ToString() + "min" : Time.realtimeSinceStartup.ToString() + "seg";
                Logger.Instance.LogMsgToFile("[SceneLogger] sceneId=" + scene.buildIndex + " scene=" + scene.name + " unloaded at " + elapsedTimeStr + " after experience started");
            }
        }
        #endregion
    }
}

