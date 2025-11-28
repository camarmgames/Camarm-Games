using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQuote
{
    List<string> dialogueContent;
    AudioClip[] characterVoice;
    float speedMultiplier; //el valor speedMultiplier se multiplica por la velocidad del dialogo asignada en cutscenedialogue. 1 sería velocidad normal pero se pueden poner frases mas rápidas o más lentas para dramatizar
    int characterIndex; //pera: 0, limon: 1, fresa: 2, platano: 3

    public DialogueQuote(List<string> _dialogueContent, AudioClip _characterVoice, float _speedMultiplier, int _characterIndex)
    {
        dialogueContent = _dialogueContent;
        characterVoice = new AudioClip[] { _characterVoice };
        speedMultiplier = _speedMultiplier;
        characterIndex = _characterIndex;
    }

    public DialogueQuote(List<string> _dialogueContent, AudioClip _characterVoice, int _characterIndex)
    {
        dialogueContent = _dialogueContent;
        characterVoice = new AudioClip[] { _characterVoice };
        speedMultiplier = 1;
        characterIndex = _characterIndex;
    }

    public string GetDialogueContent(string LANGUAGE)
    {

        if (LANGUAGE == "es")
        {
            return dialogueContent[0];
        }
        else if (LANGUAGE == "en")
        {
            return dialogueContent[1];
        }
        return dialogueContent[0];

    }

    public AudioClip[] GetCharacterVoice() { return characterVoice; }
    public float GetSpeedMultiplier() { return speedMultiplier; }
    public int GetCharacterIndex() { return characterIndex; }

}

public class Data_Cutscenes
{
    public static AudioClip VozPera = Resources.Load<AudioClip>("SFX/SFX_talkBichito");
    public static AudioClip VozLimon = Resources.Load<AudioClip>("SFX/SFX_talkBichito");
    public static AudioClip VozFresa = Resources.Load<AudioClip>("SFX/SFX_talkHermano");
    public static AudioClip VozPlatano = Resources.Load<AudioClip>("SFX/SFX_talkHermano");

    public static Dictionary<string, DialogueQuote> data_CutsceneIntro = new Dictionary<string, DialogueQuote>()
    {
        //Intro
        {"OpeningSecretary_LilBug_1",    new DialogueQuote( new List<string>{
            "Oh no, nonono. De todos los líos en los que nos has metido, este sin duda es el peor de todos.",
            "English"
            },
            VozPera,
            0)
        },
        {"OpeningSecretary_Brother_2", new DialogueQuote(
            new List<string>{
                "Vamos, relájate. Hemos revisado mil veces los horarios de los curratas, no va a venir nadie hasta mañana.",
                "English"
            },
            VozPera,
            2
        )},

        {"OpeningSecretary_LilBug_3", new DialogueQuote(
            new List<string>{
                "¡HE revisado mil veces el horario de los trabajadores! Si hubiéramos seguido tu plan, estaríamos de camino al correcional.",
                "English"
            },
            VozPera,
            3
        )},

        {"OpeningSecretary_Brother_4", new DialogueQuote(
            new List<string>{
                "Tienes razón, ¡eres el mejor, bichito! Que haría yo sin ti...",
                "English"
            },
            VozPera,
            1
        )},

        {"OpeningSecretary_Brother_5", new DialogueQuote(
            new List<string>{
                "Siempre nos han salido bien nuestras aventuras porque estamos juntos en ellas, ¡Así que cálmate e intenta divertirte!",
                "English"
            },
            VozPera,
            2
        )},

        {"OpeningSecretary_LilBug_6", new DialogueQuote(
            new List<string>{
                "Colarnos en una universidad a buscar documentos confidenciales no es exactamente mi idea de diversión...",
                "English"
            },
            VozPera,
            0
        )},

        {"OpeningSecretary_Brother_7", new DialogueQuote(
            new List<string>{
                "Te demostraré que merece la pena. Te prometo que esta vez es la buena, están todos locos con esto en Quinto Milenio.",
                "English"
            },
            VozPera,
            1
        )},

        {"OpeningSecretary_Brother_8", new DialogueQuote(
            new List<string>{
                "Ya sabes que mamá y papá no me creen.",
                "English"
            },
            VozPera,
            0
        )},

        {"OpeningSecretary_Brother_9", new DialogueQuote(
            new List<string>{
                "Tu sí lo haces, ¿verdad?",
                "English"
            },
            VozPera,
            3
        )},

        {"OpeningSecretary_LilBug_10", new DialogueQuote(
            new List<string>{
                "...",
                "English"
            },
            VozPera,
            0
        )},

        {"OpeningSecretary_LilBug_11", new DialogueQuote(
            new List<string>{
                "...siempre",
                "English"
            },
            VozPera,
            2
        )},

        {"OpeningSecretary_Brother_12", new DialogueQuote(
            new List<string>{
                "¡Ese es mi hermanito!",
                "English"
            },
            VozPera,
            1
        )},

        {"OpeningSecretary_Brother_13", new DialogueQuote(
            new List<string>{
                "¡Hecho! Ahora intenta buscar algo que te mole. ¡Quien lo encuentra se lo queda!",
                "English"
            },
            VozPera,
            0
        )},

    };

}
