using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTI;
public class DataBehviour : MonoBehaviour
{
    [Field("string")]
    public string data;
    void Start()
    {
        //FindObjectOfType<StringFieldUIBehaviour>().Bind(this);
        Debug.Log(this.GetType().IsAssignableFrom(this.GetType()));
        Debug.Log(this.GetType().IsAssignableFrom(typeof(MonoBehaviour)));
        Debug.Log(typeof(MonoBehaviour).IsAssignableFrom(this.GetType()));
    }

    // Update is called once per frame
    void Update()
    {
        //FindObjectOfType<StringFieldUIBehaviour>().RefreshImmediately();
    }
}
