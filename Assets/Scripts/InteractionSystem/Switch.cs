using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public enum ResetType { Never, OnUse, Timed, Immediately}

    public ResetType _resetType = ResetType.OnUse;
    public GameObject Target;
    public string onMessage;
    public string offMessage;
    public bool _isOn;
    public float ResetTime;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TurnOn()
    {
        if (!_isOn)
            SetState(true);
    }

    public void TurnOff()
    {
        if (_isOn && _resetType != ResetType.Never && _resetType != ResetType.Timed)
            SetState(false);
    }

    public void Toggle()
    {
        if (_isOn)
            TurnOff();
        else
            TurnOn();
    }

    void TimedReset()
    {
        SetState(false);
    }

    void SetState(bool on)
    {
        _isOn = on;
        animator.SetBool("IsOn", on);

        if (on)
        {
            if (Target != null && !string.IsNullOrEmpty(onMessage))
                Target.SendMessage(onMessage);

            if (_resetType == ResetType.Immediately)
                TurnOff();
            else if (_resetType == ResetType.Timed)
                Invoke("TimedReset", ResetTime);
        }
        else
        {
            if (Target != null && !string.IsNullOrEmpty(offMessage))
                Target.SendMessage(offMessage);
        }
    }
}
