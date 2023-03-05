using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI actionPointsText;
    [SerializeField]
    private Unit unit;
    [SerializeField]
    private Image healthBarImage;

    private HealthSystem healthSystem;

    // Start is called before the first frame update
    void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        UpdateActionPointsText();

        healthSystem = unit.GetComponent<HealthSystem>();
        healthSystem.OnTakeDamage += OnTakeDamage_UnitWorldUI;
        UpdateHealthBar();
    }

    private void OnTakeDamage_UnitWorldUI(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetNormalizedHealth();
    }

    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
