using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.UtilitySystems;
using System.Numerics;

/*
 * Cambiar m_LaunchFire
 */

public class cp_GomiNinja : BehaviourRunner
{
    LevelWizardController m_levelWizardController;
	FollowPlayer m_FollowPlayer;
    Patrol m_Patrol;
    Investigation m_Investigation;
    TrapSpawner m_TrapSpawner;
    LaunchFire m_LaunchFire;
    TeleportBehindPlayer m_TeleportBehindPlayer;
    DepartureLocation m_DepartureLocation;
    NoiseListener m_NoiseListener;
    DetectPlayer m_DetectPlayer;
	Attack m_Attack;
    Break m_Break;
    StatsGomiNinja m_StatsGomiNinja;

	private PushPerception EstadoNormalToBuffeadoPorMago_Push;
	private PushPerception BuffeadoPorMagoToEstadoNormal_Push;

    protected override void Init()
	{
        m_levelWizardController = FindAnyObjectByType<LevelWizardController>();

        m_DepartureLocation = GetComponent<DepartureLocation>();
		m_FollowPlayer = GetComponent<FollowPlayer>();
        m_Patrol = GetComponent<Patrol>();
        m_Investigation = GetComponent<Investigation>();
        m_TrapSpawner = GetComponent<TrapSpawner>();
        m_LaunchFire = GetComponent<LaunchFire>();
        m_TeleportBehindPlayer = GetComponent<TeleportBehindPlayer>();
        m_NoiseListener = GetComponent<NoiseListener>();
        m_DetectPlayer = GetComponent<DetectPlayer>();
		m_Attack = GetComponent<Attack>();
        m_Break = GetComponent<Break>();
        m_StatsGomiNinja = GetComponent<StatsGomiNinja>();

        base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		
		BehaviourTree BuffeadoPorMagoBT = new BehaviourTree();
		BehaviourTree EstadoNormalBT = new BehaviourTree();
        UtilitySystem usAcciones = new UtilitySystem();

        // Perceptions
        ConditionPerception lightNoisePerception = new ConditionPerception(m_NoiseListener.LightNoise);
        ConditionPerception highNoisePerception = new ConditionPerception(m_NoiseListener.HighNoise);
        ConditionPerception isPlayerDetectedPerception = new ConditionPerception(m_DetectPlayer.IsPlayerDetected);
        ConditionPerception finishTimerPerception = new ConditionPerception(m_DepartureLocation.FinishTimer);
        ConditionPerception checkActualDisappearPerception = new ConditionPerception(m_DepartureLocation.CheckActualDisappear);

		ConditionPerception isPlayerWithinDistancePerception = new ConditionPerception(m_FollowPlayer.IsPlayerWithinDistance);


        // MainFSM

        FSM MainFSM = new FSM();
        SubsystemAction EstadoNormalBT_action = new SubsystemAction(EstadoNormalBT);
		State EstadoNormal = MainFSM.CreateState(EstadoNormalBT_action);
		
		SubsystemAction BuffeadoPorMago_action = new SubsystemAction(BuffeadoPorMagoBT);
		State EstadoBuffeadoPorMago = MainFSM.CreateState(BuffeadoPorMago_action);

        List<BehaviourAPI.Core.Actions.Action> subActions = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_LaunchFire.pruebaFinish, null)
        };

        ParallelAction sBuffEnemyPA = new ParallelAction(true, false, subActions);

        StateTransition SpellMagoTransition = MainFSM.CreateTransition(EstadoNormal, EstadoBuffeadoPorMago, null, sBuffEnemyPA, StatusFlags.None);

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_FollowPlayer.StopFollow),
            new FunctionalAction(m_LaunchFire.pruebaFinish, null)
        };

        ParallelAction sRestoreEnemyToNormalPA = new ParallelAction(true, false, subActions2);

        StateTransition DisappearMagoTransition = MainFSM.CreateTransition(EstadoBuffeadoPorMago, EstadoNormal, null, sRestoreEnemyToNormalPA, StatusFlags.None);

        EstadoNormalToBuffeadoPorMago_Push = new PushPerception(SpellMagoTransition);
        BuffeadoPorMagoToEstadoNormal_Push = new PushPerception(DisappearMagoTransition);

        // BuffeadoPorMago BT

        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_FollowPlayer.StopFollow),
            new FunctionalAction(m_Attack.AttackP, null)
        };

        SequenceAction sAttackSA = new SequenceAction(Status.Running, subActions3);

        LeafNode AtraparAlJugador = BuffeadoPorMagoBT.CreateLeafNode(sAttackSA);
		
		ConditionNode SuficienteCercaDelJugador = BuffeadoPorMagoBT.CreateDecorator<ConditionNode>(AtraparAlJugador);
		SuficienteCercaDelJugador.SetPerception(isPlayerWithinDistancePerception);
		
		
		FunctionalAction Perseguir_action = new FunctionalAction();
		Perseguir_action.onUpdated = m_FollowPlayer.StartFollow;
		LeafNode PerseguirAlJugador = BuffeadoPorMagoBT.CreateLeafNode(Perseguir_action);
		
		SelectorNode BPM_Selector_1 = BuffeadoPorMagoBT.CreateComposite<SelectorNode>(false, SuficienteCercaDelJugador, PerseguirAlJugador);
		BPM_Selector_1.IsRandomized = false;
		
		LoopNode Main_Loop = BuffeadoPorMagoBT.CreateDecorator<LoopNode>(BPM_Selector_1);
		Main_Loop.Iterations = -1;
		BuffeadoPorMagoBT.SetRootNode(Main_Loop);

        // EstadoNormal BT

        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_LaunchFire.AttackStarted, m_LaunchFire.AttackUpdate, null)
        };

        SequenceAction sLaunchSA = new SequenceAction(Status.Running, subActions4);
        LeafNode LanzamientoProyectil = EstadoNormalBT.CreateLeafNode(sLaunchSA);
		
		ConditionNode JugadorDetectado = EstadoNormalBT.CreateDecorator<ConditionNode>(LanzamientoProyectil);
		JugadorDetectado.SetPerception(isPlayerDetectedPerception);


        List<BehaviourAPI.Core.Actions.Action> subActions5 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigateSA = new SequenceAction(Status.Running, subActions5);

        LeafNode Investigar = EstadoNormalBT.CreateLeafNode(sInvestigateSA);
		
		SelectorNode EN_Selector_3 = EstadoNormalBT.CreateComposite<SelectorNode>(false, JugadorDetectado, Investigar);
		EN_Selector_3.IsRandomized = false;
		
		ConditionNode VeAlgoORuidoFlojo = EstadoNormalBT.CreateDecorator<ConditionNode>(EN_Selector_3);
		VeAlgoORuidoFlojo.SetPerception(lightNoisePerception);


		List<BehaviourAPI.Core.Actions.Action> subActions6 = new List<BehaviourAPI.Core.Actions.Action>(3)
		{
			new FunctionalAction(m_Patrol.StopPatrol),
			new FunctionalAction(m_Investigation.StopInvestigation),
			new FunctionalAction(m_TeleportBehindPlayer.TeleportEnemyBehindPlayer, null)
		};

        SequenceAction sTeleportSA = new SequenceAction(Status.Running, subActions6);

        LeafNode TeletransportarseAlJugador = EstadoNormalBT.CreateLeafNode(sTeleportSA);
		
		ConditionNode RuidoFuerte = EstadoNormalBT.CreateDecorator<ConditionNode>(TeletransportarseAlJugador);
		RuidoFuerte.SetPerception(highNoisePerception);	

		
		SelectorNode EN_Selector_2 = EstadoNormalBT.CreateComposite<SelectorNode>(false, VeAlgoORuidoFlojo, RuidoFuerte);
		EN_Selector_2.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions7 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_DepartureLocation.DepartureLocationStarted, m_DepartureLocation.DepartureLocationUpdate, null)
        };

        SequenceAction sDisappearSA = new SequenceAction(Status.Running, subActions7);

        LeafNode DesaparecerDelEscenario = EstadoNormalBT.CreateLeafNode(sDisappearSA);
		
		ConditionNode AcabaDeDesaparecer = EstadoNormalBT.CreateDecorator<ConditionNode>(DesaparecerDelEscenario);
		AcabaDeDesaparecer.SetPerception(checkActualDisappearPerception);


        List<BehaviourAPI.Core.Actions.Action> subActions8 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_DepartureLocation.DepartureLocationStarted, m_DepartureLocation.DepartureLocationUpdate, null)
        };

        SequenceAction sDepartureSA = new SequenceAction(Status.Running, subActions8);

        LeafNode AparicionEnElEscenario = EstadoNormalBT.CreateLeafNode(sDepartureSA);
		
		SelectorNode EN_Selector_5 = EstadoNormalBT.CreateComposite<SelectorNode>(false, AcabaDeDesaparecer, AparicionEnElEscenario);
		EN_Selector_5.IsRandomized = false;
		
		ConditionNode PasoMuchoTiempoEnUnaZona = EstadoNormalBT.CreateDecorator<ConditionNode>(EN_Selector_5);
		PasoMuchoTiempoEnUnaZona.SetPerception(finishTimerPerception);

        SubsystemAction US_Acciones = new SubsystemAction(usAcciones);
        LeafNode AccionesRutinarias = EstadoNormalBT.CreateLeafNode(US_Acciones);

		
		SelectorNode EN_Selector_4 = EstadoNormalBT.CreateComposite<SelectorNode>(false, PasoMuchoTiempoEnUnaZona, AccionesRutinarias);
		EN_Selector_4.IsRandomized = false;
		
		SelectorNode EN_Selector_1 = EstadoNormalBT.CreateComposite<SelectorNode>(false, EN_Selector_2, EN_Selector_4);
		EN_Selector_1.IsRandomized = false;

        LoopNode Main_Loop_1 = EstadoNormalBT.CreateDecorator<LoopNode>(EN_Selector_1);
		Main_Loop_1.Iterations = -1;
		EstadoNormalBT.SetRootNode(Main_Loop_1);
		
        // Acciones Rutinarias US

        VariableFactor staminaFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetStamina, 0f, 100f);

        VariableFactor tiempoPatrullandoFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetTimePatrol, 0f, 100f);

        VariableFactor agotamientoFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetTakeABreak, 0f, 1f);

        WeightedFusionFactor GanasDePatrullarWeightedFusion = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, agotamientoFactor);
        GanasDePatrullarWeightedFusion.Weights = new float[] { 0.4f, 0.6f };

        SigmoidCurveFactor CurvaPatrulla = usAcciones.CreateCurve<SigmoidCurveFactor>(GanasDePatrullarWeightedFusion);
        CurvaPatrulla.GrownRate = 8f;
        CurvaPatrulla.Midpoint = 0.6f;

        SigmoidCurveFactor CurvaDeDescanso = usAcciones.CreateCurve<SigmoidCurveFactor>(staminaFactor);
        CurvaDeDescanso.GrownRate = -10f;
        CurvaDeDescanso.Midpoint = 0.6f;

        WeightedFusionFactor GanasDePonerUnaTrampaWeightedFusion = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, tiempoPatrullandoFactor);
        GanasDePonerUnaTrampaWeightedFusion.Weights = new float[] { 0.5f, 0.5f };

        SigmoidCurveFactor CurvaDeTrampa = usAcciones.CreateCurve<SigmoidCurveFactor>(GanasDePonerUnaTrampaWeightedFusion);
        CurvaDeTrampa.GrownRate = 20;
        CurvaDeTrampa.Midpoint = 0.6f;

        List<BehaviourAPI.Core.Actions.Action> subActions10 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Patrol.StartPatrol, null)
        };

        SequenceAction sPatrolSA = new SequenceAction(Status.Running, subActions10);
        UtilityAction Patrullar = usAcciones.CreateAction(CurvaPatrulla, sPatrolSA, true);


        List<BehaviourAPI.Core.Actions.Action> subActions11 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Break.TakeABreakStarted, m_Break.TakeABreakUpdate, null)
        };

        SequenceAction sBreakSA = new SequenceAction(Status.Running, subActions11);
        UtilityAction TomarUnDescanso = usAcciones.CreateAction(CurvaDeDescanso, sBreakSA, true);

        List<BehaviourAPI.Core.Actions.Action> subActions9 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_TrapSpawner.TrapSpawnerStarted, m_TrapSpawner.TrapSpawnerUpdate, null)
        };

        SequenceAction sTrapSA = new SequenceAction(Status.Running, subActions9);
        UtilityAction ColocarTrampa = usAcciones.CreateAction(CurvaDeTrampa, sTrapSA, true);

        return MainFSM;
	}

	public void SetBTSeActivationPush()
	{
		EstadoNormalToBuffeadoPorMago_Push.Fire();
	}

	public void SetBTStActivationPush()
	{
		BuffeadoPorMagoToEstadoNormal_Push.Fire();
	}
}
