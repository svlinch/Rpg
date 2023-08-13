using UnityEngine;

namespace CustomContainer
{
    public class Installer : MonoBehaviour
    {
        protected Container _container;

        public void Resolve(GameObject obj)
        {
            _container.Resolve(obj);
        }

        public void Bind(Component component)
        {
            _container.Register(component);
        }

        public void SetContainer(Container container)
        {
            _container = container;
        }
    }
}
