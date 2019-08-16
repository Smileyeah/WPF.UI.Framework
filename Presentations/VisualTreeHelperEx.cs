using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;


namespace WPF.UI.Presentations
{
    /// <summary>
    /// Provides addition visual tree helper methods.
    /// </summary>
    public static class VisualTreeHelperEx
    {
        /// <summary>
        /// Gets specified visual state group.
        /// </summary>
        public static VisualStateGroup TryGetVisualStateGroup(this DependencyObject dependencyObject, string groupName)
        {
            FrameworkElement root = GetImplementationRoot(dependencyObject);
            if (root == null)
            {
                return null;
            }
            return (from @group in VisualStateManager.GetVisualStateGroups(root).OfType<VisualStateGroup>()
                    where string.CompareOrdinal(groupName, @group.Name) == 0
                    select @group).FirstOrDefault<VisualStateGroup>();
        }

        /// <summary>
        /// Gets the implementation root.
        /// </summary>
        public static FrameworkElement GetImplementationRoot(this DependencyObject dependencyObject)
        {
            if (1 != VisualTreeHelper.GetChildrenCount(dependencyObject))
            {
                return null;
            }
            return (VisualTreeHelper.GetChild(dependencyObject, 0) as FrameworkElement);
        }

        /// <summary>
        /// Returns a collection of the visual ancestor elements of specified dependency object.
        /// </summary>
        /// <returns>
        /// A collection that contains the ancestors elements.
        /// </returns>
        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject dependencyObject)
        {
            var parent = dependencyObject;
            while ( true )
            {
                parent = GetParent(parent);
                if (parent != null)
                {
                    yield return parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns a collection of visual elements that contain specified object, and the ancestors of specified object.
        /// </summary>
        /// <returns>
        /// A collection that contains the ancestors elements and the object itself.
        /// </returns>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject dependencyObject)
        {
            if (null == dependencyObject)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            var parent = dependencyObject;
            while ( true )
            {
                if (null != parent)
                {
                    yield return parent;
                }
                else
                {
                    break;
                }
                parent = GetParent(parent);
            }
        }

        /// <summary>
        /// Gets the parent for specified dependency object.
        /// </summary>
        /// <returns>The parent object or null if there is no parent.</returns>
        public static DependencyObject GetParent(this DependencyObject dependencyObject)
        {
            if (null == dependencyObject)
            {
                throw new ArgumentNullException("dependencyObject");
            }

            var ce = dependencyObject as ContentElement;
            if (null != ce)
            {
                var parent = ContentOperations.GetParent(ce);
                if (null != parent)
                {
                    return parent;
                }

                var fce = ce as FrameworkContentElement;
                return (null != fce) ? fce.Parent : null;
            }

            return VisualTreeHelper.GetParent(dependencyObject);
        }
        

        /// <summary>
        /// 查找特定类型的父对象
        /// </summary>
        public static T GetParentObject<T>(DependencyObject objReference) where T : DependencyObject
        {
            T objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                var objParent = VisualTreeHelper.GetParent(objReference);
                if (null == objParent)
                {
                    break;
                }

                if (objParent is T)
                {
                    objRetval = (T)objParent;
                    break;
                }

                objRetval = VisualTreeHelperEx.GetParentObject<T>(objParent);
            }
            while ( false );

            return objRetval;
        }

        /// <summary>
        /// 查找特定类型、特定名称的父对象
        /// </summary>
        public static T GetParentObject<T>(DependencyObject objReference, string strName) where T : FrameworkElement
        {
            T objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if ( String.IsNullOrEmpty(strName) )
                {
                    objRetval = VisualTreeHelperEx.GetParentObject<T>(objReference);
                    break;
                }

                var objParent = VisualTreeHelper.GetParent(objReference);
                while (null != objParent)
                {
                    if (objParent is T && ((T)objParent).Name == strName)
                    {
                        objRetval = (T)objParent;
                        break;
                    }

                    objParent = VisualTreeHelper.GetParent(objParent);
                }
            }
            while ( false );
            
            return objRetval;
            
        }

        /// <summary>
        /// 深层递归遍历，查找特定类型的子对象
        /// </summary>
        public static T GetChildObject<T>(DependencyObject objReference) where T : DependencyObject
        {
            T objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (null != objChild && objChild is T)
                    {
                        objRetval = (T)objChild;
                        break;
                    }

                    T objGrandChild = VisualTreeHelperEx.GetChildObject<T>(objChild);
                    if (null != objGrandChild)
                    {
                        objRetval = objGrandChild;
                        break;
                    }
                }
            }
            while ( false );

            return objRetval;
        }

        /// <summary>
        /// 深层递归遍历，查找特定类型、特定名称的子对象
        /// </summary>
        public static T GetChildObject<T>(DependencyObject objReference, string strName) where T : FrameworkElement
        {
            T objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if ( String.IsNullOrEmpty(strName) )
                {
                    objRetval = VisualTreeHelperEx.GetChildObject<T>(objReference);
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (objChild is T && ((T)objChild).Name == strName)
                    {
                        objRetval = (T)objChild;
                        break;
                    }

                    T objGrandChild = VisualTreeHelperEx.GetChildObject<T>(objChild, strName);
                    if (null != objGrandChild)
                    {
                        objRetval = objGrandChild;
                        break;
                    }
                }
            }
            while ( false );
            
            return objRetval;
        }

        /// <summary>
        /// 深层递归遍历，查找特定类型的子对象集合
        /// </summary>
        public static List<T> GetChildObjects<T>(DependencyObject objReference) where T : FrameworkElement
        {
            List<T> listChilds = new List<T>();

            do
            {
                if (null == objReference)
                {
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (objChild is T)
                    {
                        listChilds.Add((T)objChild);
                    }

                    List<T> listGrandChilds = VisualTreeHelperEx.GetChildObjects<T>(objChild);
                    if (null != listGrandChilds && listGrandChilds.Count > 0)
                    {
                        listChilds.AddRange(listGrandChilds);
                    }
                }
            }
            while ( false );
            
            return listChilds;
        }

        /// <summary>
        /// 深层递归遍历，查找特定类型、特定名称的子对象集合
        /// </summary>
        public static List<T> GetChildObjects<T>(DependencyObject objReference, string strName) where T : FrameworkElement
        {
            List<T> listChilds = new List<T>();

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if ( String.IsNullOrEmpty(strName) )
                {
                    listChilds = VisualTreeHelperEx.GetChildObjects<T>(objReference);
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (objChild is T && ((T)objChild).Name == strName)
                    {
                        listChilds.Add((T)objChild);
                    }

                    List<T> listGrandChilds = VisualTreeHelperEx.GetChildObjects<T>(objChild, strName);
                    if (null != listGrandChilds && listGrandChilds.Count > 0)
                    {
                        listChilds.AddRange(listGrandChilds);
                    }
                }
            }
            while ( false );
            
            return listChilds;
        }

        /// <summary>
        /// 递归遍历，查找特定类型的子对象集合，对符合条件的子对象不再递归其子控件
        /// </summary>
        public static List<T> GetDirectChildObjects<T>(DependencyObject objReference) where T : FrameworkElement
        {
            List<T> listChilds = new List<T>();

            do
            {
                if (null == objReference)
                {
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (objChild is T)
                    {
                        listChilds.Add((T)objChild);
                        continue;
                    }

                    List<T> listGrandChilds = VisualTreeHelperEx.GetChildObjects<T>(objChild);
                    if (null != listGrandChilds && listGrandChilds.Count > 0)
                    {
                        listChilds.AddRange(listGrandChilds);
                    }
                }
            }
            while ( false );
            
            return listChilds;
        }

        /// <summary>
        /// 在指定根类型的情况下，从当前层级开始逐层往上查找特定类型的对象
        /// </summary>
        public static T1 GetAncestorOrSelf<T1, T2>(DependencyObject objReference) where T1 : FrameworkElement where T2 : FrameworkElement
        {
            T1 objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if (objReference is T2)
                {
                    break;
                }

                if (objReference is T1)
                {
                    objRetval = objReference as T1;
                    break;
                }

                if (objReference is FrameworkContentElement)
                {
                    objRetval = VisualTreeHelperEx.GetAncestorOrSelf<T1, T2>((objReference as FrameworkContentElement).Parent);
                    break;
                }
                
                var objParent = VisualTreeHelper.GetParent(objReference);
                objRetval = VisualTreeHelperEx.GetAncestorOrSelf<T1, T2>(objParent);
            }
            while ( false );

            return objRetval;
        }

        /// <summary>
        /// 在指定根类型的情况下，从当前层级开始逐层往上查找特定类型、指定名称的对象
        /// </summary>
        public static T1 GetAncestorOrSelf<T1, T2>(DependencyObject objReference, string strName) where T1 : FrameworkElement where T2 : FrameworkElement
        {
            T1 objRetval = null;

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if (objReference is T2)
                {
                    break;
                }

                if ( String.IsNullOrEmpty(strName) )
                {
                    objRetval = VisualTreeHelperEx.GetAncestorOrSelf<T1, T2>(objReference);
                    break;
                }

                if (objReference is T1 && ((T1)objReference).Name == strName)
                {
                    objRetval = objReference as T1;
                    break;
                }

                if (objReference is FrameworkContentElement)
                {
                    objRetval = VisualTreeHelperEx.GetAncestorOrSelf<T1, T2>((objReference as FrameworkContentElement).Parent, strName);
                    break;
                }
                
                var objParent = VisualTreeHelper.GetParent(objReference);
                objRetval = VisualTreeHelperEx.GetAncestorOrSelf<T1, T2>(objParent, strName);
            }
            while ( false );

            return objRetval;
        }

        /// <summary>
        /// 在指定子类型（T2）的情况下，从当前层级开始逐层向下查找特定类型的对象（T1）
        /// </summary>
        public static List<T1> GetChildrenOrSelf<T1, T2>(DependencyObject objReference) where T1 : FrameworkElement where T2 : FrameworkElement
        {
            List<T1> listRetval = new List<T1>();

            do
            {
                if (null == objReference)
                {
                    break;
                }

                if (objReference is T2)
                {
                    break;
                }

                if (objReference is T1)
                {
                    listRetval.Add((T1)objReference);
                    break;
                }

                int iChildrenCount = VisualTreeHelper.GetChildrenCount(objReference);
                if (0 >= iChildrenCount)
                {
                    break;
                }

                for (int i = 0; i < iChildrenCount; ++i)
                {
                    DependencyObject objChild = VisualTreeHelper.GetChild(objReference, i);
                    if (objChild is T1)
                    {
                        listRetval.Add((T1)objChild);
                        continue;
                    }

                    List<T1> listGrandChilds = VisualTreeHelperEx.GetChildrenOrSelf<T1, T2>(objChild);
                    if (null != listGrandChilds && listGrandChilds.Count > 0)
                    {
                        listRetval.AddRange(listGrandChilds);
                    }
                }
            }
            while ( false );

            return listRetval;
        }




        public static T FindAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T val = obj as T;
                if (val != null)
                {
                    return val;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        public static T FindLogicalAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T val = obj as T;
                if (val != null)
                {
                    return val;
                }
                obj = LogicalTreeHelper.GetParent(obj);
            }
            return null;
        }

        public static T FindAncestor<T>(this UIElement obj) where T : UIElement
        {
            return FindAncestor<T>((DependencyObject)obj);
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                T val = FindVisualChild<T>(child);
                if (val != null)
                {
                    return val;
                }
            }
            return null;
        }


    }
}
