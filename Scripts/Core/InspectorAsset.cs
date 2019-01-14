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

        /// <summary>
        /// 每个BindAttribute都有一个名字，这个列表则记录了要参与到Bind中的所有BindAttribute的名字。
        /// </summary>
        [Tooltip("要使用的BindAttribute的名字")]
        public List<string> activeBindNameList;

        //fixme public
    }
}