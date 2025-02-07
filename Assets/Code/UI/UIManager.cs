using System;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class UIManager : MonoBehaviour, IUIManager, IDisposable
    {
        // Main
        [SerializeField] private GameObject ammoPanel;
        [SerializeField] private GameObject crosshair;

        // Ammo Panel
        [SerializeField] private TextMeshProUGUI currentAmmoText;
        [SerializeField] private TextMeshProUGUI stackAmmoText;

        public void ShowHUD()
        {
            if (ammoPanel) ammoPanel.SetActive(true);
            if (crosshair) crosshair.SetActive(true);
        }
        public void HideHUD()
        {
            if (ammoPanel) ammoPanel.SetActive(false);
            if (crosshair) crosshair.SetActive(false);
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
        public void Dispose()
        {
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
}