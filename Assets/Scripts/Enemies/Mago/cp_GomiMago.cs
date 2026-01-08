using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UtilitySystems;
using UnityEngine;

public class cp_GomiMago: BehaviourRunner
{
    LevelWizardController m_LevelWizardController;
    protected override void Init()
    {
        m_LevelWizardController = GetComponent<LevelWizardController>();

        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
    {
        BehaviourTree MainBT = new BehaviourTree();
        UtilitySystem usAcciones = new UtilitySystem();

        SubsystemAction US_Acciones = new SubsystemAction(usAcciones);
        LeafNode AccionesRutinarias = MainBT.CreateLeafNode(US_Acciones);

        LoopNode root = MainBT.CreateDecorator<LoopNode>(AccionesRutinarias);
        root.Iterations = -1;
        MainBT.SetRootNode(root);


        VariableFactor estresFactor = usAcciones.CreateVariable(() => m_LevelWizardController.Stress01, 0f, 1f);

        VariableFactor tiempoVisibleFactor = usAcciones.CreateVariable(() => m_LevelWizardController.TiempoVisible01, 0f, 1f);

        VariableFactor tiempoEscondidoFactor = usAcciones.CreateVariable(() => m_LevelWizardController.TiempoEscondido01, 0f, 1f);

        VariableFactor tiempoPersiguiendoFactor = usAcciones.CreateVariable(() => m_LevelWizardController.TiempoPersiguiendo01, 0f, 1f);

        
        SigmoidCurveFactor curvaEstresAlto = usAcciones.CreateCurve<SigmoidCurveFactor>(estresFactor);
        curvaEstresAlto.GrownRate = 8f;
        curvaEstresAlto.Midpoint = 0.6f;

        SigmoidCurveFactor curvaMuchoTiempoVisible = usAcciones.CreateCurve<SigmoidCurveFactor>(tiempoVisibleFactor);
        curvaMuchoTiempoVisible.GrownRate = 10f;
        curvaMuchoTiempoVisible.Midpoint = 0.5f;

        SigmoidCurveFactor curvaMuchoTiempoEscondido = usAcciones.CreateCurve<SigmoidCurveFactor>(tiempoEscondidoFactor);
        curvaMuchoTiempoEscondido.GrownRate = 10f;
        curvaMuchoTiempoEscondido.Midpoint = 0.5f;

        SigmoidCurveFactor curvaPocoTiempoPersiguiendo = usAcciones.CreateCurve<SigmoidCurveFactor>(tiempoPersiguiendoFactor);
        curvaPocoTiempoPersiguiendo.GrownRate = -12f;
        curvaPocoTiempoPersiguiendo.Midpoint = 0.4f;

        SigmoidCurveFactor curvaPocoTiempoVisible = usAcciones.CreateCurve<SigmoidCurveFactor>(tiempoVisibleFactor);
        curvaPocoTiempoVisible.GrownRate = -8f;
        curvaPocoTiempoVisible.Midpoint = 0.4f;

        SigmoidCurveFactor curvaEstresBajo = usAcciones.CreateCurve<SigmoidCurveFactor>(estresFactor);
        curvaEstresBajo.GrownRate = -8f;
        curvaEstresBajo.Midpoint = 0.4f;

        // Fusiones

        WeightedFusionFactor ganasDeAparecer = usAcciones.CreateFusion<WeightedFusionFactor>(curvaEstresAlto, curvaMuchoTiempoEscondido);
        ganasDeAparecer.Weights = new float[] { 0.6f, 0.4f };

        WeightedFusionFactor ganasDeLanzarHechizo = usAcciones.CreateFusion<WeightedFusionFactor>(curvaEstresAlto, curvaMuchoTiempoVisible);
        ganasDeLanzarHechizo.Weights = new float[] { 0.6f, 0.4f };

        WeightedFusionFactor ganasDePerseguir = usAcciones.CreateFusion<WeightedFusionFactor>(curvaEstresAlto, curvaPocoTiempoPersiguiendo);
        ganasDePerseguir.Weights = new float[] {0.5f, 0.5f };

        WeightedFusionFactor ganasDeDesaparecer = usAcciones.CreateFusion<WeightedFusionFactor>(curvaMuchoTiempoVisible, curvaEstresBajo);
        ganasDeDesaparecer.Weights = new float[] { 0.8f, 0.2f };

        WeightedFusionFactor nadaMejorQueHacer = usAcciones.CreateFusion<WeightedFusionFactor>(curvaEstresBajo, curvaPocoTiempoVisible);
        nadaMejorQueHacer.Weights = new float[] { 0.8f, 0.2f };

        // Lanzar hechizo
        FunctionalAction LanzarHechizoA = new FunctionalAction(m_LevelWizardController.AffectStressedEnemies, m_LevelWizardController.onUpdate);
        UtilityAction LanzarHechizo = usAcciones.CreateAction(ganasDeLanzarHechizo, LanzarHechizoA, true);

        // Perseguir al jugador
        FunctionalAction PerseguirJugadorA = new FunctionalAction(m_LevelWizardController.ActionFollowPlayer, m_LevelWizardController.onUpdate);
        UtilityAction PerseguirJugador = usAcciones.CreateAction(ganasDePerseguir, PerseguirJugadorA, true);

        // Aparecer
        FunctionalAction AparecerA = new FunctionalAction(m_LevelWizardController.ActionAppear, m_LevelWizardController.onUpdate);
        UtilityAction Aparecer = usAcciones.CreateAction(ganasDeAparecer, AparecerA, true);

        // Desaparecer
        FunctionalAction DesaparecerA = new FunctionalAction(m_LevelWizardController.ActionDisappear, m_LevelWizardController.onUpdate);
        UtilityAction Desaparecer = usAcciones.CreateAction(ganasDeDesaparecer, DesaparecerA, true);

        // Idle
        FunctionalAction IdleA = new FunctionalAction(m_LevelWizardController.Nothing, m_LevelWizardController.onUpdate);
        UtilityAction Idle = usAcciones.CreateAction(nadaMejorQueHacer, IdleA, true);

        
        return MainBT;
    }
}
