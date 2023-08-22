using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float speedBala;
    [SerializeField] private float tiempoDeVida;

    //private void Start()
    //{
    //    StartCoroutine(DesactivarTiempo(tiempoDeVida));
    //}

    private void OnEnable()
    {
        Invoke("DestruirBala", tiempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector3.left * speedBala * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemigos"))
        {
            Destroy(gameObject);
        }
    }

    private void DestruirBala()
    {
        Destroy(gameObject);
    }

    //private IEnumerator DesactivarTiempo(float tiempoDeVida)
    //{
    //    Debug.Log("Inicio del tiempo de vida de la bala");
    //    yield return new WaitForSeconds(tiempoDeVida);
    //    Destroy(this, GameObject, tiempoDeVida);
    //    Debug.Log("Bala destruida");
    //}
}
