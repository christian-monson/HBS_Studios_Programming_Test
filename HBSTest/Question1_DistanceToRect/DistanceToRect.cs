
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

	class Launcher {

		struct Rect {
			public float minX;
			public float minY;
			public float maxX;
			public float maxY;
		};

		// Returns 0f if (x, y) is inside of rect
		// Otherwise, Returns the minimum distance from (x, y) to a point on the perimiter of rect
		static float DistanceToRect(float x, float y, ref Rect rect) {

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

		static void Test_DistanceToRect(float x, float y, ref Rect rect) {
			float distToRect = DistanceToRect(x, y, ref rect);
			Console.WriteLine(
				$"{distToRect,7:0.00} : from ({x,5},{y,5}) to the rectangle ({rect.minX,4},{rect.minY,4},{rect.maxX,4},{rect.maxY,4})");
		}

		static void Main(string[] args) {

			//Console.WriteLine("Hello World!");

			Rect test_rect = new Rect {
				minX = 100f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};

			// Test the 9 quadrants
			Test_DistanceToRect(   1f,    1f, ref test_rect);
			Test_DistanceToRect( 150f,    1f, ref test_rect);
			Test_DistanceToRect(1000f,    1f, ref test_rect);
			Test_DistanceToRect(   1f,  250f, ref test_rect);
			Test_DistanceToRect( 150f,  250f, ref test_rect);
			Test_DistanceToRect(1000f,  250f, ref test_rect);
			Test_DistanceToRect(   1f, 1000f, ref test_rect);
			Test_DistanceToRect( 150f, 1000f, ref test_rect);
			Test_DistanceToRect(1000f, 1000f, ref test_rect);

			// Test an "edge" case (lol)
			Test_DistanceToRect(100f, 250f, ref test_rect);

			// Test a "corner" case (lol!!)
			Test_DistanceToRect(100f, 200f, ref test_rect);

			// Test a negative case
			Test_DistanceToRect(-100f, 250f, ref test_rect);

			// Test a rectangle with negative positions
			Rect test_rect2 = new Rect {
				minX = -100f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};
			Test_DistanceToRect(-1000f, 250f, ref test_rect2);

			Rect test_rect3 = new Rect {  // minX larger than maxX, will fail in the Assert in DistanceToRect
				minX = 1000f,
				minY = 200f,
				maxX = 300f,
				maxY = 400f
			};
			//Test_DistanceToRect(1f, 1f, ref test_rect3);  // will fail on the Assert in DistanceToRect
		}
	}
}
