using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SouthernForge.Utils
{
    /// <summary>
    ///  Utility class that detects when the player gets into a certain volume defined by a Collider.
    ///  Player must belong to "Player" layer.
    ///  Attached Collider can be set without any problem as "isTrigger" but there might be some constraint in the Player gameObject to make it work.
    /// </summary>
    public class PlayerInsideVolumeDetector : MonoBehaviour {

        [Header("Settings")]

        // NOTE: this variable seems to be useless right now.
        // Its only purpose is to enable/disable the execution of Update method, which can be achieved by using enabled var inherited from Behaviour
        [Tooltip("Whether the enable/disable the current scrip")]
        [SerializeField]
        private bool isActive = true;

        [SerializeField]
        [Tooltip("Whether the Enter/Exit event is only trigger once or multiple times")]
        private bool triggerOnce = true;

        [Tooltip("Observers of Enter event")]
        public UnityEvent onPlayerGotInside;

        /// <summary>
        ///  Display the value if player is inside or not of the volume.
        /// </summary>
        [Header("Debugging Info")]
        [SouthernForge.Utils.ReadOnly]
        [SerializeField]
        [Tooltip("display the value if player is inside or not of the volume")]
        private bool playerIsInside = false;

        #region private data
        private bool previousState;
        private bool alreadyTriggered = false;
        #endregion private data

        #region unity events
        private void Awake()
        {
            previousState = false;
        }

        void Update () {
            if (isActive)
            {
                if (playerIsInside != previousState)
                {
                    // state was changed. send notification
                    if (playerIsInside)
                    {
                        if (!triggerOnce || !alreadyTriggered)
                        {
                            onPlayerGotInside.Invoke();
                            alreadyTriggered = true;
                        }

                    }

                }

                previousState = playerIsInside;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerIsInside = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerIsInside = true;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                playerIsInside = false;
            }
        }
        #endregion

        #region public API
        public void SetActive (bool value)
        {
            isActive = value;
        }
        #endregion
    }

}

