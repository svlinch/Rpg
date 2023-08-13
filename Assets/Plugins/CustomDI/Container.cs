using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace CustomContainer
{
    public class Container
    {
        private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();

        public Container()
        {
            _objects.Add(typeof(Container), this);
        }

        public void Register<T>()
        {
            var newObj = Activator.CreateInstance(typeof(T));
            _objects.Add(typeof(T), newObj);

            Resolve<T>(newObj);
        }

        public void Register(Component component)
        {
            _objects.Add(component.GetType(), component);
        }

        public T GetClass<T>()
        {
            return (T)_objects[typeof(T)];
        }

        public void Resolve<T>(object obj)
        {
            var method = typeof(T).GetMethod("Inject");
            if (method == null)
            {
                return;
            }

            Resolve(obj, method);
        }

        public void Resolve(GameObject obj)
        {
            var attachs = obj.GetComponents<Component>();
            foreach (var attach in attachs)
            {
                var method = attach.GetType().GetMethod("Inject");
                if (method == null)
                {
                    continue;
                }

                Resolve(attach, method);
            }
        }

        private void Resolve(object obj, MethodInfo method)
        {
            var parameters = method.GetParameters();

            var injects = new object[parameters.Length];
            for (int i = 0; i < injects.Length; i++)
            {
                injects[i] = GetObject(parameters[i].ParameterType);
            }
            method.Invoke(obj, injects);
        }

        private object GetObject(Type type)
        {
            object result;
            if (_objects.TryGetValue(type, out result))
            {
                return result;
            }
            else
            {
                Debug.LogWarning(string.Format("Container does not have object with type {0}", type.ToString()));
            }
            return null;
        }
    }
}