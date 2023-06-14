using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAction : MonoBehaviour
{
    [SerializeField] private FirstPersonController player;

    [SerializeField] private Transform attackPos;
    
    [SerializeField] private float fillAmountSpeed;
    
    [SerializeField] private bool isInVersion;

    [SerializeField] private bool isLock;

    [SerializeField] private Image slider;

    private void Start()
    {
        player = FindObjectOfType<FirstPersonController>();
    }

    private void Update()
    {
        VersionDetected();
        isLock = slider.fillAmount >= 1;

        if (isLock && isInVersion)
        {
            player.AddHuntList(this);
        }
        else
        {
            player.RemoveHuntList(this);
        }
    }

    private void VersionDetected()
    {
        if(isLock)
            return;
        
        if (isInVersion)
        {
            slider.fillAmount += fillAmountSpeed * Time.deltaTime;
        }
        else
        {
            slider.fillAmount -= fillAmountSpeed * Time.deltaTime;
        }
    }

    public Transform GetAttackPosition() => attackPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Version")
        {
            isInVersion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Version")
        {
            isInVersion = false;
        }
    }
}
