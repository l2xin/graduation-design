/*----------------------------------------------------------------
            // Copyright © 2015 Air2000
            // 
            // FileName: VisiableTest.cs
			// Describle:
			// Created By:  hsu
			// Date&Time:  2016/7/12 14:17:47
            // Modify History:
            //
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GTools
{
    public class VisiableTest : MonoBehaviour
    {
        public static bool IsVisible(Renderer renderer, Camera camera)
        {
            if (renderer == null || camera == null) return true;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }
    }
}
