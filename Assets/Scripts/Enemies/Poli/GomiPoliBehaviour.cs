using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using BehaviourAPI.StateMachines.StackFSMs;
using UnityEngine.AI;
using UnityEditor;
using UnityEngine.InputSystem;

public class GomiPoliBehaviour : BehaviourRunner
{
	//---About BehaviourGraph:
	private BehaviourGraph behaviourGraph;
	private PushPerception MageAppears;

	//--About Patrol Action:
	private NavMeshAgent navMeshAgent;
	private int index;
	public GameObject patrolPointsHolder;
	private List<Vector3> patrolPoints;
	public Animator animator;

	//--About Potenciacion Action:
	float time;

	//--About Footprint Detection Perception:
	public float viewRadius = 5.0f;
	public float playerViewRadius = 2.0f;
	public LayerMask footprintMask;
	public LayerMask obstacleMask;
	public float viewAngle = 90.0f;
	public int lastIDfootprint = -1;
	public GameObject seenFootprint;
	private Vector3 footprintNavMeshPoint;

    //--About Inspect Area Action:
    float inspectingTime;
    float timer;
    float angle;

	//--About Player Detection Perception:
	private GameObject player;

    //--About Blocking Path Action:
    [SerializeField] private List<Transform> pointsBlock;
    [SerializeField] private GameObject wallPrefab;

	//--About Attack Action:
    [SerializeField] private PlayerInput playerInput;




    #region -----------------BEHAVIOUR GRAPH-----------------
    protected override BehaviourGraph CreateGraph()
	{
		StackFSM GomiPoliFSM1 = new StackFSM();
		FSM GomiPoliFSM2 = new FSM();
        MageAppears = new PushPerception();

        SubsystemAction Nivel_2_action = new SubsystemAction(GomiPoliFSM2);
		State Nivel_2 = GomiPoliFSM1.CreateState(Nivel_2_action);

		FunctionalAction Potenciacion_accion = new FunctionalAction(
			PotenciacionStart,
			PotenciacionUpdate,
			PotenciacionStop
			);
		State Potenciacion = GomiPoliFSM1.CreateState(Potenciacion_accion);
		
		StateTransition MageAppearsTransition = GomiPoliFSM1.CreateTransition(Nivel_2, Potenciacion, statusFlags: StatusFlags.None);
		MageAppears.PushListeners.Add(MageAppearsTransition);

		StateTransition TerminaPotenciacion = GomiPoliFSM1.CreateTransition(Potenciacion, Nivel_2, statusFlags: StatusFlags.Finished);

		FunctionalAction Movimiento_accion = new FunctionalAction(
			PatrolStart,
			PatrolUpdate,
			PatrolStop
			);
		State Movimiento = GomiPoliFSM2.CreateState(Movimiento_accion);

		FunctionalAction perseguir_accion = new FunctionalAction(
			TrackerFootprintStart,
			TrackerFootprintUpdate,
			TrackerFootprintStop
			);
		State perseguir_rastro = GomiPoliFSM2.CreateState(perseguir_accion);

		ConditionPerception movimiento_inspeccionar = new ConditionPerception();
		movimiento_inspeccionar.onCheck = DetectFootprintsPerception;
		StateTransition DetectaRastro = GomiPoliFSM2.CreateTransition(Movimiento, perseguir_rastro, movimiento_inspeccionar, statusFlags: StatusFlags.Running);

        FunctionalAction inspeccionar_accion = new FunctionalAction(
			InspectAreaStart,
            InspectAreaUpdate
            );
        State inspeccionar_area = GomiPoliFSM2.CreateState(inspeccionar_accion);

		StateTransition perseguir_inspeccionar = GomiPoliFSM2.CreateTransition(perseguir_rastro, inspeccionar_area, statusFlags: StatusFlags.Success);
		FunctionalAction BloqueoCamino = new FunctionalAction(
			BlockPathStart,
			BlockPathUpdate,
			BlockPathStop
			);
		StateTransition vuelta_movimiento = GomiPoliFSM2.CreateTransition(perseguir_rastro, Movimiento, statusFlags: StatusFlags.Failure);

		StateTransition inspeccionar_perseguir = GomiPoliFSM2.CreateTransition(inspeccionar_area, perseguir_rastro, movimiento_inspeccionar, statusFlags: StatusFlags.Running);
		
		State bloquear_camino = GomiPoliFSM2.CreateState(BloqueoCamino);

		StateTransition inspeccionar_bloqueo = GomiPoliFSM2.CreateTransition(inspeccionar_area, bloquear_camino, statusFlags: StatusFlags.Finished);
        StateTransition bloqueo_movimiento = GomiPoliFSM2.CreateTransition(bloquear_camino, Movimiento, statusFlags: StatusFlags.Finished);

        FunctionalAction atacar_accion = new FunctionalAction(
			AttackStart,
			AttackUpdate
			);
        State Atacar = GomiPoliFSM2.CreateState(atacar_accion);

        ConditionPerception inspeccionar_movimiento = new ConditionPerception();
        inspeccionar_movimiento.onCheck = DetectPlayerPerception;
        StateTransition DetectarJugador = GomiPoliFSM2.CreateTransition(Movimiento, Atacar, perception: inspeccionar_movimiento, statusFlags: StatusFlags.Running);

		StateTransition Devolver = GomiPoliFSM2.CreateTransition(Atacar, Movimiento, statusFlags: StatusFlags.Finished);


		return GomiPoliFSM1;
	}

    #endregion

    #region -----------------UNITY FUNCTIONS-----------------
    public void Awake()
    {
        base.Init();
    }
    public void Start()
	{
        patrolPoints = new List<Vector3>();
        navMeshAgent = GetComponent<NavMeshAgent>();
		if (navMeshAgent == null)
			Debug.LogError("No NavMeshAgent found. Please add one.");
		index = 0;
		if (patrolPointsHolder == null)
			Debug.LogError("No holder of patrol points. Please add it to the inspector.");
		else if (patrolPointsHolder.transform.childCount == 0)
			Debug.LogError("No points to patrol in the holder. Please set up the patrol points as childs of holder.");
		else
		{
			for (int i = 0; i < patrolPointsHolder.transform.childCount; i++)
				patrolPoints.Add(patrolPointsHolder.transform.GetChild(i).position);
		}
		time = 0;
		if (animator == null)
			Debug.LogError("No animator found. Please assign one.");
		player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
			Debug.Log("No player found. Please check that.");
		base.OnStarted();
    }

    public void Update()
    {
		base.OnUpdated();
    }
    #endregion

    #region -----------------PUSH PERCEPTION FUNC.-----------------
    public void FireMageAppear()
	{
		MageAppears.Fire();
	}
    #endregion

    #region -----------------PERCEPTIONS-----------------

	public bool DetectFootprintsPerception()
	{
		Footprint fp;
		Collider[] footprints = Physics.OverlapSphere(transform.position, viewRadius, footprintMask);
		if (footprints.Length == 0)
            return false;
		foreach(Collider footprint in footprints)
		{
			fp = footprint.GetComponent<Footprint>();
			if (fp == null)
				continue;
            Vector3 toFP = (fp.transform.position - transform.position);
            Vector3 dir = toFP.normalized;
            float dist = toFP.magnitude;
            if (Vector3.Angle(transform.forward, dir) > viewAngle / 2f)
                continue;
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, dir, dist, obstacleMask))
                continue;
			if (fp.footprintID > lastIDfootprint && NavMesh.SamplePosition(fp.transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
			{
				footprintNavMeshPoint = hit.position;
				lastIDfootprint = fp.footprintID;
				seenFootprint = fp.gameObject;
				return true;
			}
        }
		return false;
	}

	public bool DetectPlayerPerception()
	{
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);

        if (Vector3.Angle(transform.forward, dirToPlayer) > viewAngle / 2 || distanceToPlayer > playerViewRadius
			|| Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToPlayer, distanceToPlayer, obstacleMask))
            return false;
		Debug.Log("Player seen TRUE");
        return true;
	}
    #endregion

    #region -----------------POTENCIACION ACTIONS-----------------

    //Esta función es el nodo potenciación, ¿activa una animación o algo?
    public void PotenciacionStart()
	{
        return;
	}
	//Esta funcion actualiza la potenciacion, debería esperar a que acabe una animación?
	public Status PotenciacionUpdate()
	{
		time += Time.deltaTime;
		if (time > 5.0f)
		{
			time = 0;
			return Status.Success; //La acción se completa.
		}
        return Status.Running; //Se sigue realizando el update.
	}

	public void PotenciacionStop()
	{
    }
    #endregion

    #region -----------------PATROL ACTIONS-----------------
    public void PatrolStart()
	{
        animator?.SetBool("isWalking", true);
        navMeshAgent.SetDestination(patrolPoints[index]);
	}

	public Status PatrolUpdate()
	{
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
		{
			index = (index + 1) % patrolPoints.Count;
            navMeshAgent.SetDestination(patrolPoints[index]);
        }
        return Status.Running;
	}

	public void PatrolStop()
	{
		navMeshAgent.ResetPath();
        animator?.SetBool("isWalking", false);
    }

	#endregion

	#region -----------------TRACKER FOOTPRINT ACTIONS-----------------

	private Vector3 lastPos;
	public void TrackerFootprintStart()
	{
        animator.SetBool("isWalking", true);
        navMeshAgent.SetDestination(footprintNavMeshPoint);
		lastPos = transform.position;
		stuckTimer = 0;
    }

	private float stuckTimer;
	public Status TrackerFootprintUpdate()
	{
		if (Vector3.Distance(transform.position, footprintNavMeshPoint) > 0.8)
		{
			if (Vector3.Distance(transform.position, lastPos) < 0.05f)
				stuckTimer += Time.deltaTime;
			else
			{
				stuckTimer = 0f;
				lastPos = transform.position;
			}
			if (stuckTimer > 3.0f)
			{
				Debug.LogWarning("Atascado, abortando seguimiento");
				return Status.Failure;
			}
		}
		else
			return Status.Success;
        return Status.Running;
	}

	public void TrackerFootprintStop()
	{
		navMeshAgent?.ResetPath();
		animator.SetBool("isWalking", false);
    }

    #endregion

    #region -----------------INSPECT AREA ACTIONS-----------------

    public void InspectAreaStart()
	{
        inspectingTime = 5f;
        timer = 0f;
        angle = transform.eulerAngles.y;
    }

    public Status InspectAreaUpdate()
	{

		if (timer < inspectingTime)
		{
			transform.rotation = Quaternion.Euler(0, angle, 0);
			angle += 60f * Time.deltaTime;
			timer += Time.deltaTime;
		}
		else
			return Status.Success;
        return Status.Running;
	}
	#endregion

	#region -----------------BLOCK PATH ACTIONS-----------------
	Transform closestBlockPoint = null;
    bool first = true;
    private Transform GetClosestBlockPoint()
	{
        Transform target = null;
        float minDist = Mathf.Infinity;

        foreach (var path in pointsBlock)
        {
            float dist = Vector3.Distance(transform.position, path.position);
            if ((dist < minDist))
            {
                minDist = dist;
                target = path;
            }
        }
		return (target);
    }

    private void BlockPathStart()
    {
        closestBlockPoint = GetClosestBlockPoint();
		navMeshAgent.SetDestination(closestBlockPoint.position);
        animator.SetBool("isWalking", true);
		first = true;
    }

    private Status BlockPathUpdate()
    {
        if (Vector3.Distance(transform.position, footprintNavMeshPoint) < 0.8)
		{
			if (first)
			{
				navMeshAgent?.ResetPath();
				animator.SetBool("isWalking", false);
				animator.SetBool("isBlocking", true);
				first = false;
			}
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName("Blocking"))
                return Status.Running;
            if (stateInfo.normalizedTime < 1f)
                return Status.Running;
            animator.SetBool("isBlocking", false);
            Debug.Log($"{name}: Bloqueando el camino...");

            float checkRadius = 0.5f; // Ajustable

            Collider[] colliders = Physics.OverlapSphere(closestBlockPoint.position, checkRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Wall")) // Asegúrate de que tu prefab tenga tag "Wall"
                {
                    Debug.Log($"{name}: Ya había un muro en {closestBlockPoint.name}, no se instancia otro.");
                    return Status.Success;
                }
            }

            Instantiate(wallPrefab, closestBlockPoint.position, closestBlockPoint.rotation);
            Debug.Log($"{name}: Camino boqueado en {closestBlockPoint.name}");
        }
		else
			return Status.Running;
        return Status.Success;
    }

	private void BlockPathStop()
	{
        navMeshAgent?.ResetPath();
        animator.SetBool("isWalking", false);
        animator.SetBool("isBlocking", false);
    }
    #endregion

    #region -----------------ATTACK ACTIONS-----------------
    private void AttackStart()
    {
    }
    private Status AttackUpdate()
	{
        animator.SetBool("isAttacking", true);

        playerInput.actions["Move"].Disable();

        EndGameManager.Instance.ShowLoseScreen();
        return Status.Success;
	}

    #endregion


}
