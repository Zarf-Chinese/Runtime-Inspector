using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using System.Reflection;
namespace RTI
{
    [UnityEngine.DisallowMultipleComponent]
    public class MemberInspector : InspectorBehaviour
    {
        public List<string> keys;
        public virtual bool IsFit(string key)
        {
            return keys.Contains(key);
        }
        /// <summary>
        /// 数据端的数据内容。对该数据内容进行修改不会影响到工作状态
        /// </summary>
        /// <value></value>
        public object MemberData
        {
            get
            {
                var ret = this.memberAttribute.GetMemberData(this.Host);
                var type = ret.GetType();
                return ret;
            }

            set => this.memberAttribute.SetMemberData(this.Host, value);
        }
        public MemberAttribute memberAttribute;
        public System.Type MemberType { get => this.memberAttribute.GetMemberType(); }
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
            //在behaviour活动期间，不断进行wantState检查
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal && this.wantState == WorkState.Inputing);
                //若需要刷新，则进行刷新
                this.CurrentWorkState = WorkState.Inputing;
                yield return this.StartCoroutine(this.InputCoroutine());
                this.CurrentWorkState = WorkState.Normal;
                this.wantState = WorkState.Normal;
            }
        }
        private IEnumerator TryRefreshCoroutine()
        {
            //在behaviour活动期间，不断进行wantState检查
            while (true)
            {
                yield return new WaitUntil(() => this.CurrentWorkState == WorkState.Normal && this.wantState == WorkState.Refreshing);
                //若需要刷新，则进行刷新
                this.CurrentWorkState = WorkState.Refreshing;
                yield return this.StartCoroutine(this.RefreshCoroutine());
                this.CurrentWorkState = WorkState.Normal;
                this.wantState = WorkState.Normal;
            }
        }
        /// <summary>
        /// ### 输入工作协程
        /// 为后端输入数据
        /// 在需要从前端得到数据，更新后端内容时被调用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator InputCoroutine()
        {
            //输出信息
            Interf.Instance.Print("[RTI] Start to accpet input of the Inspector");
            yield return null;
        }
        /// <summary>
        /// ### 刷新工作协程
        /// 重新刷新前端内容的协程
        /// 在需要从后端获取数据，更新前端内容时被调用
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator RefreshCoroutine()
        {
            //调用相应事件
            this.OnRefreshToStart.Invoke();
            yield return null;
            //输出信息
            Interf.Instance.Print("[RTI] Start to refresh the Inspector");
            yield return null;
        }

        private IEnumerator TryNormalCoroutine()
        {
            var normalCoroutine = this.NormalCoroutine();
            while (true)
            {
                //检查当前工作状态
                while (this.CurrentWorkState != WorkState.Normal || this.wantState != WorkState.Normal)
                {
                    yield return null;
                }
                if (!normalCoroutine.MoveNext())
                {
                    normalCoroutine = this.NormalCoroutine();
                }
                //执行一步NormalCoroutine之后，继续下一步
                yield return null;
            }
        }
        /// <summary>
        /// ### 正常状态协程
        /// 在该协程中，你可以改变wantState来决定是否要在yield之后进入其他某些工作状态。
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator NormalCoroutine()
        {
            yield return null;
        }
    }
}