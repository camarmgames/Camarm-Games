using UnityEngine;

public class CharacterSelection: MonoBehaviour
{
    public static CharacterSelection Instance;

    public int selectedCharacterIndex = 0;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
        Debug.Log("Personaje seleccionado: " + index);
    }
}
