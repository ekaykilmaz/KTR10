using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace KTForm
{
    public sealed class CrossThreadUI
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        private static readonly BindingFlags
            Binding = BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public;
        //***************************************************************************
        // Public Fields
        // 
        /// <summary>
        /// A bool value indicating 'True' if thread delegates should be executed synchronously. Otherwise, false.
        /// </summary>
        public static bool
            ExecSync = true;
        //***************************************************************************
        // Cross-Thread Delegates
        // 
        private delegate void RefreshControlDelegate(Control ctrl);
        private delegate void SetTextDelegate(Control ctrl, string text, bool appendIt);
        private delegate void SetBoolDelegate(Control ctrl, bool visible);
        private delegate void SetIntDelegate(Control ctrl, int value);
        private delegate void SetPropertyDelegate(Control ctrl, string propertyName, object value, object[] args);
        private delegate void ShowMessageBoxDelegate(IWin32Window owner, string msg, string caption, MessageBoxButtons btns, MessageBoxIcon icon, MessageBoxDefaultButton defBtn);
        private delegate object InvokeMethodDelegate(Control ctrl, string methName, object[] args);
        private delegate object GetPropertyInstanceDelegate(Control ctrl, string propertyName);
        private delegate object InvokePropertyMethodDelegate(Control ctrl, string propertyName, string methodName, object[] args);
        private delegate void SetObjectPropertyValueDelegate(object ctrl, string propertyName, object value, object[] args);
        #endregion

        #region Static Methods
        //***************************************************************************
        // Static Methods
        // 
        /// <summary>
        /// Tells the specified control to invalidate its client area and immediately redraw itself.
        /// </summary>
        /// <param name="ctrl"></param>
        public static void RefreshControl(Control ctrl)
        {
            if (ctrl.InvokeRequired)
            {
                RefreshControlDelegate del = new RefreshControlDelegate(CrossThreadUI.RefreshControl);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del);
                else
                    ctrl.BeginInvoke(del);
            }
            else
                ctrl.Invalidate();
        }
        /// <summary>
        /// Sets the 'Text' property of a given control.
        /// </summary>
        /// <param name="ctrl">The System.Windows.Forms.Control object whose text property will be set.</param>
        /// <param name="text">The text to be assigned to the property.</param>
        public static void SetText(Control ctrl, string text, bool appendIt)
        {
            if (ctrl.InvokeRequired)
            {
                SetTextDelegate del = new SetTextDelegate(CrossThreadUI.SetText);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, text, appendIt);
                else
                    ctrl.BeginInvoke(del, ctrl, text, appendIt);
            }
            else
            {
                if(appendIt)
                    ctrl.Text = ctrl.Text + text;
                else
                    ctrl.Text = text;
            }
        }
        public static void SetVisible(Control ctrl, bool visible)
        {
            if (ctrl.InvokeRequired)
            {
                SetBoolDelegate del = new SetBoolDelegate(CrossThreadUI.SetVisible);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, visible);
                else
                    ctrl.BeginInvoke(del, ctrl, visible);
            }
            else
                ctrl.Visible = visible;
        }
        public static void SetEnabled(Control ctrl, bool enabled)
        {
            if (ctrl.InvokeRequired)
            {
                SetBoolDelegate del = new SetBoolDelegate(CrossThreadUI.SetEnabled);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, enabled);
                else
                    ctrl.BeginInvoke(del, ctrl, enabled);
            }
            else
                ctrl.Enabled = enabled;
        }
        public static void SetWidth(Control ctrl, int value)
        {
            if (ctrl.InvokeRequired)
            {
                SetIntDelegate del = new SetIntDelegate(CrossThreadUI.SetWidth);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, value);
                else
                    ctrl.BeginInvoke(del, ctrl, value);
            }
            else
                ctrl.Width = value;
        }
        public static void SetHeight(Control ctrl, int value)
        {
            if (ctrl.InvokeRequired)
            {
                SetIntDelegate del = new SetIntDelegate(CrossThreadUI.SetHeight);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, value);
                else
                    ctrl.BeginInvoke(del, ctrl, value);
            }
            else
                ctrl.Height = value;
        }
        public static void SetChecked(Control ctrl, bool value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "Checked", value); }
            catch { throw; }
        }
        public static void SetDatasource(Control ctrl, object value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "DataSource", value); }
            catch { throw; }
        }
        public static void SetValue(Control ctrl, object value)
        {
            try
            { CrossThreadUI.SetPropertyValue(ctrl, "Value", value); }
            catch { throw; }
        }
        public static void SetPropertyValue(Control ctrl, string propertyName, object value, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                SetPropertyDelegate del = new SetPropertyDelegate(CrossThreadUI.SetPropertyValue);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, propertyName, value, args);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName, value, args);
            }
            else
            {
                Type ctrlType = ctrl.GetType();
                PropertyInfo pi = ctrlType.GetProperty(propertyName, Binding);

                // Make sure the object "Value" matches the property's Type.
                if (!value.GetType().IsAssignableFrom(pi.PropertyType))
                    throw new ArgumentException("Value Type does not match property Type.", "value");

                if (pi != null)
                    try
                    { pi.SetValue(ctrl, ((pi.PropertyType.FullName == "System.String") ? value.ToString() : value), args); }
                    catch { throw; }
                else
                    throw new ArgumentException("Specified control does not expose a '" + propertyName + "' property.", "propertyName");
            }
        }
        public static void SetPropertyValue(object ctrl, string propertyName, object value, params object[] args)
        {
            // The first thing we have to do is figure out if we can tell whether or
            //   not we're on the right thread.  If we can't tell that, then this
            //   whole thing is useless.
            bool needsInvoke = false;
            object parent = ctrl;
            // Get the object's Type.
            Type objType = ctrl.GetType();
            PropertyInfo piInvoke = null;

            // Try and see if the passed object has a property called
            //   "InvokeRequired".  Probably not, so...
            if ((piInvoke = objType.GetProperty("InvokeRequired")) != null)
            {
                object retVal = piInvoke.GetValue(ctrl, null);
                needsInvoke = (retVal == null || retVal.GetType().Name != "Boolean") ? false : (bool)retVal;
            }
            else if ((piInvoke = objType.GetProperty("Owner")) != null || (piInvoke = objType.GetProperty("Parent")) != null)
            {
                // If it doesn't have the "InvokeRequired" property, we check to
                //   see if it has a property named "Owner"
                object owner = piInvoke.GetValue(ctrl, null);
                if (owner != null)
                {
                    // If it does, grab the owner and get it's Type.
                    Type ownType = owner.GetType();
                    // Now we'll check to see if the owner has an "InvokeRequired"
                    //   property. This is the way ToolStripMenus and ToolBarMenus
                    //   work.
                    PropertyInfo pOwn = ownType.GetProperty("InvokeRequired");
                    if (pOwn != null)
                    {
                        object retVal = pOwn.GetValue(owner, null);
                        needsInvoke = (retVal == null || retVal.GetType().Name != "Boolean") ? false : (bool)retVal;
                        // We have to remember which object defines our current
                        //   thread context.
                        parent = owner;
                    }
                }
            }
            else
                // If none of that worked, then we can't figure it out, so just throw
                //   an error.  Otherwise we'll end up triggering one by accident or
                //   in an infinate loop.
                throw new ArgumentException("Cannot determine thread context from the given object.");


            // If made it this far, we found some way to determine thread context, so
            //   now this starts to look a little more familiar.
            if (needsInvoke)
            {
                // The 'parent' variable will be a reference to either the passed
                //   object or it's owner, if an "Owner" property was found.  This,
                //   also, is for ToolStripMenus and ToolBarMenus.  Individual
                //   ToolBarButtons and ToolStripMenuItems do not have "Invoke" or
                //   "BeginInvoke" methods.  We have to call their owner's methods.
                Type parType = parent.GetType();
                Type[] paramTypes = new Type[] { typeof(Delegate), typeof(object[]) };
                MethodInfo miInvoke = null;
                // Create the delegate like normal;
                SetObjectPropertyValueDelegate del = new SetObjectPropertyValueDelegate(CrossThreadUI.SetPropertyValue);
                // Then try and find the "Invoke" or "BeginInvoke" method. If we
                //   don't find either, we have to throw an exception.
                if (CrossThreadUI.ExecSync && ((miInvoke = parType.GetMethod("Invoke", paramTypes)) != null))
                    miInvoke.Invoke(parent, new object[] { del, new object[] { ctrl, propertyName, value, args } });
                else if ((miInvoke = parType.GetMethod("BeginInvoke", paramTypes)) != null)
                    miInvoke.Invoke(parent, new object[] { del, new object[] { ctrl, propertyName, value, args } });
                else
                    throw new ArgumentException("Specified object does not expose any default methods for invoking methods on its executing thread.");
            }
            else
            {
                // Now that we've determined thread context and decided we're on the
                //   correct thread, lets set the property value.
                PropertyInfo pi = objType.GetProperty(propertyName, CrossThreadUI.Binding);

                // Make sure the passed object's Type matches the property's Type.
                if (pi.PropertyType != typeof(System.String) && !value.GetType().IsAssignableFrom(pi.PropertyType))
                    throw new ArgumentException("Value Type does not match property Type.", "value");

                if (pi != null)
                    try
                    { pi.SetValue(ctrl, ((pi.PropertyType.FullName == "System.String") ? value.ToString() : value), args); }
                    catch { throw; }
            }
        }
        public static object InvokeMethod(Control ctrl, string methodName, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                InvokeMethodDelegate del = new InvokeMethodDelegate(CrossThreadUI.InvokeMethod);
                if (CrossThreadUI.ExecSync)
                    return ctrl.Invoke(del, ctrl, methodName, args);
                else
                    ctrl.BeginInvoke(del, ctrl, methodName, args);
            }
            else
            {
                Type ctrlType = ctrl.GetType();

                // Determine the type of each passed argument in order to try and
                //   determine the unique signature for the requested method.
                Type[] paramTypes = new Type[args.Length];
                for (int i = 0; i < paramTypes.Length; i++)
                    paramTypes[i] = args[i].GetType();

                // Now it's time to get a reference to the method.
                MethodInfo mi = ctrlType.GetMethod(methodName, paramTypes);
                if (mi != null)
                {
                    #region DEPRECIATED :: Checking Method Parameters
                    // We don't really need to do this, since we're using the
                    //   provided method arguments as a signature when we
                    //   search for the method.

                    //// Check to make sure the proper number of parameters were passed
                    ////   and that all the types match.
                    //ParameterInfo[] p = mi.GetParameters();
                    //if (p.Length > 0)
                    //{
                    //    // If the 'args' value is null or the lengths don't match,
                    //    //   throw an exception.
                    //    if (args == null || (args.Length != p.Length))
                    //        throw new ArgumentException("Wrong number of arguments for method '" + methodName + "'.", "args");

                    //    // Check to make sure all the parameters are of the correct type.
                    //    for (int i = 0; i < p.Length; i++)
                    //    {
                    //        Type argType = args[i].GetType();
                    //        if (argType.FullName != p[i].ParameterType.FullName && !p[i].ParameterType.IsSubclassOf(argType))
                    //            throw new ArgumentException(string.Format("Given argument is of the wrong type for parameter '{0}'. Expected '{1}' but recieved '{2}' for parameter position {3}.", p[i].Name, p[i].ParameterType.FullName, argType.FullName, p[i].Position), "args");
                    //    }
                    //}
                    #endregion

                    // If we passed all the validation, then call the invoke for the
                    //   MemberInfo object.
                    return mi.Invoke(ctrl, args);
                }
                else
                    throw new ArgumentException("Specified control does not expose a '" + methodName + "' method with the provided parameter types.", "methodName");
            }
            return null;
        }
        public static object GetPropertyInstance(Control ctrl, string propertyName)
        {
            if (ctrl.InvokeRequired)
            {
                GetPropertyInstanceDelegate del = new GetPropertyInstanceDelegate(CrossThreadUI.GetPropertyInstance);
                if (CrossThreadUI.ExecSync)
                    return ctrl.Invoke(del, ctrl, propertyName);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName);
            }
            else
            {
                Type ctrlType = ctrl.GetType();
                PropertyInfo pi = ctrlType.GetProperty(propertyName, CrossThreadUI.Binding);
                if (pi != null)
                {
                    return pi.GetValue(ctrl, null);
                }
                else
                    throw new ArgumentException("Specified control does not expose a '" + propertyName + "' property.");
            }
            return null;
        }
        public static object InvokePropertyMethod(Control ctrl, string propertyName, string methodName, params object[] args)
        {
            if (ctrl.InvokeRequired)
            {
                InvokePropertyMethodDelegate del = new InvokePropertyMethodDelegate(CrossThreadUI.InvokePropertyMethod);
                if (CrossThreadUI.ExecSync)
                    ctrl.Invoke(del, ctrl, propertyName, methodName, args);
                else
                    ctrl.BeginInvoke(del, ctrl, propertyName, methodName, args);
            }
            else
            {
                object propVal = CrossThreadUI.GetPropertyInstance(ctrl, propertyName);
                if (propVal != null)
                {
                    Type objType = propVal.GetType();

                    // Determine the type of each passed argument in order to try and
                    //   determine the unique signature for the requested method.
                    Type[] paramTypes = new Type[args.Length];
                    for (int i = 0; i < paramTypes.Length; i++)
                        paramTypes[i] = args[i].GetType();

                    // Now, it's time to get a reference to the method.
                    MethodInfo mi = objType.GetMethod(methodName, paramTypes);
                    if (mi != null)
                    {
                        return mi.Invoke(propVal, args);
                    }
                    else
                        throw new ArgumentException("Specified ('" + propertyName + "') property value does not expose a '" + methodName + "' method with the provided parameter types.");
                }
            }
            return null;
        }
        public static void ShowMessageBox(IWin32Window owner, string msg, string caption, MessageBoxButtons btns, MessageBoxIcon icon, MessageBoxDefaultButton defBtn)
        {
            Type ownerType = owner.GetType();
            PropertyInfo invokeProp = ownerType.GetProperty("InvokeRequired");
            bool invokeReq = false;
            if (invokeProp != null)
                invokeReq = Convert.ToBoolean(invokeProp.GetValue(owner, null));
            if (invokeProp == null || !invokeReq)
            {
                MessageBox.Show(owner, msg, caption, btns, icon, defBtn);
            }
            else
            {
                MethodInfo beginInvokeMeth = ownerType.GetMethod(((CrossThreadUI.ExecSync) ? "Invoke" : "BeginInvoke"), new Type[] { typeof(Delegate), typeof(Object[]) });
                if (beginInvokeMeth != null)
                {
                    ShowMessageBoxDelegate del = new ShowMessageBoxDelegate(CrossThreadUI.ShowMessageBox);
                    beginInvokeMeth.Invoke(owner, new object[] { del, new object[] { owner, msg, caption, btns, icon, defBtn } });
                }
            }
        }
        #endregion
    }
}
