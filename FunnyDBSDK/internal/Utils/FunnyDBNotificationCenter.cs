using System;
using System.Collections.Generic;

namespace SoFunny.FunnyDB
{
    internal class FunnyDBNotificationCenter
    {
        private static readonly object _lock = new object();
        private static FunnyDBNotificationCenter _instance;

        internal static FunnyDBNotificationCenter Default
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new FunnyDBNotificationCenter();
                    }
                }

                return _instance;
            }
        }


        private Dictionary<string, Dictionary<int, Action<FunnyDBNotificationValue>>> _actions;

        internal FunnyDBNotificationCenter()
        {
            _actions = new Dictionary<string, Dictionary<int, Action<FunnyDBNotificationValue>>>();
        }

        public void AddObserver(object observer, string name, Action action)
        {
            AddObserver(observer, name, (_) =>
            {
                action?.Invoke();
            });
        }

        public void AddObserver(object observer, string name, Action<FunnyDBNotificationValue> valueAction)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger.LogWarning("AddObserver - name 不可为空。");
                return;
            }

            if (!_actions.ContainsKey(name))
            {
                // 没有则创建
                _actions.Add(name, new Dictionary<int, Action<FunnyDBNotificationValue>>());
            }

            int hash = observer.GetHashCode();

            _actions[name][hash] = valueAction;
        }

        public void RemoveObserver(object observer)
        {
            if (observer is null) { return; }

            int hash = observer.GetHashCode();

            foreach (var item in _actions.Values)
            {
                if (item.ContainsKey(hash))
                {
                    item.Remove(hash);
                }
            }

        }

        public void RemoveObserver(object observer, string name)
        {
            if (observer is null) { return; }

            if (_actions.ContainsKey(name))
            {
                int hash = observer.GetHashCode();
                _actions[name].Remove(hash);
            }

        }

        public void Post(string name, FunnyDBNotificationValue value = null)
        {
            Logger.Log($"NotificationCenter - Post:{name} - Value:{value}");

            if (!_actions.ContainsKey(name)) { return; }

            if (value is null)
            {
                value = FunnyDBNotificationValue.Empty;
            }

            foreach (var action in _actions[name].Values)
            {
                action.Invoke(value);
            }
        }
    }
}

