using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;  // used for MonoScript class
#endif

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Condition that allows you to specify the script that must exist in current scene.
    /// </summary>
    public class ConditionalLevelLoaderMissingScriptCondition : ConditionalLevelLoaderCondition {

        #region public settings

#if UNITY_EDITOR
        [Header("Settings")]
        [Tooltip("Component that if missing, we load the specified level")]
        [SerializeField]
        private MonoScript componentToCheckFor;
#endif

        #endregion

        #region public interface
#if UNITY_EDITOR
        /// <summary>
        ///  Find if a certain component is missing in the current scene
        /// </summary>
        /// <returns>true if component is missing in the scene (load specified level), false otherwise.</returns>
        public override bool Evaluate()
        {
            var objFound = FindObjectOfType(componentToCheckFor.GetClass());
            return (objFound == null) ? true : false;
        }
#else
        // code compiled for build app
        public override bool Evaluate()
        {
            return false;
        }
#endif
        #endregion
    }
}

