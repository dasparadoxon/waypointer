using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Waypointer
{
    [ExecuteInEditMode]
    public class Waypoint : MonoBehaviour
    {

        #region Public Fields

        public bool debugMode = false;

        public Color colorForGizmoLine = Color.black;

        public float label3DNameSize = 1f;

        public List<Waypoint> connections = new List<Waypoint>();

        public float placementVariationRadius = 0f;

        [SerializeField]
        public bool exitWaypoint;

        [SerializeField]
        public bool entryWaypoint;

        /*SHOULD HAVE A WAY TO BE CONTROLLED BY WAYPOINTER NETWORK TODO*/
        private float distanceOfDirectionArrowsIfBothWays = 0.25f;

        public Texture2D IconEntryWaypoint;

        public Texture2D IconExitWaypoint;

        public Texture2D IconExitEntryWaypoint;

        public Texture2D IconDefaultWaypoint;

        public GameObject newWayPointPrefab;

        #endregion

        #region Private Fields



        #endregion

        #region Unity Methods

        void Start()
        {

            if (!CheckPreferences()) return;
        }

        private void Update()
        {





        }

        #endregion

        #region Class Methods

#if (UNITY_EDITOR)


        public void OnDrawGizmosSelected()
        {
            Handles.Label(this.transform.position + new UnityEngine.Vector3(0, 1, 0), this.name);

            foreach (var connectedWaypoint in connections)
            {
                if (connectedWaypoint != null)
                {

                    UnityEngine.Vector3 m_direction = (connectedWaypoint.transform.position - this.transform.position).normalized;

                    float angle = SignedAngleBetween(m_direction, Vector3.forward, Vector3.up);

                    bool anglePositv = angle > 0 ? true : false;

                    dbg($"Angle between Direction and Up : {angle.ToString()}, angleBook : {anglePositv}");

                    float m_distance = UnityEngine.Vector3.Distance(connectedWaypoint.transform.position, this.transform.position);

                    UnityEngine.Vector3 arrowheadposition = this.transform.position + (m_direction * (m_distance * 0.85f));

                    GameObject[] currentlySelectedObjectsInTheHierarchy = Selection.gameObjects;


                    dbg($"{currentlySelectedObjectsInTheHierarchy.Count()} Objects in Selection");

                    foreach (var item in currentlySelectedObjectsInTheHierarchy)
                    {
                        dbg($"Objekt selected : {item.name}");

                    }

                    if (connectedWaypoint.connections.Contains(this) && currentlySelectedObjectsInTheHierarchy.Contains(connectedWaypoint.gameObject) || IsWaypointNetworkParentSelected(currentlySelectedObjectsInTheHierarchy))
                    {
                        // orthogonalen vector

                        var ortho = Vector3.Cross(m_direction, Vector3.up) * distanceOfDirectionArrowsIfBothWays;

                        Vector3 movedPosition;

                        Vector3 movedarrowheadposition;

                        movedPosition = transform.position + (ortho);

                        movedarrowheadposition = arrowheadposition + (ortho);

                        DrawThickGizmoHandleLineDotted(movedPosition, movedarrowheadposition, 8, Color.black);

                        DrawArrowhead(movedarrowheadposition, m_direction);

                    }
                    else
                    {
                        DrawThickGizmoHandleLineDotted(transform.position, arrowheadposition, 8, Color.black);

                        DrawArrowhead(arrowheadposition, m_direction);
                    }
                }
            }

            if (placementVariationRadius > 0)
            {
                // https://discussions.unity.com/t/draw-2d-circle-with-gizmos/123578/2

                Handles.DrawWireDisc(transform.position + (transform.forward * placementVariationRadius) // position
                                          , Vector3.up                       // normal
                                          , placementVariationRadius);
            }
        }

        private bool IsWaypointNetworkParentSelected(GameObject[] currentHierarchySelection)
        {
            if (currentHierarchySelection.Contains(this.gameObject.transform.parent.gameObject))
                return true;

            return false;
        }

        private void DrawArrowhead(Vector3 position, Vector3 direction)
        {

            Gizmos.color = Color.red;

            Vector3 lineBack = new Vector3();

            lineBack = -direction * 0.35f;

            lineBack = Quaternion.AngleAxis(45, Vector3.up) * lineBack;



            DrawThickGizmoHandleLine(position, position + lineBack, 8, Color.black);

            Gizmos.color = Color.yellow;

            lineBack = new Vector3();

            lineBack = -direction * 0.35f;

            lineBack = Quaternion.AngleAxis(-45, Vector3.up) * lineBack;


            DrawThickGizmoHandleLine(position, position + lineBack, 8, Color.black);
        }


#endif
        public void OnDragExited()
        {

            dbg("On Drag Exit");
        }


        float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
        {
            // angle in [0,180]
            float angle = Vector3.Angle(a, b);
            float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

            // angle in [-179,180]
            float signed_angle = angle * sign;

            return signed_angle;
        }

        public void ClearAllEmptyOrNullWaypointsInConnections()
        {

            List<Waypoint> clearedList = new List<Waypoint>();

            foreach (var wp in connections)
            {
                if (wp != null)
                    clearedList.Add(wp);
            }

            connections = clearedList;

        }

        private bool CheckPreferences()
        {

            if (IconEntryWaypoint == null)
            {
                dbg("Missing Waypoint Item IconEntryWaypoint", true);
                return false;
            }


            if (IconExitEntryWaypoint == null)
            {
                dbg("Missing Waypoint Item IconExitEntryWaypoint", true);
                return false;
            }

            if (IconDefaultWaypoint == null)
            {
                dbg("Missing Waypoint Item IconDefaultWaypoint", true);
                return false;
            }

            if (IconExitWaypoint == null)
            {
                dbg("Missing Waypoint Item IconExitWaypoint", true);
                return false;
            }


            if (newWayPointPrefab == null)
            {
                dbg("Missing Prefab for New Waypoint", true);
                return false;
            }


            return true;



        }

        private void DrawThickGizmoHandleLine(Vector3 from, Vector3 to, int width, Color color)
        {
            var p1 = from;
            var p2 = to;
            var thickness = width;
            Handles.DrawBezier(p1, p2, p1, p2, color, null, thickness);
        }

        private void DrawThickGizmoHandleLineDotted(Vector3 from, Vector3 to, int width, Color color)
        {
            var p1 = from;
            var p2 = to;
            var thickness = width;

            // length

            float lengthOfLine = (p2 - p1).magnitude;

            // dot length + space

            float dotLength = 0.2f * 2;

            // how many segments ? 

            float numberOfSegments = lengthOfLine / dotLength;

            Vector3 direction = (p2 - p1).normalized;

            for (int i = 0; i < numberOfSegments; i++)
            {
                p1 = from + direction * dotLength * i;

                p2 = p1 + (direction * dotLength / 2);

                Handles.DrawBezier(p1, p2, p1, p2, color, null, thickness);
            }
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
