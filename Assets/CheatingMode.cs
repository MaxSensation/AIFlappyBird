using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatingMode : MonoBehaviour
{
    [SerializeField] private bool slowMove;
    [SerializeField] [Range(0, 1)] private float slowMoveSpeed;
    private void Start()
    {
        if (slowMove)
        {
            Time.timeScale = slowMoveSpeed;
        }
    }
}
