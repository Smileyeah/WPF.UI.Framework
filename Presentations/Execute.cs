
namespace WPF.UI.Core
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Threading;
    using System.ComponentModel;
    using System.Diagnostics;
    /// <summary>
    ///   将代码转到UI线程
    /// </summary>
    public static class Execute
    {
        static bool? inDesignMode;
        static Action<System.Action> executor = action => action();

        /// <summary>
        ///   Indicates whether or not the framework is in design-time mode.
        /// </summary>
        public static bool InDesignMode
        {
            get
            {
                if (inDesignMode == null)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    inDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                    if (!inDesignMode.GetValueOrDefault(false) && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                        inDesignMode = true;
                }

                return inDesignMode.GetValueOrDefault(false);
            }
        }

        /// <summary>
        ///   初始化UI线程
        /// </summary>
        public static void InitializeWithDispatcher()
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            SetUIThreadMarshaller(action => {
                try
                {
                    if (dispatcher.CheckAccess())
                        action();
                    else dispatcher.Invoke(action);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex;
                }
            });
        }

        /// <summary>
        ///   Resets the executor to use a non-dispatcher-based action executor.
        /// </summary>
        public static void ResetWithoutDispatcher()
        {
            SetUIThreadMarshaller(action => action());
        }

        /// <summary>
        /// Sets a custom UI thread marshaller.
        /// </summary>
        /// <param name="marshaller">The marshaller.</param>
        public static void SetUIThreadMarshaller(Action<System.Action> marshaller)
        {
            executor = marshaller;
        }

        /// <summary>
        ///  切换线程到UI线程
        /// </summary>
        /// <param name = "action">切换至UI线程中需要执行的Action</param>
        public static void OnUIThread(this System.Action action)
        {
            try
            {
                executor(action);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 封装System.Windows.Application.Current.Dispatcher.Invoke调用，尝试切换到主线程同步执行
        /// </summary>
        public static void UIThreadInvoke(DispatcherPriority priority, Delegate method)
        {
            Execute.OnUIThread(method as System.Action);
        }

        /// <summary>
        /// 封装System.Windows.Application.Current.Dispatcher.BeginInvoke的调用，尝试切换到主线程异步执行
        /// </summary>
        public static void UIThreadBeginInvoke(DispatcherPriority priority, Delegate method)
        {
            Application objApplication = System.Windows.Application.Current;
            if (null == objApplication)
            {
                return;
            }

            Dispatcher objDispatcher = objApplication.Dispatcher;
            if (null == objDispatcher)
            {
                return;
            }

            try
            {
                objDispatcher.BeginInvoke(priority, method);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

    }
}
