#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Waypointer
{



    [CustomEditor(typeof(WaypointNetworkVisualizer))]
    public class WaypointNetworkVisualizerInspector : Editor
    {

        #region Public Fields

        [SerializeField]
        public VisualTreeAsset m_waypointNetworVisualizerkUXML;

        VisualElement instantiatedWayPointNetworkVisualizerUXML;

        public bool debugMode = false;

        #endregion

        #region Private Fields



        #endregion

        #region Unity Methods

        private void OnEnable()
        {

            if (!CheckPreferences()) return;

            instantiatedWayPointNetworkVisualizerUXML = m_waypointNetworVisualizerkUXML.CloneTree();

            var options = DrawDefaultInspectorButUseUIToolkit();

            instantiatedWayPointNetworkVisualizerUXML.Q<VisualElement>("OptionsContainer").Add(options);


        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();

            if (instantiatedWayPointNetworkVisualizerUXML != null)
                container.Add(instantiatedWayPointNetworkVisualizerUXML);

            return container;
        }

        #endregion

        #region Class Methods

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

        private bool CheckPreferences()
        {

            if (m_waypointNetworVisualizerkUXML == null)
            {
                dbg("no m_waypointNetworVisualizerkUXML", true);
                return false;
            }
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