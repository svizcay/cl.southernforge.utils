using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Execute an event to wich gameObjects can subscribe via the editor
    /// </summary>
    public class DelayedEventTrigger : MonoBehaviour {

        [Header("Settings")]

        [SerializeField]
        private float time;

        [SerializeField]
        private UnityEvent onTimesUp;

        #region unity events
        void Start () {
            StartCoroutine(StartCounting());
        }
        #endregion

        #region private methods
        IEnumerator StartCounting ()
        {
            yield return new WaitForSeconds(time);
            onTimesUp.Invoke();
        }
        #endregion
    }

}

