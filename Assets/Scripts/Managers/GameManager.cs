using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    private GameObject MainCanvas;

    private Animator amim;

    void Start()
    {
        MainCanvas.SetActive(false);
        amim = GetComponent<Animator>();

        if (amim == null)
        {
            amim = gameObject.AddComponent<Animator>();
        }
    }

    public void MainCanvasOn()
    {
        MainCanvas.SetActive(true);
        amim.SetBool("Swipe", false);
    }

    public void MainCanvasOff()
    {
        amim.SetBool("Swipe", true);
        MainCanvas.SetActive(false);
    }

}
