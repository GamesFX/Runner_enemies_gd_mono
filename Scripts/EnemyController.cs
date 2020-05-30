using Godot;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Enemy controller class
	/// </summary>
	public class EnemyController : RigidBody2D {

		/// <summary>
		/// Signal emited when node is deleted
		/// </summary>
		[Signal]
		public delegate void NodeDeleted(Node2D node);

		/// <summary>
		/// Enemy velocity property
		/// </summary>
		[Export(PropertyHint.Range, "50, 5000")]
		public float EnemyVelocity {
			get; private set;
		} = 120f;

		/// <summary>
		/// Animation tree node
		/// </summary>
		public AnimationTree AnimationController {
			get; private set;
		}

		/// <summary>
		/// Visibility notifier node.
		/// Used to detect when node exit of screen or viewport.
		/// </summary>
		public VisibilityNotifier2D Notifier {
			get; private set;
		}

		/// <summary>
		/// Game controller property
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Scale enemy velocity
		/// </summary>
		private float _scaleVelocity;

		/// <summary>
		/// Ready method
		/// </summary>
		public override void _Ready() {
			// Set animation controller
			AnimationController = GetNode<AnimationTree>("AnimationController");
			// Set notifier
			Notifier = GetNode<VisibilityNotifier2D>("Notifier");
			// Initialize properties
			_scaleVelocity = Controller.GetConfiguration<float>("VelocityScale");
			// Connect signals
			Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnConfigurationChanged));
			// Connect notifier
			Notifier.Connect("viewport_exited", this, nameof(OnViewportExited));
		}

		/// <summary>
		/// Called each frame
		/// </summary>
		/// <param name="delta"></param>
		public override void _PhysicsProcess(float delta) {
			// Check if game is start
			if (Controller.Start && !Controller.Paused)
				Position += Vector2.Left * EnemyVelocity * _scaleVelocity * delta;
		}

		/// <summary>
		/// Change behaviour if pause property change
		/// </summary>
		private void ManagePause() {
			if (Controller.Paused) {
				AnimationController.Active = false;
			} else {
				AnimationController.Active = true;
			}
		}

		/// <summary>
		/// Handle change configuration
		/// </summary>
		/// <param name="field"></param>
		private void OnConfigurationChanged(string field) {
			// Switch field data
			switch (field) {
				case "VelocityScale":
					_scaleVelocity = Controller.GetConfiguration<float>(field);
					break;
				case "Paused":
					ManagePause();
					break;
			}
		}

		/// <summary>
		/// Manage viewport exit
		/// </summary>
		/// <param name="v"></param>
		private void OnViewportExited(Viewport v) {
			// Emit signal
			EmitSignal(nameof(NodeDeleted), this);
			// Delete node
			QueueFree();
		}

	}

}
