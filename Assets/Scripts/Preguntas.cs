using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class Preguntas : MonoBehaviour
{

    public GameObject panelPregunta;
    public GameObject panelRespuesta;

    public bool respuestaValida;

    public float tiempoRestante = 10f;
    public TextMeshProUGUI tiempoText;
    private bool tiempoAgotado = false;

    public Dificultad[] bancoPreguntas;
    public TextMeshProUGUI enunciado;
    public TextMeshProUGUI[] respuestas;
    public TextMeshProUGUI solucion;
    public int nivelPregunta;
    public Pregunta preguntaActual;

    public Button[] btnRespuesta;

    private bool isInside = false; // Variable para controlar si el jugador est� dentro del objeto planeta
    private PlayerController playerController;
    

    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();
        cargarBancoPreguntas();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isInside)
        {
            if (!tiempoAgotado)
            {
                tiempoRestante -= Time.deltaTime; // Reducir el tiempo restante
                tiempoText.text = Mathf.CeilToInt(tiempoRestante).ToString(); // Actualizar el texto del tiempo

                if (tiempoRestante <= 0)
                {
                    tiempoAgotado = true;
                    tiempoText.text = "0";
                    playerController.SetRespondioIncorrectamente(true);
                }
            }
            StartCoroutine(retrasoPregunta());
            //Debug.Log("Jugador dentro del planeta");
        }
        else
        {
            panelPregunta.SetActive(false);
            tiempoRestante = 10f; // Reiniciar el tiempo cuando el jugador sale del planeta
            tiempoAgotado = false;
            tiempoText.text = "10"; // Actualizar el texto del tiempo
            //Debug.Log("Jugador fuera del planeta");
        }
    }

    public void setPregunta()
    {
        int preguntaRandom = Random.Range(0, bancoPreguntas[nivelPregunta].preguntas.Length);
        preguntaActual = bancoPreguntas[nivelPregunta].preguntas[preguntaRandom];
        enunciado.text = preguntaActual.enunciado;
        
        for (int i = 0; i < respuestas.Length; i++)
        {
            respuestas[i].text = preguntaActual.respuestas[i].texto;
        }

        solucion.text = preguntaActual.solucion;
    }

    public void cargarBancoPreguntas()
    {
        try
        {
            bancoPreguntas =
                JsonConvert
                    .DeserializeObject<Dificultad[]>(File
                        .ReadAllText(Application.streamingAssetsPath +
                        "/QuestionBank.json"));
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            enunciado.text = ex.Message;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isInside = true;
            setPregunta();
            Debug.Log("Jugador entrando al planeta");
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isInside = false;
            Debug.Log("Jugador saliendo del planeta");
            playerController.Stop();
        }
    }

    public void evaluarPregunta(int respuestaJugador)
    {
        if (respuestaJugador == preguntaActual.respuestaCorrecta)
        {
            //respuestaValida = true;
            Debug.Log("respuesta correcta");


            panelRespuesta.SetActive(true);
            
            tiempoRestante = 10f;
            
            StartCoroutine(esperarYContinuar());
            

            /*if (nivelPregunta == bancoPreguntas.Length)
            {
                SceneManager.LoadScene("Creditos");
            }*/

        }
        else
        {
            Debug.Log("respuesta mala");
            
            //devolverse una casilla
            playerController.SetRespondioIncorrectamente(true);
        }
    }

    IEnumerator retrasoPregunta()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (isInside)
        {
            panelPregunta.SetActive(true);
        }
        
    }

    IEnumerator esperarYContinuar()
    {
        // Esperar durante 5 segundos antes de continuar
        yield return new WaitForSecondsRealtime(5f);

        // Desactivar el panel de respuesta
        
        panelRespuesta.SetActive(false);

        // Continuar con el juego
        playerController.Move();


    }

}
