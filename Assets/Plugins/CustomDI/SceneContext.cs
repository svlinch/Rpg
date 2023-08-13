using System.Collections.Generic;
using UnityEngine;

namespace CustomContainer
{
    public class SceneContext : MonoBehaviour
    {
        public readonly Container Container = new Container();

        [SerializeField]
        private List<Installer> _installers;

        private void Awake()
        {
            foreach (var installer in _installers)
            {
                installer.SetContainer(Container);
            }
        }
    }
}