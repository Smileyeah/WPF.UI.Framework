using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WPF.UI.Framework.Helper
{
    public class CreateDynamicClassHelper
    {
        // 创建编译器实例。
        private static CSharpCodeProvider provider = new CSharpCodeProvider();

        private static CompilerParameters paras = new CompilerParameters();

        public static Assembly NewAssembly(List<string> properties)
        {   
            // 设置编译参数。   
            paras = new CompilerParameters();
            paras.GenerateExecutable = false;
            paras.GenerateInMemory = true;

            //创建动态代码。   
            StringBuilder classSource = new StringBuilder();
            classSource.Append("public   class   DynamicClass \n");
            classSource.Append("{\n");

            //创建属性。   
            foreach (var item in properties)
            {
                classSource.Append(propertyString(item));
            }

            classSource.Append("}");

            System.Diagnostics.Debug.WriteLine(classSource.ToString());

            //编译代码。   
            CompilerResults result = provider.CompileAssemblyFromSource(paras, classSource.ToString());

            //获取编译后的程序集。   
            Assembly assembly = result.CompiledAssembly;

            return assembly;
        }

        public static string propertyString(string propertyName)
        {
            StringBuilder sbProperty = new StringBuilder();
            sbProperty.Append(" private   int   _" + propertyName + "   =   0;\n");
            sbProperty.Append(" public   int   " + "" + propertyName + "\n");
            sbProperty.Append(" {\n");
            sbProperty.Append(" get{   return   _" + propertyName + ";}   \n");
            sbProperty.Append(" set{   _" + propertyName + "   =   value;   }\n");
            sbProperty.Append(" }");
            return sbProperty.ToString();
        }

        public static void ReflectionSetProperty(object objClass, string propertyName, int value)
        {
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == propertyName && info.CanWrite)
                {
                    info.SetValue(objClass, value, null);
                }
            }
        }

        public static void ReflectionGetProperty(object objClass, string propertyName)
        {
            PropertyInfo[] infos = objClass.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                if (info.Name == propertyName && info.CanRead)
                {
                    System.Console.WriteLine(info.GetValue(objClass, null));
                }
            }
        }
    }
}
