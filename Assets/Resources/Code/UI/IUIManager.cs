namespace Resources.Code.UI;

public interface IUIManager
{
    void ShowAmmoPanel();
    void HideAmmoPanel();
    void UpdateAmmoPanel(int currentAmmo, int stackAmmo);
    void ShowHUD();
    void HideHUD();
}