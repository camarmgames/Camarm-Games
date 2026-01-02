using UnityEngine;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.Core.Actions;
using System.Collections.Generic;
using BehaviourAPI.UnityToolkit;

public abstract class CarryableSmartObject : SmartObject
{
    [Header("Pick up")]
    [SerializeField] protected Transform pickupTarget;
    [SerializeField] protected Transform carrySocket;
    [SerializeField] protected List<Transform> dropPoint;
    protected int indexDropPoint;
    

    [Header("Capability")]
    [SerializeField] string capabilityName;
    [SerializeField, Range(0f, 1f)] float capabilityValue;

    public SmartAgent Carrier { get; protected set; }

    public override bool ValidateAgent(SmartAgent agent)
    {
        return Carrier == null;
    }

    public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
    {
        Action action = GenerateAction(agent, requestData);
        SmartInteraction interaction = new SmartInteraction(action, agent, GetCapabilities());
        SetInteractionEvents(interaction, agent);
        return interaction;
    }

    protected virtual Action GenerateAction(SmartAgent agent, RequestData requestData)
    {
        SequenceAction sequence = new SequenceAction();

        // Ir al objecto
        sequence.SubActions.Add(new WalkAction(pickupTarget.position));

        // Recoger
        sequence.SubActions.Add(GetPickUpAction(agent));

        // Transportar
        sequence.SubActions.Add(new WalkAction(dropPoint[indexDropPoint].position));

        // Soltar
        sequence.SubActions.Add(GetDropAction(agent, requestData));

        return sequence;
    }

    protected abstract Action GetPickUpAction(SmartAgent agent);
    //protected abstract Action GetCarryAction(SmartAgent agent, RequestData requestData);
    protected abstract Action GetDropAction(SmartAgent agent, RequestData requestData);

    protected virtual void SetInteractionEvents(SmartInteraction interaction, SmartAgent agent)
    {
        interaction.OnInitialize += () => OnInitInteraction(agent);
        interaction.OnRelease += () => OnReleaseInteraction(agent);
    }

    protected virtual void OnInitInteraction(SmartAgent agent)
    {
        Carrier = agent;

        if (_registerOnManager)
            SmartObjectManager.Instance?.UnregisterSmartObject(this);
    }

    protected virtual void OnReleaseInteraction(SmartAgent agent)
    {
        Carrier = null;

        if (_registerOnManager)
            SmartObjectManager.Instance?.RegisterSmartObject(this);
    }

    protected Dictionary<string, float> GetCapabilities()
    {
        Dictionary<string, float> capabilities = new Dictionary<string, float>();

        if (!string.IsNullOrEmpty(capabilityName))
            capabilities[capabilityName] = capabilityValue;

        return capabilities;
    }

    public override float GetCapabilityValue(string capabilityName)
    {
        return this.capabilityName == capabilityName ? capabilityValue : 0f;
    }
}
