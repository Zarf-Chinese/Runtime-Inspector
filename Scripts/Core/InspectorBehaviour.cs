using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace RTI
{
    public class InspectorBehaviour : MonoBehaviour
    {
        public virtual string Name { get; set; }
        [Tooltip("用以装填子检索器的内容对象")]
        [SerializeField]
        private RectTransform content;
        public RectTransform Content
        {
            get
            {
                if (!this.content)
                {
                    this.content = this.GetComponent<RectTransform>();
                }
                return this.content;
            }
        }
        [SerializeField]
        private List<InspectorBehaviour> children = new List<InspectorBehaviour>();
        /// <summary>
        /// 所有次级索引器
        /// </summary>
        /// <value></value>
        public List<InspectorBehaviour> Children { get => this.children; }
        /// <summary>
        /// 向本索引器中添入次级索引器
        /// * **注意，不是所有索引器都对次级索引器的功能具有完善的支持！**
        /// </summary>
        /// <param name="child"></param>
        public virtual void AddChild(GameObject child)
        {
            var childTransform = (child.GetComponent<RectTransform>());
            var childInspector = child.GetComponent<InspectorBehaviour>();
            this.Children.Add(childInspector);
            childTransform.SetParent(Content, false);
        }
        /// <summary>
        /// 从本索引器中移除该次级索引器
        /// * **注意，不是所有索引器都对次级索引器的功能具有完善的支持！**
        /// </summary>
        /// <param name="child"></param>
        public virtual void RemoveChild(GameObject child)
        {
            var childTransform = (child.GetComponent<RectTransform>());
            var childInspector = child.GetComponent<InspectorBehaviour>();
            if (this.Children.Contains(childInspector))
            {
                this.Children.Remove(childInspector);
            }
            childTransform.SetParent(null, false);
        }
    }
}