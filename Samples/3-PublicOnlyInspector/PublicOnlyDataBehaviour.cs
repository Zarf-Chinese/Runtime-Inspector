using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTI;
public class PublicOnlyDataBehaviour : MonoBehaviour
{
    //这个属性将不会被检索
    private string privateData = "I'm a private data";
    //这个属性将会被检索
    public string publicData = "I'm a public data";

    //加入检索标记后，该属性会被检索
    [Field("string")]
    private string privateData2 = "I'm a private data, but is inspected.";
    void Start()
    {
        InspectorManager.Instance.Initialize();
        InspectorManager.Instance.Inspect(this, "public only data");
    }
}
