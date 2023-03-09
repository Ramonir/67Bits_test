using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : Person
{
    [Header("Player")]
    [SerializeField] private Transform childsParent;
    [SerializeField] private float childsParentPosY = 2.5f;    
    [SerializeField] private List<Enemy> enemysCollected;
    [SerializeField] private Material material;
    [SerializeField] private int startEnemysCapacity;
    private int enemysCapacity;

    private void Start() {
        enemysCapacity = startEnemysCapacity;
        UpdateCapacity(1);
        ChangeColor(0);
    }

    private void Update()
    {       
        SetChildsParentPosition();
        Move();
        MoveChildsScript();
        FixPosYChild();
    }

    // Coloca o pai da pilha sempre acima do Player
    void SetChildsParentPosition(){
        childsParent.position = new Vector3(transform.position.x, childsParentPosY, transform.position.z);
    }

    void Move(){
        rb.velocity = new Vector3(GameManager.Instance.Hud.Joystick.Horizontal * moveSpeed, 0, GameManager.Instance.Hud.Joystick.Vertical * moveSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity.normalized);

        if (GameManager.Instance.Hud.Joystick.Horizontal != 0 || GameManager.Instance.Hud.Joystick.Vertical != 0) // Detectar se o joystick está sendo ultilizado
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            animator.SetBool("isRunning", true);
        }
        else
            animator.SetBool("isRunning", false);
    }

    // Efeito elastico da pilha por script
    void MoveChildsScript(){
        if(GameManager.Instance.fisicType != GameManager.EnumFisicType.Script) return;
        if(enemysCollected.Count < 1) return;

        enemysCollected[0].transform.position = childsParent.position;

        // Percorre a lista de objetos empilhados, movendo cada objeto para a posição do objeto à sua frente
        for (int i = 1; i < enemysCollected.Count; i++)
        {
            Vector3 nextPosition = new Vector3(enemysCollected[i - 1].transform.position.x, 
                childsParent.position.y + ((float)i/2) + .5f, enemysCollected[i - 1].transform.position.z);
            enemysCollected[i].transform.position = Vector3.Lerp(enemysCollected[i].transform.position, nextPosition, moveSpeed * Time.deltaTime);
        }

        // Rotaciona o objeto filho em torno de seu pai para seguir a pilha
        transform.RotateAround(childsParent.position, Vector3.up, moveSpeed * Time.deltaTime);
    }

    // Fixa PosY dos filhos no caso de usar EnumFisicType.Joint
    void FixPosYChild(){
        if(GameManager.Instance.fisicType != GameManager.EnumFisicType.Joint) return;
        if(enemysCollected.Count < 1) return;

        enemysCollected[0].transform.position = childsParent.position;

        for (int i = 1; i < enemysCollected.Count; i++)
        {
            float posY = childsParentPosY + ((float)i / 2);
            Vector3 fixedY = new Vector3(enemysCollected[i].transform.position.x, posY, enemysCollected[i].transform.position.z);
            enemysCollected[i].transform.position = fixedY;
        }
    }

    private void OnCollisionEnter(Collision c) {
        var enemy = c.gameObject.GetComponent<Enemy> ();
        if(enemy) HitEnemy(enemy);
    }

    // Detecta colisão com inimigo, que sofre um hit ou é coletado
    void HitEnemy(Enemy enemy){
        if(!enemy.IsRagdoll) {
            enemy.Ragdoll(true);
            TweenHit(enemy.transform);
        }
        else Collect(enemy);
    }

    // Feedback de colisão
    void TweenHit(Transform obj){
        float originalScale = obj.transform.localScale.x;
        float scaleMax = 1.5f;
        float scaleDurantion = 0.2f;
        obj.DOScale(scaleMax, scaleDurantion);
        obj.DOScale(originalScale, scaleDurantion).SetDelay(scaleDurantion);
    }

    // Coleta o inimigo para ser empilhado
    void Collect(Enemy enemy){
        var enemysCapacityFinal = enemysCapacity - 1;
        float enemysCollectedFinal = enemysCollected.Count;
        if(enemysCapacityFinal < enemysCollectedFinal) return; // Retorna caso já tenha alcançado o limite de inimigos

        enemy.Collected();
        enemysCollected.Add(enemy);

        if(GameManager.Instance.fisicType == GameManager.EnumFisicType.NoFisic) {
            enemy.gameObject.transform.SetParent(childsParent);
            float posY = ((float)enemysCollected.Count / 2) - .5f;
            float rotY = Random.Range(0, 360);
            enemy.transform.localPosition = new Vector3(0, posY, 0);
            enemy.transform.localEulerAngles = new Vector3(0, rotY, 0);

        } else if(GameManager.Instance.fisicType == GameManager.EnumFisicType.Joint) {
            Rigidbody stackRb;
            if(enemysCollected.Count == 1) stackRb = rb;
            else stackRb = enemysCollected[enemysCollected.Count - 2].gameObject.GetComponent<Rigidbody>();
            enemy.ConnectJoint(stackRb);

        }
    }

    // Melhora a capacidade de inimigos empilhados possiveis de acordo com o lvl
    public void UpdateCapacity(int lvl){
        enemysCapacity = startEnemysCapacity + lvl;
    }

    private void OnTriggerEnter(Collider c) {
        var enemy = c.gameObject.GetComponent<Enemy> ();
        if(enemy) HitEnemy(enemy);
    }

    private void OnTriggerStay(Collider c) {
        if(c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject) {
            if(enemysCollected.Count <= 0) return;
            if(GameManager.Instance.Hud.SellingChilds() == 1) { // Quando completa o botão
                SellChildrens();
            }
        } else if(c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject) {
            GameManager.Instance.Hud.BuyingHormone(); // Apenas chama a função do botão
        }
    }

    // Muda cor do player de acordo com o lvl
    public void ChangeColor(int lvl){
        float modifier = 1 - ((float)lvl / 10);
        Color newColor = new Color(modifier, modifier, modifier, 1f);
        material.color = newColor;
    }

    private void OnTriggerExit(Collider c) {
        if(c.gameObject == GameManager.Instance.Hud.BtnSellChild.gameObject) 
            GameManager.Instance.Hud.ResetSelling();
        else if(c.gameObject == GameManager.Instance.Hud.BtnBuyHormone.gameObject) 
            GameManager.Instance.Hud.ResetBuying();
    }

    // Vende as crianças empilhadas
    void SellChildrens(){
        var gain = enemysCollected.Count * GameManager.Instance.Hud.ChildPrice;
        GameManager.Instance.Status.AddMoney(gain);

        foreach (Enemy enemy in enemysCollected){
            enemy.gameObject.transform.SetParent(null);
            GameManager.Instance.ObjectPooler.Recicle (enemy.gameObject);
        }
        
        GameManager.Instance.Spawner.AddEnemyCount(-enemysCollected.Count);
        enemysCollected.Clear();
    }
}
