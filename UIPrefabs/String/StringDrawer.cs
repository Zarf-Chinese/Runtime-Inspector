using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.UI;
using System.Text;
namespace RTI
{
    [Drawer("string")]
    public class StringDrawer : DrawerBehaviour
    {
        public InputField text;
        public bool isInputing { get => this.text.isFocused; }
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Utils.ModelizeInputFieldByType(this.text, this.memberAttribute.GetMemberType());
        }
        public override IEnumerator InputCoroutine()
        {
            yield return base.InputCoroutine();
            this.memberAttribute.SetMemberData(this.Host, this.Parse(this.text.text, this.memberAttribute.GetMemberType()));
        }
        /// <summary>
        /// 将接收到的Input数据反序列化为目标数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object Parse(string data, System.Type type)
        {
            //fixme parse int/float 等数据
            return data;
        }
        public override IEnumerator NormalCoroutine()
        {
            yield return base.NormalCoroutine();
            if (this.isInputing)
            {
                yield return this.wantState = WorkState.Inputing;
            }
            //检查数据端的数据是否发生了变化
            if (text.text != (this.memberAttribute.GetMemberData(this.Host) as string))
            {
                yield return this.wantState = WorkState.Refreshing;
            }
        }
        public override IEnumerator RefreshCoroutine()
        {
            yield return base.RefreshCoroutine();
            var data = this.memberAttribute.GetMemberData(this.Host);
            Debug.Log(data);
            //更新数据
            this.text.text = (data as string);
        }
    }
}
