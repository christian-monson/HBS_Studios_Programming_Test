
// NOTES:
//
// 1) I implemented my answers to all the questions in this programming test in C#. The code snippets
//    in Question 1 are in C/C++ syntax, hence I made some minor modifications to the definitions of both
//    the Rect struct as well as the DistanceToRect function. Specifically,
//
//    a) struct fields in C# are private by default, while they are public in C, so I made the struct fields public
//    b) The syntax for passing by reference is different in C# than in C/C++, so I made the appropriate modifications
//
// 2) I interpret the "distance to the nearest edge of the rectangle" to mean the distance to the closest point on the
//    parimeter of the rectangle (and not merely the perpendicular distance to an infinite extension of an edge of
//    the rectangle).

using System;
using System.Diagnostics;

namespace Question1_DistanceToRect {

	public class DistanceToRectCalculator {

		public struct Rect {
			public float minX;
			public float minY;
			public float maxX;
			public float maxY;
		};

		// Returns 0f if (x, y) is inside of rect (a rectangle),
		// Otherwise, Returns the minimum distance from (x, y) to a point on the perimiter of rect.
		public static float DistanceToRect(float x, float y, ref Rect rect) {

			if (rect.minX >= rect.maxX || rect.minY >= rect.maxY) {
				throw new ArgumentException($"Invalid rectangle: ({rect.minX,4},{rect.minY,4},{rect.maxX,4},{rect.maxY,4})");
			}

			Debug.Assert(rect.minX < rect.maxX);
			Debug.Assert(rect.minY < rect.maxY);

			float xDistToRect = 0f;
			float yDistToRect = 0f;

			if (x < rect.minX) {
				xDistToRect = rect.minX - x;
			} else if (x > rect.maxX) {
				xDistToRect = x - rect.maxX;
			}

			if (y < rect.minY) {
				yDistToRect = rect.minY - y;
			} else if (y > rect.maxY) {
				yDistToRect = y - rect.maxY;
			}

			float dist = MathF.Sqrt(xDistToRect * xDistToRect + yDistToRect * yDistToRect);

			return dist;
		}
	}
}
