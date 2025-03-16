using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private List<Cell> m_potentialMatch;

    private float m_timeAfterFill;

    private bool m_hintIsShown;

    private bool m_gameOver;

    private bool m_win;

    private List<Cell> queueCell;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
            case GameManager.eStateGame.WIN:
                m_win = true;
                break;
        }
    }
    private bool isClickHandled = false;

    public void Update()
    {
        ChooseItem();
    }
    private void ChooseItem()
    {
        if (IsLose()) m_gameManager.SetState(GameManager.eStateGame.GAME_OVER);
        else if (IsWin()) m_gameManager.SetState(GameManager.eStateGame.WIN);
        if (m_gameOver || IsBusy || m_win) return;

        if (!m_hintIsShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
            }
        }

        if (Input.GetMouseButtonDown(0) && !isClickHandled)
        {
            isClickHandled = true;
            StartCoroutine(ResetClickFlag());

            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == null) return;

            int boardLayer = LayerMask.NameToLayer("Board");
            if (hit.collider.gameObject.layer != boardLayer) return;

            Cell clickedCell = hit.collider.GetComponent<Cell>();
            if (clickedCell != null)
            {
                HandleCellClick(clickedCell);
            }
        }
    }
    private void Start()
    {
        int boardLayer = LayerMask.NameToLayer("Board");
        SetLayerRecursively(gameObject, boardLayer);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }


    IEnumerator ResetClickFlag()
    {
        yield return new WaitForEndOfFrame();
        isClickHandled = false;
    }


    private void HandleCellClick(Cell cell)
    {
        QueueController.Instance.m_queueItem.AddItem(cell);
        cell.Free();
    }

 


    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetSortingLayerHigher();
        if (cell2.Item != null) cell2.Item.SetSortingLayerLower();
    }

    internal void Clear()
    {
        m_board.Clear();
    }
    private bool IsLose()
    {
        queueCell = QueueController.Instance.m_queueItem.queueCell;
        for (int i = 0; i < queueCell.Count; i++)
        {
            if (queueCell[i].Item == null) return false;
        }
        return true;
    }
    private bool IsWin()
    {
        int score = QueueController.Instance.m_queueItem.score;
        return m_board.BoardSizeX * m_board.BoardSizeY / 3 == score;
    }
}
