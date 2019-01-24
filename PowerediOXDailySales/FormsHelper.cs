using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;
using System;

namespace PowerediOXDailySales
{
    public static class FormsHelper
    {

        public static void ClearBindings(this List<Control> controlList)
        {
            foreach (Control control in controlList)
            {
                control.DataBindings.Clear();
            }
        }

        public static void CenterControl(this Control controlValue)
        {
            var parentForm = controlValue.FindForm();
            if (parentForm != null)
            {
                controlValue.Left = (parentForm.Width - controlValue.Width) / 2;
                controlValue.Top = (parentForm.Height - controlValue.Height) / 2;
            }
        }
        public static void ClearText(this Control form, params dynamic[] objectsList)
        {
            foreach (var obj in objectsList)
                obj.Text = "";
        }

        public static void SetValue<T>(this object targetObject, T value)
        {
            targetObject = value;
        }
        public static T GetControl<T>(this Control formValue, string controlName)
        {
            return formValue.GetAllControls().ToList().Where(ctrl => ctrl.Name == controlName).Cast<T>().FirstOrDefault();
        }
        public static IEnumerable<Control> GetAllTypeOf<T>(this Control controlParent)
        {
            foreach(Control parentControl in controlParent.GetAllControls())
            {
                ToastNotification.Show(parentControl,parentControl.Name);
                if (parentControl is T)
                    yield return parentControl;
            }
        }
        public static IEnumerable<Control> GetAllControls(this Control controlParent)
        {
            foreach (Control parentControl in controlParent.Controls)
            {
                if (parentControl.HasChildren)
                    foreach (Control childControl in parentControl.GetAllControls())
                        yield return childControl;
                yield return parentControl;
            }
        }

        public static bool VisibleDock(this Control controlValue,bool visibleValue, Control dockParent,DockStyle dockType = DockStyle.Top)
        {
            controlValue.Visible = visibleValue;
            controlValue.Parent = visibleValue ? dockParent : null;
            controlValue.Dock = visibleValue ? dockType : DockStyle.None;
            if(visibleValue) controlValue.BringToFront();
            return visibleValue;
        }
    }
}
