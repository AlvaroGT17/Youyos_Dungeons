using UnityEngine;

public class ControlPared : MonoBehaviour
{
    private PlayerJuego PlayerJuegoPared;
    [SerializeField] public bool estaEnPared;

    private void Start()
    {
        PlayerJuegoPared = GameObject.Find("PlayerTesteo").GetComponent<PlayerJuego>();
        estaEnPared = false; // Establecemos el valor inicial como falso.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Deslizadores"))
        {
            //Debug.Log("Colisionando con pared");
            estaEnPared = true; // Establecemos la variable como verdadera al colisionar con la pared.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Deslizadores"))
        {
            estaEnPared = false; // Establecemos la variable como falsa al salir de la pared.
        }
    }
}