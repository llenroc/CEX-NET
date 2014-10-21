﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VTDev.Projects.CEX.CryptoGraphic.Helpers
{
    class Compare
    {
        internal static bool AreEqual(byte[] a, byte[] b)
        {
            int i = a.Length;

            if (i != b.Length)
                return false;

            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        internal static bool AreEqual(char[] a, char[] b)
        {
            int i = a.Length;

            if (i != b.Length)
                return false;

            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }

        internal static bool AreEqual(int[] a, int[] b)
        {
            int i = a.Length;

            if (i != b.Length)
                return false;

            while (i != 0)
            {
                --i;
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}
