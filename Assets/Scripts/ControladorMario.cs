using UnityEngine;
using UnityEngine.InputSystem;

public class ControladorMario : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float velocidadRotacion = 360f;

    [Header("Base de Datos")]
    [SerializeField] private bool autoGuardar = true;
    [SerializeField] private float intervaloAutoGuardado = 10f;
    [SerializeField] private float distanciaAutoGuardado = 5f;

    private DataBaseManager dbManager;
    private string jugadorId;
    private string jugadorNombre = "Mario";
    private int puntuacion = 0;
    private int vida = 100;
    private int nivel = 1;

    private Vector3 ultimaPosicionGuardada;
    private float temporizadorAutoGuardado = 0f;

    private Animator animator;
    private Rigidbody rb;

    private float inputX;
    private float inputZ;
    private Vector3 direccionMovimiento;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        Debug.Log("=== INICIANDO JUEGO ===");

        dbManager = DataBaseManager.Instancia;

        CargarOCrearIdJugador();
        CargarPartida();

        ultimaPosicionGuardada = transform.position;
    }

    void Update()
    {
        Vector2 entrada = Vector2.zero;
        Keyboard teclado = Keyboard.current;

        if (teclado != null)
        {
            if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed)
                entrada.y = 1f;

            if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed)
                entrada.y = -1f;

            if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed)
                entrada.x = 1f;

            if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed)
                entrada.x = -1f;
        }

        inputX = entrada.x;
        inputZ = entrada.y;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        direccionMovimiento = (forward * inputZ + right * inputX).normalized;

        if (animator != null)
        {
            float suavizado = Time.deltaTime * 10f;
            float xVal = Mathf.Lerp(animator.GetFloat("xVal"), inputX, suavizado);
            float yVal = Mathf.Lerp(animator.GetFloat("yVal"), inputZ, suavizado);

            animator.SetFloat("xVal", xVal);
            animator.SetFloat("yVal", yVal);
        }

        if (autoGuardar && dbManager != null)
        {
            temporizadorAutoGuardado += Time.deltaTime;

            if (temporizadorAutoGuardado >= intervaloAutoGuardado)
            {
                temporizadorAutoGuardado = 0f;
                GuardarPartida("Auto-guardado por tiempo");
            }

            float distanciaMovida = Vector3.Distance(transform.position, ultimaPosicionGuardada);

            if (distanciaMovida >= distanciaAutoGuardado)
            {
                ultimaPosicionGuardada = transform.position;
                GuardarPartida("Auto-guardado por distancia");
            }
        }

        if (teclado != null)
        {
            if (teclado.gKey.wasPressedThisFrame)
                GuardarPartida("Manual");

            if (teclado.lKey.wasPressedThisFrame)
                CargarPartida();

            if (teclado.cKey.wasPressedThisFrame)
                MostrarEstado();

            if (teclado.xKey.wasPressedThisFrame)
                SumarPuntos(10);

            if (teclado.zKey.wasPressedThisFrame)
                RecibirDanio(10);

            if (teclado.rKey.wasPressedThisFrame)
                Curar(10);
        }
    }

    void FixedUpdate()
    {
        if (direccionMovimiento.magnitude > 0.01f)
        {
            float dt = Time.fixedDeltaTime;
            Vector3 desplazamiento = direccionMovimiento * velocidadMovimiento * dt;

            if (rb != null)
            {
                rb.MovePosition(rb.position + desplazamiento);

                if (inputZ >= 0)
                {
                    Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
                    rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, rotacionObjetivo, velocidadRotacion * dt));
                }
                else if (Mathf.Abs(inputX) > 0.01f)
                {
                    float giro = inputX * velocidadRotacion * dt;
                    rb.MoveRotation(rb.rotation * Quaternion.Euler(0, giro, 0));
                }
            }
        }
    }

    private void CargarOCrearIdJugador()
    {
        if (PlayerPrefs.HasKey("JugadorId"))
        {
            jugadorId = PlayerPrefs.GetString("JugadorId");
            Debug.Log("ID existente: " + jugadorId);
        }
        else
        {
            jugadorId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("JugadorId", jugadorId);
            PlayerPrefs.Save();

            Debug.Log("Nuevo ID creado: " + jugadorId);
        }
    }

    private void GuardarPartida(string motivo)
    {
        if (dbManager == null)
            return;

        GameData datos = new GameData();

        datos.jugador_id = jugadorId;
        datos.jugador_nombre = jugadorNombre;
        datos.puntuacion = puntuacion;
        datos.posicion_x = transform.position.x;
        datos.posicion_z = transform.position.z;
        datos.vida = vida;
        datos.nivel = nivel;

        dbManager.GuardarPartida(datos);

        Debug.Log("Partida guardada (" + motivo + ") - Puntos: " + puntuacion);
    }

    private void CargarPartida()
    {
        if (dbManager == null)
            return;

        StartCoroutine(dbManager.CargarPartida(jugadorId, (datos) =>
        {
            if (datos != null)
            {
                jugadorNombre = datos.jugador_nombre;
                puntuacion = datos.puntuacion;
                vida = datos.vida;
                nivel = datos.nivel;

                Vector3 posicion = new Vector3(datos.posicion_x, transform.position.y, datos.posicion_z);
                transform.position = posicion;
                ultimaPosicionGuardada = posicion;

                if (rb != null)
                    rb.position = posicion;

                Debug.Log("Partida cargada - Puntos: " + puntuacion + ", Vida: " + vida);
            }
            else
            {
                Debug.Log("Nueva partida - Datos por defecto");
            }
        }));
    }

    public void SumarPuntos(int puntos)
    {
        puntuacion += puntos;

        Debug.Log("+" + puntos + " puntos. Total: " + puntuacion);

        GuardarPartida("Suma de puntos");
    }

    public void RecibirDanio(int danio)
    {
        vida -= danio;

        if (vida < 0)
            vida = 0;

        Debug.Log("Daño recibido: -" + danio + ". Vida restante: " + vida);

        GuardarPartida("Daño recibido");
    }

    public void Curar(int cantidad)
    {
        vida += cantidad;

        if (vida > 100)
            vida = 100;

        Debug.Log("Curación: +" + cantidad + ". Vida actual: " + vida);

        GuardarPartida("Curación");
    }

    private void MostrarEstado()
    {
        Debug.Log("=== ESTADO DEL JUGADOR ===");
        Debug.Log("ID: " + jugadorId);
        Debug.Log("Nombre: " + jugadorNombre);
        Debug.Log("Puntuación: " + puntuacion);
        Debug.Log("Vida: " + vida);
        Debug.Log("Nivel: " + nivel);
        Debug.Log("Posición: (" + transform.position.x.ToString("F2") + ", " + transform.position.z.ToString("F2") + ")");
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 220, 90), "=== MARIO ===");
        GUI.Label(new Rect(20, 35, 200, 25), "PUNTOS: " + puntuacion);
        GUI.Label(new Rect(20, 55, 200, 25), "VIDA: " + vida);
        GUI.Label(new Rect(20, 75, 200, 25), "NIVEL: " + nivel);

        GUI.Box(new Rect(10, Screen.height - 120, 280, 105), "=== CONTROLES ===");
        GUI.Label(new Rect(20, Screen.height - 95, 260, 20), "WASD / Flechas: Movimiento");
        GUI.Label(new Rect(20, Screen.height - 75, 260, 20), "G: Guardar | L: Cargar | C: Estado");
        GUI.Label(new Rect(20, Screen.height - 55, 260, 20), "X: Puntos | Z: Daño | R: Curar");
    }
}