using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    private int _lvl;
    public int Lvl { get => _lvl; }

    private int _money;
    public int Money { get => _money; }

    private void Start() {
        _lvl = 100;
    }

    public void AddMoney(int value){
        _money = _money + value;
        GameManager.Instance.Hud.SetMoney(_money);
    }

    public void AddLvl(){
        _lvl += 50;

        int realLvl = (int) (_lvl / 100);
        GameManager.Instance.Player.UpdateCapacity(realLvl);
        GameManager.Instance.Player.ChangeColor(realLvl);

        GameManager.Instance.Hud.SetLvl(_lvl);
        AddMoney(-GameManager.Instance.Hud.HormonePrice);
    }

}
