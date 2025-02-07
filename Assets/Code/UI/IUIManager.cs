
namespace Code.UI
{
    public interface IUIManager
    {
        void ShowHUD();
        void HideHUD();
        void UpdateAmmoPanel(int currentAmmo, int stackAmmo);
    }
}