using System;
using System.Collections;
using UnityEngine;

public class Bala : MonoBehaviour
{
    [SerializeField] private float speedBala;
    [SerializeField] private float tiempoDeVida;
    private Action<Bala> desactivarAccion;

    private void Start()
    {
        StartCoroutine(DesactivarTiempo());
    }
    void Update()
    {
        transform.Translate(Vector3.left * speedBala * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemigos"))
        {
            desactivarAccion(this);
        }
    }

    private IEnumerator DesactivarTiempo()
    {
        yield return new WaitForSeconds(tiempoDeVida);
        desactivarAccion(this);
    }

    public void DesactivarBala(Action<Bala> desactivarAccionParametro)
    {
        desactivarAccion = desactivarAccionParametro;
        desactivarAccion(this);
    }


}
