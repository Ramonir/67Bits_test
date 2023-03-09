using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Hud : MonoBehaviour
{
    [Header("Hud")]
    [SerializeField] private FloatingJoystick _joystick;
    public FloatingJoystick Joystick { get => _joystick; }

    [SerializeField] private TextMeshProUGUI txtMoney;
    [SerializeField] private TextMeshProUGUI txtLvl;
    [SerializeField] private Image imgLvl;

    [Header("World Space Hud")]
    [SerializeField] private Image _btnSellChild;
    public Image BtnSellChild { get => _btnSellChild; }
    [SerializeField] private int _childPrice = 100;
    public int ChildPrice { get => _childPrice; }

    [SerializeField] private Image _btnBuyHormone;
    public Image BtnBuyHormone { get => _btnBuyHormone; }
    [SerializeField] private int _hormonePrice = 500;
    public int HormonePrice { get => _hormonePrice; }

    [SerializeField] private float fillLoadTime;

    // Mostra money na Hud
    public void SetMoney(int value){
        txtMoney.text = value.ToString();
    }

    // Calcula e seta o Lvl final na Hud
    public void SetLvl(int value){
        if(imgLvl.fillAmount == 0) imgLvl.fillAmount = 0.5f;
        else imgLvl.fillAmount = 0;

        value = (int) (value / 100);
        txtLvl.text = $"LEVEL {value}";
    }

    public float SellingChilds(){
        _btnSellChild.fillAmount += (1f / fillLoadTime) * Time.deltaTime;

        if(_btnSellChild.fillAmount == 1) {
            ResetFill(_btnSellChild);
            RectTransform txtMoneyParent = txtMoney.transform.parent.GetComponentInParent<RectTransform>();
            TweenCompleteFill(txtMoneyParent);
            return 1;
        }

        return _btnSellChild.fillAmount;
    }

    public void ResetSelling(){
        ResetFill(_btnSellChild);
    }

    public float BuyingHormone(){
        if(GameManager.Instance.Status.Money < _hormonePrice) return 0;

        _btnBuyHormone.fillAmount += (1f / fillLoadTime) * Time.deltaTime;

        if(_btnBuyHormone.fillAmount == 1) {
            GameManager.Instance.Status.AddLvl();
            ResetFill(_btnBuyHormone);
            RectTransform txtLvlParent = txtLvl.transform.parent.GetComponentInParent<RectTransform>();
            TweenCompleteFill(txtLvlParent);
            return 1;
        }

        return _btnBuyHormone.fillAmount;
    }

    public void ResetBuying(){
        ResetFill(_btnBuyHormone);
    }

    void ResetFill(Image fill){
        fill.fillAmount = 0;
    }

    // Feedback de interação completa
    void TweenCompleteFill(RectTransform obj){
        float originalScale = obj.transform.localScale.x;
        float scaleMax = 1.1f;
        float scaleDurantion = 0.2f;
        obj.DOScale(scaleMax, scaleDurantion);
        obj.DOScale(originalScale, scaleDurantion).SetDelay(scaleDurantion);
    }
}
