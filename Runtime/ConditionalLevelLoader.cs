using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;  // used for MonoScript class
#endif
using UnityEngine;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Load a particular scene index based on a criteria (like a missing object in a scene or a connection to a server).
    ///  Used mostly during development in order to play a "Experiece" from a scene being modified
    ///  ie: without being forced of pressing play in the initial scene.
    ///  If there are gameObjects in the current scene that are marked as dontDestroyable and this scene is suposed to be laoded again,
    ///  make sure to add them to the list of gameObjects to destroy before loading the desired level.
    /// </summary>
    [AddComponentMenu("SouthernForge/Utils/ConditionalLevelLoader")]
    public class ConditionalLevelLoader : MonoBehaviour {

        #region public settings
        [Header("Settings")]

        [Tooltip("Level to load if something is missing")]
        [SerializeField]
        private int sceneIndexToLoad = 0;

        [Tooltip("List of conditions. If one evalutes to true, then specifed level is loaded")]
        [SerializeField]
        private ConditionalLevelLoaderCondition[] conditions;

        [Tooltip("List of game objects in current scene to destroy before loading a different scene")]
        [SerializeField]
        private Transform[] dontDestroyableGameObjects;
        #endregion

        #region unity events
#if UNITY_EDITOR
        private void Awake()
        {
            bool conditionMet = false;
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Evaluate())
                {
                    conditionMet = true;
                    break;
                }

            }
            if (conditionMet)
            {
                // destroy "dontDestroyOnLoad" gameObjects and load desired level
                for (int i = 0; i < dontDestroyableGameObjects.Length; i++)
                {
                    Destroy(dontDestroyableGameObjects[i].gameObject);
                }
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
#endif
        #endregion
    }
}

