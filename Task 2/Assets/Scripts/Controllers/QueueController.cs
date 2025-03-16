using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class QueueController : MonoBehaviour
{
    private static QueueController _instance;
    public static QueueController Instance => _instance;
    private QueueController() { }
    private void Awake()
    {
        _instance = this;
    }

    public bool IsBusy { get; private set; }

    public QueueItem m_queueItem;

    private bool m_gameOver;

    private void Start()
    {
        transform.position = new Vector3(0,-4,0);
    }
    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_queueItem = new QueueItem(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_queueItem.Fill();
    }
}
