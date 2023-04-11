using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace InitialProject.Resources.UIHelper
{
    public class UIHelper
    {
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject  // XAML ListBox item that has checkbox/button.. -> CODE BEHIND variable Checkbox,Button, stackoverflow
        {
            if (parent == null)
            {
                return null;
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T foundChild = FindVisualChild<T>(child);
                    if (foundChild != null)
                    {
                        return foundChild;
                    }
                }
            }
            return null;
        }
        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
            {
                return null;
            }

            if (parent is T)
            {
                return (T)parent;
            }

            return FindVisualParent<T>(parent);
        }
    }
}
