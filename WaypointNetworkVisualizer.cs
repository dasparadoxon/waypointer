#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Waypointer
{



    public class WaypointNetworkVisualizer : MonoBehaviour
    {

        #region Public Fields

        public float distanceOfDirectionArrowsIfBothWays = 0.45f;




        public bool visible = true;

        List<Waypoint> allWaypoints;

        public bool debugMode = false;

        #endregion

        #region Private Fields



        #endregion

        #region Unity Methods

        public void OnDrawGizmos()
        {
            if (visible)
            {

                allWaypoints = GetAllWaypointsInThisGroup();

                dbg($"Found {allWaypoints.Count} Waypoints in Children of {this.name}.");

                foreach (var waypoint in allWaypoints)
                {

                    if (Selection.activeTransform != waypoint.transform)
                    {


                        GameObject waypointGameObject = waypoint.gameObject;

                        dbg($"Waypoint {waypointGameObject.name} has {waypoint.connections.Count} Waypoints.");

                        foreach (Waypoint w in waypoint.connections)
                        {

                            if (w == null) break;
                            if (waypointGameObject == null) break;


                            UnityEngine.Vector3 m_direction = (w.gameObject.transform.position - waypointGameObject.transform.position).normalized;

                            float angle = SignedAngleBetween(m_direction, Vector3.forward, Vector3.up);

                            bool anglePositv = angle > 0 ? true : false;

                            //dbg($"Angle between Direction and Up : {angle.ToString()}, angleBook : {anglePositv}");

                            float m_distance = UnityEngine.Vector3.Distance(w.gameObject.transform.position, waypointGameObject.transform.position);

                            UnityEngine.Vector3 arrowheadposition = waypoint.transform.position + (m_direction * (m_distance * 0.85f));

                            // one way connection from this waypoint to the connected waypoint
                            // draw single line only
                            if (!w.connections.Contains(waypoint))
                            {
                                //Draw.Arrowhead(arrowheadposition, m_direction, 0.5f);

                                //Draw.DashedLine(waypoint.transform.position, arrowheadposition, 1, 0.3f);

                                Gizmos.color = Color.blue;
                                //Gizmos.DrawLine(transform.position, arrowheadposition);

                            }

                            else
                            {
                                // orthogonalen vector

                                var ortho = Vector3.Cross(m_direction, Vector3.up) * distanceOfDirectionArrowsIfBothWays;

                                Vector3 movedPosition;

                                Vector3 movedarrowheadposition;

                                movedPosition = waypoint.transform.position + (ortho);

                                movedarrowheadposition = arrowheadposition + (ortho);


                                //Draw.Arrowhead(movedarrowheadposition, m_direction, 0.4f);

                                //Draw.DashedLine(movedPosition, movedarrowheadposition, 1, 0.2f);


                                Gizmos.color = Color.blue;
                                //Gizmos.DrawLine(movedPosition, movedarrowheadposition);

                            }

                        }



                    }

                }
            }
        }

        private List<Waypoint> GetAllWaypointsInThisGroup()
        {
            Waypoint[] waypoints;

            waypoints = this.GetComponentsInChildren<Waypoint>();

            List<Waypoint> waypointslist = new List<Waypoint>();

            foreach (var waypoint in waypoints)
            {
                waypointslist.Add(waypoint);
            }

            return waypointslist;

        }

        #endregion

        #region Class Methods

        float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
        {
            // angle in [0,180]
            float angle = Vector3.Angle(a, b);
            float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

            // angle in [-179,180]
            float signed_angle = angle * sign;

            // angle in [0,360] (not used but included here for completeness)
            //float angle360 =  (signed_angle + 180) % 360;

            return signed_angle;
        }

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