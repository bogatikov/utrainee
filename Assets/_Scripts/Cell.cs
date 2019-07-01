using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public GameObject _block;

    private Ray ray;
    private RaycastHit hit;

    // Заблокирована ли клетка
    public bool IsBlocked { get; private set; } = false;
    // Помечена ли клетка
    public bool IsChecked { get; private set; } = false;


    public void ChangeState()
    {
        IsChecked = !IsChecked;
        if (IsChecked)
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
        }
        else
        {
            this.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
        }
    }

    public void SetBlock()
    {
        IsBlocked = true;
        _block.SetActive(true);
        _block.GetComponent<Collider>().enabled = true;
    }
    public void ResetBlock()
    {
        IsBlocked = false;
        _block.SetActive(false);
        _block.GetComponent<Collider>().enabled = false;
    }
}
