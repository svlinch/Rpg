using System;
using UnityEngine;

namespace CustomContainer
{
    public class Binder : MonoBehaviour
    {
        [SerializeField]
        private Installer _installer;

        private void Awake()
        {
            var attachs = gameObject.GetComponents<Component>();
            foreach (var attach in attachs)
            {
                var type = attach.GetType();
                if (AvailableComponent(attach.GetType()))
                {
                    _installer.Bind(attach);
                }
            }
        }


        private bool AvailableComponent(Type type)
        {
            if (type == typeof(Installer) || type == typeof(Transform) || type == typeof(Binder))
            {
                return false;
            }
            return true;
        }
    }
}