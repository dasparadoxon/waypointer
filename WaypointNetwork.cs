using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Waypointer
{



    [ExecuteAlways]
    public class WaypointNetwork : MonoBehaviour
    {

        #region Public Fields

        public List<Waypoint> waypointsOfThisNetwork = null;

        public GameObject prefabNewWaypoint;

        public bool debugMode = false;

        #endregion

        #region Private Fields



        #endregion

        #region Unity Methods

        void Start()
        {

            if (!CheckPreferences()) return;
        }


        void Update()
        {

        }

        void OnTransformChildrenChanged()
        {


            dbg("Children have changed, rescanning.");

            waypointsOfThisNetwork.Clear();

            foreach (Transform g in transform.GetComponentsInChildren<Transform>())
            {
                Waypoint wp = g.GetComponent<Waypoint>();

                if (wp != null)
                    waypointsOfThisNetwork.Add(wp);
            }

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
}