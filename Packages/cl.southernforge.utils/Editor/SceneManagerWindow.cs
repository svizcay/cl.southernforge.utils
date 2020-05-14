using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SouthernForge.Utils
{
    // there are three Unity's scene clases:
    // * UnityEngine.SceneManagement.Scene (has path and name fields)
    // * UnityEditor.SceneAsset (it has only name field)
    // * EditorBuildSettingsScene (has path and boolean saying if it's enabled)

    public class SceneManagerWindow : EditorWindow
    {
        [System.Serializable]
        public class Experience
        {
            public string label;
            public List<string> scenePaths = new List<string>();
            public bool foldout;    // for GUI. if it's folded or not
        }

        [System.Serializable]
        public class ListWrapper
        {
            public List<Experience> list;
        }

        ListWrapper experiences = new ListWrapper();

        // loaded in runtime.
        // it contains the list of sceneAssets per each scene
        List<List<SceneAsset>> experiencesSceneAssets = new List<List<SceneAsset>>();

        // window variables
        private GUIStyle guiStyle = new GUIStyle();
        private string newExperienceName;
        private Vector2 scrollPos;

        [MenuItem("SouthernForge/Utils/Scene Manager")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(SceneManagerWindow));
        }

        // gets called whenever the window is instantiated (or maybe whenever the window is displayed with show?)
        // restore values from previous invocations
        private void Awake()
        {
            guiStyle.richText = true;

            // restore initial values
            RestorePreviousValues();
        }
        
        void RestorePreviousValues ()
        {
            string serializedJson = EditorPrefs.GetString("JSON_KEY", "");
            // Debug.LogWarning(serializedJson);
            experiences = JsonUtility.FromJson<ListWrapper>(serializedJson);
            // Debug.LogWarning(experiences == null ? "NULL" : "NOT NULL");
            if (experiences != null)
            {
                // foldoutExperiences = new List<bool>(experiences.list.Count);

                // load experiences's scene assets
                for (int experienceID = 0; experienceID < experiences.list.Count; experienceID++)
                {
                    List<SceneAsset> sceneAssets = new List<SceneAsset>();
                    for (int sceneID = 0; sceneID < experiences.list[experienceID].scenePaths.Count; sceneID++)
                    {
                        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(experiences.list[experienceID].scenePaths[sceneID]);
                        sceneAssets.Add(sceneAsset);
                    }
                    experiencesSceneAssets.Add(sceneAssets);

                    // foldoutExperiences.Add(experiences.list[experienceID].foldout);
                }
            }
            else
            {
                experiences = new ListWrapper();
                experiences.list = new List<Experience>();
                // foldoutExperiences = new List<bool>();
            }
        }

        void SaveCurrentValues ()
        {
            string encodedJson = JsonUtility.ToJson(experiences);
            EditorPrefs.SetString("JSON_KEY", encodedJson);
        }

        void OnGUI()
        {
            // list experiences
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Experiences", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            if (experiences != null)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                // display current experiences
                // Debug.Log("nr experiences: " + experiences.list.Count);
                for (int i = 0; i < experiences.list.Count; i++)
                {
                    // BEGIN FIRST ROW
                    EditorGUILayout.BeginHorizontal();
                    // bool previousFoldoutOpt = experiences.list[i].foldout;
                    experiences.list[i].foldout = EditorGUILayout.Foldout(experiences.list[i].foldout, experiences.list[i].label);

                    if (GUILayout.Button("Remove", GUILayout.ExpandWidth(false)))
                    {
                        experiences.list.RemoveAt(i);
                        experiencesSceneAssets.RemoveAt(i);
                        SaveCurrentValues();
                        return;
                        // break;
                    }

                    if (GUILayout.Button("Activate", GUILayout.ExpandWidth(false)))
                    {
                        SceneManager emotiveSceneManager = FindObjectOfType<SceneManager>();
                        if (emotiveSceneManager != null)
                        {
                            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
                            emotiveSceneManager.sceneNames = new string[experiencesSceneAssets[i].Count];
                            
                            for (int sceneAssetId = 0; sceneAssetId < experiencesSceneAssets[i].Count; sceneAssetId++)
                            {
                                SceneAsset scene = experiencesSceneAssets[i][sceneAssetId];
                                string path = AssetDatabase.GetAssetPath(scene);
                                editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(path, true));
                                emotiveSceneManager.sceneNames[sceneAssetId] = scene.name;
                            }
                            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();

                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        } else
                        {
                            Debug.LogError("[SceneManagerWindow] No SceneManager found. Make sure you are using SceneManager in the right scene.");
                            return;
                        }
                    }

                    if (GUILayout.Button("Add Scene", GUILayout.ExpandWidth(false)))
                    {
                        experiencesSceneAssets[i].Add(null);
                        experiences.list[i].scenePaths.Add("");
                    }

                    EditorGUILayout.EndHorizontal();        
                    // END FIRST ROW

                    // show additional rows only if experience was foldout
                    if (experiences.list[i].foldout)
                    {
                        // Debug.Log("experience " + experiences.list[i].label + " has nr " + experiences.list[i].scenePaths.Count + " scene paths and nr " + experiencesSceneAssets[i].Count + " scene assets");
                        for (int sceneAssetId = 0; sceneAssetId < experiencesSceneAssets[i].Count; sceneAssetId++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            SceneAsset previousSceneAsset = experiencesSceneAssets[i][sceneAssetId];
                            experiencesSceneAssets[i][sceneAssetId] = (SceneAsset)EditorGUILayout.ObjectField(previousSceneAsset, typeof(SceneAsset), false);
                            if (previousSceneAsset != experiencesSceneAssets[i][sceneAssetId])
                            {
                                // save new path
                                // Debug.LogWarning("scene was modified");
                                string path = AssetDatabase.GetAssetPath(experiencesSceneAssets[i][sceneAssetId]);
                                experiences.list[i].scenePaths[sceneAssetId] = path;
                                SaveCurrentValues();
                            }

                            // show paths
                            EditorGUILayout.LabelField(experiences.list[i].scenePaths[sceneAssetId]);
                            if (GUILayout.Button(EditorGUIUtility.IconContent("winbtn_win_close"), GUILayout.ExpandWidth(false)))
                            {
                                experiences.list[i].scenePaths.RemoveAt(sceneAssetId);
                                experiencesSceneAssets[i].RemoveAt(sceneAssetId);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();        
                        }
                        EditorGUILayout.Space();
                    }   // end experience was foldout
                }
                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.TextArea("",GUI.skin.horizontalSlider);

            EditorGUILayout.LabelField("Add a new experience", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            newExperienceName = EditorGUILayout.TextField("name: ", newExperienceName);
            if (GUILayout.Button("Add", GUILayout.ExpandWidth(false)))
            {
                // Debug.LogWarning("name" + newExperienceName);

                Experience experience = new Experience();
                experience.label = newExperienceName;
                experience.scenePaths = new List<string>();
                experience.foldout = true;

                if (experiences == null) 
                {
                    experiences = new ListWrapper();
                    experiences.list = new List<Experience>();
                }

                experiences.list.Add(experience);
                experiencesSceneAssets.Add(new List<SceneAsset>());

                SaveCurrentValues();

                // reset text field
                newExperienceName = "";
            }
            EditorGUILayout.EndHorizontal();        
            EditorGUILayout.Space();

        }

        private void OnDestroy()
        {
            SaveCurrentValues();
        }
    }

}

