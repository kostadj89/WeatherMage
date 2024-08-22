using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMeshProUGUI;
    [SerializeField]
    private Button button;
    [SerializeField]
    private GameObject selectedImage;
    [SerializeField]
    private Image actionIcon;

    private BaseAction baseAction;

    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        textMeshProUGUI.text = baseAction.GetActionName();
        actionIcon.sprite = baseAction.GetActionIcon();
        button.onClick.AddListener(() => { UnitActionSystem.Instance.SetSelectedAction(baseAction); });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedImage.SetActive(baseAction == selectedAction);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
