using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.BehaviourTrees;
using Unity.VisualScripting;

public class Cp_GomiGeo : BehaviourRunner
{

	PathingNinja m_PathingNinja;
	Investigation m_Investigation;
	NoiseListener m_NoiseListener;
	DetectPlayer m_DetectPlayer;
	JumpAttack m_JumpAttack;
	TrapSpawner m_TrapSpawner;
	HitGround m_HitGround;

    protected override void Init()
    {
        m_PathingNinja = GetComponent<PathingNinja>();
        m_Investigation = GetComponent<Investigation>();
        m_TrapSpawner = GetComponent<TrapSpawner>();
        m_NoiseListener = GetComponent<NoiseListener>();
        m_DetectPlayer = GetComponent<DetectPlayer>();
		m_JumpAttack = GetComponent<JumpAttack>();
		m_HitGround = GetComponent<HitGround>();

        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
	{
		BehaviourTree MainBT = new BehaviourTree();

        // Perceptions
        ConditionPerception lightNoisePerception = new ConditionPerception(m_NoiseListener.LightNoise);
        ConditionPerception highNoisePerception = new ConditionPerception(m_NoiseListener.HighNoise);
        ConditionPerception isPlayerDetectedPerception = new ConditionPerception(m_DetectPlayer.IsPlayerDetected);
        ConditionPerception canJumpPerception = new ConditionPerception(m_JumpAttack.CanJump);

		OrPerception lightOrHighPerception = new OrPerception(lightNoisePerception, highNoisePerception);

        // MainBT
        List<BehaviourAPI.Core.Actions.Action> subActions1 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_HitGround.HitGroundStarted, m_HitGround.HitGroundUpdate, null)
        };

		SequenceAction sHitGround = new SequenceAction(Status.Running, subActions1);
		LeafNode Hit_Ground = MainBT.CreateLeafNode(sHitGround);

        ConditionNode Detect_Player = MainBT.CreateDecorator<ConditionNode>(Hit_Ground);
        Detect_Player.SetPerception(isPlayerDetectedPerception);

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_PathingNinja.StopPatrol),
			new FunctionalAction(m_TrapSpawner.PlaceTrapPrueba),
            new FunctionalAction(m_Investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions2);

        LeafNode Investigate = MainBT.CreateLeafNode(sInvestigate);

        SelectorNode Selector_3 = MainBT.CreateComposite<SelectorNode>(false, Detect_Player, Investigate);
        Selector_3.IsRandomized = false;

        ConditionNode LightOrHighNoise = MainBT.CreateDecorator<ConditionNode>(Selector_3);
        LightOrHighNoise.SetPerception(lightOrHighPerception);

        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_PathingNinja.StartPatrol, null)
        };

        SequenceAction sPatrol = new SequenceAction(Status.Running, subActions3);
        LeafNode Patrol = MainBT.CreateLeafNode(sPatrol);

        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_PathingNinja.StopPatrol),
            new FunctionalAction(m_JumpAttack.JumpAttackStarted, m_JumpAttack.JumpAttackUpdate, null)
        };

        SequenceAction sJumpAttack = new SequenceAction(Status.Running, subActions4);
        LeafNode JumpAttack = MainBT.CreateLeafNode(sJumpAttack);

        ConditionNode CanJump = MainBT.CreateDecorator<ConditionNode>(JumpAttack);
        CanJump.SetPerception(canJumpPerception);

        ProbabilityBranchNode RandomSelector = MainBT.CreateComposite<ProbabilityBranchNode>(false, Patrol, CanJump);
        RandomSelector.probabilities = new List<float>() { 0.9f, 0.1f };
        RandomSelector.IsRandomized = false;

        SelectorNode Selector_1 = MainBT.CreateComposite<SelectorNode>(false, LightOrHighNoise, RandomSelector);
        Selector_1.IsRandomized = false;

        LoopNode Main_Loop_1 = MainBT.CreateDecorator<LoopNode>(Selector_1);
        Main_Loop_1.Iterations = -1;
        MainBT.SetRootNode(Main_Loop_1);

        return MainBT;
	}
}
