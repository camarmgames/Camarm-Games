using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQuote
{
    List<string> dialogueContent;
    AudioClip[] characterVoice;
    float speedMultiplier; //el valor speedMultiplier se multiplica por la velocidad del dialogo asignada en cutscenedialogue. 1 sería velocidad normal pero se pueden poner frases mas rápidas o más lentas para dramatizar
    int characterIndex; //pera: 0, limon: 1, fresa: 2, platano: 3
    string nameFruta;

    public DialogueQuote(List<string> _dialogueContent, AudioClip _characterVoice, float _speedMultiplier, int _characterIndex, string nameFrutaIndex)
    {
        dialogueContent = _dialogueContent;
        characterVoice = new AudioClip[] { _characterVoice };
        speedMultiplier = _speedMultiplier;
        characterIndex = _characterIndex;
        nameFruta = nameFrutaIndex;
    }

    public DialogueQuote(List<string> _dialogueContent, AudioClip _characterVoice, int _characterIndex,string nameFrutaIndex)
    {
        dialogueContent = _dialogueContent;
        characterVoice = new AudioClip[] { _characterVoice };
        speedMultiplier = 1;
        characterIndex = _characterIndex;
        nameFruta = nameFrutaIndex;
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
    public string GetNameFruta() { return nameFruta; }

}

public class Data_Cutscenes
{
    public static AudioClip VozPera = Resources.Load<AudioClip>("SFX/SFX_talkBichito");
    public static AudioClip VozLimon = Resources.Load<AudioClip>("SFX/SFX_talkBichito");
    public static AudioClip VozFresa = Resources.Load<AudioClip>("SFX/SFX_talkHermano");
    public static AudioClip VozPlatano = Resources.Load<AudioClip>("SFX/SFX_talkHermano");

    public static Dictionary<string, DialogueQuote> data_CutsceneIntro = new Dictionary<string, DialogueQuote>()
    {
        //Lorem ipsum dolor sit amet, consectetur adipiscing elit. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        //Intro
        {"OpeningSecretary_LilBug_1",    new DialogueQuote( new List<string>{
            "Recordad chicos, tiene que ser un atraco rápido y sencillo sin ningún daño colateral.",
            "English"
            },
            VozPera,
            3,
            "Seikan")
        },
        {"OpeningSecretary_Brother_2", new DialogueQuote(
            new List<string>{
                "Eso lo dice por ti Daved. Esta vez no vale que ataques a ningún guardia. La mejor manera de no ser vistos es usando el sigilo a nuestro favor.",
                "English"
            },
            VozPera,
            2,
            "Luu"
        )},

        {"OpeningSecretary_LilBug_3", new DialogueQuote(
            new List<string>{
                "Ir con cuidado y sin hacer ruido. Eso no es divertido.",
                "English"
            },
            VozPera,
            1,
            "Daved"
        )},

        {"OpeningSecretary_Brother_4", new DialogueQuote(
            new List<string>{
                "Luu tiene razón, no sabemos a que tipo de enemigos nos enfrentamos. Con algunos bastara simplemente con evitarlos yendo por caminos diferentes.",
                "English"
            },
            VozPera,
            0,
            "Quesada"
        )},

        {"OpeningSecretary_Brother_5", new DialogueQuote(
            new List<string>{
                "Pero con otros, necesitaremos usar diferentes herramientas del castillo para poder salir ilesos.",
                "English"
            },
            VozPera,
            0,
            "Quesada"
        )},

        {"OpeningSecretary_LilBug_6", new DialogueQuote(
            new List<string>{
                "Pero que estás diciendo. Es simplemente robar en un castillo no conquistar el reino. No debe ser tan difícil hacer algo que hago todos los días.",
                "English"
            },
            VozPera,
            1,
            "Daved"
        )},

        {"OpeningSecretary_Brother_7", new DialogueQuote(
            new List<string>{
                "Quieres dejar de bromear. Yo he oído que los guardias de ese castillo han conseguido paralizar a sus víctimas e incluso cegarlas.",
                "English"
            },
            VozPera,
            2,
            "Luu"
        )},

        {"OpeningSecretary_Brother_8", new DialogueQuote(
            new List<string>{
                "Asi que para poder tener éxito hay que usar la astucia y el ingenio. No ir ahí como un loco y sin pensar.",
                "English"
            },
            VozPera,
            2,
            "Luu"
        )},

        {"OpeningSecretary_Brother_9", new DialogueQuote(
            new List<string>{
                "Me estas llamando tonto, enana de mierda.",
                "English"
            },
            VozPera,
            1,
            "Daved"
        )},

        {"OpeningSecretary_LilBug_10", new DialogueQuote(
            new List<string>{
                "Quieres ver como te patea esta enana.",
                "English"
            },
            VozPera,
            2,
            "Luu"
        )},

        {"OpeningSecretary_LilBug_11", new DialogueQuote(
            new List<string>{
                "Ya empiezan otra vez . . .",
                "English"
            },
            VozPera,
            0,
            "Quesada"
        )},

        {"OpeningSecretary_Brother_12", new DialogueQuote(
            new List<string>{
                "Nada de peleas en mi sótano. Si queréis mataros, salir a la calle. Pero antes debemos realizar el robo.",
                "English"
            },
            VozPera,
            3,
            "Seikan"
        )},

        {"OpeningSecretary_Brother_13", new DialogueQuote(
            new List<string>{
                "Recordad, nadie sale del castillo sin las tres piezas del osito de oro. Ánimo equipo.",
                "English"
            },
            VozPera,
            3,
            "Seikan"
        )},

    };

}
