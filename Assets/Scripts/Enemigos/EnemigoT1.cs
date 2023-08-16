using System.Collections;
using UnityEngine;

public class EnemigoT1 : MonoBehaviour
{
    [Header("Interactuar con Unity")]
    private Rigidbody2D rb2dEnemigo;

    [SerializeField] private float velocidadDeMovimiento;
    [SerializeField] private float distancia;
    [SerializeField] private float levitationAmplitude;
    [SerializeField] private float levitationSpeed;
    [SerializeField] LayerMask queEsSuelo;
    private bool mirarDerecha;
    private float initialY;
    private bool informacionSuelo;
    private bool puedeGirar = true;


    void Start()
    {
        rb2dEnemigo = GetComponent<Rigidbody2D>();
        initialY = transform.position.y;
        informacionSuelo = false; // Establecemos el valor inicial como falso.
        mirarDerecha = true;
    }

    void Update()
    {
        rb2dEnemigo.velocity = new Vector2(velocidadDeMovimiento * transform.right.x, rb2dEnemigo.velocity.y);

        float verticalOffset = levitationAmplitude * Mathf.Sin(Time.time * levitationSpeed);
        Vector3 newPosition = new Vector3(transform.position.x, initialY + verticalOffset, transform.position.z);
        transform.position = newPosition;

        if (informacionSuelo && puedeGirar)
        {
            StartCoroutine(Girar());
        }

    }
    private IEnumerator Girar()
    {
        // Deshabilita la capacidad de girar temporalmente.
        puedeGirar = false;

        // Realiza el giro.
        mirarDerecha = !mirarDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);

        // Espera durante un segundo.
        yield return new WaitForSeconds(1.0f);

        // Habilita la capacidad de girar nuevamente.
        puedeGirar = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Background"))
        {
            //Debug.Log("Colisionando con suelo");
            informacionSuelo = true; // Establecemos la variable como verdadera al colisionar con el suelo.
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Background"))
        {
            informacionSuelo = false; // Establecemos la variable como falsa al salir del suelo.
        }
    }
}