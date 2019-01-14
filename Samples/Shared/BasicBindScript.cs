using System;
using System.Collections.Generic;
using RTI;

public class BasicBind
{
    [Bind("basic")]
    public static IEnumerable<Binder> basicBindList;
    static BasicBind()
    {
        var bindList = new List<Binder>();
        bindList.Add(new Binder("integer",
            typeof(Int16),
            typeof(Int32),
            typeof(Int64)));
        bindList.Add(new Binder("string",
            typeof(String)));
        bindList.Add(new Binder("decimal",
            typeof(Single),
            typeof(Double),
            typeof(Decimal)));
        basicBindList = bindList;
    }
}