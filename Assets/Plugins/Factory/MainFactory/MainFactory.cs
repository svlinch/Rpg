using System.Collections.Generic;
using UnityEngine;
using CustomContainer;

namespace Assets.Scripts.Factories
{
    public class MainFactory : IObjectFactory
    {
        private class Template
        {
            public GameObject Prefab;
        }

        #region Injection
        private Container _container;

        public void Inject(Container container)
        {
            _container = container;
        }
        #endregion
        private readonly Dictionary<string, Template> _loadedTemplates = new Dictionary<string, Template>();

        public GameObject Create(FactoryDescription description)
        {
            var template = GetTemplate(description);
            if (template == null)
            {
                return null;
            }

            var result = InstantiateFromTemplate(template, description);
            return result;
        }

        private Template GetTemplate(FactoryDescription description)
        {
            Template template;
            if (!_loadedTemplates.TryGetValue(description.PrefabName, out template))
            {
                template = LoadTemplate(description.PrefabName);
                if (template == null)
                {
                    Debug.LogWarning(string.Format("Prefab {0} not found", description.PrefabName));
                    return null;
                }
                _loadedTemplates[description.PrefabName] = template;
            }
            return template;
        }

        private Template LoadTemplate(string prefabName)
        {
            var template = new Template();
            template.Prefab = Resources.Load<GameObject>(prefabName);
            if (template.Prefab == null)
            {
                return null;
            }
            return template;
        }

        private GameObject InstantiateFromTemplate(Template template, FactoryDescription description)
        {
            GameObject result = null;
            if (description.Parent != null)
            {
                result = Object.Instantiate(template.Prefab, description.Parent);
            }
            else
            {
                result = Object.Instantiate(template.Prefab);
            }
            result.transform.position = description.Position;

            _container.Resolve(result);
            return result;
        }
    }
}