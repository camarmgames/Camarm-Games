using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit.GUIDesigner.Framework;

public class BehaviourNinja : BehaviourRunner
{
	[SerializeField] private DepartureLocation m_DepartureLocation;
	[SerializeField] private PathingNinja m_PathingNinja;
	
	protected override void Init()
	{
		m_DepartureLocation = GetComponent<DepartureLocation>();
		m_PathingNinja = GetComponent<PathingNinja>();
		
		base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		FSM FSMNinja0 = new FSM();
		StackFSM EstadoNormal = new StackFSM();
		
		DelayAction MainRoot_action = new DelayAction();
		MainRoot_action.delayTime = 60f;
		State MainRoot = FSMNinja0.CreateState(MainRoot_action);
		
		SubsystemAction Normal_action = new SubsystemAction(EstadoNormal, ExecutionInterruptOptions.None, false);
		State Normal = FSMNinja0.CreateState(Normal_action);
		
		SimpleAction DepartureLocation_action = new SimpleAction();
		DepartureLocation_action.onStarted = m_DepartureLocation.CalculatePositionToExit;
		State DepartureLocation = FSMNinja0.CreateState(DepartureLocation_action);
		
		UnityTimePerception SalidaTriunfal_perception = new UnityTimePerception();
		SalidaTriunfal_perception.TotalTime = 180f;
		SimpleAction SalidaTriunfal_action = new SimpleAction();
		SalidaTriunfal_action.onStarted = m_DepartureLocation.SetInvisible;
		StateTransition SalidaTriunfal = FSMNinja0.CreateTransition(Normal, DepartureLocation, SalidaTriunfal_perception, SalidaTriunfal_action);
		
		ConditionPerception EntradaTriunfal_perception = new ConditionPerception();
		EntradaTriunfal_perception.onCheck = m_DepartureLocation.CheckInvisible;
		StateTransition EntradaTriunfal = FSMNinja0.CreateTransition(DepartureLocation, Normal, EntradaTriunfal_perception);
		
		UnityTimePerception CheckInterval_perception = new UnityTimePerception();
		CheckInterval_perception.TotalTime = 5f;
		StateTransition CheckInterval = FSMNinja0.CreateTransition(MainRoot, DepartureLocation, CheckInterval_perception);
		
		SimpleAction Alerta_action = new SimpleAction();
		Alerta_action.onStarted = m_PathingNinja.Alert;
		State Alerta = FSMNinja0.CreateState(Alerta_action);
		
		ConditionPerception DetectarJugador_perception = new ConditionPerception();
		DetectarJugador_perception.onCheck = m_PathingNinja.DetectPlayer;
		SimpleAction DetectarJugador_action = new SimpleAction();
		DetectarJugador_action.onStarted = m_PathingNinja.CancelMove;
		StateTransition DetectarJugador = FSMNinja0.CreateTransition(Normal, Alerta, DetectarJugador_perception, DetectarJugador_action);
		
		SimpleAction Ataque_action = new SimpleAction();
		Ataque_action.onStarted = m_PathingNinja.Attack;
		State Ataque = FSMNinja0.CreateState(Ataque_action);
		
		UnityTimePerception Ataque_1_perception = new UnityTimePerception();
		Ataque_1_perception.TotalTime = 15f;
		StateTransition Ataque_1 = FSMNinja0.CreateTransition(Ataque, Alerta, Ataque_1_perception);
		
		UnityTimePerception ConfirmadoJugador_perception_sub1 = new UnityTimePerception();
		ConfirmadoJugador_perception_sub1.TotalTime = 15f;
		ConditionPerception ConfirmadoJugador_perception_sub2 = new ConditionPerception();
		ConfirmadoJugador_perception_sub2.onCheck = m_PathingNinja.DetectPlayer;
		AndPerception ConfirmadoJugador_perception = new AndPerception(ConfirmadoJugador_perception_sub1, ConfirmadoJugador_perception_sub2);
		StateTransition ConfirmadoJugador = FSMNinja0.CreateTransition(Alerta, Ataque, ConfirmadoJugador_perception);
		
		UnityTimePerception NoDetectaJugador_perception_sub1 = new UnityTimePerception();
		NoDetectaJugador_perception_sub1.TotalTime = 5f;
		ConditionPerception NoDetectaJugador_perception_sub2 = new ConditionPerception();
		NoDetectaJugador_perception_sub2.onCheck = m_PathingNinja.NoDetectPlayer;
		AndPerception NoDetectaJugador_perception = new AndPerception(NoDetectaJugador_perception_sub1, NoDetectaJugador_perception_sub2);
		StateTransition NoDetectaJugador = FSMNinja0.CreateTransition(Alerta, Normal, NoDetectaJugador_perception);
		
		ProbabilisticState QuietoPensativo = EstadoNormal.CreateProbabilisticState();
		QuietoPensativo.probabilities = new List<float>() {0.7f, 0.3f};
		
		SimpleAction Patrullar_action = new SimpleAction();
		Patrullar_action.onStarted = m_PathingNinja.PathingMove;
		State Patrullar = EstadoNormal.CreateState(Patrullar_action);
		
		ConditionPerception DecisionTomada_perception = new ConditionPerception();
		DecisionTomada_perception.onCheck = m_PathingNinja.NoDetectPlayer;
		StateTransition DecisionTomada = EstadoNormal.CreateTransition(QuietoPensativo, Patrullar, DecisionTomada_perception);
		
		SimpleAction PonerTrampa_action = new SimpleAction();
		PonerTrampa_action.onStarted = m_PathingNinja.PutTrap;
		State PonerTrampa = EstadoNormal.CreateState(PonerTrampa_action);
		
		ConditionPerception DecisionTomada_1_perception = new ConditionPerception();
		DecisionTomada_1_perception.onCheck = m_PathingNinja.NoDetectPlayer;
		StateTransition DecisionTomada_1 = EstadoNormal.CreateTransition(QuietoPensativo, PonerTrampa, DecisionTomada_1_perception);
		
		ConditionPerception LlegoZona_perception = new ConditionPerception();
		LlegoZona_perception.onCheck = m_PathingNinja.IsNotMoving;
		StateTransition LlegoZona = EstadoNormal.CreateTransition(Patrullar, QuietoPensativo, LlegoZona_perception);
		
		UnityTimePerception TrampaLista_perception = new UnityTimePerception();
		TrampaLista_perception.TotalTime = 5f;
		StateTransition TrampaLista = EstadoNormal.CreateTransition(PonerTrampa, QuietoPensativo, TrampaLista_perception);
		
		ConditionPerception unnamed_perception = new ConditionPerception();
		unnamed_perception.onCheck = m_PathingNinja.DetectPlayer;
		ExitTransition unnamed = EstadoNormal.CreateExitTransition(Patrullar, Status.Success, unnamed_perception);
		
		ConditionPerception unnamed_1_perception = new ConditionPerception();
		unnamed_1_perception.onCheck = m_PathingNinja.DetectPlayer;
		ExitTransition unnamed_1 = EstadoNormal.CreateExitTransition(PonerTrampa, Status.Success, unnamed_1_perception);
		
		ConditionPerception unnamed_2_perception_sub1 = new ConditionPerception();
		unnamed_2_perception_sub1.onCheck = m_PathingNinja.DetectPlayer;
		UnityTimePerception unnamed_2_perception_sub2 = new UnityTimePerception();
		unnamed_2_perception_sub2.TotalTime = 180f;
		OrPerception unnamed_2_perception = new OrPerception(unnamed_2_perception_sub1, unnamed_2_perception_sub2);
		ExitTransition unnamed_2 = EstadoNormal.CreateExitTransition(QuietoPensativo, Status.Success, unnamed_2_perception);
		
		return FSMNinja0;
	}
}
