using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento del astronauta.
    private int currentPlanetIndex = 0; // �ndice del planeta actual.
    private int previousPlanetIndex = 0; // �ndice del planeta anterior

    public GameObject[] planets; // Un arreglo para almacenar todos los planetas en la escena.
    public GameObject perdida;
    private bool isMoving = false;
    private float journeyLength;
    private float startTime;
    private Transform destination;
    private Preguntas pregunta;
    private bool respuesta;
    private bool respondioIncorrectamente = false; // Para controlar si el jugador respondi� incorrectamente.

    private void Start()
    {
        //planets = GameObject.FindGameObjectsWithTag("Planet");
    }

    private void Update()
    {

        if (respuesta && !isMoving)
        {
            // Verifica que todav�a hay planetas disponibles para avanzar.
            if (currentPlanetIndex < planets.Length - 1)
            {
                // Calcula la longitud del viaje y guarda el tiempo de inicio.
                destination = planets[currentPlanetIndex + 1].transform;
                journeyLength = Vector3.Distance(transform.position, destination.position);
                startTime = Time.time;

                // Marca que estamos en movimiento.
                isMoving = true;

                // Actualiza el �ndice del planeta.
                currentPlanetIndex++;
            }
        }
        else if (respondioIncorrectamente && !isMoving)
        {
            if (currentPlanetIndex > 0) // Verifica que haya un planeta anterior para retroceder.
            {
                // Calcula la longitud del viaje de regreso al planeta anterior.
                destination = planets[currentPlanetIndex - 1].transform;
                journeyLength = Vector3.Distance(transform.position, destination.position);
                startTime = Time.time;

                // Marca que estamos en movimiento.
                isMoving = true;

                // Restablece la variable respondioIncorrectamente.
                respondioIncorrectamente = false;

                // Retrocede al planeta anterior.
                currentPlanetIndex--;
            }
            else
            {
                Morir();
            }
        }

        // Realiza el movimiento si isMoving es igual a true.
        if (isMoving)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // Calcula la direcci�n y la magnitud del movimiento.
            Vector3 direction = (destination.position - transform.position).normalized;
            float step = moveSpeed * Time.deltaTime;

            // Mueve al astronauta usando transform.Translate.
            transform.Translate(direction * step);

            // Cuando llegamos al destino, det�n el movimiento y actualiza el �ndice del planeta.
            if (fractionOfJourney >= 1.0f)
            {
                isMoving = false;
                //currentPlanetIndex++;
            }
        }

        
    }

    public void Move()
    {
        respuesta = true;
    }

    public void Stop()
    {
        respuesta = false;
    }

    public void SetRespondioIncorrectamente(bool value)
    {
        respondioIncorrectamente = value;
    }

    public void Morir()
    {
        if (!isMoving)
        {
            // Coloca al jugador en la posici�n del objeto "Sol".
            Transform solTransform = GameObject.Find("Sol").transform;
            destination = solTransform;

            // Calcula la longitud del viaje y guarda el tiempo de inicio.
            journeyLength = Vector3.Distance(transform.position, destination.position);
            startTime = Time.time;

            // Marca que estamos en movimiento.
            isMoving = true;
        }

        //perdida.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Sol"))
        {
            perdida.SetActive(true);
        }
    }
}

