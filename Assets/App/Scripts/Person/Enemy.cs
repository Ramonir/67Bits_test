using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Person
{
    [Header("Enemy")]
    [SerializeField] private Transform tansformAvatar;
    [SerializeField] private BoxCollider mainBoxC;
    [SerializeField] private int timeChangeDirection;
    private Vector2 moveDirection;
    private bool _isRagdoll;
    public bool IsRagdoll { get => _isRagdoll; }
    bool isColleted;

    private void Start() {
        StartCoroutine(CoroutineSetDirection());
        Ragdoll(false);
    }

    // Seta a direção que o inimigo vai andar
    void SetDirection(){
        float variantX = Random.Range(-1f, 1f);
        float variantY = Random.Range(-1f, 1f);

        moveDirection = new Vector2(variantX, variantY);
    }

    // Muda direção que o inimigo vai andar
    IEnumerator CoroutineSetDirection(){
        SetDirection();
        yield return new WaitForSeconds(timeChangeDirection);
        StartCoroutine(CoroutineSetDirection());
    }

    private void Update()
    {
        Move();
    }

    void Move(){
        if(_isRagdoll) return; // Não movimenta caso esteja em modo Ragdoll

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, 0, moveDirection.y * moveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    // Função para ativar e desativar o Ragdoll
    public void Ragdoll(bool active){
        mainBoxC.enabled = !active;
        _isRagdoll = active;
        animator.enabled = !active;
        tansformAvatar.gameObject.SetActive(active);
        if(!active) return;
        if(isColleted) return;
        StartCoroutine(CoroutineActiveTriggerInteraction());
    }

    // Ativa o mainBoxC como Trigger para o inimigo ser coletado em vez de atacado
    IEnumerator CoroutineActiveTriggerInteraction(){
        yield return new WaitForSeconds(1);
        mainBoxC.isTrigger = true;
        mainBoxC.enabled = true;
    }
    
    public void Collected(){
        isColleted = true;
        rb.isKinematic = true;
        mainBoxC.enabled = false;
        animator.SetBool("isWalking", false);

        if(GameManager.Instance.fisicType == GameManager.EnumFisicType.Joint) AddJoint();
    }

    // Adiciona Joint para inercia diretamente pela fisica
    void AddJoint(){
        var springJ = gameObject.AddComponent<SpringJoint>();
        springJ.autoConfigureConnectedAnchor = false;
        springJ.spring = 10000;
        springJ.damper = 100;

        rb.isKinematic = false;
        tansformAvatar.gameObject.SetActive(false);
    }

    // Conecta o Joint a pilha
    public void ConnectJoint(Rigidbody stackRb){
        var springJ = GetComponent<SpringJoint>();
        springJ.connectedBody = stackRb;
    }

    // Reseta o inimigo quando desabilitado para poder ser reciclado e reutilizado
    private void OnDisable() {
        ResetEnemy();
    }

    public void ResetEnemy(){
        Ragdoll(false);

        mainBoxC.isTrigger = false;
        isColleted = false;
        rb.isKinematic = false;
        animator.SetBool("isWalking", true);
    }
}
