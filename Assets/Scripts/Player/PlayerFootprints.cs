using UnityEngine;

public class PlayerFootprints : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundMask;
    [SerializeField]
    private Transform footOrigin;
    [SerializeField]
    private float stepDistance = 1.5f;

    public AudioClip footClip;

    private Vector3 lastFootPos;
    private int nextFootprintID = 0;
    public int fruitIndex;

    void Start()
    {
        lastFootPos = transform.position;

        if (CharacterSelection.Instance != null)
        {
            fruitIndex = CharacterSelection.Instance.selectedCharacterIndex;
        }
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
                Vector3 forward = transform.forward;
                forward.y = -90;
                forward.Normalize();

                // Rotación de la huella: mira hacia la orientación del jugador y mantiene "arriba"
                Quaternion footRotation = Quaternion.LookRotation(forward, Vector3.up);

                GameObject footprint = Instantiate(
                    surface.surfaceType.footprintPrefab,
                    hit.point + Vector3.up * 0.01f,
                    footRotation
                );

                footprint.transform.GetChild(fruitIndex).gameObject.SetActive(true);
                AudioManager.Instance.PlaySFXAtPosition(footClip, footprint.transform.position, 1f, 1f);
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
