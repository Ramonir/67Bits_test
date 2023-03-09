using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset; // A distância da câmera em relação ao objeto

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
