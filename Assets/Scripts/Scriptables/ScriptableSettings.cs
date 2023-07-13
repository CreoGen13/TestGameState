using Base;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "ScriptableSettings", menuName = "Scriptables/Scriptable Settings", order = 0)]
    public class ScriptableSettings : BaseScriptable<ScriptableSettings>
    {
        [Header("Skill Settings")]
        public Color skillColorActive;
        public Color skillColorAvailable;
        public Color skillColorInactive;

        [Header("Line Settings")]
        public float lineHeight;
    }
}