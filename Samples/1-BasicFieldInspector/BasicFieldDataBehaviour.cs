using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTI;
public class BasicFieldDataBehaviour : MonoBehaviour
{
    [RTI.Field("string")]
    public string stdata;
    [RTI.Field("integer")]
    public int inData;
    [RTI.Field("decimal")]
    public float flData;
    public void test<T>()
    {
        var data = System.Convert.ChangeType(1234.5678d, typeof(T));
        Debug.Log(data + " " + data.GetType());
    }
    void Start()
    {
        Debug.Log(System.DateTime.Now);
        //初始化InspectorManager
        InspectorManager.Instance.Initialize();
        //指定一个要Inspect的对象
        InspectorManager.Instance.Inspect(this, "basic field data");
    }
}
