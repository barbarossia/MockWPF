using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AddIn
{
    public static class AutoScrollPreventerBehavior
    {
        #region IsBroughtIntoViewWhenSelected

        /// <summary>
        /// Dependency property getter--not actually used since XAML calls GetValue directly but part of the dependency pattern
        /// </summary>
        public static bool GetPrevent(FrameworkElement control)
        {
            return (bool)control.GetValue(PreventProperty);
        }

        /// <summary>
        /// Dependency property setter--not actually used since XAML calls SetValue directly but part of the dependency pattern
        /// </summary>
        public static void SetPrevent(
          FrameworkElement control, bool value)
        {
            control.SetValue(PreventProperty, value);
        }

        /// <summary>
        /// Swallow RequestBringIntoView routed events if Prevent is set = true
        /// </summary>
        public static readonly DependencyProperty PreventProperty =
            DependencyProperty.RegisterAttached(
            "Prevent",
            typeof(bool),
            typeof(AutoScrollPreventerBehavior),
            new UIPropertyMetadata(false, OnPreventChanged));

        /// <summary>
        /// This method is called when XAML is parsed, as the attached property Prevent is set = true (or false for no-op).
        /// </summary>
        static void OnPreventChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            var item = depObj as FrameworkElement;
            if (item == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                item.RequestBringIntoView += SwallowBringIntoViewRequest;
            else
                item.RequestBringIntoView -= SwallowBringIntoViewRequest;
        }

        /// <summary>
        /// stop this event from bubbling so that a scrollviewer doesn't try to BringIntoView..             
        /// </summary>
        static void SwallowBringIntoViewRequest(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
        #endregion // IsBroughtIntoViewWhenSelected
    }
}
