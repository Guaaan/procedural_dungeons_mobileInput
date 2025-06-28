// Script para la generación procedural de layouts de habitaciones y pasillos en Unity
// Permite crear niveles de forma dinámica, visualizando el resultado en tiempo real
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class LayoutGeneratorRooms : MonoBehaviour
{
    // Dimensiones totales del nivel (ancho y largo en unidades)
    [SerializeField] int width = 64;
    [SerializeField] int length = 64;

    // Rango de tamaño permitido para las habitaciones (mínimo y máximo)
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;

    // Referencia al objeto que muestra visualmente el layout generado
    [SerializeField] GameObject levelLayoutDisplay;
    // Lista de pasillos abiertos disponibles para conectar nuevas habitaciones
    [SerializeField] List<Hallway> openDoorways;

    // Generador de números aleatorios para la generación procedural
    System.Random random;
    // Objeto que representa el nivel generado
    Level level;

    // Método principal para generar el layout del nivel
    // Se puede ejecutar desde el menú contextual del componente en el editor
    [ContextMenu("Generate Level Layout")]
    public void GenerateLevel()
    {
        // Inicializa el generador aleatorio y la lista de pasillos abiertos
        random = new System.Random();
        openDoorways = new List<Hallway>();
        // Crea el objeto Level con las dimensiones especificadas
        level = new Level(width, length);
        // Calcula la posición y tamaño de la habitación inicial
        RectInt roomRect = GetStartRoomRect();
        Debug.Log(roomRect);
        // Crea la habitación inicial y calcula sus posibles pasillos
        Room room = new Room(roomRect);
        List<Hallway> hallways = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);
        // Asocia cada pasillo a la habitación inicial
        hallways.ForEach(h => h.StartRoom = room);
        // Agrega los pasillos abiertos a la lista global
        hallways.ForEach(h => openDoorways.Add(h));
        // Añade la habitación inicial al nivel
        level.AddRoom(room);

        // Selecciona aleatoriamente un pasillo de entrada y calcula un pasillo de salida compatible para la siguiente habitación
        Hallway selectedEntryway = openDoorways[random.Next(0, openDoorways.Count)];
        Hallway selectedExit = SelectHallwayCandidate(new RectInt(0, 0, 5, 7), selectedEntryway);
        Debug.Log(selectedExit.StartPosition);
        Debug.Log(selectedExit.StartDirection);

        // Calcula la posición donde se colocará la segunda habitación, conectada al pasillo seleccionado
        Vector2Int roomCandidatePosition = CalculateRoomPosition(selectedEntryway, 5, 7, 3, selectedExit.StartPosition);
        // Crea la segunda habitación y la conecta al pasillo de entrada
        Room secondRoom = new Room(new RectInt(roomCandidatePosition.x, roomCandidatePosition.y, 5, 7));
        selectedEntryway.EndRoom = secondRoom;
        selectedEntryway.EndPosition = selectedExit.StartPosition;
        // Añade la segunda habitación y el pasillo al nivel
        level.AddRoom(secondRoom);
        level.AddHallway(selectedEntryway);

        // Dibuja el layout generado en la textura del visualizador
        DrawLayout(selectedEntryway, roomRect);
    }

    // Calcula la posición y tamaño de la habitación inicial de forma aleatoria dentro de los límites del nivel
    RectInt GetStartRoomRect()
    {
        // Selecciona aleatoriamente el ancho de la habitación dentro del rango permitido
        int roomWidth = random.Next(roomWidthMin, roomWidthMax);
        // Calcula el espacio disponible para centrar la habitación
        int availableWidthX = width / 2 - roomWidth;
        int randomX = random.Next(0, availableWidthX);
        int roomX = randomX + width / 4;

        // Selecciona aleatoriamente el largo de la habitación dentro del rango permitido
        int roomLength = random.Next(roomLengthMin, roomLengthMax);
        int availableLengthY = length / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + width / 4;

        // Devuelve el rectángulo que representa la habitación inicial
        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    // Dibuja el layout del nivel en la textura del objeto visualizador
    // Resalta la habitación candidata y los pasillos
    void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt())
    {
        // Obtiene el renderer y la textura donde se dibujará el layout
        Renderer renderer = levelLayoutDisplay.GetComponent<Renderer>();
        Texture2D layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;
        // Ajusta la textura y el objeto visualizador al tamaño del nivel
        layoutTexture.Reinitialize(width, length);
        levelLayoutDisplay.transform.localScale = new Vector3(width, length, 1);
        layoutTexture.FillWithColor(Color.black);
        // Dibuja todas las habitaciones en blanco
        Array.ForEach(level.Rooms, room => layoutTexture.DrawRectangle(room.Area, Color.white));
        // Dibuja todos los pasillos en blanco
        Array.ForEach(level.Hallways, hallway => layoutTexture.DrawLine(hallway.StartPositionAbsolute, hallway.EndPositionAbsolute, Color.white));
        // Dibuja la habitación candidata en azul
        layoutTexture.DrawRectangle(roomCandidateRect, Color.blue);
        // Dibuja los posibles pasillos abiertos en su color correspondiente
        openDoorways.ForEach(hallway => layoutTexture.SetPixel(hallway.StartPositionAbsolute.x, hallway.StartPositionAbsolute.y, hallway.StartDirection.GetColor()));
        // Resalta el pasillo de entrada seleccionado en rojo
        if (selectedEntryway != null)
        {
            layoutTexture.SetPixel(selectedEntryway.StartPositionAbsolute.x, selectedEntryway.StartPositionAbsolute.y, Color.red);
        }
        // Guarda la textura modificada
        layoutTexture.SaveAsset();
    }

    // Selecciona un pasillo candidato de una habitación candidata, filtrando por la dirección opuesta al pasillo de entrada
    // roomCandidateRect: rectángulo de la habitación candidata
    // entryway: pasillo de entrada al que se debe conectar
    // Devuelve un pasillo de la habitación candidata que conecta correctamente, o null si no hay candidatos
    Hallway SelectHallwayCandidate(RectInt roomCandidateRect, Hallway entryway)
    {
        // Crea una instancia temporal de la habitación candidata
        Room room = new Room(roomCandidateRect);
        // Calcula todos los posibles pasillos de la habitación candidata
        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);
        // Determina la dirección opuesta a la del pasillo de entrada
        HallwayDirection requiredDirection = entryway.StartDirection.GetOppositeDirection();
        // Filtra los pasillos que tienen la dirección requerida
        List<Hallway> filteredHallwayCandidates = candidates.Where(hc => hc.StartDirection == requiredDirection).ToList();
        // Si hay candidatos, selecciona uno aleatorio; si no, devuelve null
        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(filteredHallwayCandidates.Count)] : null;
    }

    // Calcula la posición de la nueva habitación a colocar, en función del pasillo de entrada, dimensiones y dirección
    // entryway: pasillo de entrada seleccionado
    // roomWidth, roomLength: dimensiones de la nueva habitación
    // distance: separación entre habitaciones
    // endPosition: posición relativa del pasillo de salida en la nueva habitación
    Vector2Int CalculateRoomPosition(Hallway entryway, int roomWidth, int roomLength, int distance, Vector2Int endPosition)
    {
        // Se parte de la posición absoluta del inicio del pasillo de entrada
        Vector2Int roomPosition = entryway.StartPositionAbsolute;
        // Se ajusta la posición según la dirección del pasillo de entrada
        switch (entryway.StartDirection)
        {
            case HallwayDirection.Left:
                // Si el pasillo viene desde la izquierda, la habitación se coloca a la izquierda del pasillo
                roomPosition.x -= distance + roomWidth;
                roomPosition.y -= endPosition.y;
                break;
            case HallwayDirection.Top:
                // Si el pasillo viene desde arriba, la habitación se coloca arriba del pasillo
                roomPosition.x -= endPosition.x;
                roomPosition.y += distance + 1;
                break;
            case HallwayDirection.Right:
                // Si el pasillo viene desde la derecha, la habitación se coloca a la derecha del pasillo
                roomPosition.x += distance + 1;
                roomPosition.y -= endPosition.y;
                break;
            case HallwayDirection.Bottom:
                // Si el pasillo viene desde abajo, la habitación se coloca debajo del pasillo
                roomPosition.x -= endPosition.x;
                roomPosition.y -= distance + roomLength;
                break;
        }
        // Devuelve la posición calculada para la nueva habitación
        return roomPosition;
    }

}
