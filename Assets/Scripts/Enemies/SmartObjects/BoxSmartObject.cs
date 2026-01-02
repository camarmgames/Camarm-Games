using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using UnityEngine;

public class BoxSmartObject: CarryableSmartObject
{
    [SerializeField] Transform objectTransform;

    private void Start()
    {
        indexDropPoint = 0;
    }
    protected override Action GetPickUpAction(SmartAgent agent)
    {
        return new FunctionalAction(() => PickUpStart(agent.transform), onUpdate);
    }

    void PickUpStart(Transform agent)
    {
        agent.gameObject.GetComponent<SmartObjectSensor>().hasSmartObjectInHand = true;
        //Desactivar físicas
        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log("Colocar caja");
        // Hacer hijo del socket
        objectTransform.SetParent(agent.transform.GetChild(1));
        objectTransform.localPosition = Vector3.zero;
        objectTransform.localRotation = Quaternion.identity;
    }

    Status onUpdate()
    {
        return Status.Success;
    }

    protected override Action GetDropAction(SmartAgent agent, RequestData requestData)
    {
        return new FunctionalAction(() => DropStart(agent), onUpdate);
    }

    void DropStart(SmartAgent agent)
    {
        if(objectTransform == null)
        {
            Debug.Log("Me lo robaron");
            return;
        }

        // Soltar del agente
        transform.position = objectTransform.position;
        objectTransform.SetParent(transform);
        objectTransform.position = dropPoint[indexDropPoint].position;

        indexDropPoint = (indexDropPoint + 1)%dropPoint.Count;

        agent.gameObject.GetComponent<SmartObjectSensor>().hasSmartObjectInHand = false;
        agent.gameObject.GetComponent<SmartObjectSensor>().BlockVisionCoroutine(15);
        // Reactivar físicas
        Rigidbody rb = objectTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
