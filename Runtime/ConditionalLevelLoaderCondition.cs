using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Abstract class that defines conditions to decide if to load or not a particular level
    /// </summary>
    public abstract class ConditionalLevelLoaderCondition : MonoBehaviour
    {
        /// <summary>
        ///  Evaluate condition.
        /// </summary>
        /// <returns>true if level specified level should be loaded; false otherwise.</returns>
        public abstract bool Evaluate();
    }
}
