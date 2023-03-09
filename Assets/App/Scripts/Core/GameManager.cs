using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    public enum EnumFisicType { NoFisic, Joint, Script }
    public EnumFisicType fisicType;
    
    private Player _player;
    public Player Player { get => _player; }
    private Spawner _spawner;
    public Spawner Spawner { get => _spawner; }
    private ObjectPooler _objectPooler;
    public ObjectPooler ObjectPooler { get => _objectPooler; }
    private Hud _hud;
    public Hud Hud { get => _hud; }
    private Status _status;
    public Status Status { get => _status; }

    void Awake()
    {
        this._player = FindObjectOfType<Player>();
        this._spawner = FindObjectOfType<Spawner>();
        this._objectPooler = FindObjectOfType<ObjectPooler>();
        this._hud = FindObjectOfType<Hud>();
        this._status = FindObjectOfType<Status>();
    }
}