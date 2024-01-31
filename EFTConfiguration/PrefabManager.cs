using System.Collections.Generic;
using UnityEngine;

namespace EFTConfiguration
{
    [CreateAssetMenu(fileName = "PrefabManager", menuName = "EFTConfiguration/PrefabManager")]
    public class PrefabManager : ScriptableObject
    {
        [SerializeField] public GameObject eftConfiguration;

        [SerializeField] public GameObject pluginInfo;

        [SerializeField] public GameObject config;

        [SerializeField] public GameObject header;

        [SerializeField] public GameObject toggle;

        [SerializeField] public GameObject vector2;

        [SerializeField] public GameObject vector3;

        [SerializeField] public GameObject vector4;

        [SerializeField] public GameObject quaternion;

        [SerializeField] public GameObject @int;

        [SerializeField] public GameObject intSlider;

        [SerializeField] public GameObject @float;

        [SerializeField] public GameObject floatSlider;

        [SerializeField] public GameObject color;

        [SerializeField] public GameObject @string;

        [SerializeField] public GameObject stringDropdown;

        [SerializeField] public GameObject @enum;

        [SerializeField] public GameObject keyboardShortcut;

        [SerializeField] public GameObject stringAction;

        [SerializeField] public GameObject unknown;

        [SerializeField] public GameObject unknownCustom;

        public IEnumerable<GameObject> AllGameObject
        {
            get
            {
                return new[]
                {
                    eftConfiguration,
                    pluginInfo,
                    config,
                    header,
                    toggle,
                    vector2,
                    vector3,
                    vector4,
                    quaternion,
                    @int,
                    intSlider,
                    @float,
                    floatSlider,
                    color,
                    @string,
                    stringDropdown,
                    @enum,
                    keyboardShortcut,
                    stringAction,
                    unknown,
                    unknownCustom
                };
            }
        }
    }
}