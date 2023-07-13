using UnityEngine;

namespace Base
{
    public class BaseScriptable <T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    T[] results = Resources.FindObjectsOfTypeAll<T>();
                    if (results.Length == 0)
                    {
                        Debug.LogError("There is no scriptable type of " + typeof(T));
                        return null;
                    }

                    _instance = results[0];
                    _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
                }

                return _instance;
            }
        }

        public static void SetInstance(T instance)
        {
            _instance = instance;
        }
    }
}