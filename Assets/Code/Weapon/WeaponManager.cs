using System;
using Code.Player;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Code.Weapon
{
    public class WeaponManager : MonoBehaviour, IDisposable
    {
        private Transform _weaponSlotTransform;

        private SignalBus _signalBus;
        private PlayerController _playerController;

        [CanBeNull] private Weapon _activeWeapon;
        private Weapon _m1911;
        private Weapon _ak74;
        private Weapon _bennelliM4;


        [Inject]
        public void Construct(
            [Inject(Id = "M1911")] Weapon m1911,
            [Inject(Id = "AK74")] Weapon ak74,
            [Inject(Id = "BENNELLIM4")] Weapon bennelliM4,
            SignalBus signalBus,
            PlayerController playerController)
        {
            _m1911 = m1911;
            m1911.gameObject.SetActive(false);
            _ak74 = ak74;
            ak74.gameObject.SetActive(false);
            _bennelliM4 = bennelliM4;
            bennelliM4.gameObject.SetActive(false);
            _signalBus = signalBus;
            _playerController = playerController;
            signalBus.Subscribe<FirstSlotActivatedSignal>(OnFirstSlotActivated);
            signalBus.Subscribe<SecondSlotActivatedSignal>(OnSecondSlotActivated);
            signalBus.Subscribe<ThirdSlotActivatedSignal>(OnThirdSlotActivated);
        }
        private void Start()
        {
            _weaponSlotTransform = _playerController.GetWeaponSlotTransform();
            _m1911.transform.SetParent(_weaponSlotTransform);
            _ak74.transform.SetParent(_weaponSlotTransform);
            _bennelliM4.transform.SetParent(_weaponSlotTransform);
        }
        private void Equip(Weapon weapon)
        {
            if (_activeWeapon == weapon)
            {
                weapon?.gameObject.SetActive(false);
                _activeWeapon = null;
                return;
            }

            if (_activeWeapon != null)
            {
                _activeWeapon.gameObject.SetActive(false);
            }

            _activeWeapon = weapon;
            _activeWeapon?.gameObject.SetActive(true);
        }
        private void OnFirstSlotActivated()
        {
            Equip(_ak74);
        }
        private void OnSecondSlotActivated()
        {
            Equip(_m1911);
        }
        private void OnThirdSlotActivated()
        {
            Equip(_bennelliM4);
        }
        public void Dispose()
        {
            _signalBus.Unsubscribe<FirstSlotActivatedSignal>(OnFirstSlotActivated);
            _signalBus.Unsubscribe<SecondSlotActivatedSignal>(OnSecondSlotActivated);
            _signalBus.Unsubscribe<ThirdSlotActivatedSignal>(OnThirdSlotActivated);
        }
    }
}