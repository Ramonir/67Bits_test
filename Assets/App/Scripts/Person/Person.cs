using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    [Header("Person")]
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float turnSpeed;
    [SerializeField] protected Animator animator;
}
