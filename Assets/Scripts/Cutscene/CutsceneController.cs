using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] TalkAnimationController[] characterAnimations;
    float[] angulos = {0,90,180,270};
    int charIndex=0;

    public TextMeshProUGUI cutsceneText;
    public TextMeshProUGUI name;
    private List<DialogueQuote> cutsceneQuotes = new List<DialogueQuote>();
    int cutsceneIndex=0;
    public float writingSpeed;
    public GameObject flechita;

    public bool pressedE;
    public bool pressedE_twice;
    int charId;

    bool start;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        camera.transform.localPosition = new Vector3(0, 1.5f, 0);
        camera.transform.localRotation = Quaternion.Euler(new Vector3(0, 270, 0));

        flechita.SetActive(false);
        cutsceneText.text = "";

        GetQuotes();

        NextSentence();
        start = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!start) return;
        LoopTime();
    }


    public void NextDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PressE();

            //charIndex++;
            //charIndex = charIndex % characters.Count();

            //charIndex=Random.Range(0, 4);
            //RotarCamara(angulos[charIndex]);
            //Debug.Log("pressed skip");
        }
    }

    public void PressE()
    {

        if (cutsceneText.text != "")
        {
            if (!pressedE)
            {
                pressedE = true;
            }
            else
            {
                pressedE_twice = true;
            }
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


    public void GetQuotes()
    {
        cutsceneQuotes = Data_Cutscenes.data_CutsceneIntro.Values.ToList();
    }

    public void NextSentence()
    {
        if (cutsceneIndex<cutsceneQuotes.Count())
        {
            StartCoroutine(TypeSentence(cutsceneQuotes[cutsceneIndex]));
            cutsceneIndex++;
            //RotarCamara(angulos[cutsceneQuotes[cutsceneIndex].characterIndex]);
            //StartCoroutine(TypeSentence(cutsceneQuotes[cutsceneIndex], cutsceneQuotes[cutsceneIndex].GetCharacterIndex()));

        }
        else
        {
            //terminar cinematica
            FindFirstObjectByType<CinematicaSegundaParte>().Play();
;            //SceneManager.LoadScene("Mazmorra_V2");
        }
    }

    public void LoopTime()
    {
        if (pressedE_twice)
        {
            pressedE = false;
            pressedE_twice = false;
            flechita.SetActive(false);

            StopAllCoroutines();
            cutsceneText.text = "";
            characterAnimations[charId].StopTalking();
            NextSentence();
        }
    }

    //IEnumerator TypeSentence(string sentence)
    IEnumerator TypeSentence(DialogueQuote dialogue)
    {
        //rotacion camara
        charId = dialogue.GetCharacterIndex();
        RotarCamara(angulos[charId]);
        yield return new WaitForSeconds(.5f);

        name.text = dialogue.GetNameFruta();
        string sentence = dialogue.GetDialogueContent("es");
        AudioClip[] audio = dialogue.GetCharacterVoice();
        float speed = dialogue.GetSpeedMultiplier();

        characterAnimations[charId].StartTalking();
        cutsceneText.text = sentence;
        cutsceneText.maxVisibleCharacters = 0;
        cutsceneText.ForceMeshUpdate();
        yield return null;

        yield return new WaitForSeconds(.1f);

        pressedE = false;

        for (int i = 0; i < cutsceneText.textInfo.characterCount; i++)
        {
            if (pressedE) { break; }
            cutsceneText.maxVisibleCharacters = i + 1;
            cutsceneText.ForceMeshUpdate();
            yield return null;

            var charInfo = cutsceneText.textInfo.characterInfo[i];
            if (charInfo.isVisible)
            {
                //if (audio != null && SoundManager.instance != null)
                //{
                //    SoundManager.instance.Play2D_SFX(audio);
                //}
                yield return new WaitForSeconds(writingSpeed / speed);
            }
        }
        pressedE = true;
        characterAnimations[charId].StopTalkingMouth();
        StartCoroutine(SentenceEnd());
    }

    IEnumerator SentenceEnd()
    {
        var textFinal = cutsceneText.text;
        cutsceneText.ForceMeshUpdate();
        yield return null;
        cutsceneText.maxVisibleCharacters = cutsceneText.textInfo.characterCount;

        flechita.SetActive(true);
    }
}
