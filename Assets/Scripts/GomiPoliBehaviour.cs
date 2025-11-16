using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using BehaviourAPI.StateMachines.StackFSMs;

public class GomiPoliBehaviour : BehaviourRunner
{
	[SerializeField] private Patrol m_Patrol;
	[SerializeField] private Track m_Track;
	[SerializeField] private BlockPath m_BlockPath;
	[SerializeField] private Attack m_Attack;
	[SerializeField] private DetectPlayer m_DetectPlayer;
	private PushPerception ApareceMago;
	
	protected override void Init()
	{
		m_Patrol = GetComponent<Patrol>();
		m_Track = GetComponent<Track>();
		m_BlockPath = GetComponent<BlockPath>();
		m_Attack = GetComponent<Attack>();
		m_DetectPlayer = GetComponent<DetectPlayer>();
		
		base.Init();
	}
	
	protected override BehaviourGraph CreateGraph()
	{
		StackFSM GomiPoliFSM1 = new StackFSM();
		FSM GomiPoliFSM2 = new FSM();
		
		SubsystemAction Movimiento_action = new SubsystemAction(GomiPoliFSM2);
		State Movimiento = GomiPoliFSM1.CreateState(Movimiento_action);
		
		DelayAction Potenciacion_action = new DelayAction();
		Potenciacion_action.delayTime = 4f;
		State Potenciacion = GomiPoliFSM1.CreateState(Potenciacion_action);
		
		FunctionalAction T12_action = new FunctionalAction();
		T12_action.onStarted = m_Patrol.DisablePatrol;
		T12_action.onUpdated = () => Status.Running;
		StateTransition T12 = GomiPoliFSM1.CreateTransition(from: Movimiento, Potenciacion, action: T12_action, statusFlags: StatusFlags.None);
		
		StateTransition T21 = GomiPoliFSM1.CreateTransition(Potenciacion, Movimiento, statusFlags: StatusFlags.Finished);
		
		FunctionalAction Normal_action = new FunctionalAction();
		Normal_action.onStarted = m_Patrol.EnablePatrol;
		Normal_action.onUpdated = () => Status.Running;
		State Normal = GomiPoliFSM2.CreateState(Normal_action);
		
		FunctionalAction Alerta_action = new FunctionalAction();
		Alerta_action.onStarted = m_Track.Detect;
		Alerta_action.onUpdated = () => Status.Running;
		State Alerta = GomiPoliFSM2.CreateState(Alerta_action);
		
		ConditionPerception DetectaRastro_perception = new ConditionPerception();
		DetectaRastro_perception.onCheck = m_Track.DetectFootprint;
		SimpleAction DetectaRastro_action = new SimpleAction();
		DetectaRastro_action.action = m_Patrol.DisablePatrol;
		StateTransition DetectaRastro = GomiPoliFSM2.CreateTransition(Normal, Alerta, DetectaRastro_perception, DetectaRastro_action, statusFlags: StatusFlags.Running);
		
		ConditionPerception VolverPatrullar_perception = new ConditionPerception();
		VolverPatrullar_perception.onCheck = m_Track.HasEndedFollowing;
		SimpleAction VolverPatrullar_action = new SimpleAction();
		VolverPatrullar_action.action = m_BlockPath.ABlockPath;
		StateTransition VolverPatrullar = GomiPoliFSM2.CreateTransition(Alerta, Normal, VolverPatrullar_perception, VolverPatrullar_action, statusFlags: StatusFlags.Running);
		
		SimpleAction Ataque_action = new SimpleAction();
		Ataque_action.action = m_Attack.AttackP;
		State Ataque = GomiPoliFSM2.CreateState(Ataque_action);
		
		ConditionPerception DetectaPersonaje_perception = new ConditionPerception();
		DetectaPersonaje_perception.onCheck = m_DetectPlayer.PDetectPlayer;
		StateTransition DetectaPersonaje = GomiPoliFSM2.CreateTransition(Normal, Ataque, DetectaPersonaje_perception, statusFlags: StatusFlags.None);
		
		StateTransition VolverAtaque = GomiPoliFSM2.CreateTransition(Ataque, Normal, statusFlags: StatusFlags.Finished);
		
		ApareceMago = new PushPerception(T12);
		return GomiPoliFSM1;
	}
}
