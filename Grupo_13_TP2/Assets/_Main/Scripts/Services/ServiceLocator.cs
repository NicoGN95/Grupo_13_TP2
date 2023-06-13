using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _main.Scripts.Services.MicroServices.EventsServices;
using UnityEngine;
using UnityEngine.Assertions;

namespace _main.Scripts.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, Type> m_serviceDefinitions = new();
        private static readonly Dictionary<Type, IGameService> m_serviceInstances = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void DefineServices()
        {
            Register<IEventService, EventService>();
        }

        private static void Register<TInterface, TInstance>(bool p_immediateInit = false)
            where TInterface : IGameService where TInstance : class, TInterface
        {
            var l_interfaceType = typeof(TInterface);
            var l_instanceClassType = typeof(TInstance);

            Assert.IsFalse(m_serviceDefinitions.ContainsKey(l_interfaceType),
                $"Service {l_interfaceType} is already registered");
            m_serviceDefinitions.Add(l_interfaceType, l_instanceClassType);

            if (p_immediateInit)
            {
                InitializeService<TInterface>();
            }
        }

        public static void Unregister<T>() where T : IGameService
        {
            var l_type = typeof(T);
            Assert.IsFalse(!m_serviceDefinitions.ContainsKey(l_type), $"Service {l_type} is not registered");
            m_serviceDefinitions.Remove(l_type);
        }

        public static T Get<T>() where T : IGameService
        {
            var l_type = typeof(T);
            return (T)Get(l_type);
        }

        private static IGameService Get(Type p_type)
        {
            return m_serviceInstances.TryGetValue(p_type, out var l_instance) ? l_instance : InitializeService(p_type);
        }

        private static T InitializeService<T>() where T : IGameService
        {
            var l_type = typeof(T);
            return (T)InitializeService(l_type);
        }

        private static IGameService InitializeService(Type p_serviceType)
        {
            if (!m_serviceDefinitions.ContainsKey(p_serviceType))
                throw new Exception($"Service {p_serviceType} not found");

            var l_concreteType = m_serviceDefinitions[p_serviceType];

            var l_newInstance = CreateInstance(l_concreteType);
            m_serviceInstances.Add(p_serviceType, l_newInstance);

            l_newInstance.Initialize();

            return l_newInstance;
        }

        private static bool AreParametersValid(ConstructorInfo p_constructorInfo)
        {
            return p_constructorInfo.GetParameters().All(p_p => typeof(IGameService).IsAssignableFrom(p_p.ParameterType));
        }

        private static IGameService CreateInstance(Type p_concreteType)
        {
            var l_constructors = p_concreteType.GetConstructors();

            if (l_constructors.Length == 0 || l_constructors[0].GetParameters().Length == 0)
            {
                return (IGameService)Activator.CreateInstance(p_concreteType);
            }

            if (l_constructors.Length > 1 || !AreParametersValid(l_constructors[0]))
            {
                throw new Exception(
                    $"The Service {p_concreteType} can't be created. It should define only one constructor with IGameService parameters or an empty constructor.");
            }

            var l_constructorParameters = l_constructors[0].GetParameters();
            var l_instanceParameters = new object[l_constructorParameters.Length];

            for (var l_i = 0; l_i < l_constructorParameters.Length; l_i++)
            {
                l_instanceParameters[l_i] = Get(l_constructorParameters[l_i].ParameterType);
            }

            return (IGameService)Activator.CreateInstance(p_concreteType, l_instanceParameters);
        }

        public static void TearDown()
        {
            foreach (var l_type in m_serviceDefinitions.Keys)
            {
                if (!m_serviceInstances.ContainsKey(l_type))
                    continue;

                if (m_serviceInstances[l_type] is IDisposable l_target)
                {
                    l_target.Dispose();
                }

                m_serviceInstances.Remove(l_type);
            }
        }
    }

    public interface IGameService
    {
        void Initialize();
    }
}