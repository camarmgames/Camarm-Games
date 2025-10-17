using BehaviourAPI.Core;
using System.Collections.Generic;
using UnityEngine;

public class Pathing: MonoBehaviour
{
    [Header("Configuration of pathing")]
    [Tooltip("Enemy speed")]
    public float speed;
    [Tooltip("Positions that have to follow")]
    public List<Transform> positions;
    [Tooltip("The distance the agent must be from one point to go to the next")]
    public float distanceThreshold;
    [Tooltip("The trap is going to put")]
    public GameObject trapPrefab;


    int currentTargetPosId = 0;

    private Vector3 target;

    bool isMoving;

    public void CancelMove()
    {
        target = Vector3.zero;
        isMoving = false;
    }

    public bool IsNotMoving()
    {
        return !isMoving;
    }

    public bool CanMove(Vector3 targetPos)
    {
        return true;
    }

    public Vector3 GetTarget()
    {
        return target;
    }

    public bool HasArrived()
    {
        return Vector3.Distance(transform.position, target) < distanceThreshold;
    }

    public void MoveInstant(Vector3 targetPos, Quaternion targetRot = default)
    {
        transform.SetLocalPositionAndRotation(targetPos, targetRot);
    }

    public void SetTarget(Vector3 targetPos)
    {
        target = targetPos;
        isMoving = true;
    }

    public void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);


            if (HasArrived())
            {
                currentTargetPosId++;

                if (currentTargetPosId >= positions.Count)
                {
                    currentTargetPosId = 0;
                    CancelMove();
                }
                else
                {
                    CancelMove();
                }
            }
        }
    }


    public void PathingMove()
    {
        if (positions.Count > 0)
        {
            SetTarget(positions[currentTargetPosId].position);
        }
    }

    public void PutTrap()
    {
        Vector3 trapPosition = new Vector3(GetComponent<Transform>().position.x, trapPrefab.GetComponent<Transform>().position.y, GetComponent<Transform>().position.z);
        Instantiate(trapPrefab, trapPosition, Quaternion.identity);
    }
    
}
