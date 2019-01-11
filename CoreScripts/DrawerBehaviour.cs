using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;
namespace RTI
{
    [UnityEngine.DisallowMultipleComponent]
    [Drawer("null")]
    public class DrawerBehaviour : InspectorBehaviour
    {
        public MemberAttribute memberAttribute;
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
            /// 正在进行数据刷新，从数据端接收数据更新，此时不应该接受输入
            /// </summary>
            Refreshing,
            /// <summary>
            /// 正在接受数据输入，此时不会从数据端接收数据更新。
            /// </summary>
            Inputing
        }
        /// <summary>
        /// 希望要去执行/进入的状态
        /// </summary>
        public WorkState wantState;
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
            this.wantState = WorkState.Refreshing;
            this.CurrentWorkState = WorkState.Normal;
            this.StartCoroutine(this.TryNormalCoroutine());
            this.StartCoroutine(this.TryRefreshCoroutine());
            this.StartCoroutine(this.TryInputCoroutine());
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
        private IEnumerator TryInputCoroutine()
        {
            //在behaviour活动期间，不断进行isDirty检查
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal && this.wantState == WorkState.Inputing);
                //若需要刷新，则进行刷新
                this.CurrentWorkState = WorkState.Inputing;
                yield return this.InputCoroutine();
                this.CurrentWorkState = WorkState.Normal;
                this.wantState = WorkState.Normal;
            }
        }
        private IEnumerator TryRefreshCoroutine()
        {
            //在behaviour活动期间，不断进行isDirty检查
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal && this.wantState == WorkState.Refreshing);
                //若需要刷新，则进行刷新
                this.CurrentWorkState = WorkState.Refreshing;
                yield return this.RefreshCoroutine();
                this.CurrentWorkState = WorkState.Normal;
                this.wantState = WorkState.Normal;
            }
        }
        /// <summary>
        /// ### 输入工作协程
        /// 为数据域输入数据
        /// 在IsNeedInput为true后会被调用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator InputCoroutine()
        {
            //输出信息
            Interf.Instance.Print("[RTI] Start to accpet input of the Drawer");
            yield return null;
        }
        /// <summary>
        /// ### 刷新工作协程
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
            Interf.Instance.Print("[RTI] Start to refresh the Drawer");
            yield return null;
        }

        private IEnumerator TryNormalCoroutine()
        {
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal);
                yield return this.NormalCoroutine();
            }
        }
        /// <summary>
        /// ### 正常状态协程
        /// 在该协程中，你可以改变wantState来决定是否要进入其他某些工作状态。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator NormalCoroutine()
        {
            //等待进入无欲无求的状态，防止某个工作需求被忽略
            yield return new WaitUntil(() => this.wantState == WorkState.Normal);
        }
    }
}