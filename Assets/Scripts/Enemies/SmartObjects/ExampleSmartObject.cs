using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSmartObject : SmartObject
{
    public Vector3 pos;

    private void Awake()
    {
        _registerOnManager = false;
    }
    public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
    {
        BehaviourTree bt = new BehaviourTree();

        BehaviourAPI.Core.Actions.Action movementAction = new WalkAction(pos);
        LeafNode movementNode = bt.CreateLeafNode(movementAction);

        BehaviourAPI.Core.Actions.Action logAction = new DebugLogAction("El agente ha llegado al destino");
        LeafNode logNode = bt.CreateLeafNode(logAction);

        SequencerNode seq = bt.CreateComposite<SequencerNode>(false, movementNode, logNode);
        bt.SetRootNode(seq);

        BehaviourAPI.Core.Actions.Action action = new SubsystemAction(bt);

        Dictionary<string, float> capabilities = new Dictionary<string, float>();
        capabilities["ejercicio"] = 0.5f;
        return new SmartInteraction(action, agent, capabilities);
    }

    public override bool ValidateAgent(SmartAgent agent)
    {
        return true;
    }
    public override float GetCapabilityValue(string capabilityName)
    {
        if (capabilityName == "ejercicio") return 0.5f;
        else return 0f;
    }
}
