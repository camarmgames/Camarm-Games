using UnityEngine;

public class PlayerNoise: MonoBehaviour
{
    public NoiseType walkNoise;
    public NoiseType runNoise;
    public NoiseType crouchNoise;

    private PlayerMovement playerMovement;
    private NoiseEmitter noiseEmitter;

    private float noiseCooldown = 0.5f;
    private float timer = 0f;

    void Start()
    {
        noiseEmitter = GetComponent<NoiseEmitter>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        
        bool isMoving = playerMovement != null && playerMovement.moveSpeed > 0f && playerMovement.enabled;

        if(isMoving && timer <= 0f)
        {
            NoiseType typeToEmit = walkNoise;

            if(Input.GetKey(KeyCode.LeftShift))
                typeToEmit = runNoise;
            else if (Input.GetKey(KeyCode.LeftControl))
                typeToEmit = crouchNoise;

            noiseEmitter.EmitNoise(typeToEmit);
            timer = noiseCooldown;
        }
    }
}
