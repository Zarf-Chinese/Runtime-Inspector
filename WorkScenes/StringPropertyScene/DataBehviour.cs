using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTI;
public class DataBehviour : MonoBehaviour
{
    [RTI.Field("string")]
    public string stdata;
    [RTI.Field("integer")]
    public int inData;
    [RTI.Field("float")]
    public float flData;
    void Start()
    {
        //FindObjectOfType<StringFieldUIBehaviour>().Bind(this);
        InspectorManager.Instance.Initialize();
        InspectorManager.Instance.Inspect(this);
    }

    // Update is called once per frame
    void Update()
    {
        //FindObjectOfType<StringFieldUIBehaviour>().RefreshImmediately();
    }
}
