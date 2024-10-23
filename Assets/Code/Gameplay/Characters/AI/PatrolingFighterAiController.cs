using System.Collections.Generic;
using UnityEngine;

namespace ComixArea.AI
{
    public class PatrolingFighterAiController : BotAiController
    {
        [field:
            SerializeField,
            Tooltip("A bot within this distance from a waypoint thinks that it reached the waypoint")
        ]
        public float StoppingDistance { get; private set; } = 0.5f;

        public IReadOnlyList<Vector3> Waypoints { get; private set; }
        public Vector3 CurrentWaypoint => Waypoints[_currentWayPoint];

        protected int _currentWayPoint = 0;

        public void SetWayPoints(Vector3[] wayPoints)
        {
            Waypoints = wayPoints;
        }

        public void SwitchToNextWaypoint()
        {
            _currentWayPoint++;
            if (_currentWayPoint >= Waypoints.Count)
            {
                _currentWayPoint = 0;
            }
        }

        public void SwitchToClosestWaypoint()
        {
            if (Waypoints == null || Waypoints.Count == 0)
            {
                return;
            }
            var botPos = transform.position;
            float minDistSqr = float.MaxValue;
            var closestWayPoint = 0;
            for (int i = 1; i < Waypoints.Count; i++)
            {
                var distSqr = (Waypoints[i] - botPos).sqrMagnitude;
                if (distSqr < minDistSqr)
                {
                    closestWayPoint = i;
                }
            }
            _currentWayPoint = closestWayPoint;
        }
    }
}
