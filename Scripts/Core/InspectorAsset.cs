using UnityEngine;
using System.Collections.Generic;
namespace RTI
{
    [CreateAssetMenu()]
    public class InspectorAsset : ScriptableObject
    {
        /// <summary>
        /// fixme 根检索器预置
        /// </summary>
        [Tooltip("要使用的根检索器预置")]
        public GameObject rootInspectorPrefab;
        /// <summary>
        /// 所有记录在册的检索器预置
        /// </summary>
        [Tooltip("所有要使用的类成员检索器预置")]
        public List<GameObject> memberInpectorPrefabList;
    }
}