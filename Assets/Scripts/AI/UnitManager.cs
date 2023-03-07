using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private List<Unit> unitList;
    private List<Unit> friendlyUnits;
    private List<Unit> enemyUnits;

    public static UnitManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("UnitManager duplicate!");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        unitList = new List<Unit>();
        friendlyUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Unit.OnAnyUnitSpawned += OnAnyUnitSpawned_UnitManager;
        Unit.OnAnyUnitDead += OnAnyUnitDead_UnitManager;
    }

    private void OnAnyUnitDead_UnitManager(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.IsEnemy())
        {
            enemyUnits.Remove(unit);
        }
        else
        {
            friendlyUnits.Remove(unit);
        }

        unitList.Remove(unit);
        //Debug.Log(unit + " unit ded.");
    }

    private void OnAnyUnitSpawned_UnitManager(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.IsEnemy())
        {
            enemyUnits.Add(unit);
        }
        else
        {
            friendlyUnits.Add(unit);
        }

        unitList.Add(unit);
        //Debug.Log(unit + " unit spawned at " +unit.GetGridPosition());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetEnemyUnits()
    {
        return enemyUnits;
    }

    public List<Unit> GetFriendlyUnits() 
    { 
        return friendlyUnits;
    }
}
