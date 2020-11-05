using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public Transform pathHolder;
    public float speed =5;
    public float waitTime = .3f;
    public float turnSpeed = 90;

    public float viewDistance;
    public float viewAngle;
    Transform player;
    public LayerMask viewMask;
    public GameObject cylinder;

    public float timeToSpotPlayer = .5f;
    float playerVisibleTimer;
    public static event System.Action onGuardHasSpottedPlayer;
    bool disabled;

    public Transform eyes;

    private void Start()
    {
        player = cylinder.transform;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            playerVisibleTimer += Time.deltaTime;
        }
        else
        {
            playerVisibleTimer -= Time.deltaTime;
        }
        if (!disabled)
        {
            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);
            //GetComponent<Renderer>().material.color = Color.Lerp(Color.grey, Color.red, playerVisibleTimer / timeToSpotPlayer);
        }
        if (playerVisibleTimer >= timeToSpotPlayer)
        {
            Disable();
            if (onGuardHasSpottedPlayer != null)
                onGuardHasSpottedPlayer();
        }
    }

    void Disable()
    {
        disabled = true;
    }

    bool CanSeePlayer()
    {
        if (Vector3.Distance(eyes.position, player.position) < viewDistance)
        {
            Vector3 dirToPlayer = (player.position - eyes.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(eyes.forward, dirToPlayer);
            if (angleBetweenGuardAndPlayer < viewAngle / 2f)
            {
                if (!Physics.Linecast(eyes.position, player.position, viewMask))
                    return true;
            }
        }
        return false;
    }

    IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;
        while ((!disabled) && (Math.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f))
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    IEnumerator FollowPath(Vector3[] waypoints)
    {
        transform.position = waypoints[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = waypoints[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while(!disabled)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(TurnToFace(targetWaypoint));
            }
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(eyes.position, transform.forward * viewDistance);
    }
} 
