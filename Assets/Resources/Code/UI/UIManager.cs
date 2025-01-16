using System;
using TMPro;
using UnityEngine;

namespace Resources.Code.UI;

public class UIManager : MonoBehaviour, IUIManager, IDisposable
{
    // Main
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject ammoPanel;
    [SerializeField] private GameObject crosshair;

    // Ammo Panel
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI stackAmmoText;

    public void ShowAmmoPanel()
    {
        if (ammoPanel != null && !ammoPanel.activeSelf)
        {
            ammoPanel.SetActive(true);
        }
    }
    public void HideAmmoPanel()
    {
        if (ammoPanel != null && ammoPanel.activeSelf)
        {
            ammoPanel.SetActive(false);
        }
    }
    public void UpdateAmmoPanel(int currentAmmo, int stackAmmo)
    {
        if (currentAmmoText != null)
        {
            currentAmmoText.text = currentAmmo.ToString();
        }

        if (stackAmmoText != null)
        {
            stackAmmoText.text = stackAmmo.ToString();
        }
    }
    public void ShowHUD()
    {
        hud?.SetActive(true);
    }
    public void HideHUD()
    {
        hud?.SetActive(false);
    }
    public void Dispose()
    {
        if (hud != null)
        {
            Destroy(hud);
        }

        if (ammoPanel != null)
        {
            Destroy(ammoPanel);
        }

        if (crosshair != null)
        {
            Destroy(crosshair);
        }

        if (currentAmmoText != null)
        {
            Destroy(currentAmmoText.gameObject);
        }

        if (stackAmmoText != null)
        {
            Destroy(stackAmmoText.gameObject);
        }
    }
}