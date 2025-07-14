using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevForBeginners
{
    public class SerializedInterfaceAttribute : PropertyAttribute
    {
        public Type[] baseTypes;
        public bool sceneObjects;

        public SerializedInterfaceAttribute(Type[] baseTypes, bool sceneObjects)
        {
            this.baseTypes = baseTypes;
            this.sceneObjects = sceneObjects;
        }
    }
}