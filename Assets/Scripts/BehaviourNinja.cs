using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.BehaviourTrees;

public class BehaviourNinja : BehaviourRunner
{
	PathingNinja pathingNinja;
	Investigation investigation;
	TrapSpawner trapSpawner;
	LaunchFire launchFire;
	TeleportBehindPlayer teleportBehindPlayer;
	DepartureLocation departureLocation;
	NoiseListener noiseListener;
	DetectPlayer detectPlayer;


    protected override void Init()
	{
		

        pathingNinja = GetComponent<PathingNinja>();
        investigation = GetComponent<Investigation>();
        trapSpawner = GetComponent<TrapSpawner>();
        launchFire = GetComponent<LaunchFire>();
        teleportBehindPlayer = GetComponent<TeleportBehindPlayer>();
        departureLocation = GetComponent<DepartureLocation>();
        noiseListener = GetComponent<NoiseListener>();
        detectPlayer = GetComponent<DetectPlayer>();

        base.Init();
    }
    protected override BehaviourGraph CreateGraph()
	{
        BehaviourTree BTStandar = new BehaviourTree();
		if (departureLocation == null)
			Debug.Log("Algo fallo");
        ConditionPerception checkAppearPerception = new ConditionPerception(departureLocation.CheckAppear);
		ConditionPerception checkDisappearPerception = new ConditionPerception(departureLocation.CheckDisappear);
		ConditionPerception lightNoisePerception = new ConditionPerception(noiseListener.LightNoise);
		ConditionPerception finishLaunchingPerception = new ConditionPerception(launchFire.FinishLaunching);
		ConditionPerception highNoisePerception = new ConditionPerception(noiseListener.HighNoise);
		ConditionPerception isPlayerDetectedPerception = new ConditionPerception(detectPlayer.IsPlayerDetected);
		ConditionPerception finishingPlacingTrapPerception = new ConditionPerception(trapSpawner.FinishPlacingTrap);
		ConditionPerception finishTimerPerception = new ConditionPerception(departureLocation.FinishTimer);
		ConditionPerception checkActualDisappearPerception = new ConditionPerception(departureLocation.CheckActualDisappear);

		AndPerception checkAppearDisappearAndPerception = new AndPerception(checkAppearPerception, checkDisappearPerception);
		AndPerception lightNoiseAndPerception = new AndPerception(lightNoisePerception, finishLaunchingPerception);
		AndPerception highNoiseAndPerception = new AndPerception(highNoisePerception, finishLaunchingPerception);
		AndPerception checkTrapFinishAndPerception = new AndPerception(finishingPlacingTrapPerception, finishLaunchingPerception);

        List<BehaviourAPI.Core.Actions.Action> subActions = new List<BehaviourAPI.Core.Actions.Action>(4)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(trapSpawner.StopTrapCoroutine),
            new FunctionalAction(launchFire.Attack, null)
        };

		SequenceAction sLaunch = new SequenceAction(Status.Running, subActions);

		LeafNode Launch_candy = BTStandar.CreateLeafNode(sLaunch);
		
		ConditionNode Detect_Player = BTStandar.CreateDecorator<ConditionNode>(Launch_candy);
		Detect_Player.SetPerception(isPlayerDetectedPerception);
		
		SequencerNode Sequencer_5 = BTStandar.CreateComposite<SequencerNode>(false, Detect_Player);
		Sequencer_5.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(trapSpawner.StopTrapCoroutine),
            new FunctionalAction(investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions2);

        LeafNode Investigate = BTStandar.CreateLeafNode(sInvestigate);
		
		SelectorNode Selector_1 = BTStandar.CreateComposite<SelectorNode>(false, Sequencer_5, Investigate);
		Selector_1.IsRandomized = false;
		
		ConditionNode LightNoise = BTStandar.CreateDecorator<ConditionNode>(Selector_1);
		LightNoise.SetPerception(lightNoiseAndPerception);
		
		SequencerNode Sequencer_3 = BTStandar.CreateComposite<SequencerNode>(false, LightNoise);
		Sequencer_3.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(4)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(trapSpawner.StopTrapCoroutine),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(teleportBehindPlayer.TeleportEnemyBehindPlayer, null)
        };
			
        SequenceAction sTeleport = new SequenceAction(Status.Running, subActions3);

        LeafNode Teleport = BTStandar.CreateLeafNode(sTeleport);
		
		ConditionNode HighNoise = BTStandar.CreateDecorator<ConditionNode>(Teleport);
		HighNoise.SetPerception(highNoiseAndPerception);
		
		SequencerNode Sequencer_4 = BTStandar.CreateComposite<SequencerNode>(false, HighNoise);
		Sequencer_4.IsRandomized = false;
		
		SelectorNode Selector_0 = BTStandar.CreateComposite<SelectorNode>(false, Sequencer_3, Sequencer_4);
		Selector_0.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(departureLocation.SetInvisible, null)
        };

        SequenceAction sDisappear = new SequenceAction(Status.Running, subActions4);

        LeafNode Disappear = BTStandar.CreateLeafNode(sDisappear);
		
		ConditionNode CheckActualDisappear = BTStandar.CreateDecorator<ConditionNode>(Disappear);
		CheckActualDisappear.SetPerception(checkActualDisappearPerception);
		
		SequencerNode Sequencer_1 = BTStandar.CreateComposite<SequencerNode>(false, CheckActualDisappear);
		Sequencer_1.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions5 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(departureLocation.CalculatePositionToExit, null)
        };

        SequenceAction sDeparture = new SequenceAction(Status.Running, subActions5);

        LeafNode Departure_Location = BTStandar.CreateLeafNode(sDeparture);
		
		SelectorNode Selector_4 = BTStandar.CreateComposite<SelectorNode>(false, Sequencer_1, Departure_Location);
		Selector_4.IsRandomized = false;
		
		ConditionNode Time_Finish = BTStandar.CreateDecorator<ConditionNode>(Selector_4);
		Time_Finish.SetPerception(finishTimerPerception);
		
		SequencerNode Sequencer_6 = BTStandar.CreateComposite<SequencerNode>(false, Time_Finish);
		Sequencer_6.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions6 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(trapSpawner.PlaceRandomTrap, null)
        };

        SequenceAction sTrap = new SequenceAction(Status.Running, subActions6);


        LeafNode Trap_Candy = BTStandar.CreateLeafNode(sTrap);

        List<BehaviourAPI.Core.Actions.Action> subActions7 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(pathingNinja.StartPatrol, null)
        };

        SequenceAction sPatrol = new SequenceAction(Status.Running, subActions7);

        LeafNode Patrol = BTStandar.CreateLeafNode(sPatrol);
		
		ProbabilityBranchNode Probability_Selector = BTStandar.CreateComposite<ProbabilityBranchNode>(false, Trap_Candy, Patrol);
		Probability_Selector.probabilities = new List<float>() {0.05f, 0.95f};
		Probability_Selector.IsRandomized = false;
		
		SelectorNode Selector_3 = BTStandar.CreateComposite<SelectorNode>(false, Sequencer_6, Probability_Selector);
		Selector_3.IsRandomized = false;
		
		ConditionNode CheckTrapFinish = BTStandar.CreateDecorator<ConditionNode>(Selector_3);
		CheckTrapFinish.SetPerception(checkAppearDisappearAndPerception);
		
		SequencerNode Sequencer_7 = BTStandar.CreateComposite<SequencerNode>(false, CheckTrapFinish);
		Sequencer_7.IsRandomized = false;
		
		SelectorNode Selector_2 = BTStandar.CreateComposite<SelectorNode>(false, Selector_0, Sequencer_7);
		Selector_2.IsRandomized = false;
		
		ConditionNode CheckAppearDisapear = BTStandar.CreateDecorator<ConditionNode>(Selector_2);
		CheckAppearDisapear.SetPerception(checkAppearDisappearAndPerception);
		
		LoopNode Main_Loop = BTStandar.CreateDecorator<LoopNode>(CheckAppearDisapear);
		Main_Loop.Iterations = -1;
        BTStandar.SetRootNode(Main_Loop);

        return BTStandar;
	}
}
