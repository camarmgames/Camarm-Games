using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using BehaviourAPI.BehaviourTrees;

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


        base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		
		BehaviourTree BTSecondary_1 = new BehaviourTree();
		BehaviourTree BTStandar_1 = new BehaviourTree();

        // Perceptions
        ConditionPerception checkAppearPerception = new ConditionPerception(m_DepartureLocation.CheckAppear);
        ConditionPerception checkDisappearPerception = new ConditionPerception(m_DepartureLocation.CheckDisappear);
        ConditionPerception lightNoisePerception = new ConditionPerception(m_NoiseListener.LightNoise);
        ConditionPerception finishLaunchingPerception = new ConditionPerception(m_LaunchFire.FinishLaunching);
        ConditionPerception highNoisePerception = new ConditionPerception(m_NoiseListener.HighNoise);
        ConditionPerception isPlayerDetectedPerception = new ConditionPerception(m_DetectPlayer.IsPlayerDetected);
        ConditionPerception finishingPlacingTrapPerception = new ConditionPerception(m_TrapSpawner.FinishPlacingTrap);
        ConditionPerception finishTimerPerception = new ConditionPerception(m_DepartureLocation.FinishTimer);
        ConditionPerception checkActualDisappearPerception = new ConditionPerception(m_DepartureLocation.CheckActualDisappear);

		ConditionPerception checkNoInvisiblePerception = new ConditionPerception(m_DepartureLocation.CheckNoInvisible);
		ConditionPerception hasAppearedPerception = new ConditionPerception(m_GomiMagoAppearance.HasAppeared);
		ConditionPerception hasDisappearedPerception = new ConditionPerception(m_GomiMagoAppearance.HasDisappeared);
		ConditionPerception isPlayerWithinDistancePerception = new ConditionPerception(m_FollowPlayer.IsPlayerWithinDistance);

        AndPerception checkAppearDisappearAndPerception = new AndPerception(checkAppearPerception, checkDisappearPerception);
        AndPerception lightNoiseAndPerception = new AndPerception(lightNoisePerception, finishLaunchingPerception);
        AndPerception highNoiseAndPerception = new AndPerception(highNoisePerception, finishLaunchingPerception);
        AndPerception checkTrapFinishAndPerception = new AndPerception(finishingPlacingTrapPerception, finishLaunchingPerception);

        // MainFSM
        FSM MainFSM = new FSM();
        SubsystemAction BTStandar_action = new SubsystemAction(BTStandar_1);
		State BTStandar = MainFSM.CreateState(BTStandar_action);
		
		SubsystemAction BTSecondary_action = new SubsystemAction(BTSecondary_1);
		State BTSecondary = MainFSM.CreateState(BTSecondary_action);

        List<BehaviourAPI.Core.Actions.Action> subActions = new List<BehaviourAPI.Core.Actions.Action>(4)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_TrapSpawner.StopTrapCoroutine),
			new FunctionalAction(m_LaunchFire.StopLaunchCoroutine),
            new FunctionalAction(m_GomiMagoAppearance.BuffEnemy, null)
        };

        ParallelAction sBuffEnemy = new ParallelAction(true, false, subActions);

        StateTransition AppearGomiMago = MainFSM.CreateTransition(BTStandar, BTSecondary, null, sBuffEnemy, StatusFlags.None);

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(4)
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
		
		SequencerNode BTSe_Sequencer_1 = BTSecondary_1.CreateComposite<SequencerNode>(false, Near);
		BTSe_Sequencer_1.IsRandomized = false;
		
		FunctionalAction Perseguir_action = new FunctionalAction();
		Perseguir_action.onUpdated = m_FollowPlayer.StartFollow;
		LeafNode Perseguir = BTSecondary_1.CreateLeafNode(Perseguir_action);
		
		SelectorNode BTSe_Selector_1 = BTSecondary_1.CreateComposite<SelectorNode>(false, BTSe_Sequencer_1, Perseguir);
		BTSe_Selector_1.IsRandomized = false;
		
		LoopNode Main_Loop = BTSecondary_1.CreateDecorator<LoopNode>(BTSe_Selector_1);
		Main_Loop.Iterations = -1;
		BTSecondary_1.SetRootNode(Main_Loop);

        // BTStandar
        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(4)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_TrapSpawner.StopTrapCoroutine),
            new FunctionalAction(m_LaunchFire.Attack, null)
        };

        SequenceAction sLaunch = new SequenceAction(Status.Running, subActions4);
        LeafNode Launch_candy = BTStandar_1.CreateLeafNode(sLaunch);
		
		ConditionNode Detect_Player = BTStandar_1.CreateDecorator<ConditionNode>(Launch_candy);
		Detect_Player.SetPerception(isPlayerDetectedPerception);

		SequencerNode BTSt_Sequencer_5 = BTStandar_1.CreateComposite<SequencerNode>(false, Detect_Player);
		BTSt_Sequencer_5.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions5 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_TrapSpawner.StopTrapCoroutine),
            new FunctionalAction(m_Investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions5);

        LeafNode Investigate = BTStandar_1.CreateLeafNode(sInvestigate);
		
		SelectorNode BTSt_Selector_3 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Sequencer_5, Investigate);
		BTSt_Selector_3.IsRandomized = false;
		
		ConditionNode LightNoise = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_3);
		LightNoise.SetPerception(lightNoiseAndPerception);

		SequencerNode BTSt_Sequencer_2_1 = BTStandar_1.CreateComposite<SequencerNode>(false, LightNoise);
		BTSt_Sequencer_2_1.IsRandomized = false;

		List<BehaviourAPI.Core.Actions.Action> subActions6 = new List<BehaviourAPI.Core.Actions.Action>(4)
		{
			new FunctionalAction(m_PathingNinja.StopPatrol),
			new FunctionalAction(m_TrapSpawner.StopTrapCoroutine),
			new FunctionalAction(m_Investigation.StopInvestigation),
			new FunctionalAction(m_TeleportBehindPlayer.TeleportEnemyBehindPlayer, null)
		};

        SequenceAction sTeleport = new SequenceAction(Status.Running, subActions6);

        LeafNode Teleport = BTStandar_1.CreateLeafNode(sTeleport);
		
		ConditionNode HighNoise = BTStandar_1.CreateDecorator<ConditionNode>(Teleport);
		HighNoise.SetPerception(highNoiseAndPerception);	

		SequencerNode BTSt_Sequencer_2_2 = BTStandar_1.CreateComposite<SequencerNode>(false, HighNoise);
		BTSt_Sequencer_2_2.IsRandomized = false;
		
		SelectorNode BTSt_Selector_2 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Sequencer_2_1, BTSt_Sequencer_2_2);
		BTSt_Selector_2.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions7 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_LaunchFire.StopLaunchCoroutine),
			new FunctionalAction(m_TrapSpawner.StopTrapCoroutine),
            new FunctionalAction(m_DepartureLocation.SetInvisible, null)
        };

        SequenceAction sDisappear = new SequenceAction(Status.Running, subActions7);

        LeafNode Disappear = BTStandar_1.CreateLeafNode(sDisappear);
		
		ConditionNode CheckActualDisappear = BTStandar_1.CreateDecorator<ConditionNode>(Disappear);
		CheckActualDisappear.SetPerception(checkActualDisappearPerception);

		SequencerNode BTSt_Sequencer_4 = BTStandar_1.CreateComposite<SequencerNode>(false, CheckActualDisappear);
		BTSt_Sequencer_4.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions8 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_DepartureLocation.CalculatePositionToExit, null)
        };

        SequenceAction sDeparture = new SequenceAction(Status.Running, subActions8);

        LeafNode Departure_Location = BTStandar_1.CreateLeafNode(sDeparture);
		
		SelectorNode BTSt_Selector_5 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Sequencer_4, Departure_Location);
		BTSt_Selector_5.IsRandomized = false;
		
		ConditionNode Time_Finish = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_5);
		Time_Finish.SetPerception(finishTimerPerception);

		SequencerNode BTSt_Sequencer_3 = BTStandar_1.CreateComposite<SequencerNode>(false, Time_Finish);
		BTSt_Sequencer_3.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions9 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_TrapSpawner.PlaceRandomTrap, null)
        };

        SequenceAction sTrap = new SequenceAction(Status.Running, subActions9);

        LeafNode Trap_Candy = BTStandar_1.CreateLeafNode(sTrap);

        List<BehaviourAPI.Core.Actions.Action> subActions10 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_PathingNinja.StartPatrol, null)
        };

        SequenceAction sPatrol = new SequenceAction(Status.Running, subActions10);

        LeafNode Patrol = BTStandar_1.CreateLeafNode(sPatrol);
		
		ProbabilityBranchNode BTSt_Probability_Selector = BTStandar_1.CreateComposite<ProbabilityBranchNode>(false, Trap_Candy, Patrol);
		BTSt_Probability_Selector.probabilities = new List<float>() {0.02f, 0.98f};
		BTSt_Probability_Selector.IsRandomized = false;
		
		SelectorNode BTSt_Selector_4 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Sequencer_3, BTSt_Probability_Selector);
		BTSt_Selector_4.IsRandomized = false;
		
		ConditionNode CheckTrapFinish = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_4);
        CheckTrapFinish.SetPerception(checkAppearDisappearAndPerception);

        SequencerNode BTSt_Sequencer_2 = BTStandar_1.CreateComposite<SequencerNode>(false, CheckTrapFinish);
		BTSt_Sequencer_2.IsRandomized = false;
		
		SelectorNode BTSt_Selector_1 = BTStandar_1.CreateComposite<SelectorNode>(false, BTSt_Selector_2, BTSt_Sequencer_2);
		BTSt_Selector_1.IsRandomized = false;
		
		ConditionNode CheckAppearDisapear = BTStandar_1.CreateDecorator<ConditionNode>(BTSt_Selector_1);
        CheckAppearDisapear.SetPerception(checkAppearDisappearAndPerception);

        LoopNode Main_Loop_1 = BTStandar_1.CreateDecorator<LoopNode>(CheckAppearDisapear);
		Main_Loop_1.Iterations = -1;
		BTStandar_1.SetRootNode(Main_Loop_1);
		
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
