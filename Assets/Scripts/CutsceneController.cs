using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] GameObject[] characters;
    float[] angulos = {0,90,180,270};
    int charIndex=0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //charIndex++;
            //charIndex = charIndex % characters.Count();
            charIndex=Random.Range(0, 4);
            RotarCamara(angulos[charIndex]);
            Debug.Log("pressed skip");
        }
    }


    public float rotationDuration = 0.4f;

    public void RotarCamara(float targetY)
    {
        Vector3 targetRotation = new Vector3(
            camera.transform.eulerAngles.x,
            targetY,
            camera.transform.eulerAngles.z
        );

        camera.transform.DORotate(targetRotation, rotationDuration, RotateMode.Fast);
    }


}
