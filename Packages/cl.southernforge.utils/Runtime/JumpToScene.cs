using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Jump to a different scene after pressing a key.
    ///  It relies on SceneManager to load the next scene.
    /// </summary>
    public class JumpToScene : MonoBehaviour {

        public enum JumpMode { Next, Previous , Id };

        [Header("Settings")]

        [Tooltip("Jump to next scene in SceneManager, previous, or one specified by a given Id")]
        [SerializeField]
        private JumpMode jumpMode;

        [Tooltip("Id of the scene if jumpMode is Id")]
        [SerializeField]
        private int sceneIdToJump;

        [SerializeField]
        private KeyCode key = KeyCode.Return;

        // external components
        private SceneManager sceneManager;

        #region unity events
        private void Awake()
        {
            sceneManager = FindObjectOfType<SceneManager>();
        }
        
        void Update () {
            if (Input.GetKeyDown (key))
            {
                SouthernForge.Utils.Logger.Instance.LogKeyPressed(GetType().Name, key.ToString(), "Jump to another scene");
                Jump();
            }
        }
        #endregion

        #region public interface
        /// <summary>
        ///  Jump to the specified scene id
        ///  This method can be called either by a key being pressed or by a certain event
        /// </summary>
        public void Jump ()
        {
            switch (jumpMode)
            {
                case JumpMode.Next:
                    sceneManager.LoadNextScene();
                    break;
                case JumpMode.Previous:
                    sceneManager.LoadPreviousScene();
                    break;
                case JumpMode.Id:
                    sceneManager.LoadSceneId(sceneIdToJump);
                    break;
            }
        }

        #endregion
    }

}

