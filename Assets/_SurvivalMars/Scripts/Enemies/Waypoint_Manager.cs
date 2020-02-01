using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Manager : MonoBehaviour
{

    public static Waypoint_Manager Instance;

    private void Awake()
    {
        Instance = this;
    }


    [System.Serializable]
    public struct WaypointSections
    {
        public int m_waypointSection;
        public List<Waypoints> m_waypoints;
    }

    [System.Serializable]
    public struct RadiusSectors
    {
        public int m_radiusSector;
        public Waypoint_Radius m_radiusPoints;
    }

    public List<WaypointSections> m_waypointSectors;
    public List<RadiusSectors> m_radiusSectors;

    public Waypoints GetClosestWaypoint(int p_waypointSector, Vector3 p_position)
    {
        float closestDistance = Mathf.Infinity; 
        int closestPoint = 0;
        foreach(Waypoints way in m_waypointSectors[p_waypointSector].m_waypoints)
        {
            float currentDis = Vector3.Distance(new Vector3(p_position.x, 0, p_position.z), new Vector3(way.transform.position.x, 0, way.transform.position.z));
            if (currentDis > closestDistance)
            {
                closestDistance = currentDis;
                closestPoint = m_waypointSectors[p_waypointSector].m_waypoints.IndexOf(way);
            }
        }
        return m_waypointSectors[p_waypointSector].m_waypoints[closestPoint];
    }

    public Vector3 GiveNewPointInRadius(int p_radiusSector)
    {
        return m_radiusSectors[p_radiusSector].m_radiusPoints.GiveNewPosition();
    }
}
