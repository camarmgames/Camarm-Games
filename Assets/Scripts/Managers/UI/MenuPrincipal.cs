using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [Header ("Canvas")]
    public GameObject menuPrincipal;
    public GameObject creditos;
    public GameObject creditos2;
    public GameObject creditos3;
    public GameObject ajustes;
    public GameObject controles1;
    public GameObject controles2;

    public AudioClip musicMainmenu;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(musicMainmenu);
    }
    //Mostrar el menu principal
    public void AbrirMenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        creditos.SetActive(false);
        controles1.SetActive(false);
        controles2.SetActive(false);
        creditos2.SetActive(false);
        creditos3.SetActive(false);
        ajustes.SetActive(false);
    }

    public void AbrirCreditos()
    {
        menuPrincipal.SetActive(false);
        creditos.SetActive(true);
    }

    public void AbrirControles()
    {
        menuPrincipal.SetActive(false);
        controles1.SetActive(true);
    }

    public void AbrirAjustes()
    {
        menuPrincipal.SetActive(false);
        ajustes.SetActive(true);
    }
    public void PasarPagina()
    {
        controles1.SetActive(false);
        controles2.SetActive(true);
    }

    public void PaginaAtras()
    {
        controles1.SetActive(true);
        controles2.SetActive(false);
    }
    public void PasarPaginaC()
    {
        creditos.SetActive(false);
        creditos2.SetActive(true);
    }
    public void PasarPaginaC2()
    {
        creditos2.SetActive(false);
        creditos3.SetActive(true);
    }
    public void PaginaAtrasC()
    {
        creditos.SetActive(true);
        creditos2.SetActive(false);
    }
    public void PaginaAtrasC2()
    {
        creditos2.SetActive(true);
        creditos3.SetActive(false);
    }
}
