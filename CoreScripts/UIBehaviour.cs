using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;
namespace RTI
{
    public class UIBehaviour : MonoBehaviour
    {
        public FieldAttribute fieldAttribute;
        public object Host
        {
            get => this.host;
            set
            {
                this.host = value;
            }
        }
        private object host;
        /// <summary>
        /// 工作状态。各个工作状态之间不可以重叠
        /// </summary>
        public enum WorkState
        {
            Normal,
            /// <summary>
            /// 正在进行数据刷新
            /// </summary>
            Refreshing
        }
        /// <summary>
        /// 是否需要在下一帧进行刷新。
        /// </summary>
        public bool isDirty;
        public WorkState CurrentWorkState { get; private set; }

        /// <summary>
        /// 当refresh即将开始
        /// 将会先于所有refresh逻辑执行。
        /// </summary>
        [Tooltip("当refresh即将开始时的瞬间触发")]
        public UnityEvent OnRefreshToStart;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            this.isDirty = true;
            this.CurrentWorkState = WorkState.Normal;
            this.StartCoroutine(this.CheckDirtyCoroutine());
        }
        /// <summary>
        /// 立刻强制并即刻完成刷新，不判断是否需要。
        /// </summary>
        public void RefreshImmediately()
        {
            var i = this.RefreshCoroutine();
            //i不再有下一步时，MoveNext会返回false
            while (!i.MoveNext()) ;
        }
        private IEnumerator CheckDirtyCoroutine()
        {
            //在behaviour活动期间，不断进行isDirty检查
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal && this.isDirty);
                //若需要刷新，则进行刷新
                this.CurrentWorkState = WorkState.Refreshing;
                yield return this.RefreshCoroutine();
                this.CurrentWorkState = WorkState.Normal;
                this.isDirty = false;
            }
        }
        /// <summary>
        /// 重新刷新数据内容的协程
        /// 在isDirty为true后会被调用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator RefreshCoroutine()
        {
            //调用相应事件
            this.OnRefreshToStart.Invoke();
            yield return null;
            //输出信息
            Interf.Instance.Print("[RTI] Start to refresh the Property UI");
            yield return null;
        }
    }
}