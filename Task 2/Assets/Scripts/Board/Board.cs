using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;
    public int BoardSizeX => boardSizeX;
    private int boardSizeY;
    public int BoardSizeY => boardSizeY;
    private Cell[,] m_cells;

    private Transform m_root;

    private int m_matchMin;

    public List<Cell> boardCell;
    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }

    }

    internal void Fill()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                emptyCells.Add(new Vector2Int(x, y));
            }
        }

        Dictionary<NormalItem.eNormalType, int> itemCount = new Dictionary<NormalItem.eNormalType, int>();
        int totalCells = boardSizeX * boardSizeY;
        int groupCount = totalCells / 3;

        List<NormalItem.eNormalType> allTypes = Enum.GetValues(typeof(NormalItem.eNormalType))
            .Cast<NormalItem.eNormalType>()
            .ToList();

        while (emptyCells.Count >= 3)
        {
            List<Vector2Int> selectedCells = new List<Vector2Int>();
            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, emptyCells.Count);
                selectedCells.Add(emptyCells[index]);
                emptyCells.RemoveAt(index);
            }

            List<NormalItem.eNormalType> availableTypes = allTypes
                .Where(type => !itemCount.ContainsKey(type) || itemCount[type] < groupCount)
                .ToList();

            if (availableTypes.Count == 0)
            {
                availableTypes = allTypes;
            }

            NormalItem.eNormalType selectedType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];

            if (!itemCount.ContainsKey(selectedType))
            {
                itemCount[selectedType] = 0;
            }
            itemCount[selectedType] += 3;

            foreach (var pos in selectedCells)
            {
                Cell cell = m_cells[pos.x, pos.y];
                NormalItem item = new NormalItem();

                item.SetType(selectedType);
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }


    internal void ExplodeAllItems()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.ExplodeItem();
            }
        }
    }
    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}
