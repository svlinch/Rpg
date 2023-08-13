using System;
using UnityEngine;

namespace CustomContainer
{
    public class Resolver : MonoBehaviour
    {
        [SerializeField]
        private Installer _installer;

        private void Awake()
        {
            _installer.Resolve(gameObject);
        }
    }
}