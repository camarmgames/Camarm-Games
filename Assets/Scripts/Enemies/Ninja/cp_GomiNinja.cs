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
 * Meter US
 */

public class cp_GomiNinja : BehaviourRunner
{
	GomiMagoAppearance m_GomiMagoAppearance;
	FollowPlayer m_FollowPlayer;
    PathingNinja m_PathingNinja;
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

	private PushPerception BTSe_ActivationPush;
	private PushPerception BTSt_ActivationPush;

    protected override void Init()
	{
		m_DepartureLocation = GetComponent<DepartureLocation>();
		m_GomiMagoAppearance = GetComponent<GomiMagoAppearance>();
		m_FollowPlayer = GetComponent<FollowPlayer>();
        m_PathingNinja = GetComponent<PathingNinja>();
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
		
		BehaviourTree BTSecondary_1 = new BehaviourTree();
		BehaviourTree BTStandar_1 = new BehaviourTree();
        UtilitySystem usAcciones = new UtilitySystem();

        // Perceptions
        ConditionPerception lightNoisePerception = new ConditionPerception(m_NoiseListener.LightNoise);
        ConditionPerception highNoisePerception = new ConditionPerception(m_NoiseListener.HighNoise);
        ConditionPerception isPlayerDetectedPerception = new ConditionPerception(m_DetectPlayer.IsPlayerDetected);
        ConditionPerception finishTimerPerception = new ConditionPerception(m_DepartureLocation.FinishTimer);
        ConditionPerception checkActualDisappearPerception = new ConditionPerception(m_DepartureLocation.CheckActualDisappear);

		ConditionPerception hasAppearedPerception = new ConditionPerception(m_GomiMagoAppearance.HasAppeared);
		ConditionPerception hasDisappearedPerception = new ConditionPerception(m_GomiMagoAppearance.HasDisappeared);
		ConditionPerception isPlayerWithinDistancePerception = new ConditionPerception(m_FollowPlayer.IsPlayerWithinDistance);


        // MainFSM
        FSM MainFSM = new FSM();
        SubsystemAction BTStandar_action = new SubsystemAction(BTStandar_1);
		State BTStandar = MainFSM.CreateState(BTStandar_action);
		
		SubsystemAction BTSecondary_action = new SubsystemAction(BTSecondary_1);
		State BTSecondary = MainFSM.CreateState(BTSecondary_action);

        List<BehaviourAPI.Core.Actions.Action> subActions = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_GomiMagoAppearance.BuffEnemy, null)
        };

        ParallelAction sBuffEnemy = new ParallelAction(true, false, subActions);

        StateTransition AppearGomiMago = MainFSM.CreateTransition(BTStandar, BTSecondary, null, sBuffEnemy, StatusFlags.None);

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_FollowPlayer.StopFollow),
            new FunctionalAction(m_GomiMagoAppearance.RestoreEnemyToNormal, null)
        };

        ParallelAction sRestoreEnemyToNormal = new ParallelAction(true, false, subActions2);

        StateTransition DisappearGomiMago = MainFSM.CreateTransition(BTSecondary, BTStandar, null, sRestoreEnemyToNormal, StatusFlags.None);

        BTSe_ActivationPush = new PushPerception(AppearGomiMago);
        BTSt_ActivationPush = new PushPerception(DisappearGomiMago);

        // BTSecondary

        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_FollowPlayer.StopFollow),
            new FunctionalAction(m_Attack.AttackP, null)
        };

        SequenceAction sAttack = new SequenceAction(Status.Running, subActions3);

        LeafNode Attack = BTSecondary_1.CreateLeafNode(sAttack);
		
		ConditionNode Near = BTSecondary_1.CreateDecorator<ConditionNode>(Attack);
		Near.SetPerception(isPlayerWithinDistancePerception);
		
		
		FunctionalAction Perseguir_action = new FunctionalAction();
		Perseguir_action.onUpdated = m_FollowPlayer.StartFollow;
		LeafNode Perseguir = BTSecondary_1.CreateLeafNode(Perseguir_action);
		
		SelectorNode BTSe_Selector_1 = BTSecondary_1.CreateComposite<SelectorNode>(false, Near, Perseguir);
		BTSe_Selector_1.IsRandomized = false;
		
		LoopNode Main_Loop = BTSecondary_1.CreateDecorator<LoopNode>(BTSe_Selector_1);
		Main_Loop.Iterations = -1;
		BTSecondary_1.SetRootNode(Main_Loop);

        // BTStandar
        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_LaunchFire.AttackStarted, m_LaunchFire.AttackUpdate, null)
        };

        SequenceAction sLaunch = new SequenceAction(Status.Running, subActions4);
        LeafNode Launch_candy = BTStandar_1.CreateLeafNode(sLaunch);
		
		ConditionNode Detect_Player = BTStandar_1.CreateDecorator<ConditionNode>(Launch_candy);
		Detect_Player.SetPerception(isPlayerDetectedPerception);


        List<BehaviourAPI.Core.Actions.Action> subActions5 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions5);

        LeafNode Investigate = BTStandar_1.CreateLeafNode(sInvestigate);
		
		SelectorNode BTSt_Selector_3 = BTStandar_1.CreateComposite<SelectorNode>(false, Detect_Player, Investigate);
		BTSt_Selector_3.IsRandomized = false;
		
		ConditionNode LightNoise = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_3);
		LightNoise.SetPerception(lightNoisePerception);


		List<BehaviourAPI.Core.Actions.Action> subActions6 = new List<BehaviourAPI.Core.Actions.Action>(3)
		{
			new FunctionalAction(m_PathingNinja.StopPatrol),
			new FunctionalAction(m_Investigation.StopInvestigation),
			new FunctionalAction(m_TeleportBehindPlayer.TeleportEnemyBehindPlayer, null)
		};

        SequenceAction sTeleport = new SequenceAction(Status.Running, subActions6);

        LeafNode Teleport = BTStandar_1.CreateLeafNode(sTeleport);
		
		ConditionNode HighNoise = BTStandar_1.CreateDecorator<ConditionNode>(Teleport);
		HighNoise.SetPerception(highNoisePerception);	

		
		SelectorNode BTSt_Selector_2 = BTStandar_1.CreateComposite<SelectorNode>(false, LightNoise, HighNoise);
		BTSt_Selector_2.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions7 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_DepartureLocation.DepartureLocationStarted, m_DepartureLocation.DepartureLocationUpdate, null)
        };

        SequenceAction sDisappear = new SequenceAction(Status.Running, subActions7);

        LeafNode Disappear = BTStandar_1.CreateLeafNode(sDisappear);
		
		ConditionNode CheckActualDisappear = BTStandar_1.CreateDecorator<ConditionNode>(Disappear);
		CheckActualDisappear.SetPerception(checkActualDisappearPerception);


        List<BehaviourAPI.Core.Actions.Action> subActions8 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_DepartureLocation.DepartureLocationStarted, m_DepartureLocation.DepartureLocationUpdate, null)
        };

        SequenceAction sDeparture = new SequenceAction(Status.Running, subActions8);

        LeafNode Departure_Location = BTStandar_1.CreateLeafNode(sDeparture);
		
		SelectorNode BTSt_Selector_5 = BTStandar_1.CreateComposite<SelectorNode>(false, CheckActualDisappear, Departure_Location);
		BTSt_Selector_5.IsRandomized = false;
		
		ConditionNode Time_Finish = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_5);
		Time_Finish.SetPerception(finishTimerPerception);

        SubsystemAction US_Acciones = new SubsystemAction(usAcciones);
        LeafNode US_Action = BTStandar_1.CreateLeafNode(US_Acciones);

		
		SelectorNode BTSt_Selector_4 = BTStandar_1.CreateComposite<SelectorNode>(false, Time_Finish, US_Action);
		BTSt_Selector_4.IsRandomized = false;
		
		SelectorNode BTSt_Selector_1 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Selector_2, BTSt_Selector_4);
		BTSt_Selector_1.IsRandomized = false;

        LoopNode Main_Loop_1 = BTStandar_1.CreateDecorator<LoopNode>(BTSt_Selector_1);
		Main_Loop_1.Iterations = -1;
		BTStandar_1.SetRootNode(Main_Loop_1);
		
        // US Acciones
        

        VariableFactor staminaFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetStamina, 0f, 100f);

        VariableFactor timePatrolFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetTimePatrol, 0f, 100f);

        VariableFactor takeABreakFactor = usAcciones.CreateVariable(m_StatsGomiNinja.GetTakeABreak, 0f, 1f);

        WeightedFusionFactor weightedFusionPatrol = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, takeABreakFactor);
        weightedFusionPatrol.Weights = new float[] { 0.4f, 0.6f };

        SigmoidCurveFactor PatrolCurve = usAcciones.CreateCurve<SigmoidCurveFactor>(weightedFusionPatrol);
        PatrolCurve.GrownRate = 8f;
        PatrolCurve.Midpoint = 0.6f;

        SigmoidCurveFactor BreakCurve = usAcciones.CreateCurve<SigmoidCurveFactor>(staminaFactor);
        BreakCurve.GrownRate = -10f;
        BreakCurve.Midpoint = 0.6f;

        WeightedFusionFactor weightedFusionTrap = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, timePatrolFactor);
        weightedFusionTrap.Weights = new float[] { 0.5f, 0.5f };

        SigmoidCurveFactor TrapCurve = usAcciones.CreateCurve<SigmoidCurveFactor>(weightedFusionTrap);
        TrapCurve.GrownRate = 20;
        TrapCurve.Midpoint = 0.6f;

        List<BehaviourAPI.Core.Actions.Action> subActions10 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_PathingNinja.StartPatrol, null)
        };

        SequenceAction sPatrol = new SequenceAction(Status.Running, subActions10);
        UtilityAction PatrolUtilityAction = usAcciones.CreateAction(PatrolCurve, sPatrol, true);


        List<BehaviourAPI.Core.Actions.Action> subActions11 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Break.TakeABreakStarted, m_Break.TakeABreakUpdate, null)
        };

        SequenceAction sBreak = new SequenceAction(Status.Running, subActions11);
        UtilityAction BreakUtilityAction = usAcciones.CreateAction(BreakCurve, sBreak, true);

        List<BehaviourAPI.Core.Actions.Action> subActions9 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_TrapSpawner.TrapSpawnerStarted, m_TrapSpawner.TrapSpawnerUpdate, null)
        };

        SequenceAction sTrap = new SequenceAction(Status.Running, subActions9);
        UtilityAction TrapUtilityAction = usAcciones.CreateAction(TrapCurve, sTrap, true);

        return MainFSM;
	}

	public void SetBTSeActivationPush()
	{
		BTSe_ActivationPush.Fire();
	}

	public void SetBTStActivationPush()
	{
		BTSt_ActivationPush.Fire();
	}
}
