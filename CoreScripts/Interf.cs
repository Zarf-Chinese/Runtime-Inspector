using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RTI
{
    /// <summary>
    /// 对外的接口类，你可以自定义这些接口的实现
    /// </summary>
    public class Interf
    {
        private static Interf instance;

        public static Interf Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Interf();
                }
                return instance;
            }
            set => instance = value;
        }

        public virtual void Print(object toPrint)
        {
            Debug.Log(toPrint);
        }
        public virtual void Print(string message, params object[] args)
        {
            Debug.LogFormat(message, args);
        }
    }
}
