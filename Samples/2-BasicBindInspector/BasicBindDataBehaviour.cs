using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTI;
/// <summary>
/// 使用Bind方式检索数据
/// </summary>
public class BasicBindDataBehaviour : MonoBehaviour
{
    public string stdata;
    public int inData;
    public float flData;
    void Start()
    {
        InspectorManager.Instance.Initialize();
        InspectorManager.Instance.Inspect(this, "basic bind data");
    }
}
