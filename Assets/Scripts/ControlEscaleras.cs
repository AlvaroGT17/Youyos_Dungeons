using UnityEngine;

public class ControlEscaleras : MonoBehaviour
{
    [SerializeField] public bool estaEnEscalera;
    private PlayerJuego PlayerJuegoEscalera;
    public GameObject escaleraActiva;

    private void Start()
    {
        PlayerJuegoEscalera = GameObject.Find("PlayerTesteo").GetComponent<PlayerJuego>();
        estaEnEscalera = false; // Establecemos el valor inicial como falso.
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escaleras"))
        {
            //Debug.Log("Colisionando con Escalera");
            estaEnEscalera = true; // Establecemos la variable como verdadera al colisionar con la escalera.
            escaleraActiva = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escaleras"))
        {
            estaEnEscalera = false; // Establecemos la variable como falsa al salir de la escalera.
        }
    }
}