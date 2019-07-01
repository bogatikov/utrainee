using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Field : MonoBehaviour
{
    public GameObject cellPrefab;
    public List<GameObject> cells;
    public int size;

    private int[,] matrix;

    private Ray ray;
    private RaycastHit hit;

    public CellBlockEvent e_CellBlock = new CellBlockEvent();
    public CellCheckEvent e_CellCheck = new CellCheckEvent();
    void Start()
    {
        size = UnityEngine.Random.Range(5, 10);
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);

                cell.transform.position = new Vector3(i, 0, j);
                cells.Add(cell);
                
            }
        }
        matrix = new int[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (cells[i * size + j].GetComponent<Cell>().IsBlocked)
                    matrix[i, j] = 0;
                else
                    matrix[i, j] = 1;
            }
        }
   }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                for(int i = 0; i < cells.Count; i++)
                {
                    if (hit.collider.gameObject == cells[i])
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            cells[i].GetComponent<Cell>().ChangeState();
                            int x = Mathf.RoundToInt(cells[i].transform.position.x);
                            int z = Mathf.RoundToInt(cells[i].transform.position.z);
                            e_CellCheck.Invoke(x, z, cells[i].GetComponent<Cell>().IsChecked);
                        }
                        else
                        {
                            if (!cells[i].GetComponent<Cell>().IsBlocked)
                            {
                                cells[i].GetComponent<Cell>().SetBlock();
                                int x = (int)cells[i].transform.position.x;
                                int z = (int)cells[i].transform.position.z;
                                matrix[x, z] = 0;
                                e_CellBlock.Invoke(x, z, true);
                            }
                        }
                    }
                    else if (hit.collider.gameObject == cells[i].GetComponent<Cell>()._block.gameObject)
                    {
                        int x = Mathf.RoundToInt(cells[i].transform.position.x);
                        int z = Mathf.RoundToInt(cells[i].transform.position.z);
                        matrix[x, z] = 1;
                        cells[i].GetComponent<Cell>().ResetBlock();
                        e_CellBlock.Invoke(x, z, false);
                    }
                        
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            int[,] matrix = GetMatrix();
            string str = "";
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    str += matrix[i, j] + " ";
                }
                str += "\n";
            }
            Debug.Log(str);
        }
    }
    // Матрица 
    public int[,] GetMatrix()
    {
        return matrix;
    }
    public List<GameObject> GetCheckedNonBlockedCells()
    {
        return cells.FindAll(c => c.GetComponent<Cell>().IsChecked && !c.GetComponent<Cell>().IsBlocked);
    }
    public (int, int) GetRandomNonBlockedCell()
    {
        (int, int) res;
        List<GameObject> availableCells = new List<GameObject>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (!cells[i * size + j].GetComponent<Cell>().IsBlocked)
                {
                    availableCells.Add(cells[i * size + j]);
                }
            }
        }

        if (availableCells.Count == 0)
        {
            throw new AllCellsBlockingException("All cells blocking");
        }

        GameObject obj = availableCells[UnityEngine.Random.Range(0, availableCells.Count)];
        res = (Mathf.RoundToInt(obj.transform.position.x), Mathf.RoundToInt(obj.transform.position.z));

        return res;
    }

    [System.Serializable]
    public class CellBlockEvent : UnityEvent<int, int, bool>
    {
    }
    [System.Serializable]
    public class CellCheckEvent : UnityEvent<int, int, bool>
    {
    }
}
