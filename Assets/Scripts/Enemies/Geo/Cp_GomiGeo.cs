using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.BehaviourTrees;
using Unity.VisualScripting;
using BehaviourAPI.UtilitySystems;

public class Cp_GomiGeo : BehaviourRunner
{

	Patrol m_Patrol;
	Investigation m_Investigation;
	NoiseListener m_NoiseListener;
	DetectPlayer m_DetectPlayer;
	JumpAttack m_JumpAttack;
	HitGround m_HitGround;
    StatsGomiGeo m_StatsGomiGeo;
    Break m_Break;

    // SmartObjects
    SmartObjectSensor m_SmartObjectSensor;

    protected override void Init()
    {
        m_Patrol = GetComponent<Patrol>();
        m_Investigation = GetComponent<Investigation>();
        m_NoiseListener = GetComponent<NoiseListener>();
        m_DetectPlayer = GetComponent<DetectPlayer>();
		m_JumpAttack = GetComponent<JumpAttack>();
		m_HitGround = GetComponent<HitGround>();

        m_StatsGomiGeo = GetComponent<StatsGomiGeo>();
        m_Break = GetComponent<Break>();

        m_SmartObjectSensor = GetComponent<SmartObjectSensor>();

        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
	{
		BehaviourTree MainBT = new BehaviourTree();
        UtilitySystem usAcciones = new UtilitySystem();

        // Perceptions
        ConditionPerception lightNoisePerception = new ConditionPerception(m_NoiseListener.LightNoise);
        ConditionPerception highNoisePerception = new ConditionPerception(m_NoiseListener.HighNoise);
        ConditionPerception isPlayerDetectedPerception = new ConditionPerception(m_DetectPlayer.IsPlayerDetected);
        ConditionPerception canJumpPerception = new ConditionPerception(m_JumpAttack.CanJump);

		OrPerception lightOrHighPerception = new OrPerception(lightNoisePerception, highNoisePerception);

        // MainBT
        List<BehaviourAPI.Core.Actions.Action> subActions1 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_HitGround.HitGroundStarted, m_HitGround.HitGroundUpdate, null)
        };

		SequenceAction sHitGround = new SequenceAction(Status.Running, subActions1);
		LeafNode GolpearAlSuelo = MainBT.CreateLeafNode(sHitGround);

        ConditionNode JugadorDetectado = MainBT.CreateDecorator<ConditionNode>(GolpearAlSuelo);
        JugadorDetectado.SetPerception(isPlayerDetectedPerception);

        List<BehaviourAPI.Core.Actions.Action> subActions2 = new List<BehaviourAPI.Core.Actions.Action>(2)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.InvestigateArea, null)
        };

        SequenceAction sInvestigate = new SequenceAction(Status.Running, subActions2);

        LeafNode Investigar = MainBT.CreateLeafNode(sInvestigate);

        SelectorNode Selector_3 = MainBT.CreateComposite<SelectorNode>(false, JugadorDetectado, Investigar);
        Selector_3.IsRandomized = false;

        ConditionNode SePercibeAlgunRuidoOSeVeAlgo = MainBT.CreateDecorator<ConditionNode>(Selector_3);
        SePercibeAlgunRuidoOSeVeAlgo.SetPerception(lightOrHighPerception);

        SubsystemAction US_Acciones = new SubsystemAction(usAcciones);
        LeafNode AccionesRutinarias = MainBT.CreateLeafNode(US_Acciones);

        SelectorNode Selector_1 = MainBT.CreateComposite<SelectorNode>(false, SePercibeAlgunRuidoOSeVeAlgo, AccionesRutinarias);
        Selector_1.IsRandomized = false;

        LoopNode Main_Loop_1 = MainBT.CreateDecorator<LoopNode>(Selector_1);
        Main_Loop_1.Iterations = -1;
        MainBT.SetRootNode(Main_Loop_1);

        // Acciones Rutinarias US

        VariableFactor staminaFactor = usAcciones.CreateVariable(m_StatsGomiGeo.GetStamina, 0f, 100f);

        VariableFactor tiempoPatrullandoFactor = usAcciones.CreateVariable(m_StatsGomiGeo.GetTimePatrol, 0f, 100f);

        VariableFactor agotamientoFactor = usAcciones.CreateVariable(m_StatsGomiGeo.GetTakeABreak, 0f, 1f);

        WeightedFusionFactor GanasDePatrullarWeightedFusion = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, agotamientoFactor);
        GanasDePatrullarWeightedFusion.Weights = new float[] { 0.4f, 0.6f };

        SigmoidCurveFactor CurvaPatrulla = usAcciones.CreateCurve<SigmoidCurveFactor>(GanasDePatrullarWeightedFusion);
        CurvaPatrulla.GrownRate = 8f;
        CurvaPatrulla.Midpoint = 0.6f;

        SigmoidCurveFactor CurvaDeDescanso = usAcciones.CreateCurve<SigmoidCurveFactor>(staminaFactor);
        CurvaDeDescanso.GrownRate = -10f;
        CurvaDeDescanso.Midpoint = 0.6f;

        WeightedFusionFactor GanasDeSaltarWeightedFusion = usAcciones.CreateFusion<WeightedFusionFactor>(staminaFactor, tiempoPatrullandoFactor);
        GanasDeSaltarWeightedFusion.Weights = new float[] { 0.5f, 0.5f };

        SigmoidCurveFactor CurvaDeSalto = usAcciones.CreateCurve<SigmoidCurveFactor>(GanasDeSaltarWeightedFusion);
        CurvaDeSalto.GrownRate = 20;
        CurvaDeSalto.Midpoint = 0.6f;


        // SmartObjects

        // Factor: solo existe si hay smart objects visibles
        VariableFactor smartObjectFactor = usAcciones.CreateVariable(m_SmartObjectSensor.SmartObjectSignal, 0f, 1f);

        // Fusión
        WeightedFusionFactor oportunidadFusion =
            usAcciones.CreateFusion<WeightedFusionFactor>(
                smartObjectFactor,
                staminaFactor
            );

        oportunidadFusion.Weights = new float[] { 0.9f, 0.1f };

        // Acción
        UtilityAction OportunidadSmartObject =
            usAcciones.CreateAction(
                oportunidadFusion,
                CreateSmartObjectAction("Oso")
            );

        // Acciones
        List<BehaviourAPI.Core.Actions.Action> subActions3 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Patrol.StartPatrol, null)
        };

        SequenceAction sPatrolSA = new SequenceAction(Status.Running, subActions3);
        UtilityAction Patrullar = usAcciones.CreateAction(CurvaPatrulla, sPatrolSA, true);

        List<BehaviourAPI.Core.Actions.Action> subActions11 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Break.TakeABreakStarted, m_Break.TakeABreakUpdate, null)
        };

        SequenceAction sBreakSA = new SequenceAction(Status.Running, subActions11);
        UtilityAction TomarUnDescanso = usAcciones.CreateAction(CurvaDeDescanso, sBreakSA, true);

        List<BehaviourAPI.Core.Actions.Action> subActions4 = new List<BehaviourAPI.Core.Actions.Action>(3)
        {
            new FunctionalAction(m_Investigation.StopInvestigation),
            new FunctionalAction(m_Patrol.StopPatrol),
            new FunctionalAction(m_JumpAttack.JumpAttackStarted, m_JumpAttack.JumpAttackUpdate, null)
        };

        SequenceAction sJumpAttackSA = new SequenceAction(Status.Running, subActions4);
        UtilityAction ColocarTrampa = usAcciones.CreateAction(CurvaDeSalto, sJumpAttackSA, true);

        return MainBT;
	}

    private BehaviourAPI.Core.Actions.Action CreateSmartObjectAction(string needName)
    {
        var bt = new BehaviourTree();
        bt.SetRootNode(
            bt.CreateDecorator<LoopNode>("loop",
                bt.CreateComposite<SelectorNode>("sel",
                    false,
                    bt.CreateLeafNode("request", new NeedRequestAction(needName)),
                    bt.CreateLeafNode("delay", new DelayAction(5f))
                )
            )
        );
        return new SubsystemAction(bt);
    }
}
