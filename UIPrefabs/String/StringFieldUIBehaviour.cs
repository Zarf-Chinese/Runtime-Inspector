using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
namespace RTI
{
    [Bind("string")]
    public class StringFieldUIBehaviour : UIBehaviour
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }
        public override IEnumerator RefreshCoroutine()
        {
            yield return base.RefreshCoroutine();

        }
    }
}
