#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using UnityEditor.Search;
using UnityEditor.UIElements;

namespace Waypointer
{




    [CanEditMultipleObjects]
    [CustomEditor(typeof(Waypoint))]
    public class WaypointInspector : Editor
    {

        #region Public Fields

        [SerializeField]
        VisualTreeAsset m_CustomInspectorAsset;

        [SerializeField]
        VisualTreeAsset m_connectionsListEntryAsset;

        public bool debugMode = true;

        SerializedProperty connectionsProperty;

        public List<Waypoint> connections = new List<Waypoint>();

        private int currentActiveConnectionToChangePropertyArrayIndex = 0;

        #endregion

        #region Private Fields

        private bool eventExecuted = false;

        private Object activeObjectAtTheTimeOfTheButtonPressed;
        private Object lastSelectedObject;

        private bool buttonPressed = false;

        private bool selectionModeCreateNewConnectedWaypoint = false;

        private bool selectionModeCreateNewConnection = false;

        private int newWaypointCount = 0;

        private bool selectionOfPositionForNewConnectedWaypoint = false;

        private SerializedProperty exitWaypointProperty;

        private SerializedProperty entryWaypointProperty;

        private bool isEntryWaypointControlValue;

        private bool isExitWaypointControlValue;

        private VisualElement root;

        private ListView connectionsListView;

        [SerializeField]
        private bool bothWaysToggleValue;

        private string buttonTextConnectToExistingWaypointSingleWay = "New One Way Connection to other Waypoint";

        private string buttonTextConnectToExistingWaypointBothWays = "New Both Ways Connection to other Waypoint";

        private string buttonTextCreateNewWayPoint = "Add new connected Waypoint";

        public override VisualElement CreateInspectorGUI()
        {

            serializedObject.ApplyModifiedProperties();

            return root;
        }

        private void scanelement(VisualElement ve)
        {
            foreach (VisualElement childElement in ve.Children())
            {
                dbg(childElement.name.ToString());
                scanelement(childElement);
            }
        }

        private void OnEnable()
        {
            foreach (SceneView scene in SceneView.sceneViews)
            {
                //Rect r = position;

                //dbg($"Scene Rect : " + scene.position.ToString());
            }

            entryWaypointProperty = serializedObject.FindProperty("entryWaypoint");

            exitWaypointProperty = serializedObject.FindProperty("exitWaypoint");

            if (m_CustomInspectorAsset == null)
            {
                Debug.LogError("You have to assign a Custom Inspector UXML to the Waypoint Inspector");
                return;
            }

            root = m_CustomInspectorAsset.CloneTree();

            Label waypointnamelabel = root.Query<Label>("WaypointNameLabel");

            waypointnamelabel.bindingPath = this.name;

            waypointnamelabel.text = ((Waypoint)target).name;

            SetUpConnectionsListView();

            SetUpIsEntryToogle();

            SetupIsExitToggle();

            SetUpPlacementRadiusVariationSlider();

            SetIconForWaypointDependingOnExitEntry();

            Button button = root.Query<Button>("AddOneWayConnectionButton");
            button.text = buttonTextConnectToExistingWaypointSingleWay;
            SetUpHoverForButton(button);

            button.clicked += () =>
            {
                bothWaysToggleValue = false;
                AddNewOneWayConnectionButtonClicked();
                foreach (SceneView scene in SceneView.sceneViews)
                    scene.ShowNotification(new GUIContent("Click on the Waypoint to connect to."));
            };

            button = root.Query<Button>("NewBothWaysConnectionButton");
            SetUpHoverForButton(button);
            button.text = buttonTextConnectToExistingWaypointBothWays;

            button.clicked += () =>
            {
                bothWaysToggleValue = true; AddNewOneWayConnectionButtonClicked();
                foreach (SceneView scene in SceneView.sceneViews)
                    scene.ShowNotification(new GUIContent("Click on the Waypoint to connect to."));
            };

            button = root.Query<Button>("AddNewConnectedButton");
            SetUpHoverForButton(button);
            button.text = buttonTextCreateNewWayPoint;

            button.clicked += () =>
            {
                AddNewConnectedWaypointButtonClicked();
                foreach (SceneView scene in SceneView.sceneViews)
                    scene.ShowNotification(new GUIContent("Click anywhere in the scene to place a new waypoint there."));
            };

        }

        private void AddNewOneWayConnectionButtonClicked()
        {
            selectionModeCreateNewConnection = true;

            buttonPressed = true;

            Selection.selectionChanged += OnSelectionChanged;

            Debug.Log($"After Clicking Button : Selection Active Object : {Selection.activeObject.ToString()}");

            activeObjectAtTheTimeOfTheButtonPressed = Selection.activeObject;
        }

        private void AddNewConnectedWaypointButtonClicked()
        {

            selectionOfPositionForNewConnectedWaypoint = true;

        }

        private void SetUpHoverForButton(Button button)
        {
            //button.style.backgroundColor = Color.gray;

            Color32 nothover = new Color32(241, 241, 241, 255);
            Color32 hover = new Color32(251, 251, 251, 255);

            button.RegisterCallback<MouseOverEvent>((type) =>
            {
                button.style.backgroundColor = (Color)hover;
            });

            button.RegisterCallback<MouseOutEvent>((type) =>
            {
                button.style.backgroundColor = (Color)nothover;
            });
        }

        /*     private void OnDisable()
            {
                // Unsubscribe from the selection change event
                Selection.selectionChanged -= OnSelectionChanged;
            } */


        private void OnSelectionChanged()
        {

            dbg($"selectionModeCreateNewConnectedWaypoint is {selectionModeCreateNewConnectedWaypoint}");
            dbg($"selectionModeCreateNewConnection is {selectionModeCreateNewConnection}");

            // Check if the user has selected a new object
            if (selectionModeCreateNewConnectedWaypoint & buttonPressed & Selection.activeGameObject != null & Selection.activeGameObject != lastSelectedObject)
            {

                Debug.Log("selectionModeCreateNewConnectedWaypoint !");

                connectionsProperty = serializedObject.FindProperty("connections");

                lastSelectedObject = Selection.activeGameObject;

                // Execute your event logic here
                //if (!eventExecuted)
                //{//
                Debug.Log($"User selected: {lastSelectedObject.name}");
                //    eventExecuted = true;

                // Optionally, you can reset the eventExecuted flag after some time or condition
                // For example, you can reset it after a delay or when a specific condition is met
                //}

                Debug.Log($"Current active Object in OnSelectionChanged : {activeObjectAtTheTimeOfTheButtonPressed.ToString()}");

                //Selection.activeObject = activeObject;

                Selection.selectionChanged -= OnSelectionChanged;

                buttonPressed = false;

                EditorApplication.delayCall += RestoreActiveObject;

                Waypoint waypointToReplaceWith = Selection.activeGameObject.GetComponent<Waypoint>();

                connectionsProperty.DeleteArrayElementAtIndex(currentActiveConnectionToChangePropertyArrayIndex);

                connectionsProperty.InsertArrayElementAtIndex(currentActiveConnectionToChangePropertyArrayIndex);

                connectionsProperty.GetArrayElementAtIndex(currentActiveConnectionToChangePropertyArrayIndex).objectReferenceValue = waypointToReplaceWith;

                serializedObject.ApplyModifiedProperties();

                selectionModeCreateNewConnectedWaypoint = false;
            }

            if (selectionModeCreateNewConnection & buttonPressed & Selection.activeGameObject != null & Selection.activeGameObject != lastSelectedObject)
            {
                dbg("selectionModeCreateNewConnection !");

                connectionsProperty = serializedObject.FindProperty("connections");

                lastSelectedObject = Selection.activeGameObject;

                // Execute your event logic here
                //if (!eventExecuted)
                //{//
                Debug.Log($"User selected : {lastSelectedObject.name}");
                //    eventExecuted = true;

                // Optionally, you can reset the eventExecuted flag after some time or condition
                // For example, you can reset it after a delay or when a specific condition is met
                //}

                Debug.Log($"Current active Object in OnSelectionChanged : {activeObjectAtTheTimeOfTheButtonPressed.ToString()}");

                //Selection.activeObject = activeObject;

                Selection.selectionChanged -= OnSelectionChanged;

                buttonPressed = false;

                EditorApplication.delayCall += RestoreActiveObject;

                Waypoint waypointToConnectTo = Selection.activeGameObject.GetComponent<Waypoint>();

                dbg($"There are {connectionsProperty.arraySize} in Connections of {Selection.activeGameObject.name} ");

                //connectionsProperty.arraySize += 1;

                connectionsProperty.InsertArrayElementAtIndex(0);

                connectionsProperty.GetArrayElementAtIndex(0).objectReferenceValue = waypointToConnectTo;

                serializedObject.ApplyModifiedProperties();

                if (bothWaysToggleValue)
                {
                    waypointToConnectTo.connections.Add((Waypoint)target);
                }

                selectionModeCreateNewConnection = false;
            }
        }

        private void RestoreActiveObject()
        {
            // Re-select the originally active object
            Selection.activeObject = activeObjectAtTheTimeOfTheButtonPressed;
        }

        private void SetUpConnectionsListView()
        {

            connectionsListView = root.Q<ListView>();

            connectionsListView.itemsSource = ((Waypoint)target).connections;

            connectionsListView.makeItem = m_connectionsListEntryAsset.CloneTree;

            connectionsListView.bindItem = (element, i) =>
            {
                Waypoint w = (Waypoint)connectionsListView.itemsSource[i];

                VisualElement result = element.Query("WayPointObjectField").First();

                UnityEditor.UIElements.ObjectField o = (UnityEditor.UIElements.ObjectField)result;

                o.value = (Object)w;

                o.RegisterValueChangedCallback(evt =>
                {
                    connectionsListView.itemsSource[i] = evt.newValue;
                });

                Button reconnectButton = (Button)element.Query("ReconnectButton").First();

                reconnectButton.clicked += () => { ReconnectButtonClicked(w, i); };

            };
        }

        private void ReconnectButtonClicked(Waypoint waypointToReconnect, int indexInConnectionsArray)
        {
            dbg($"Button clicked for Waypoint {waypointToReconnect.name} to reconnect.");

            buttonPressed = true;

            Selection.selectionChanged += OnSelectionChanged;

            selectionModeCreateNewConnectedWaypoint = true;

            Debug.Log($"After Clicking Button : Selection Active Object : {Selection.activeObject.ToString()}");

            activeObjectAtTheTimeOfTheButtonPressed = Selection.activeObject;

            currentActiveConnectionToChangePropertyArrayIndex = indexInConnectionsArray;
        }

        private void SetUpIsEntryToogle()
        {
            Toggle isEntryToogle = root.Query<Toggle>().First();

            isEntryToogle.bindingPath = "entryWaypoint";

            isEntryToogle.value = entryWaypointProperty.boolValue;

            isEntryToogle.RegisterValueChangedCallback(evt =>
            {

                // dbg($"isEntryToogle : neuer wert {evt.newValue} , alter wert{((Waypoint)target).entryWaypoint}");

                ((Waypoint)target).entryWaypoint = evt.newValue;

                SetIconForWaypointDependingOnExitEntry();

                EditorUtility.SetDirty(((Waypoint)target)); // Mark the object as dirty
            });
        }

        private void SetupIsExitToggle()
        {
            Toggle toogle = root.Query<Toggle>("exitWaypointToggle");

            toogle.bindingPath = "exitWaypoint";

            toogle.value = exitWaypointProperty.boolValue;

            toogle.RegisterValueChangedCallback(evt =>
            {

                // dbg($"toogle : neuer wert {evt.newValue} , alter wert{((Waypoint)target).entryWaypoint}");

                ((Waypoint)target).exitWaypoint = evt.newValue;

                SetIconForWaypointDependingOnExitEntry();

                EditorUtility.SetDirty(((Waypoint)target)); // Mark the object as dirty
            });
        }

        private void SetUpPlacementRadiusVariationSlider()
        {
            Slider slider = root.Query<Slider>("PlacmentVariationRadiusSlider");

            slider.bindingPath = "placementVariationRadius";

            slider.RegisterValueChangedCallback(evt =>
            {
                ((Waypoint)target).placementVariationRadius = evt.newValue;

                EditorUtility.SetDirty(((Waypoint)target));

            });

        }

        void OnSceneGUI()
        {

            Handles.BeginGUI();

            Rect buttonRect = new Rect(10, 10 + EditorGUIUtility.singleLineHeight, 100, 30);


            if (GUI.Button(buttonRect, "Press Me"))
                Debug.Log("Got it to work.");

            Handles.EndGUI();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {

                // create object here at pointsPos
                if (selectionOfPositionForNewConnectedWaypoint)
                {
                    Debug.Log("Click on Scene to choose new waypoint position happend.");

                    Event.current.Use();

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        CreateNewConnectedWaypoint(hit.point);
                    }

                    selectionOfPositionForNewConnectedWaypoint = false;
                }
            }
        }

        private void SetIconForWaypointDependingOnExitEntry()
        {

            var wp = (Waypoint)target;

            var dbgstr = $"Trying to set Icon for Waypoint {wp.name} with {wp.entryWaypoint}/{wp.exitWaypoint}";

            //dbg(dbgstr);

            //Debug.Log(dbgstr);

            Texture2D texture2Dtouse = null;

            if (wp.entryWaypoint & !wp.exitWaypoint)
                texture2Dtouse = wp.IconEntryWaypoint;

            if (!wp.entryWaypoint & wp.exitWaypoint)
                texture2Dtouse = wp.IconExitWaypoint;

            if (wp.entryWaypoint & wp.exitWaypoint)
                texture2Dtouse = wp.IconExitEntryWaypoint;

            if (!wp.entryWaypoint & !wp.exitWaypoint)
                texture2Dtouse = wp.IconDefaultWaypoint;

            //SceneView.RepaintAll(); 

            //if (GUI.changed) EditorUtility.SetDirty (target);


            EditorGUIUtility.SetIconForObject(wp.gameObject, texture2Dtouse);
        }

        private void CreateNewConnectedWaypoint(Vector3 positionForNewWayPoint)
        {
            //GameObject newWayPointGameObject = new GameObject();

            var wp = (Waypoint)target;

            GameObject waypointPrefab = Instantiate(wp.newWayPointPrefab);



            newWaypointCount++;

            waypointPrefab.name = $"New Waypoint ({newWaypointCount})";

            Waypoint newWP = waypointPrefab.GetComponent<Waypoint>();

            dbg("New Waypoint : " + newWP);



            //var i = EditorGUIUtility.GetIconForObject(wp.gameObject);

            //dbg("I: " + i.ToString());

            newWP.gameObject.transform.parent = wp.transform.parent;

            newWP.gameObject.transform.position = positionForNewWayPoint;
            newWP.gameObject.transform.position += new Vector3(0, 1.2f, 0);

            //var iconContent = EditorGUIUtility.IconContent(i.name); (Texture2D)iconContent.image

            EditorGUIUtility.SetIconForObject(newWP.gameObject, wp.IconDefaultWaypoint);

            wp.connections.Add(newWP);

            newWP.connections.Add(wp);

            SceneView.RepaintAll();
        }

        #endregion

        #region Unity Methods

        void Start()
        {

            if (!CheckPreferences()) return;
        }

        #endregion

        #region Class Methods

        private bool CheckPreferences()
        {
            return true;
        }

        protected void dbg(string message, bool error = false)
        {
            if (debugMode & !error)
                Debug.Log("[ " + this.GetType().Name + " (" + Time.time + ")] " + message);

            if (error)
                Debug.LogError("<color=\"red\">[" + this.GetType().Name + " (" + Time.time + ")] " + message + "</color>");
        }

        #endregion

    }
#endif

}