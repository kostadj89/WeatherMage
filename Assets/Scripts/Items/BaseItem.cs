using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    EnergySword,
    MissileOrb,
    FireballWand,
    DancingShoes
}
public class BaseItem : MonoBehaviour, IInteractible
{
    [SerializeField]
    private ItemType itemType;

    private GridPosition itemGridPos;
    private float timer;
    private bool isInteracting = false;
    //added so that we can control action flow, ie so that the interact action is not ended instantly
    Action onInteractionCompleted;
    //[SerializeField]
    //private BaseAction grantedAction;

    public void Interact(Unit unit, Action onInteractionComplete)
    {
        timer = 0.5f;
        isInteracting = true;
        GrantActionToInteractingUnit(unit);
        //Destroy(gameObject, 5);
        onInteractionCompleted = onInteractionComplete;
    }

    private void GrantActionToInteractingUnit(Unit unit)
    {
        switch (itemType)
        {
            case ItemType.EnergySword:
                unit.gameObject.AddComponent<MeleeAction>();
                break;
            case ItemType.MissileOrb:
                unit.gameObject.AddComponent<ShootAction>();
                break;
            case ItemType.FireballWand:
                unit.gameObject.AddComponent<AreaShootAction>();
                break;
            case ItemType.DancingShoes:
            default:
                unit.gameObject.AddComponent<SpinAction>();
                break;
        }

        unit.UpdateAllAbilities();

    }

    // Start is called before the first frame update
    void Start()
    {
        itemGridPos = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddInteractibleAtGridPosition(itemGridPos, this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isInteracting)
        {
            return;
        }

        timer -= Time.deltaTime;
        if (timer<=0)
        {
            isInteracting = false;
            onInteractionCompleted();
            Destroy(gameObject, 1);
        }
    }
}
