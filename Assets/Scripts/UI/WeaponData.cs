using UnityEngine;
using UnityEngine.UI;

public class WeaponData : MonoBehaviour
{
    [SerializeField]
    private Text m_weaponName;
    [SerializeField]
    private Text m_weaponAmmo;
    private string m_weaponAmmoSample = "{0}/{1}";
    private string m_weaponNameDefault = "no weapon";
    private string m_weaponAmmoDefault = "0/0";

    public void Setup(Rifle weapon)
    {
        m_weaponName.text = weapon == null ? m_weaponNameDefault.ToUpper() : weapon.GetName().ToUpper();
        m_weaponAmmo.text = weapon == null ? m_weaponAmmoDefault.ToUpper() : string.Format(m_weaponAmmoSample, weapon.GetAmmo(), weapon.GetMaxAmmo());
    }
}
