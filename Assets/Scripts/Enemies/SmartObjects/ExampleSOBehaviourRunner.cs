using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using UnityEngine;

public class ExampleSOBehaviourRunner: BehaviourRunner
{
    public SmartAgent agent;
    public CarryableSmartObject SmartObject;

    protected override BehaviourGraph CreateGraph()
    {
        BehaviourTree bt = new BehaviourTree();
        Action action = new TargetRequestAction(agent, SmartObject, new RequestData());
        bt.CreateLeafNode(action);
        return bt;
    }
}
