using System.Collections.Generic;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    public GameObject gameField;
    public GameObject unitPrefab;
    public List<GameObject> units;
    public int size;

    private readonly Dictionary<(int, int), Unit> checkedCellToUnit = new Dictionary<(int, int), Unit>();

    public bool _staying = false;



    private Field field;
    void Start()
    {
        field = gameField.GetComponent<Field>();
        field.e_CellBlock.AddListener(CellBlockListener);
        field.e_CellCheck.AddListener(CellCheckListener);
        size = UnityEngine.Random.Range(2, 5);

        for (int i = 0; i < size; i++)
        {
            GameObject unit = Instantiate(unitPrefab, transform);
            unit.transform.position = new Vector3(i, 0, 0);
            units.Add(unit);
        }
    }

    void Update()
    {
        foreach (GameObject unitObject in units)
        {
            Unit unit = unitObject.GetComponent<Unit>();
            if (!unit.Stay && !unit.Moving)
            {
                try
                {
                    (int, int) goal = field.GetRandomNonBlockedCell();
                    unit.Goal = (goal.Item1, goal.Item2);
                    DirectUnit(unit);
                }
                catch (AllCellsBlockingException e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
    }

    // Если была заблокирована клетка содержащаяся в пути у юнита, то необходимо перестроить путь.
    public void CellBlockListener(int x, int z, bool val)
    {
        foreach (GameObject unitObject in units)
        {
            Unit unit = unitObject.GetComponent<Unit>();
            if (unit.Moving && !unit.Stay)
            {
                DirectUnit(unit);
            }
        }
        if (_staying)
            DirectUnitsToCheckedCells();
        
    }
    public void CellCheckListener(int x, int z, bool val)
    {
        if (checkedCellToUnit.ContainsKey((x, z)))
        {
            if (checkedCellToUnit[(x, z)] != null)
                checkedCellToUnit[(x, z)].Stay = false;
            checkedCellToUnit.Remove((x, z));
        }
        else
        {
            checkedCellToUnit.Add((x, z), null);
        }
        if (_staying)
            DirectUnitsToCheckedCells();
    }

    //  Если пути не существует - юнит стоит
    public void DirectUnit(Unit unit)
    {
        List<(int, int)> s = AStar.FindPath(field.GetMatrix(), ((int)unit.transform.position.x, (int)unit.transform.position.z), unit.Goal);
        if (s != null)
        {
            Queue<(int, int)> steps = new Queue<(int, int)>(s);
            unit.SetQueue(steps);
            unit.Moving = true;
        }
        else
        {
            unit.Stay = false;
            unit.Moving = false;
        }
        
    }

    private void DirectUnitsToCheckedCells()
    {
        List<(int, int)> keys = new List<(int, int)>(checkedCellToUnit.Keys);
        foreach (var k in keys)
        {
            if (checkedCellToUnit[k] != null)
                checkedCellToUnit[k].Stay = false;

            checkedCellToUnit[k] = null;
        }
        foreach (GameObject unitObject in units)
        {
            Unit unit = unitObject.GetComponent<Unit>();
            foreach(KeyValuePair<(int, int), Unit> cell in checkedCellToUnit)
            {
                // Если к помеченной клетке относится юнит, то проверяем, сможет ли он до неё добраться. Если нет, то перезапишим.
                if (cell.Value != null)
                {
                    continue;
                }
                if (AStar.FindPath(field.GetMatrix(), unit.Position, cell.Key) != null)
                {
                    checkedCellToUnit[cell.Key] = unit;
                    unit.Goal = (cell.Key.Item1, cell.Key.Item2);
                    DirectUnit(unit);
                    unit.Stay = true;
                    break;
                }
            }
        }
    }
    public void OnToggleClick()
    {
        _staying = !_staying;

        if (_staying)
        {
            DirectUnitsToCheckedCells();
        }
        else
        {
            // Сбрасываем отношения cell to unit
            foreach (GameObject unitObject in units)
            {
                Unit unit = unitObject.GetComponent<Unit>();
                unit.Stay = false;
            }

            List<(int, int)> keys = new List<(int, int)>(checkedCellToUnit.Keys);
            foreach(var k in keys)
            {
                checkedCellToUnit[k] = null;
            }
        }
    }
}
