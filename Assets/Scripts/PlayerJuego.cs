using System;
using System.Collections;
using UnityEngine;

public class PlayerJuego : MonoBehaviour
{
    //DECLARACION DE VARIABLES

    [Header("Interacion con Unity")]
    public static PlayerJuego playerJuego;
    private Rigidbody2D rb;
    public ControlSuelo controlSuelo;
    private ControlPared controlPared;
    private ControlEscaleras controlEscaleras;
    private PlataformasAtravesables plataformasAtravesables;
    private Animator animator;


    public enum Estado
    {
        Idle,
        Run,
        Jump,
        Caida,
        DeslizarseSuelo,
        DeslizarseAire,
        DeslizandoPared,
        DispararIdle,
        DispararCorriendo,
        DispararSaltando,
        EscaleraAscender,
        EscaleraEstatico,
        EscaleraDescender,
        EscaleraDispara
    };
    public Estado estadoActual = Estado.Idle;


    [Header("Movimiento")]
    [SerializeField] private float velocidadMovimiento;
    [Range((float)0, 3)][SerializeField] private float resvalar;
    [SerializeField] private float inputX;
    [SerializeField] public float inputY;
    private Vector3 velocidad = Vector3.zero;
    private float movimientoHorizontal = 0f;
    private bool mirarDerecha = true;

    [Header("Saltar")]
    [SerializeField] private float fuezaDeSalto;
    [SerializeField] private int saltosMaximos;
    [SerializeField] private int saltosRestantes;
    [SerializeField] private float velocidadY;

    [Header("Deslizarse Suelo/Aire")]
    [SerializeField] private float velocidadDeslizarse;
    [SerializeField] private float tiempoDeslizando;
    [SerializeField] private int deslizamientosTotales;
    [SerializeField] private int deslizamientosRestantes;
    private float gravedadInicial;
    private bool puedeDeslizarseEnX = true;
    private bool sePuedeMover = true;

    [Header("Deslizarse en pared")]
    [SerializeField] private float velocidadDeslizamientoY;
    [SerializeField] private bool estaEnPared;
    [SerializeField] private bool estaDeslizandoseY;

    [Header("Salto Pared")]
    [SerializeField] private float fuerzaSaltoParedX;
    [SerializeField] private float fuerzaSaltoParedY;
    [SerializeField] private float tiempoSaltoPared;
    private bool saltandoDePared;

    [Header("Escaleras")]
    [SerializeField] private float velocidadEnEscalera;
    [SerializeField] public bool estaEnEscalera;
    [SerializeField] private bool escalando;
    private Vector2 posicionSaltoEscalera;


    // EJECUCION DEL JUEGO

    private void Awake()
    {
        playerJuego = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controlSuelo = GameObject.Find("controlSuelo").GetComponent<ControlSuelo>();
        controlPared = GameObject.Find("controlPared").GetComponent<ControlPared>();
        controlEscaleras = GameObject.Find("controlEscalera").GetComponent<ControlEscaleras>();
        plataformasAtravesables = GameObject.Find("PlataformasAtravesables").GetComponent<PlataformasAtravesables>();
        gravedadInicial = rb.gravityScale;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        movimientoHorizontal = inputX * velocidadMovimiento;
        velocidadY = rb.velocity.y;
        estaEnPared = controlPared.estaEnPared;
        estaEnEscalera = controlEscaleras.estaEnEscalera;
        estaDeslizandoseY = false;

        if (!puedeDeslizarseEnX)
        {
            Shadows.me.SombrasSkill();
        }

        if (controlPared.estaEnPared && velocidadY < 0 && inputX != 0)
        {
            estaDeslizandoseY = true;
            DeslizarseEnPared();
        }

        if (estaEnEscalera && inputX != 0 && (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            Debug.Log("Saltando desde la escalera.");
            posicionSaltoEscalera = rb.position;
            SaltarEscalera();
        }
        else if (estaEnEscalera && inputY == 0 && (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button0)))
        {
            saltosRestantes = 1;
            Debug.Log("Salir Escalera");
            SalirDeEscalera();
        }

        if (sePuedeMover && Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            if (estaDeslizandoseY && estaEnPared)
            {
                SaltarPared();
            }
            else
            {
                Saltar();
            }
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Joystick1Button1) && puedeDeslizarseEnX)
        {
            StartCoroutine(Deslizarse());
        }


        // La animacion de salto tiene que ir en el update por que si se hace en el metodo no comprueva en tiempo real cual es la animacion que toca en el caso de subir o bajar.

        if (velocidadY > 0 && !controlSuelo.estaEnSuelo && !controlEscaleras.estaEnEscalera)
        {
            estadoActual = Estado.Jump;
        }
        else if (velocidadY < 0 && !controlSuelo.estaEnSuelo && !controlPared.estaEnPared && !escalando)
        {
            estadoActual = Estado.Caida;
        }
        animator.SetFloat("EstadoActual", (int)estadoActual);
    }

    private void FixedUpdate()
    {
        if (sePuedeMover && !saltandoDePared)
        {
            Movimiento(movimientoHorizontal * Time.fixedDeltaTime);
        }

        Escaleras();
    }

    // METODOS.

    private void Escaleras()
    {
        if ((inputY != 0 || escalando) && controlEscaleras.estaEnEscalera)
        {
            Vector2 nuevaPosicion = new Vector2(controlEscaleras.escaleraActiva.transform.position.x, rb.position.y);
            rb.position = nuevaPosicion;
            Vector2 velocidadSubida = new Vector2(inputX * velocidadEnEscalera, inputY * velocidadEnEscalera);
            rb.velocity = velocidadSubida;
            rb.gravityScale = 0;
            escalando = true;

            if (inputY > 0)
            {
                estadoActual = Estado.EscaleraAscender;
            }
            else if (inputY == 0)
            {
                estadoActual = Estado.EscaleraEstatico;
            }
            else if (inputY < 0 && controlEscaleras.estaEnEscalera)
            {
                estadoActual = Estado.EscaleraDescender;
            }
            animator.SetFloat("EstadoActual", (int)estadoActual);
        }
        else
        {
            rb.gravityScale = gravedadInicial;
            escalando = false;
        }

        if (controlSuelo.estaEnSuelo)
        {
            escalando = false;
        }
    }

    private IEnumerator Deslizarse()
    {
        sePuedeMover = false;
        puedeDeslizarseEnX = false;

        // Condicionales para limitar los deslizamientos en el aire.

        if (controlSuelo.estaEnSuelo == true)
        {
            deslizamientosRestantes = deslizamientosTotales;
        }
        else if (deslizamientosRestantes > 0)
        {
            deslizamientosRestantes--;
        }
        else
        {
            yield break;
        }

        rb.gravityScale = 0f;
        rb.velocity = new Vector2(velocidadDeslizarse * transform.localScale.x, 0);

        //ANIMACION DEL METODO

        if (controlSuelo.estaEnSuelo)
        {
            estadoActual = Estado.DeslizarseSuelo;
        }
        else
        {
            estadoActual = Estado.DeslizarseAire;
        }
        animator.SetFloat("EstadoActual", (int)estadoActual);


        yield return new WaitForSeconds(tiempoDeslizando);

        sePuedeMover = true;
        rb.gravityScale = gravedadInicial;
        puedeDeslizarseEnX = true;
    }

    private void DeslizarseEnPared()
    {
        estadoActual = Estado.DeslizandoPared;
        animator.SetFloat("EstadoActual", (int)estadoActual);

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -velocidadDeslizamientoY, float.MaxValue));
    }

    private void Movimiento(float mover)
    {
        Vector3 velocidadObjetivo = new Vector2(mover, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, velocidadObjetivo, ref velocidad, resvalar);

        //ANIMACION DEL METODO

        if (inputX != 0 && controlSuelo.estaEnSuelo)
        {
            estadoActual = Estado.Run;
        }
        else
        {
            estadoActual = Estado.Idle;
        }
        animator.SetFloat("EstadoActual", (int)estadoActual);

        if (inputX > 0 && !mirarDerecha)
        {
            GirarOrientacion();
        }
        else if (inputX < 0 && mirarDerecha)
        {
            GirarOrientacion();
        }

    }

    private void Saltar()
    {
        if (!puedeDeslizarseEnX)  // evita salto mientras se desliza.
        {
            return;
        }

        if (controlSuelo.estaEnSuelo == true)
        {
            saltosRestantes = saltosMaximos;
        }
        else if (saltosRestantes > 1)
        {
            saltosRestantes--;
        }
        else
        {
            return;
        }

        deslizamientosRestantes = deslizamientosTotales;
        rb.velocity = new Vector2(rb.velocity.x, fuezaDeSalto);

    }

    private void SaltarPared()
    {
        rb.velocity = new Vector2(fuerzaSaltoParedX * -inputX, fuerzaSaltoParedY);
        StartCoroutine(CambioSaltoPared());
    }

    private void SaltarEscalera()
    {
        rb.gravityScale = gravedadInicial; // Restaurar la gravedad
        escalando = false; // Dejar de escalar

        // Calcular la dirección del salto (hacia arriba y posiblemente hacia la derecha o izquierda)
        Vector2 direccionSalto = new Vector2(inputX, 1).normalized;

        // Aplicar el salto desde la posición en la escalera
        rb.position = posicionSaltoEscalera;
        rb.velocity = new Vector2(rb.velocity.x, 0); // Detener la velocidad vertical actual
        rb.AddForce(direccionSalto * fuezaDeSalto, ForceMode2D.Impulse);
    }

    private void SalirDeEscalera()
    {
        rb.gravityScale = gravedadInicial;
        escalando = false;
    }

    IEnumerator CambioSaltoPared()
    {
        saltandoDePared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoDePared = false;
    }

    private void GirarOrientacion()
    {
        mirarDerecha = !mirarDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
