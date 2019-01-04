using UnityEngine;
using Pathfinding;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]

public class EnemyAIOld : MonoBehaviour
{

    public Transform target;

    //how many times per second we update our path
    public float updateRate = 2f;

    private Seeker seeker;
    private Rigidbody2D rb;



    //enemy speed
    public float speed = 300f;
    public ForceMode2D fMode;

    public Path path;

    [HideInInspector]
    public bool pathIsEnded = false;

    // Max distance from AI to a waypoint for it to go on to the next waypoint
    public float nextWayPointDistance = 3f;

    // waypoint we are currently moving towards
    private int currentWayPoint = 0;

    private bool searchingForPlayer = false;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        // start a new path to the target position, return the result to the OnPathComplete method
        //if (seeker == null)
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());

    }

    private IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield return false;
        }
        if (target != null)
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1f / updateRate);
        /*if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }*/
        StartCoroutine(UpdatePath());

    }

    private IEnumerator SearchForPlayer()
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            target = sResult.transform;
            searchingForPlayer = false;
            StartCoroutine(UpdatePath());
        }

        yield return false;
    }

    public void OnPathComplete(Path p)
    {

        if (!p.error)
        {
            path = p;
            currentWayPoint = 0;
        }
    }

    private void FixedUpdate()
    {

        if (target == null)
        {
            if (!searchingForPlayer)
            {
                searchingForPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        if (path == null) return;

        if (currentWayPoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;

            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        // direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWayPoint] - transform.position).normalized;

        dir = dir * speed * Time.deltaTime;

        //move the AI
        rb.AddForce(dir, fMode);

        float dist = Vector2.Distance(transform.position, path.vectorPath[currentWayPoint]);
        if (dist < nextWayPointDistance)
        {
            currentWayPoint++;
            return;
        }
    }

}
