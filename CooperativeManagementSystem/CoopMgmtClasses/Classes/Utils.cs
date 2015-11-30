﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace CoopManagement.Classes
{
    public class Utils
    {
        public static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(Byte),
            typeof(SByte),
            typeof(UInt16),
            typeof(UInt32),
            typeof(UInt64),
            typeof(Int16),
            typeof(Int32),
            typeof(Int64),
            typeof(Decimal),
            typeof(Double),
            typeof(Single)
        };

        public static String Format(params object[] param)
        {
            String f = "";
            String placeholder = "";
            for (int i = 0; i < param.Length; i++)
            {
                placeholder = NumericTypes.Contains(param[i].GetType()) ? "{{{0}}}" : "'{{{0}}}'";
                f += String.Format(placeholder, i);
                f += i == param.Length - 1 ? "" : ", ";
            }
            
            String output = String.Format(f, param);
            return output;
        }

        public static void EscapeKeyHandler(object sender, KeyEventArgs e, Control ctl)
        {
            if (e.KeyCode == Keys.Escape)
            {
                GetParentForm(ctl).ActiveControl = ctl;
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        public static Form GetParentForm(Control parent)
        {
            Form form = parent as Form;
            if (form != null)
            {
                return form;
            }
            if (parent != null)
            {
                // Walk up the control hierarchy
                return GetParentForm(parent.Parent);
            }
            return null; // Control is not on a Form
        }
    }

    public static class Extensions
    {
        public static string ToText(this string[] array)
        {
            String s = "";
            for(int i=0;i<array.Length;i++)
            {
                s += array[i];
                s += i == array.Length - 1 ? "" : ", ";
            }
            return s;
        }
    }

}
