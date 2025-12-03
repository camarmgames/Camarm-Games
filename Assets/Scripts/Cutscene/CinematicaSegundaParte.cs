using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicaSegundaParte : MonoBehaviour
{
    [Header("Animators")]

    [SerializeField] Animator pera;
    [SerializeField] Animator limon;
    [SerializeField] Animator fresa;
    [SerializeField] Animator platano;

    [Header("Camaras")]

    [SerializeField] GameObject camaraDialogo;
    [SerializeField] GameObject camaraCinematica;

    [Header("Cinematica")]

    [SerializeField] PlayableDirector PlayableDirector;
    [SerializeField] GameObject dialogue;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        PlayableDirector.Play();

        dialogue.SetActive(false);
        StartCoroutine(WalkTrigger(5.2f, pera));
        StartCoroutine(WalkTrigger(7.0f, limon));
        StartCoroutine(WalkTrigger(1f, fresa));
        StartCoroutine(WalkTrigger(0.85f, platano));

        StartCoroutine(SwitchCameras());

        GetComponent<AudioSource>().Play();
    }

    IEnumerator WalkTrigger(float time, Animator anim)
    {
        yield return new WaitForSeconds(time);
        anim.SetTrigger("walk");
    }

    IEnumerator SwitchCameras()
    {
        camaraCinematica.SetActive(true);
        yield return new WaitForEndOfFrame();
        camaraDialogo.SetActive(false);
    }
}
