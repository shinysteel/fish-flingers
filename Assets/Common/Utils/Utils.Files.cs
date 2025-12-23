using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace ShinyOwl.Common.Utils
{
    public static partial class Utils
    {
        public static class Files
        {
            public static Type FindTypeInAssembly(string typeName)
            {
                return Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .FirstOrDefault(t => t.Name == typeName);
            }
        }
    }
}