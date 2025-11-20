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

    private Coroutine appearCoroutine;
    private Coroutine disappearCoroutine;
    private bool actualDisappear = true;
    private NavMeshAgent agent;

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

    #region Actions
    public Status CalculatePositionToExit()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (playerPosition == null || spawnPoints.Count == 0) return Status.Failure;

        // Reinicio timer
        timer = cooldownTime;
        timerRunning = true;


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

        Vector3 departurePosition = new Vector3(chosenPoint.position.x, chosenPoint.position.y, chosenPoint.position.z);
        Debug.Log(departurePosition);

        agent.enabled = false;
        // Animations
        if (appearCoroutine == null)
        {
            appearCoroutine = StartCoroutine(AppearSequence(departurePosition));
            return Status.Success;
        }
        return Status.Success; 
    }

    private IEnumerator AppearSequence(Vector3 targetPos)
    {
        Vector3 startPos = new Vector3(targetPos.x, targetPos.y + appearHeightOffset, targetPos.z);
        Vector3 endPos = targetPos;

        renderPrefab.SetActive(true);
        SetMaterial(appearDisapearMaterial);

        Transform model = GetComponent<Transform>();
        model.position = startPos;

        while(Vector3.Distance(model.position, endPos) > 0.05f)
        {
            model.position = Vector3.MoveTowards(model.position, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        model.position = endPos;
        SetMaterial(standardMaterial);
        appearCoroutine = null;
        actualDisappear = false;
        agent.enabled = true;
    }

    public Status SetInvisible()
    {
        if (disappearCoroutine != null) StopCoroutine(disappearCoroutine);
        agent.enabled = false;
        disappearCoroutine = StartCoroutine(DisappearSequence());

        return Status.Success;
    }

    private IEnumerator DisappearSequence()
    {
        SetMaterial(appearDisapearMaterial);

        Transform model = GetComponent<Transform>();
        Vector3 startPos = model.position;
        Vector3 endPos = new Vector3(startPos.x, startPos.y + appearHeightOffset, startPos.z);

        while(Vector3.Distance(model.position, endPos) > 0.05f)
        {
            model.position = Vector3.MoveTowards(model.position, endPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        renderPrefab.SetActive(false);
        disappearCoroutine = null;
        actualDisappear = true;
    }

    #endregion

    #region Perceptions
    public bool CheckAppear()
    {
        return appearCoroutine == null;
    }

    public bool CheckDisappear()
    {
        return disappearCoroutine == null;
    }

    public bool CheckInvisible()
    {
        return !renderPrefab.activeSelf;
    }

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
