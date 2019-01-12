using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.Text;
namespace RTI
{
    public class InputInspector : MemberInspector
    {
        public InputField inputField;
        /// <summary>
        /// 从输入框中可以得到的数据内容
        /// 对该数据内容进行修改不会影响到工作状态
        /// </summary>
        /// <value></value>
        public object InputData
        {
            get
            {
                var ret = this.GetDataFromInput(this.inputField.text);
                var type = ret.GetType();
                return ret;
            }
            set { this.inputField.text = this.GetInputFromData(value); }
        }
        public bool isInputing { get => this.inputField.isFocused; }
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Utils.ModelizeInputFieldByType(this.inputField, this.memberAttribute.GetMemberType());
        }
        public override IEnumerator InputCoroutine()
        {
            yield return base.StartCoroutine(base.InputCoroutine());
            this.memberAttribute.SetMemberData(this.Host, this.GetDataFromInput(this.inputField.text));
        }
        public virtual object GetDataFromInput(string input)
        {
            return System.Convert.ChangeType(input, this.MemberType);
        }
        public virtual string GetInputFromData(object data)
        {
            return data.ToString();
        }
        public override IEnumerator NormalCoroutine()
        {
            yield return this.StartCoroutine(base.NormalCoroutine());
            while (this.isInputing)
            {
                //处于input状态时，不断向后端输入数据
                yield return this.wantState = WorkState.Inputing;
            }
            //不处在input状态时，检查数据端的数据是否发生了变化
            if (!this.InputData.Equals(this.MemberData))
            {
                yield return this.wantState = WorkState.Refreshing;
            }
        }
        public override IEnumerator RefreshCoroutine()
        {
            yield return this.StartCoroutine(base.RefreshCoroutine());
            //更新数据
            this.InputData = this.MemberData;
        }
    }
}
