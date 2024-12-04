#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using Unity.VisualScripting;
//using Sirenix.OdinInspector;
using System.Linq;
using System.Net;

namespace Waypointer
{



    [CustomEditor(typeof(WaypointNetwork))]
    public class WaypointNetworkInspector : Editor
    {
        /*     public override void OnInspectorGUI() {
                base.OnInspectorGUI();

            } */

        [SerializeField]
        VisualTreeAsset m_waypointNetworkUXML;

        private Object lastSelectedObject;

        private Object activeObjectAtTheTimeOfTheButtonPressed;

        private bool buttonPressed = false;

        [SerializeField]
        private int newWaypointCounter = 0;

        VisualElement uxmlCustomINspector;

        private bool selectionOfPositionForNewWaypoint = false;

        private void OnEnable()
        {

            /*         // force dirty SceneView.s_ActiveEditors
                    // maybe this is a unity bug
                    var method = typeof(SceneView).GetMethod("SetActiveEditorsDirty", BindingFlags.NonPublic | BindingFlags.Static);
                    // ReSharper disable once PossibleNullReferenceException
                    method.Invoke(null, new object[] { true }); */

            uxmlCustomINspector = m_waypointNetworkUXML.CloneTree();

            Button button = uxmlCustomINspector.Query<Button>("CreateNewWaypointButton");

            button.clicked += () =>
            {
                AddNewWaypointButtonClicked();

                /*             foreach (SceneView scene in SceneView.sceneViews)
                                scene.ShowNotification(new GUIContent("Click anywhere in the scene to place a new waypoint there.")); */
            };

            uxmlCustomINspector.Q<Label>("WaypointNameLabel").text = ((WaypointNetwork)target).gameObject.name;


        }

        [MenuItem("GameObject/Waypointer/Add Waypoint Network",false,10)]
        static void AddNewWaypointNetwork(MenuCommand menuCommand)
        {
            Debug.Log("Doing Something...");
            //WaypointNetwork waypointNetwork = new WaypointNetwork();

            GameObject newWaypointNetworkGameObjectInHierarchy = new GameObject("New Waypoint Network");

            WaypointNetwork wpn = new WaypointNetwork();

            newWaypointNetworkGameObjectInHierarchy.AddComponent(typeof(WaypointNetwork));
 

            Selection.activeObject = newWaypointNetworkGameObjectInHierarchy;

        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            container.Add(uxmlCustomINspector);

            HideOrShowNoWaypointMessageAndListView(uxmlCustomINspector);

            container.Add(FoldOutOptions(uxmlCustomINspector, DrawDefaultInspectorButUseUIToolkit()));

            return container;
        }

        private VisualElement FoldOutOptions(VisualElement root, VisualElement content)
        {

            Foldout foldout = root.Q<Foldout>("FoldoutOptions");

            foldout.Add(content);

            foldout.value = false;

            return foldout;

        }

        private void HideOrShowNoWaypointMessageAndListView(VisualElement root)
        {

            Debug.Log(target.ToSafeString());

            VisualElement messageBox = root.Q<VisualElement>("NoWaypointsMessage");

            ListView listView = root.Q<ListView>("NetworkWaypointsListView");

            if(((WaypointNetwork)target).waypointsOfThisNetwork == null)
            Debug.Log("WTF");

            if (messageBox != null)
            {

                if (
                    ((WaypointNetwork)target).waypointsOfThisNetwork != null &&
                    ((WaypointNetwork)target).waypointsOfThisNetwork.Count() > 0
                    )



                {
                    messageBox.style.display = DisplayStyle.None;
                    listView.style.display = DisplayStyle.Flex;
                }

                else
                {
                    messageBox.style.display = DisplayStyle.Flex;
                    listView.style.display = DisplayStyle.None;
                }


            }

        }

        private void AddNewWaypointButtonClicked()
        {
            //Debug.Log("AddNewWaypointButtonClicked");

            selectionOfPositionForNewWaypoint = true;

        }

        void OnSceneGUI()
        {

            //Debug.Log("OnSceneGUI Mouse Click :" + selectionOfPositionForNewWaypoint);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {

                //Debug.Log("OnSceneGUI Mouse Click :" + selectionOfPositionForNewWaypoint);

                if (selectionOfPositionForNewWaypoint)
                {
                    //Debug.Log("Click on Scene to choose new waypoint position happend.");

                    Event.current.Use();

                    Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        CreateNewWaypoint(hit.point);
                    }

                    selectionOfPositionForNewWaypoint = false;

                    Repaint();

                    HideOrShowNoWaypointMessageAndListView(uxmlCustomINspector);
                }
            }
        }



        private void CreateNewWaypoint(Vector3 positionForNewWaypoint)
        {
            newWaypointCounter++;

            //Debug.Log("Trying to create a new waypoint (initial)");

            var wpn = (WaypointNetwork)target;

            if (wpn.prefabNewWaypoint == null)
            {
                Debug.LogError("You need to set a Waypoint Prefab.");
                return;
            }

            GameObject waypointPrefab = Instantiate(wpn.prefabNewWaypoint);

            waypointPrefab.name = $"Unnamed Waypoint {newWaypointCounter}";

            Waypoint newWP = waypointPrefab.GetComponent<Waypoint>();

            newWP.gameObject.transform.parent = wpn.transform;

            newWP.gameObject.transform.position = positionForNewWaypoint;

            newWP.gameObject.transform.position += new Vector3(0, 1.2f, 0);
        }

        private VisualElement DrawDefaultInspectorButUseUIToolkit()
        {
            var container = new VisualElement();

            var iterator = serializedObject.GetIterator();

            if (iterator.NextVisible(true))
            {
                do
                {
                    var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                    //Debug.Log($"PropertyField : {propertyField.name}");

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                        propertyField.SetEnabled(value: false);

                    var propertiesToIgnore = new string[] { "waypointsOfThisNetwork", "m_Script" };

                    if (!propertiesToIgnore.Contains(iterator.propertyPath))
                        container.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }

            return container;
        }
    }
#endif
}