//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class TouchInputManager : MonoBehaviour
//{
//    public List<GameObject> objectPrefabs; // Lista de prefabs que se generarán al tocar la pantalla
//    private List<GameObject> spawnedObjects = new List<GameObject>(); // Lista para almacenar los objetos generados

//    void Update()
//    {
//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.GetTouch(0); // Obtener el primer toque

//            if (touch.phase == TouchPhase.Began)
//            {
//                Ray ray = Camera.main.ScreenPointToRay(touch.position);
//                RaycastHit hit;

//                if (Physics.Raycast(ray, out hit))
//                {
//                    Vector3 touchPosition = hit.point;

//                    // Crear un nuevo objeto desde la lista de prefabs en el punto de intersección del rayo
//                    GenerateObject(touchPosition);

//                    // Mostrar la posición en la consola
//                    Debug.Log("Posición del raycast: " + touchPosition);

//                    // Información sobre el objeto impactado
//                    if (hit.collider != null)
//                    {
//                        Debug.Log("Objeto impactado: " + hit.collider.gameObject.name);
//                        Debug.Log("Layer del objeto: " + hit.collider.gameObject.layer);
//                        Debug.Log("Tag del objeto: " + hit.collider.gameObject.tag);
//                    }
//                }
//            }

//            // Verificar y destruir objetos antiguos si hay más de 100 en escena
//            if (spawnedObjects.Count > 100)
//            {
//                DestroyOldestObject();
//            }
//        }
//    }

//    void GenerateObject(Vector3 position)
//    {
//        if (objectPrefabs.Count > 0)
//        {
//            // Seleccionar un prefab aleatorio de la lista
//            GameObject selectedPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Count)];

//            // Instanciar el objeto en el punto de intersección del rayo
//            GameObject newObject = Instantiate(selectedPrefab, position, Quaternion.identity);

//            // Agregar el objeto a la lista de objetos generados
//            spawnedObjects.Add(newObject);

//            // Puedes hacer más cosas con el nuevo objeto si es necesario
//            // newObject.GetComponent<YourComponent>().DoSomething();
//        }
//        else
//        {
//            Debug.LogWarning("La lista de prefabs está vacía. Agrega prefabs para generar.");
//        }
//    }

//    void DestroyOldestObject()
//    {
//        GameObject oldestObject = spawnedObjects[0]; // Obtener el objeto más antiguo
//        spawnedObjects.RemoveAt(0); // Eliminarlo de la lista
//        Destroy(oldestObject); // Destruir el objeto más antiguo
//    }
//}
