using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class DataBaseManager : MonoBehaviour
{
    private static DataBaseManager _instancia;

    public static DataBaseManager Instancia
    {
        get
        {
            if (_instancia == null)
            {
                _instancia = FindAnyObjectByType<DataBaseManager>();

                if (_instancia == null)
                {
                    GameObject go = new GameObject("DataBaseManager");
                    _instancia = go.AddComponent<DataBaseManager>();
                    DontDestroyOnLoad(go);
                }
            }

            return _instancia;
        }
    }

    [Header("Configuración API")]
    [SerializeField] private string urlAPI = "http://localhost:8080/Juego/game.php";

    private void Awake()
    {
        if (_instancia != null && _instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        _instancia = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GuardarPartida(GameData datos)
    {
        StartCoroutine(CoroutineGuardar(datos));
    }

    private IEnumerator CoroutineGuardar(GameData datos)
    {
        string json = JsonUtility.ToJson(datos);

        Debug.Log("Enviando JSON: " + json);

        using (UnityWebRequest request = new UnityWebRequest(urlAPI, "POST"))
        {
            byte[] cuerpo = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(cuerpo);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            Debug.Log("Código HTTP: " + request.responseCode);
            Debug.Log("Respuesta servidor: " + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Partida guardada correctamente.");
            }
            else
            {
                Debug.LogError("Error al guardar: " + request.error);
            }
        }
    }

    public IEnumerator CargarPartida(string jugadorId, System.Action<GameData> alCompletar)
    {
        string url = urlAPI + "?jugador_id=" + jugadorId;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            Debug.Log("Respuesta carga: " + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                GameData datos = JsonUtility.FromJson<GameData>(request.downloadHandler.text);
                alCompletar?.Invoke(datos);
            }
            else
            {
                Debug.LogError("Error al cargar: " + request.error);
                alCompletar?.Invoke(null);
            }
        }
    }
}