using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// using Valve.VR;

namespace SouthernForge.Utils
{
    /*
     * TODO: we used to offer as "loaders" steamvr's LoadLevel and photon LoadLevel.
     * Break that depency. We don't want to depend on those packages.
     * */
    /// <summary>
    ///  Utility class that abstract the process of loading the following scene.
    ///  Values can be set by "experience" by adding entries in the "Scene Manager" menu and activating the right one.
    /// </summary>
    [AddComponentMenu("SouthernForge/Utils/SceneManager")]
    public class SceneManager : MonoBehaviour {

        /// <summary>
        ///  Types of internal methods available to load a scene.
        /// </summary>
        public enum LOADER_TYPE { UNITY, VIVE, PHOTON};

        [Header("Settings")]
        [Tooltip("Allows other scripts to execute Awake method")]
        [SerializeField]
        private bool delayStart = true;  // if true, experience starts on Start; if false, experience starts on Awake
        [SerializeField]
        private bool verbose = false;


        [Header("Debugging Info")]
        // [SouthernForge.Utils.BeginReadOnlyGroup]
        [SouthernForge.Utils.ReadOnly]
        [Tooltip("List of scenes to be played in the experience. Use Scene Manager menu to set them up")]
        public string[] sceneNames;

        [SerializeField]
        [SouthernForge.Utils.ReadOnly]
        [Tooltip("Index of the scene that is going to be played by calling LoadNextScene")]
        private int sceneToPlay = 1;
        // [SouthernForge.Utils.EndReadOnlyGroup]

        /// <summary>
        ///  Delegate to get notified when a scene transition is going to happen
        /// </summary>
        public delegate void SceneTransitionHandler();
        public static event SceneTransitionHandler onSceneTransition;

        #pragma warning disable 0414
        private static string classTag = "SceneManager";
        #pragma warning restore 0414

        #region unity events
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (!delayStart) LoadInitialScene();
        }

        private void Start()
        {
            if (delayStart) LoadInitialScene();
        }
        #endregion

        #region public interface
        /// <summary>
        ///  Load next scene set it up for the current experience.
        /// </summary>
        /// <param name="loaderToUse">Loader to use.</param>
        public void LoadNextScene (LOADER_TYPE loaderToUse = LOADER_TYPE.UNITY)
        {
            if (sceneToPlay < sceneNames.Length)
            {
                // if (verbose) Debug.Log("tryint to load " + sceneNames[sceneToPlay]);
                string name = sceneNames[sceneToPlay];
                if (verbose) SouthernForge.Utils.Logger.Instance.Log("trying to load " + name, classTag);
                NotifySceneTransition();
                switch (loaderToUse)
                {
                    case LOADER_TYPE.UNITY:
                        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.VIVE:
                        throw new Exception("Not implemented yet. We broke the dependency from steamvr at this point");
                        // SteamVR_LoadLevel.Begin(name, true, 1.0f);

                        // UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.PHOTON:
                        throw new Exception("Not implemented yet. We broke the dependency from photon at this point");
                        // Photon.Pun.PhotonNetwork.LoadLevel(name);
                        break;
                }
                // NOTE: since this is a don't destryable gameObject, this part of the code gets also executed even after loading the next scence.
                sceneToPlay++;
            }
        }

        /// <summary>
        ///  Load previous scene in the current experience.
        /// </summary>
        /// <param name="loaderToUse">Loader to use.</param>
        public void LoadPreviousScene (LOADER_TYPE loaderToUse = LOADER_TYPE.UNITY)
        {
            // sceneToPlay points to the "next" scene. therefore, currentScene is (sceneToPlay-1)
            int targetSceneId = sceneToPlay - 2;
            if (targetSceneId >= 0 && targetSceneId < sceneNames.Length)
            {
                // if (verbose) Debug.Log("tryint to load " + sceneNames[sceneToPlay]);
                string name = sceneNames[targetSceneId];
                if (verbose) SouthernForge.Utils.Logger.Instance.Log("trying to load " + name, classTag);
                NotifySceneTransition();
                switch (loaderToUse)
                {
                    case LOADER_TYPE.UNITY:
                        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.VIVE:
                        throw new Exception("Not implemented yet. We broke the dependency from steamvr at this point");
                        // SteamVR_LoadLevel.Begin(name, true, 1.0f);
                        // UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.PHOTON:
                        throw new Exception("Not implemented yet. We broke the dependency from photon at this point");
                        // Photon.Pun.PhotonNetwork.LoadLevel(name);
                        break;
                }
                // NOTE: since this is a don't destryable gameObject, this part of the code gets also executed even after loading the next scence.
                sceneToPlay = targetSceneId + 1;    // update pointer to next scene to play
            }
        }

        /// <summary>
        ///  Load a scene id in the current experience.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="loaderToUse"></param>
        public void LoadSceneId (int id, LOADER_TYPE loaderToUse = LOADER_TYPE.UNITY)
        {
            if (id >= 0 && id < sceneNames.Length)
            {
                // if (verbose) Debug.Log("tryint to load " + sceneNames[sceneToPlay]);
                string name = sceneNames[id];
                if (verbose) SouthernForge.Utils.Logger.Instance.Log("trying to load " + name, classTag);
                NotifySceneTransition();
                switch (loaderToUse)
                {
                    case LOADER_TYPE.UNITY:
                        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.VIVE:
                        throw new Exception("Not implemented yet. We broke the dependency from steamvr at this point");
                        // SteamVR_LoadLevel.Begin(name, true, 1.0f);
                        // UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                        break;
                    case LOADER_TYPE.PHOTON:
                        throw new Exception("Not implemented yet. We broke the dependency from photon at this point");
                        // Photon.Pun.PhotonNetwork.LoadLevel(name);
                        break;
                }
                // NOTE: since this is a don't destryable gameObject, this part of the code gets also executed even after loading the next scence.
                sceneToPlay = id + 1;
            }
        }
        #endregion

        #region private
        private void LoadInitialScene ()
        {
            if (sceneNames != null && sceneNames.Length > sceneToPlay)
            {
                // assumming scene number 0 is the "scene manager" scene, that means first real scene to play should be index 1
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNames[sceneToPlay]); 
                sceneToPlay++;
            }
        }

        private void NotifySceneTransition ()
        {
            if (onSceneTransition != null) onSceneTransition();
        }
        #endregion
    }
}