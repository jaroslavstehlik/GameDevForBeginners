using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevForBeginners
{
    public class SerializedInterfaceAttribute : PropertyAttribute
    {
        public Type interfaceType;
        public bool sceneObjects;

        public SerializedInterfaceAttribute(Type interfaceType, bool sceneObjects)
        {            
            this.interfaceType = interfaceType;
            this.sceneObjects = sceneObjects;
        }
    }
}