﻿using Godot;


namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Parallax script.
	/// </summary>
	class SceneBackground : ParallaxBackground {

		/// <summary>
		/// Parallax velocity.
		/// </summary>
		[Export(PropertyHint.Range, "20, 1000, 0.25")]
		public float ParallaxVelocity {
			get; private set;
		} = 50f;

		/// <summary>
		/// Controller
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Parallax velocity scale.
		/// </summary>
		private float _parallaxScaleVelocity;

		/// <summary>
		/// Ready method
		/// </summary>
		public override void _Ready() {
			// Set property scale
			_parallaxScaleVelocity = Controller.GetConfiguration<float>("VelocityScale");
			// Connect controller signal
			Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnControllerPropertyChanged));
		}

		/// <summary>
		/// This method is called each frame.
		/// </summary>
		/// <param name="delta">Elapsed time between 2 frames</param>
		public override void _Process(float delta) {
			if (Controller.Start && !Controller.Paused)
				ScrollOffset += Vector2.Left * ParallaxVelocity * _parallaxScaleVelocity * delta;
		}

		/// <summary>
		/// Event to detect property changed.
		/// </summary>
		/// <param name="property">property changed.</param>
		private void OnControllerPropertyChanged(string property) {
			// Verify if velocity scale change
			if (property.Equals("VelocityScale"))
				_parallaxScaleVelocity = Controller.GetConfiguration<float>(property);
		}

	}

}
