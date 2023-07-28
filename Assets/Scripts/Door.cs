using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour,IInteractible
{
    [SerializeField]
    private bool isOpen=false;

    //animator, since we made the animations by changing scale of the door wings over time
    private Animator doorAnimator;
    //the grid position on which the doors are located
    private GridPosition doorGridPos;
    private bool isOpening = false;

    //added so that we can control action flow, ie so that the interact action is not ended instantly
    Action onInteractionComplete;
    private float timer;
    public void Interact(Action onInteractionComplete)
    {
        isOpening = true;
        timer = 0.5f;
        this.onInteractionComplete = onInteractionComplete;

        Debug.Log("Trying to open the door...");
        
        if (isOpen)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }

    void Awake()
    {
        doorAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        doorGridPos = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddInteractibleAtGridPosition(doorGridPos, this);

        //cheat because we used obstacles layer mask to mark all tiles as walkable
        if (isOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOpening) 
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            isOpening = false;
            onInteractionComplete();
        }
    }

    private void OpenDoor()
    {
        isOpen= true;
        doorAnimator.SetBool("IsOpen", isOpen);
        PathfindingSquareGrid.Instance.SetGridPositionWalkable(doorGridPos, true);
    }

    private void CloseDoor()
    {
        isOpen = false;
        doorAnimator.SetBool("IsOpen", isOpen);
        PathfindingSquareGrid.Instance.SetGridPositionWalkable(doorGridPos, false);
    }
}
