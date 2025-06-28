using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// Clase principal para la generación procedural de layouts de habitaciones en Unity
public class LayoutGeneratorRooms : MonoBehaviour
{
    // Dimensiones generales del nivel
    [SerializeField] int width = 64;
    [SerializeField] int length = 64;

    // Rango de tamaño permitido para las habitaciones
    [SerializeField] int roomWidthMin = 3;
    [SerializeField] int roomWidthMax = 5;
    [SerializeField] int roomLengthMin = 3;
    [SerializeField] int roomLengthMax = 5;

    // Referencia al objeto visualizador del layout y lista de pasillos abiertos
    [SerializeField] GameObject levelLayoutDisplay;
    [SerializeField] List<Hallway> openDoorways;

    // Generador de números aleatorios y referencia al nivel generado
    System.Random random;
    Level level;

    // Método principal para generar el layout del nivel (se puede ejecutar desde el editor con botón derecho)
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
        hallways.ForEach(h => h.StartRoom = room);
        hallways.ForEach(h => openDoorways.Add(h));
        // Añade la habitación inicial al nivel
        level.AddRoom(room);

        // Selecciona aleatoriamente un pasillo de entrada y calcula un pasillo de salida compatible
        Hallway selectedEntryway = openDoorways[random.Next(0, openDoorways.Count)];
        Hallway selectedExit = SelectHallwayCandidate(new RectInt(0, 0, 5, 7), selectedEntryway);
        Debug.Log(selectedExit.StartPosition);
        Debug.Log(selectedExit.StartDirection);

        // Dibuja el layout generado en la textura del visualizador
        DrawLayout(selectedEntryway, roomRect);
    }

    // Calcula la posición y tamaño de la habitación inicial de forma aleatoria dentro de los límites
    RectInt GetStartRoomRect()
    {
        int roomWidth = random.Next(roomWidthMin, roomWidthMax);
        int availableWidthX = width / 2 - roomWidth;
        int randomX = random.Next(0, availableWidthX);
        int roomX = randomX + width / 4;

        int roomLength = random.Next(roomLengthMin, roomLengthMax);
        int availableLengthY = length / 2 - roomLength;
        int randomY = random.Next(0, availableLengthY);
        int roomY = randomY + width / 4;

        return new RectInt(roomX, roomY, roomWidth, roomLength);
    }

    // Dibuja el layout del nivel y resalta la habitación candidata y los pasillos
    void DrawLayout(Hallway selectedEntryway = null, RectInt roomCandidateRect = new RectInt())
    {
        Renderer renderer = levelLayoutDisplay.GetComponent<Renderer>();

        Texture2D layoutTexture = (Texture2D)renderer.sharedMaterial.mainTexture;

        layoutTexture.Reinitialize(width, length);
        levelLayoutDisplay.transform.localScale = new Vector3(width, length, 1);
        layoutTexture.FillWithColor(Color.black);

        // Dibuja habitaciones y pasillos existentes
        Array.ForEach(level.Rooms, room => layoutTexture.DrawRectangle(room.Area, Color.white));
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

        layoutTexture.SaveAsset();
    }

    // Selecciona un pasillo candidato para conectar una nueva habitación, filtrando por dirección compatible
    Hallway SelectHallwayCandidate(RectInt roomCandidateRect, Hallway entryway)
    {
        Room room = new Room(roomCandidateRect);
        List<Hallway> candidates = room.CalculateAllPossibleDoorways(room.Area.width, room.Area.height, 1);
        HallwayDirection requiredDirection = entryway.StartDirection.GetOppositeDirection();
        List<Hallway> filteredHallwayCandidates = candidates.Where(hc => hc.StartDirection == requiredDirection).ToList();
        return filteredHallwayCandidates.Count > 0 ? filteredHallwayCandidates[random.Next(filteredHallwayCandidates.Count)] : null;
    }

}
