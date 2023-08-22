using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    //[SerializeField] private DisparoJugador[] balaPrefab;
    [SerializeField] private List<GameObject> balaPrefab = new List<GameObject>();
    [SerializeField] private float maximaCarga;
    [SerializeField] private float tiempoCarga;
    [SerializeField] private float tiempoEntreDisparos;
    private int tipoBala;
    private float tiempoSiguienteDisparo = 0;
    private ObjectPool<DisparoJugador> balasPool;


    private void Update()
    {
        if (tiempoSiguienteDisparo > 0)
        {
            tiempoSiguienteDisparo -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.V))
        {

            if (tiempoCarga <= maximaCarga)
            {
                tiempoCarga += Time.deltaTime;
            }
        }
        if (Input.GetKeyUp(KeyCode.V))
        {
            if (tiempoSiguienteDisparo <= 0)
            {
                tipoBala = (int)tiempoCarga;
                Disparar((int)tiempoCarga);
                tiempoCarga = 0;
                tiempoSiguienteDisparo = tiempoEntreDisparos;
            }
        }
    }

    private void Disparar(int tiempoCarga)
    {
        Instantiate(balaPrefab[tiempoCarga], controladorDisparo.position, controladorDisparo.rotation);
    }

}