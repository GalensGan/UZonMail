using System.Reflection.Emit;
using System.Reflection;

namespace UZonMailService.Services.Settings
{
    /// <summary>
    /// 传入的 T 应是接口，不要传递 class
    /// 后期再实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setting"></param>
    public class SettingChain<T>(T setting) where T : ISettingChain
    {
        /// <summary>
        /// 当前级别的设置
        /// </summary>
        private T _setting = setting;

        /// <summary>
        /// 父级设置
        /// </summary>
        private SettingChain<T>? _parent { get; set; }
        private SettingChain<T>? _sub { get; set; }

        /// <summary>
        /// 设置父级
        /// 返回父级设置
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public SettingChain<T> Parent(SettingChain<T> parent)
        {
            _parent = parent;
            _parent._sub = this;

            return parent;
        }

        /// <summary>
        /// 构建设置
        /// </summary>
        /// <returns></returns>
        public T Build()
        {
            var typeOfT = typeof(T);
            var assemblyName = new AssemblyName("DynamicChainSettingAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicChainSettingModule");

            // 定义一个公共类，继承自 T
            var typeBuilder = moduleBuilder.DefineType("DynamicChainSettingType", TypeAttributes.Public, typeOfT);
            // 定义一个私有字段
            var fieldBuilder = typeBuilder.DefineField("_settings", typeof(List<ISettingChain>), FieldAttributes.Private);
            // 定义一个构造函数
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(List<ISettingChain>) });
            var ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ilGenerator.Emit(OpCodes.Ret);

            // 获取 T 的所有公共方法
            var methods = typeOfT.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var method in methods)
            {
                // 定义一个新的方法，与 T 的方法具有相同的签名
                var methodBuilder = typeBuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual
                    , method.ReturnType, method.GetParameters().Select(p => p.ParameterType).ToArray());

                ilGenerator = methodBuilder.GetILGenerator();

                var foreachLoopEnd = ilGenerator.DefineLabel();
                var foreachLoopStart = ilGenerator.DefineLabel();

                ilGenerator.DeclareLocal(typeof(int));
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Stloc_0);

                ilGenerator.MarkLabel(foreachLoopStart);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Callvirt, typeof(List<ISettingChain>).GetProperty("Count").GetGetMethod());
                ilGenerator.Emit(OpCodes.Bge, foreachLoopEnd);

                ilGenerator.Emit(OpCodes.Ldarg_0);
                ilGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Callvirt, typeof(List<ISettingChain>).GetMethod("get_Item"));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Callvirt, typeof(ISettingChain).GetMethod("Test"));
                ilGenerator.Emit(OpCodes.Brtrue, foreachLoopEnd);

                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ldc_I4_1);
                ilGenerator.Emit(OpCodes.Add);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Br, foreachLoopStart);

                ilGenerator.MarkLabel(foreachLoopEnd);
                ilGenerator.Emit(OpCodes.Ldc_I4_0);
                ilGenerator.Emit(OpCodes.Ret);

                // 重写基类的方法
                typeBuilder.DefineMethodOverride(methodBuilder, method);
            }

            // 创建类型
            var dynamicType = typeBuilder.CreateType();
            return default;
        }
    }
}
