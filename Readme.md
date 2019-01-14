# Runtime Inspector

-   作者/author 张和(zarf)
-   版本/version 0.8
-   指导老师/tutor 黄东晋

## 简介

Runtime Inspector 是一套基于 Unity(2018.3) 的运行时属性检索框架，可以让你的 Unity 游戏可以在运行时检索、编辑你所需要的对象属性。

Unity 编辑器提供了在 Editor 环境下的属性检索器（也就是 Unity Inspector 面板）。但如果你想要在移动端等非 Editor 环境下对属性进行实时检索，很抱歉，Unity 不会帮我们考虑这个。考虑到这一点，我制作了 Runtime Inspector ，让它可以在非 Editor 环境下正常工作。

使用 Runtime Inspector，你只要进行几步配置，就可以在游戏运行时实时检索到某个游戏对象的内部成员了！

### 使用说明

-   有什么可以供我参考的实例呢？

    > 打开 Samples 文件夹，你可以在里面找到很多使用参考

-   我应该做什么来让 Runtime Inspector 来识别并检索我的类成员呢？

    在 Samples 中，你会发现，通过某些配置， Runtime Inspector 的识别方法可以和 Unity Inspector 非常类似。

    > Unity Inspector 的基本识别方法大致如下：
    >
    > 1. Unity Inspector 只能检索**继承了 UnityEngine.Object 的类型**的成员。
    > 2. 目标对象中的 public Field （并且只支持一部分数据类型）可以被检索
    > 3. 使用 _SerializedField_ 可以让 non-public Field 被检索
    > 4. 使用 _System.Serializable_ 可以让某个数据类型被检索
    >
    > 而 Runtime Inspector 的识别方法更加个性化。
    > 在 Samples 中，出现过一下几种检索条件
    >
    > 1. 使用 RTI.Field(key) 可以标记某个 Field 被检索
    > 2. 使用 RTI.Property(key) 可以标记某个 Property 被检索
    > 3. 在标记中注明检索关键词，可以根据关键词来指定检索器类型
    > 4. 使用一个绑定脚本，可以将一些类型与某个检索关键词绑定。该类型或其派生类型的 Field 或 Property 将被检索
    > 5. Runtime Inspector 使用一组过滤器来决定是否检索某个类成员。
    > 6. 通过使用 RTI.RegistFilter(filterName,index)标记一个静态函数，来注册一个过滤器。
    > 7. 在 FieldInspectInfo 和 PropertyInspectInfo 注册了一个过滤器。由于这个过滤器存在，第 4 点才能生效
    > 8. 在 InspectManager 中注册了一个过滤器。由于这个过滤器的存在，第 1、2 点才能生效
    >
    > 通过学习 Samples 的一些范例，你甚至可以通过自定义的方法来让 Runtime Inspector 的识别方式和 Unity Inspector 完全一致。
    > 除此之外，Runtime Inspector 提供了更加个性化的检索方式
    > Runtime Inspector 的 UI 系统完全基于 UGUI，你可以通过制作自己的 UI 预置，来让 Runtime Inspector 变得无比强大！

## 项目内容

Runtime Inspector 框架包含以下三部分内容：

1. [核心逻辑](#核心逻辑)
2. [检索器预置](#检索器预置)
3. 入门场景

### 核心逻辑

-   MemberAttribute:Attribute
    > 1. 标记一个需要检索的 Member(包括 Field、Method、Property 等)
-   InspectorBehaviour:MonoBehaviour
    > 1. 定义了增减次一层检索器的方法
-   MemberInspector:InspectorBehaviour
    > 1. 提供了显示目标 Member 的功能
    > 2. 提供了修改目标 Member 的输入监听功能
    > 3. 使用一组 Key 值来决定该检索器适用的 Member 类型范围
    > 4. 在检索目标对象的所有类成员时，使用**检索过滤器**来决定是否识别并检索某个类成员
-   InspectorManager:MonoBehaviour
    > 1. 使用单例模式
    > 2. 存储了所有检索器预置体
    > 3. 定义了为单一物体创建检索器的方法
    > 4. 定义了其他一些核心逻辑方法
-   InspectInfo
    > 1. 为 MemberInspector 提供了检索一个类成员的信息
    > 2. 定义了从宿主对象索取、修改 Member 内容的方法
    > 3. 使用一个 Key 值来标记该 Member 所的适用 Inspector 类型
-   (delegate)InspectInfoFilter
    > 1. 作为**检索过滤器**，用于从一个目标对象中滤取要进行检索的成员信息
    > 2. 在 InspectorManager 内部，将顺序执行一系列已注册的**检索过滤器**来决定是否检索某对象的成员信息

### 检索器预置

由于本框架基于 Unity2018.3 版本实现，此版本的 Unity 提供 Prefab 的嵌套与变体功能。框架也借助此功能，使得框架内原生提供的各个 Prefab 具有继承关系。

-   MemberInspectorBase
    > 所有类成员检索器的基础预置
    > 定义了类成员与检索器内容的基本布局方法
-   InputInspector:MemberInspector
    > 使用字符串输入方法更改、显示数据
-   StepInsector:MemberInspectorBase
    > 使用"+"与"-"两个按钮和字符串输入方法更改、显示数据
