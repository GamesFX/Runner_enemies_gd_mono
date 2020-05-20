using System;
using Godot;

namespace RunnerEnemyGD.Scripts.Core {

	/// <summary>
	/// Operating system enumerator
	/// </summary>
	public enum OSType {
		WINDOWS,
		UWP,
		X11,
		OSX,
		ANDROID,
		IOS,
		HTML5,
		HAIKU,
		SERVER
	}

	/// <summary>
	/// Extension methods to <c>OSType</c>
	/// </summary>
	public static class OSTypeExtensions {

		/// <summary>
		/// Check target platform.
		/// </summary>
		/// <param name="sType">Target enum extension</param>
		/// <returns><c>true</c> if is current platform, <c>false</c> otherwise.</returns>
		public static bool Evaluate(this OSType sType) {
			// Save current os platform
			string currentOS = OS.GetName().ToLower();
			string selectedOS = sType.ToString().ToLower();

			return currentOS.Equals(selectedOS);
		}


	}

}
