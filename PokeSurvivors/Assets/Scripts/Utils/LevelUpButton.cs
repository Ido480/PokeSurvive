using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpButton : MonoBehaviour
{
    public TMP_Text weaponName;
    public TMP_Text weaponDescription;
    public Image weaponIcon;

    private Weapon assignedWeapon;

    public void ActivateButton(Weapon weapon)
    {
        assignedWeapon = weapon;
        if (weapon.weaponData == null) return;

        weaponIcon.sprite = weapon.weaponData.icon;

        if (weapon.gameObject.activeSelf)
        {
            weaponName.text = weapon.weaponData.weaponName;

            // If current level is 1, next level is 2. 
            // Level 2 stats live at Index 1 (weaponLevel).
            int nextLevelIndex = weapon.weaponLevel;

            if (nextLevelIndex < weapon.weaponData.levels.Count)
            {
                weaponDescription.text = weapon.weaponData.levels[nextLevelIndex].levelDescription;
            }
        }
        else
        {
            weaponName.text = "NEW " + weapon.weaponData.weaponName;
            // New weapon shows the very first stats (Index 0)
            weaponDescription.text = weapon.weaponData.description;
        }
    }
    public void SelectUpgrade()
    {
        if (assignedWeapon.gameObject.activeSelf == true)
        {
            assignedWeapon.LevelUp();
        }
        else
        {
            PlayerController.Instance.ActivateWeapon(assignedWeapon);
        }

        UIController.Instance.LevelUpPanelClose();
        AudioController.Instance.PlaySound(AudioController.Instance.selectUpgrade);
    }
}