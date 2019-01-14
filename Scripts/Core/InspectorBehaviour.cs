using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace RTI
{
    public class InspectorBehaviour : MonoBehaviour
    {
        [Tooltip("用于显示检索目标名称的UI.Text组件")]
        public Text nameField;
        /// <summary>
        /// 要进行检索的目标名称
        /// </summary>
        /// <value></value>
        public virtual string InspectName
        {
            get => nameField.text;
            set => nameField.text = value;
        }
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
        /// <param name="childInspector"></param>
        public virtual void AddChild(InspectorBehaviour childInspector)
        {
            this.Children.Add(childInspector);
            childInspector.transform.SetParent(Content, false);
        }
        /// <summary>
        /// 从本索引器中移除该次级索引器
        /// * **注意，不是所有索引器都对次级索引器的功能具有完善的支持！**
        /// </summary>
        /// <param name="childInspector"></param>
        public virtual void RemoveChild(InspectorBehaviour childInspector, bool destroy = true)
        {
            if (this.Children.Contains((InspectorBehaviour)childInspector))
            {
                this.Children.Remove((InspectorBehaviour)childInspector);
            }
            if (destroy)
            {
                Destroy(childInspector.gameObject);
            }
            else
            {
                childInspector.transform.SetParent(null, false);
            }
        }
        /// <summary>
        /// 清空所有子检索器
        /// </summary>
        /// <param name="destroy"></param>
        public void ClearChildren(bool destroy = true)
        {
            foreach (var child in this.Children.AsReadOnly())
            {
                this.RemoveChild(child, destroy);
            }
        }
    }
}