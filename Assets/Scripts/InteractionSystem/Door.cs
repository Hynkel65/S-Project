using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool _isOpen;
    public int switchToOpenValue = 1;
    int switchToOpen;


    Animator animator;
    Collider2D col;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void Open()
    {
        switchToOpen++;
        if (!_isOpen && switchToOpen >= switchToOpenValue)
            setState(true);
    }
    public void Close()
    {
        switchToOpen--;
        if (_isOpen && switchToOpen < switchToOpenValue)
            setState(false);
    }

    public void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    void setState(bool open)
    {
        _isOpen = open;
        col.isTrigger = open;
        animator.SetBool("IsOpen", open);
    }
}