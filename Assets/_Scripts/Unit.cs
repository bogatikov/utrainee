using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float _speed = 1f;
    private bool moving = false;
    private bool stay = false;

    //Конечная точка юнита
    private (int, int) goal;
    public (int, int) Goal { get => goal; set => goal = value; }
    // Путь юнита
    private Queue<(int, int)> steps;
    public Queue<(int, int)> Steps { get => steps; set => steps = value; }
    public bool Moving { get => moving; set => moving = value; }
    
    // Стоит ли юнит на помеченной клетке
    public bool Stay { get => stay; set => stay = value; }


    public (int, int) Position { get => ((int) transform.position.x, (int)transform.position.z); }

    private void Awake()
    {
        Steps = new Queue<(int, int)>();
    }

    void Update()
    {
        if (Moving && Steps.Count > 0) {
            (int, int) start = ((int)transform.position.x, (int)transform.position.z);
            foreach ((int, int) coor in steps)
            {
                Debug.DrawLine(new Vector3(start.Item1, transform.position.y + 1, start.Item2), new Vector3(coor.Item1, transform.position.y + 1, coor.Item2), Color.red);
                start = coor;
            }
            if (Steps.Peek().Item1 != transform.position.x)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(Steps.Peek().Item1, transform.position.y, transform.position.z), _speed * Time.deltaTime);
            }
            else if (Steps.Peek().Item2 != transform.position.z)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, Steps.Peek().Item2), _speed * Time.deltaTime);
            }
            else
            {
                Steps.Dequeue();
            }
        }
        else if (Steps.Count == 0 )
        {
            Moving = false;
        }

    }
    public void SetQueue(Queue<(int, int)> queue)
    {
        Steps = queue;
    }
}
