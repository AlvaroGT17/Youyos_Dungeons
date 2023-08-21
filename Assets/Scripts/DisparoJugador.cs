using UnityEngine;
using UnityEngine.Pool;

public class DisparoJugador : MonoBehaviour
{
    [SerializeField] private Transform controladorDisparo;
    [SerializeField] private DisparoJugador[] balaPrefab;
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

    private void OnEnable()
    {
        balasPool = new ObjectPool<DisparoJugador>(() =>
        {
            Bala bala = Instantiate(tipoBala, controladorDisparo.position, controladorDisparo.rotation);
            bala.DesactivarBala(DesactivarBala);
            return bala;
        }, bala =>
        {
            bala.transform.position = controladorDisparo.position;
            bala.gameObject.SetActive(true);
        }, bala =>
        {
            bala.gameObject.SetActive(false);
        }, bala =>
        {
            Destroy(bala.gameObject);
        }, true, 10, 25);
    }

    private void Disparar(int tiempoCarga)
    {
        balasPool.Get();
    }

    private void DesactivarBalaPool(DisparoJugador bala)
    {
        balasPool.Release(bala);
    }

}