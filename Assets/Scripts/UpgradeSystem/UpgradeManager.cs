using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [System.Serializable]
    public class Upgrade
    {
        public string upgradeName;
        public string description;
        public Sprite icon;
        public Action applyUpgrade;
        public bool isAvailable = true;

        public Upgrade(string name, string desc, Sprite ico, Action apply)
        {
            upgradeName = name;
            description = desc;
            icon = ico;
            applyUpgrade = apply;
        }
    }

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private Transform upgradeContainer;

    [Header("Upgrade Settings")]
    [SerializeField] private int upgradesPerSelection = 3;
    [SerializeField] private Sprite[] upgradeIcons;

    private List<Upgrade> allUpgrades = new List<Upgrade>();
    private PlayerController player;
    private float originalMoveSpeed;
    private float originalShootCooldown;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            originalMoveSpeed = player.moveSpeed;
            originalShootCooldown = player.shootCooldown;
        }

        InitializeUpgrades();
        upgradePanel.SetActive(false);
    }

    private void InitializeUpgrades()
    {
        // Initialize all upgrades with their effects
        allUpgrades.Add(new Upgrade("CHONK", 
            "MAKES YOUR CROSSHAIR 20% BIGGER", 
            upgradeIcons[0], 
            () => {
                player.getBigger();
            }));

        allUpgrades.Add(new Upgrade("SMOL", 
            "MAKES YOUR CROSSHAIR 20% SMALLER", 
            upgradeIcons[1], 
            () => {
                player.getSmaller();
            }));

        allUpgrades.Add(new Upgrade("LIVE LONG", 
            "GRANTS 3 ADDITIONAL LIVES", 
            upgradeIcons[2], 
            () => {
                player.AddLife();
            }));

        allUpgrades.Add(new Upgrade("I AM SPEED", 
            "INCREASES MOVEMENT SPEED BY 15%", 
            upgradeIcons[3], 
            () => {
                player.moveSpeed *= 1.15f;
            }));

        allUpgrades.Add(new Upgrade("*RELOAD SOUNDS*", 
            "DECREASES RELOAD TIME 20%", 
            upgradeIcons[4], 
            () => {
                player.shootCooldown *= 0.8f;
            }));

        allUpgrades.Add(new Upgrade("POOF", 
            "PRESS SHIFT TO DASH IN MOVEMENT DIRECTION", 
            upgradeIcons[5], 
            () => {
                player.UnlockDash();
            }));

        allUpgrades.Add(new Upgrade("LIFE STEAL", 
            "5% CHANCE TO REGAIN LOST LIVES WHEN KILLING A DUCK", 
            upgradeIcons[6], 
            () => {
                GlobalUpgradeSettings.lifeStealChance = 0.05f;
            }));

        allUpgrades.Add(new Upgrade("SHIELD", 
            "GRANTS A SHIELD THAT BLOCKS A SINGLE SHOT EVERY MINUTE.", 
            upgradeIcons[7], 
            () => {
               player.unlockShield();
            }));
    }

    public void ShowUpgradeSelection()
    {
        // Clear existing upgrade options
        foreach (Transform child in upgradeContainer)
        {
            Destroy(child.gameObject);
        }

        // Get available upgrades
        List<Upgrade> availableUpgrades = allUpgrades.FindAll(u => u.isAvailable);
        
        // Shuffle and select random upgrades
        ShuffleList(availableUpgrades);
        int count = Mathf.Min(upgradesPerSelection, availableUpgrades.Count);

        if (count == 0)
        {
            Debug.Log("No more upgrades available");
            return;
        }

        // Display upgrades
        for (int i = 0; i < count; i++)
        {
            GameObject upgradeButton = Instantiate(upgradePrefab, upgradeContainer);
            
            // Configure the button
            Button button = upgradeButton.GetComponent<Button>();
            TextMeshProUGUI titleText = upgradeButton.transform.Find("Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descText = upgradeButton.transform.Find("Description").GetComponent<TextMeshProUGUI>();
            Image iconImage = upgradeButton.transform.Find("Icon").GetComponent<Image>();
            
            Upgrade upgrade = availableUpgrades[i];
            
            titleText.text = upgrade.upgradeName;
            descText.text = upgrade.description;
            
            if (upgrade.icon != null)
                iconImage.sprite = upgrade.icon;
                
            int index = i; // Capture for lambda
            button.onClick.AddListener(() => SelectUpgrade(availableUpgrades[index]));
        }

        // Show the panel
        Time.timeScale = 0;
        upgradePanel.SetActive(true);
    }

    private void SelectUpgrade(Upgrade selectedUpgrade)
    {
        // Apply the upgrade
        selectedUpgrade.applyUpgrade?.Invoke();
        
        // Mark as no longer available
        selectedUpgrade.isAvailable = false;
        
        // Hide the panel and resume the game
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            int r = i + UnityEngine.Random.Range(0, n - i);
            T temp = list[r];
            list[r] = list[i];
            list[i] = temp;
        }
    }
}