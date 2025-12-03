using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerNoise: MonoBehaviour
{
    public NoiseType walkNoise;
    public NoiseType runNoise;
    public NoiseType crouchNoise;

    private PlayerMovement playerMovement;
    private NoiseEmitter noiseEmitter;



    private bool isCrouch;

    void Start()
    {
        noiseEmitter = GetComponent<NoiseEmitter>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void WalkNoise()
    {
        if(!playerMovement._sprint && !playerMovement._crouch)
        {
            NoiseType typeToEmit = walkNoise;
            noiseEmitter.EmitNoise(typeToEmit);
        }
    }

    public void RunNoise() {
        if (!playerMovement._crouch)
        {
            NoiseType typeToEmit = runNoise;
            noiseEmitter.EmitNoise(typeToEmit);
        }

    }

    public void CrouchNoise()
    {
        if (!playerMovement._sprint)
        {
            NoiseType typeToEmit = crouchNoise;
            noiseEmitter.EmitNoise(typeToEmit);
        }

    }


}
