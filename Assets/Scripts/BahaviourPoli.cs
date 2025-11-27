using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.BehaviourTrees;

public class BahaviourPoli : BehaviourRunner
{
    PathingNinja pathingNinja;
    Investigation investigation;
    Track track;
	BlockPath blockPath;
    NoiseListener noiseListener;
    DetectPlayer detectPlayer;
	Attack attack;

    protected override void Init()
	{
		pathingNinja = GetComponent<PathingNinja>();
		investigation = GetComponent<Investigation>();
		track = GetComponent<Track>();
		blockPath = GetComponent<BlockPath>();
		noiseListener = GetComponent<NoiseListener>();
		detectPlayer = GetComponent<DetectPlayer>();
		attack = GetComponent<Attack>();

		base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		BehaviourTree BTMainStandar = new BehaviourTree();

		ConditionPerception lightNoisePerception = new ConditionPerception(noiseListener.LightNoise);
		ConditionPerception isPlayerDetectedPerception = new ConditionPerception(detectPlayer.IsPlayerDetected);
		ConditionPerception beforeShowPrintPerception = new ConditionPerception(track.BeforeShowPrint);
		ConditionPerception hasTrackPerception = new ConditionPerception(track.HasTrack);
		ConditionPerception detectFootprintPerception = new ConditionPerception(track.DetectFootprint);
		ConditionPerception isBlockingPathPerception = new ConditionPerception(blockPath.IsBlockingPath);

		AndPerception beforeFollowPrintAndPerception = new AndPerception(beforeShowPrintPerception, hasTrackPerception);
		AndPerception hasTrackAndPerception = new AndPerception(isBlockingPathPerception, hasTrackPerception);

        List<BehaviourAPI.Core.Actions.Action> subActions = new List<BehaviourAPI.Core.Actions.Action>(5)
        {
			new FunctionalAction(track.StopCoroutines),
			new FunctionalAction(blockPath.StopCoroutine),
            new FunctionalAction(investigation.StopInvestigation),
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(attack.AttackP, null)
        };

        SequenceAction sAttack = new SequenceAction(Status.Running, subActions);

        LeafNode Attack = BTMainStandar.CreateLeafNode(sAttack);
		
		ConditionNode DetectPlayer = BTMainStandar.CreateDecorator<ConditionNode>(Attack);
		DetectPlayer.SetPerception(isPlayerDetectedPerception);
		
		SequencerNode Sequencer_1 = BTMainStandar.CreateComposite<SequencerNode>(false, DetectPlayer);
		Sequencer_1.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(4)
        {
            new FunctionalAction(track.StopCoroutines),
            new FunctionalAction(blockPath.StopCoroutine),
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions2);

        LeafNode Investigate = BTMainStandar.CreateLeafNode(sInvestigate);
		
		SelectorNode Selector_1 = BTMainStandar.CreateComposite<SelectorNode>(false, Sequencer_1, Investigate);
		Selector_1.IsRandomized = false;
		
		ConditionNode DetectSomething = BTMainStandar.CreateDecorator<ConditionNode>(Selector_1);
		DetectSomething.SetPerception(lightNoisePerception);
		
		SequencerNode Sequencer_0 = BTMainStandar.CreateComposite<SequencerNode>(false, DetectSomething);
		Sequencer_0.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(track.StopCoroutines),
            new FunctionalAction(blockPath.ABlockPath, null)
        };

        SequenceAction sBlockingWall = new SequenceAction(Status.Running, subActions3);

        LeafNode BlockingWall = BTMainStandar.CreateLeafNode(sBlockingWall);
		
		ConditionNode BeforeFollowFootprint = BTMainStandar.CreateDecorator<ConditionNode>(BlockingWall);
		BeforeFollowFootprint.SetPerception(beforeFollowPrintAndPerception);
		
		SequencerNode Sequencer_2 = BTMainStandar.CreateComposite<SequencerNode>(false, BeforeFollowFootprint);
		Sequencer_2.IsRandomized = false;

        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(pathingNinja.StopPatrol),
            new FunctionalAction(track.Detect, null)
        };

        SequenceAction sFollowprints = new SequenceAction(Status.Running, subActions4);

        LeafNode FollowFootprints = BTMainStandar.CreateLeafNode(sFollowprints);
		
		ConditionNode DetectFootprints = BTMainStandar.CreateDecorator<ConditionNode>(FollowFootprints);
		DetectFootprints.SetPerception(detectFootprintPerception);
		
		SequencerNode Sequencer_3 = BTMainStandar.CreateComposite<SequencerNode>(false, DetectFootprints);
		Sequencer_3.IsRandomized = false;
		
		ProbabilityBranchNode Selector_Probability = BTMainStandar.CreateComposite<ProbabilityBranchNode>(false, Sequencer_2, Sequencer_3);
		Selector_Probability.probabilities = new List<float>() {0.02f, 0.98f};
		Selector_Probability.IsRandomized = false;

        BehaviourAPI.Core.Actions.Action Patrol_action = new FunctionalAction(pathingNinja.StartPatrol, null);
		LeafNode Patrol = BTMainStandar.CreateLeafNode(Patrol_action);
		
		ConditionNode HasTrack = BTMainStandar.CreateDecorator<ConditionNode>(Patrol);
		HasTrack.SetPerception(hasTrackAndPerception);
		
		SequencerNode Sequencer_4 = BTMainStandar.CreateComposite<SequencerNode>(false, HasTrack);
		Sequencer_4.IsRandomized = false;
		
		SelectorNode Selector_0 = BTMainStandar.CreateComposite<SelectorNode>(false, Sequencer_0, Selector_Probability, Sequencer_4);
		Selector_0.IsRandomized = false;
		
		LoopNode Main_Loop = BTMainStandar.CreateDecorator<LoopNode>(Selector_0);
		Main_Loop.Iterations = -1;
        BTMainStandar.SetRootNode(Main_Loop);

        return BTMainStandar;
	}
}
