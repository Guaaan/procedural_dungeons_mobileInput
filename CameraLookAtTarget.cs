//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CameraLookAtTarget : MonoBehaviour
//{
//    public string targetTag = "Player"; // o cualquier tag que uses para el objetivo
//    private Transform target;

//    void Start()
//    {
//        // Buscar automáticamente el GameObject con el tag especificado
//        GameObject foundTarget = GameObject.FindWithTag(targetTag);

//        if (foundTarget != null)
//        {
//            target = foundTarget.transform;
//            // Hace que la cámara mire al objetivo
//            transform.LookAt(target);
//        }
//        else
//        {
//            Debug.LogWarning("No se encontró un objeto con el tag: " + targetTag);
//        }
//    }

//    void Update()
//    {
//        if (target != null)
//        {
//            // Si querés que siga mirando al objetivo cada frame (por si se mueve)
//            transform.LookAt(target);
//        }
//    }
//}
