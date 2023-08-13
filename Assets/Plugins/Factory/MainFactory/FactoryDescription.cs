using UnityEngine;

namespace Assets.Scripts.Factories
{
    public class FactoryDescription
    {
        public string PrefabName;
        public string Kind;
        public Vector3 Position;
        public Transform Parent;
    }

    public enum EObjectType
    {
        UI,
        Unit
    }

    public class FactoryDescriptionBuilder
    {
        private string _kind;
        private Vector3 _position;
        private EObjectType _objectType;
        private Transform _parent;

        public static FactoryDescriptionBuilder Object()
        {
            return new FactoryDescriptionBuilder();
        }

        public FactoryDescriptionBuilder Parent(Transform parent)
        {
            _parent = parent;
            return this;
        }

        public FactoryDescriptionBuilder Position(Vector3 position)
        {
            _position = position;
            return this;
        }

        public FactoryDescriptionBuilder Kind(string kind)
        {
            _kind = kind;
            return this;
        }
        
        public FactoryDescriptionBuilder Type(EObjectType type)
        {
            _objectType = type;
            return this;
        }

        public FactoryDescription Build()
        {
            var result = new FactoryDescription();

            result.Parent = _parent;
            result.PrefabName = string.Format("{0}{1}", GetPath(), _kind);
            result.Position = _position;
            result.Kind = _kind;

            return result;
        }

        private string GetPath()
        {
            if (string.IsNullOrEmpty(_kind))
            {
                Debug.LogWarning(string.Format("Cant get prefab with parameters: {0},{1} ",_objectType, _kind));
                return null;
            }
            switch (_objectType)
            {
                case EObjectType.UI: return "Prefabs/UI/";
                case EObjectType.Unit: return "Prefabs/Units/";
                default: return null;
            }
        }
    }
}
