using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [Header ("Canvas")]
    public GameObject menuPrincipal;
    public GameObject creditos;
    public GameObject controles1;
    public GameObject controles2;

    //Mostrar el menu principal
    public void AbrirMenuPrincipal()
    {
        menuPrincipal.SetActive(true);
        creditos.SetActive(false);
        controles1.SetActive(false);
        controles2.SetActive(false);
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
}
