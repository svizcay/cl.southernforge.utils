using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SouthernForge.Utils
{
    // used for ScriptableObjects to create instances of that class (since they can not be instanced by adding them to gameobjects)
    // values:
    //  * fileName = default file name for new "fileName.asset" file
    //  * menuName = how to find it in the create asset menu
    //  * order = the position in the menu
    [CreateAssetMenu(fileName = "loggerData", menuName = "SouthernForge/Utils/LoggerData")]
    public class LoggerData :  ScriptableObject {

        // [Header("Settings")]
        [SerializeField]
        private string fileName = "log.txt";

        private static LoggerData mInstance;
        public static LoggerData Instance
        {
            get {
                if (mInstance == null) mInstance = new LoggerData();
                return mInstance;
            }
        }




        // // Use this for initialization
        // void Start () {
        //     
        // }
        // 
        // // Update is called once per frame
        // void Update () {
        //     
        // }
    }

}

