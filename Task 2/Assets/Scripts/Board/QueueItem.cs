using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueItem
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int queueItemSizeX;

    private int queueItemSizeY;

    private Cell[,] m_cells;

    private Transform m_root;

    private int m_matchMin;

    public QueueItem(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.queueItemSizeX = gameSettings.QueueSizeX;
        this.queueItemSizeY = gameSettings.QueueSizeY;

        m_cells = new Cell[queueItemSizeX, queueItemSizeY];

        CreateQueueItem();
    }

    private void CreateQueueItem()
    {
        Vector3 origin = new Vector3(-queueItemSizeX * 0.5f + 0.5f, -queueItemSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < queueItemSizeX; x++)
        {
            for (int y = 0; y < queueItemSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

    }
    public int score = 0;
    public void AddItem(Cell cell)
    {
        for (int i = 0; i < queueCell.Count; i++)
        {
            if (queueCell[i].HavaItem()) continue;

            queueCell[i].Assign(cell.Item);
            queueCell[i].ApplyItemPosition(false);
            break;
        }
        if (HasThreeSameCells())
        {
            score++;
            RemoveThreeSameCells();
        }
 
    }
    private int deletedItem = 0;
    private bool HasThreeSameCells()
    {
        for (int i = 0; i < queueCell.Count; i++)
        {
            if (!queueCell[i].HavaItem()) continue;

            int count = 1;
            for (int j = 0; j < queueCell.Count; j++)
            {
                if (i != j && queueCell[i].IsSameType(queueCell[j]))
                    count++;

                if (count >= 3) 
                {
                    deletedItem = j;
                    return true; 
                }
            }
        }
        return false;
    }
    private void RemoveThreeSameCells()
    {
        for (int i = 0; i < queueCell.Count; i++)
        {
            if (queueCell[i].IsSameType(queueCell[deletedItem])) queueCell[i].Clear();
        }
    }

    public List<Cell> queueCell;
    internal void Fill()
    {
        queueCell = new List<Cell>();
        for (int x = 0; x < queueItemSizeX; x++)
        {
            for (int y = 0; y < queueItemSizeY; y++)
            {
                queueCell.Add(m_cells[x, y]);
            }
        }
    }
}
