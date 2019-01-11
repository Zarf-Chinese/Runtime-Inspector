using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
namespace RTI
{
    public static class Utils
    {

#if (UNITY_WSA && !ENABLE_IL2CPP) && !UNITY_EDITOR
        public static List<Assembly> _assemblies;
        public static List<Assembly> GetAssemblies()
        {
            if (_assemblies == null)
            {
                System.Threading.Tasks.Task t = new System.Threading.Tasks.Task(() =>
                {
                    _assemblies = GetAssemblyList().Result;
                });
                t.Start();
                t.Wait();
            }
            return _assemblies;
            
        }
        public static async System.Threading.Tasks.Task<List<Assembly>> GetAssemblyList()
        {
            List<Assembly> assemblies = new List<Assembly>();
            //return assemblies;
            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files.Where(file => file.FileType == ".dll" || file.FileType == ".exe"))
            {
                try
                {
                    assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }
            return assemblies;
        }
        public static IEnumerable<Type> GetAllTypes(bool exclude_generic_definition = true)
        {
            var assemblies = GetAssemblies();
            return from assembly in assemblies
                   where !(assembly.IsDynamic)
                   from type in assembly.GetTypes()
                   where exclude_generic_definition ? !type.GetTypeInfo().IsGenericTypeDefinition : true
                   select type;
        }
#else
        public static List<Type> GetAllTypes(bool exclude_generic_definition = true)
        {
            List<Type> allTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    allTypes.AddRange(assemblies[i].GetTypes()
                        .Where(type => exclude_generic_definition ? !type.IsGenericTypeDefinition() : true)
                        );
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }

            return allTypes;
        }
#endif

        public struct InputFieldModel
        {
            public InputFieldModel(Type[] types, TouchScreenKeyboardType keyboardType, InputField.ContentType contentType)
            {
                this.types = new List<Type>(types);
                this.keyboardType = keyboardType;
                this.contentType = contentType;
            }
            public List<Type> types;
            public TouchScreenKeyboardType keyboardType;
            public InputField.ContentType contentType;
        }
        static List<InputFieldModel> inputFieldModels;
        /// <summary>
        /// 输入框配置与类型的关联集合
        /// </summary>
        /// <value></value>
        public static List<InputFieldModel> InputFieldModels
        {
            get
            {
                if (inputFieldModels == null)
                {
                    inputFieldModels = new List<InputFieldModel>(){
                            new InputFieldModel(
                                new Type[] { typeof(decimal),typeof(byte),typeof(Int16), typeof(Int32), typeof(Int64) },
                                TouchScreenKeyboardType.NumberPad,
                                InputField.ContentType.IntegerNumber
                            ),
                            new InputFieldModel(
                                new Type[] { typeof(Single), typeof(Double)},
                                 TouchScreenKeyboardType.NumbersAndPunctuation,
                                InputField.ContentType.DecimalNumber
                            ),
                            new InputFieldModel(
                                new Type[]{ typeof(string), typeof(char) },
                                 TouchScreenKeyboardType.Default,
                                 InputField.ContentType.Standard)
                        };
                }
                return inputFieldModels;
            }
        }
        /// <summary>
        /// 自动根据一种c#基础数值类型为该 inputField 配置合适的输入模式。
        /// </summary>
        /// <param name="inputField"></param>
        /// <param name="type"></param>
        public static void ModelizeInputFieldByType(InputField inputField, Type type)
        {
            //fixme 考虑更多基础类型
            foreach (var model in InputFieldModels)
            {
                if (model.types.Contains(type))
                {
                    //发现了匹配的model，开始配置
                    inputField.keyboardType = model.keyboardType;
                    inputField.contentType = model.contentType;
                    break;
                }
            }
        }

    }
}