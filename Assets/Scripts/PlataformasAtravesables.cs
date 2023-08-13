using UnityEngine;

public class PlataformasAtravesables : MonoBehaviour
{
    public PlayerJuego playerJuego;
    public ControlEscaleras controlEscaleras;
    private GameObject player;
    private BoxCollider2D ccPlayer;
    public BoxCollider2D ccPlataforma;
    private Bounds ccPlataformaBounds;
    [SerializeField] private int topPlataforma;
    [SerializeField] private int piesPlayer;
    [SerializeField] private bool escaleraDetectada;
    [SerializeField] private float inputY;


    // Start is called before the first frame update
    void Start()
    {
        playerJuego = GameObject.Find("PlayerTesteo").GetComponent<PlayerJuego>();
        controlEscaleras = GameObject.Find("Escaleras").GetComponent<ControlEscaleras>();
        player = GameObject.FindGameObjectWithTag("Player");
        ccPlayer = player.GetComponent<BoxCollider2D>();
        ccPlataforma = GetComponent<BoxCollider2D>();
        ccPlataformaBounds = ccPlataforma.bounds;
        topPlataforma = (int)ccPlataformaBounds.center.y + (int)ccPlataformaBounds.extents.y;
        inputY = playerJuego.inputY;
    }

    // Update is called once per frame
    void Update()
    {
        piesPlayer = (int)player.transform.position.y - (int)ccPlayer.size.y / 2;

        if (ccPlataforma.isTrigger && (piesPlayer > topPlataforma))
        {
            ccPlataforma.isTrigger = false;
            gameObject.tag = "Background";
            gameObject.layer = LayerMask.NameToLayer("Background");
        }
        else if (!ccPlataforma.isTrigger && (piesPlayer < topPlataforma - 0.1f))
        {
            ccPlataforma.isTrigger = true;
            gameObject.tag = "PlataformasAtravesables";
            gameObject.layer = LayerMask.NameToLayer("PlataformasAtravesables");
        }

        if (ControlEscaleras.controlEscaleras.estaEnEscalera && PlayerJuego.playerJuego.inputY < 0)
        {
            Debug.Log("Puede Bajar por la escalera");
            ccPlataforma.isTrigger = true;
            gameObject.tag = "PlataformasAtravesables";
            gameObject.layer = LayerMask.NameToLayer("PlataformasAtravesables");
        }
    }
}
