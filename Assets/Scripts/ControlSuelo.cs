using UnityEngine;

public class ControlSuelo : MonoBehaviour
{
    public PlayerJuego playerJuego;
    public bool estaEnSuelo;

    void Start()
    {
        playerJuego = GameObject.Find("PlayerTesteo").GetComponent<PlayerJuego>();
        estaEnSuelo = false; // Establecemos el valor inicial como falso.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Background"))
        {
            //Debug.Log("Colisionando con suelo");
            estaEnSuelo = true; // Establecemos la variable como verdadera al colisionar con el suelo.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Background") || other.CompareTag("PlataformasAtravesables"))
        {
            estaEnSuelo = false; // Establecemos la variable como falsa al salir del suelo.
        }
    }
}
