using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    
    private void OnValidate()
    {
        if (titleText == null)
            titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
            
        if (descriptionText == null)
            descriptionText = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        /*    
        if (iconImage == null)
            iconImage = transform.Find("Icon").GetComponent<Image>();
            */
            
        if (button == null)
            button = GetComponent<Button>();
    }
    
    public void Setup(string title, string description, Sprite icon, System.Action onClickAction)
    {
        titleText.text = title;
        descriptionText.text = description;
        
        if (icon != null)
            iconImage.sprite = icon;
            
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClickAction?.Invoke());
    }
}