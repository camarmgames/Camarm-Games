using UnityEngine;

public class PlayerFootprints : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private Transform footOrigin;
    [SerializeField]
    private float stepDistance = 1.5f;

    private Vector3 lastFootPos;
    private int nextFootprintID = 0;

    void Start()
    {
        lastFootPos = transform.position;    
    }

    void Update()
    {
        // Dejar huella cada cierta distancia recorrida
        if (Vector3.Distance(transform.position, lastFootPos) > stepDistance)
        {
            LeaveFootprint();
            lastFootPos = transform.position;
        }
    }

    void LeaveFootprint()
    {
        if (Physics.Raycast(footOrigin.position, Vector3.down, out RaycastHit hit, 2f, groundMask))
        {
            SurfaceIdentifier surface = hit.collider.GetComponent<SurfaceIdentifier>();

            if(surface != null && surface.surfaceType.leavesFootprints)
            {
                // Crear huella
                GameObject footprint = Instantiate(
                    surface.surfaceType.footprintPrefab,
                    hit.point + Vector3.up * 0.01f,
                    Quaternion.LookRotation(transform.forward)
                );

                footprint.tag = "Footprint";
                footprint.layer = LayerMask.NameToLayer("Footprints");

                // Asignar un ID
                Footprint fp = footprint.GetComponent<Footprint>();
                if (fp != null)
                {
                    fp.footprintID = nextFootprintID;
                }

                nextFootprintID++;

                // Destruir tras un tiempo
                Destroy(footprint, surface.surfaceType.footprintLifetime);
            }
        }
    }
}
