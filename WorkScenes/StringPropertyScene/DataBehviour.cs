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
    [RTI.Field("decimal")]
    public float flData;
    public void test<T>()
    {
        var data = System.Convert.ChangeType(1234.5678d, typeof(T));
        Debug.Log(data + " " + data.GetType());
    }
    void Start()
    {
        //FindObjectOfType<StringFieldUIBehaviour>().Bind(this);
        InspectorManager.Instance.Initialize();
        InspectorManager.Instance.Inspect(this);
        // test<int>();
        // test<double>();
        // test<string>();
        // test<decimal>();
    }
    public IEnumerator TestEmptyCoroutine(int i)
    {
        //空协程指的是在运行期间没有任何yield的协程
        if (false)
        {
            yield return null;
        }
        if (i-- > 0)
        {
            //运行下一个协程。如果StartCoroutine内部没有进行任何yield，则这些过程都会在一帧内执行
            this.StartCoroutine(TestEmptyCoroutine(i));
        }
        Debug.LogFormat("运行了倒数第{0}个空协程！", i);
    }
    void Update()
    {
        // Debug.Log("过了一帧！");
    }

    // Update is called once per frame
}
