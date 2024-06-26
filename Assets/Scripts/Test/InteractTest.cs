using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTest : MonoBehaviour, IInteractable
{
    private SpriteRenderer sr;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
    }

    public void EnableInteract()
    {
        sr.color = Color.green;
    }

    public void DisableInteract()
    {
        sr.color = Color.red;
    }

    public void ExecuteInteract(MouseController player)
    {
        player.StateSwitch();
        sr.color = Color.blue;
    }
}
