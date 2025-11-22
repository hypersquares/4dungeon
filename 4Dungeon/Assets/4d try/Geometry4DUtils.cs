using System.Collections.Generic;
using UnityEngine;

public static class Geometry4DUtils
{
	public static int Intersection(List<Vector4> list, Vector4 v0, Vector4 v1)
	{
		// Both points are 3D ==> the entire segment lies in the 3D space
		if (v1.w == 0 && v0.w == 0)
		{
			list.Add(v0);
			list.Add(v1);
			return 2;
		}

		// Both w coordinates are equale
		// If they are both 0 ==> the entire line is in the 3D space (already tested)
		// If they are not 0 ==> the entire line is outside the 3D space
		if (v1.w - v0.w == 0)
			return 0;

		// Time of intersection
		float t = -v0.w / (v1.w - v0.w);

		// No intersection
		if (t < 0 || t > 1)
			return 0;

		// One intersection
		Vector4 x = v0 + (v1 - v0) * t;
		list.Add(x);
		return 1;
	}
}