using BehaviourAPI.Core;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class DepartureLocation: MonoBehaviour
{
    [Header("Configuration of Spawn")]
    [SerializeField]
    [Tooltip("Minimum distance to player")]
    private float minDistance = 10f;
    [SerializeField]
    [Tooltip("Maximum distance to player")]
    private float maxDistance = 50f;
    [SerializeField]
    [Tooltip("List of posible locations")]
    private List<Transform> spawnPoints = new List<Transform>();
    [SerializeField]
    [Tooltip("Prefab of renderObject")]
    private GameObject renderPrefab;

    [Header("Materials")]
    [SerializeField] private Material appearDisapearMaterial;
    [SerializeField] private Material standardMaterial;

    [Header("Animation Settings")]
    [SerializeField] private float appearHeightOffset = -1.5f;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Timer Settings")]
    [SerializeField]
    [Tooltip("Cooldown time before allowing next spawn")]
    private float cooldownTime = 10f;

    private Vector3 playerPosition;
    private float timer = 0f;
    private bool timerRunning = false;

    private bool actualDisappear = true;
    private NavMeshAgent agent;

    private bool appearing = false;
    private bool appear = false;
    private Vector3 appearStartPos;
    private Vector3 appearEndPos;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                timerRunning = false;
                timer = 0f;
            }
        }
    }

    public void DepartureLocationStarted()
    {
        appearing = false;
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        Vector3 departurePosition = Vector3.zero;

        if (!appear){
            // Reinicio timer
            timer = cooldownTime;
            timerRunning = true;

            departurePosition = CalculatePositionToExit();
        }

        agent.enabled = false;


        StartAppearDisappearSequence(departurePosition);
    }

    public Status DepartureLocationUpdate()
    {
        if (playerPosition == null || spawnPoints.Count == 0) return Status.Failure;
        
        if(!appearing) return Status.Running;

        Transform model = transform;
        model.position = Vector3.MoveTowards(
            model.position,
            appearEndPos,
            moveSpeed * Time.deltaTime
        );

        if (!appear)
        {
            if (Vector3.Distance(model.position, appearEndPos) <= 0.05f)
            {
                model.position = appearEndPos;

                SetMaterial(standardMaterial);

                agent.enabled = true;
                appear = true;
                actualDisappear = false;
            }


            if (appear)
                return Status.Success;

            return Status.Running;
        }
        else
        {
            if (Vector3.Distance(model.position, appearEndPos) <= 0.05f)
            {
                model.position = appearEndPos;

                SetMaterial(standardMaterial);

                renderPrefab.SetActive(false);
                appear = false;
                actualDisappear = true;
            }


            if (!appear)
                return Status.Success;

            return Status.Running;
        }
        
    }

    #region Actions
    public Vector3 CalculatePositionToExit()
    {
        // Lista temporal con las posiciones validas segun distancia
        List<Transform> validPoints = new List<Transform>();
        while (validPoints.Count == 0)
        {
            foreach (Transform point in spawnPoints)
            {
                float dist = Vector3.Distance(point.position, playerPosition);
                if (dist >= minDistance && dist <= maxDistance)
                {
                    validPoints.Add(point);
                }
            }
            if (validPoints.Count == 0)
            {
                maxDistance += 50;
            }
        }

        Transform chosenPoint = validPoints[Random.Range(0, validPoints.Count)];

        // Poner Posiciones de ruta
        PathingNinja pathing = GetComponent<PathingNinja>();
        pathing.patrolPoints.Clear();

        for (int i = 0; i < chosenPoint.childCount; i++)
        {
            pathing.patrolPoints.Add(chosenPoint.GetChild(i));
        }

        return new Vector3(chosenPoint.position.x, chosenPoint.position.y, chosenPoint.position.z);
    }

    private void StartAppearDisappearSequence(Vector3 targetPos)
    {
        if(!appear)
        {
            appearStartPos = new Vector3(targetPos.x, targetPos.y + appearHeightOffset, targetPos.z);
            appearEndPos = targetPos;

            renderPrefab.SetActive(true);
            SetMaterial(appearDisapearMaterial);

            transform.position = appearStartPos;
        }
        else
        {
            appearStartPos = transform.position;
            appearEndPos = new Vector3(appearStartPos.x, appearStartPos.y + appearHeightOffset, appearStartPos.z); ;

            SetMaterial(appearDisapearMaterial);
        }
        appearing = true;
    }

    #endregion

    #region Perceptions

    public bool CheckActualDisappear()
    {
        return !actualDisappear;
    }

    public bool FinishTimer()
    {
        if (!timerRunning)
        {
            return true;
        }
        return false;
    }
    #endregion

    private void SetMaterial(Material mat)
    {
       Transform secondChild = renderPrefab.transform.GetChild(1);
        SkinnedMeshRenderer r = secondChild.GetComponent<SkinnedMeshRenderer>();
        if (r != null)
            r.material = mat;
    }
}
