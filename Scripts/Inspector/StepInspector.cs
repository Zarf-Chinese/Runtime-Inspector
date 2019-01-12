using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
namespace RTI
{
    public class StepInspector : InputInspector
    {
        public double step = 1;
        public NumberStyles numberStyle = NumberStyles.Any;

        public Button more;
        public Button less;
        protected override void Start()
        {
            base.Start();
            this.more.onClick.AddListener(() => this.ChangeByStep(true));
            this.less.onClick.AddListener(() => this.ChangeByStep(false));
        }
        public virtual void ChangeByStep(bool more = true)
        {
            var delta = more ? this.step : -this.step;
            this.MemberData = Convert.ChangeType((double)Convert.ChangeType(this.MemberData, typeof(double)) + delta, this.MemberType);
        }
    }
}