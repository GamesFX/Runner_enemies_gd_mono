using System;
using Godot;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Player controller class
	/// </summary>
	class PlayerController : RigidBody2D {

		/// <summary>
		/// Called when player is destroyed
		/// </summary>
		[Signal]
		public delegate void PlayerDestroyed();

		/// <summary>
		/// Forse to jump
		/// </summary>
		[Export]
		public Vector2 ImpulseJump {
			get; private set;
		} = new Vector2(0, -300f);

		/// <summary>
		/// Animation controller node.
		/// </summary>
		public AnimationTree AnimationController {
			get; private set;
		}

		/// <summary>
		/// State machine playback
		/// </summary>
		public AnimationNodeStateMachinePlayback StateMachine {
			get; private set;
		}

		/// <summary>
		/// Visibilty notification node
		/// </summary>
		public VisibilityNotifier2D Notifier {
			get; private set;
		}

		/// <summary>
		/// Game controller
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Check if player can jump.
		/// </summary>
		private bool _canJump = true;

		/// <summary>
		/// Ready method.
		/// </summary>
		public override void _Ready() {
			AnimationController = GetNode<AnimationTree>("AnimationController");
			StateMachine = AnimationController.Get("parameters/playback") as AnimationNodeStateMachinePlayback;
			Notifier = GetNode<VisibilityNotifier2D>("Notifier");
			// Connect collision
			Connect("body_entered", this, nameof(OnBodyCollisionEntered));
			Controller.Connect(nameof(GameController.ConfigurationChanged), this, nameof(OnConfigurationChanged));
			Notifier.Connect("viewport_exited", this, nameof(OnViewportExited));
		}

		/// <summary>
		/// Handle defined input methods
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process(float delta) {

			// Check jump
			if (Input.IsActionJustPressed("ui_jump")) {
				if (!Controller.Start) {
					Controller["Start"] = true;
				} else {
					PlayerJump();
				}
			}
		}

		/// <summary>
		/// Check unhandled input method.
		/// </summary>
		/// <param name="event">Input event</param>
		public override void _Input(InputEvent @event) {
			// Check if is touch input
			if (@event is InputEventScreenTouch)
				OnTouchEvent(@event as InputEventScreenTouch);
			if (@event is InputEventMouseButton)
				OnClickScreenEvent(@event as InputEventMouseButton);
		}

		/// <summary>
		/// Action to player jump.
		/// </summary>
		private void PlayerJump() {
			// Check if player can jump
			if (!_canJump || Controller.Paused)
				return;

			// Apply impulse
			StateMachine.Travel("Jump");
			ApplyImpulse(Vector2.Zero, ImpulseJump);
			_canJump = false;
		}

		/// <summary>
		/// Configure player if pause state change
		/// </summary>
		private void ConfigurePlayerWithPause() {
			// Disable physics and detection collisions
			if (Controller.Paused) {
				CallDeferred("set_mode", ModeEnum.Static);
				ContactMonitor = false;
				AnimationController.Active = false;
			} else {
				CallDeferred("set_mode", ModeEnum.Character);
				ContactMonitor = true;
				AnimationController.Active = true;
			}
		}

		/// <summary>
		/// Check if <paramref name="position"/> has a control node
		/// </summary>
		/// <param name="position"></param>
		private bool RayCastHasControlByPosition(Vector2 position) {
			// Get world physics
			Physics2DDirectSpaceState physics = GetWorld2d().DirectSpaceState;
			// raycast intersection
			Godot.Collections.Array excludes = new Godot.Collections.Array(new[] { this });
			Godot.Collections.Array result = physics.IntersectPoint(
				position,
				100,
				excludes
			);

			// Iterate all nodes
			if (result.Count > 0) {
				foreach (Godot.Collections.Dictionary detection in result) {
					if (!detection.Contains("collider"))
						break;

					Node collider = detection["collider"] as Node;
					if (collider.IsInGroup("UI"))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Detect screen touch
		/// </summary>
		/// <param name="input"></param>
		private void OnTouchEvent(InputEventScreenTouch evt) {

			if (evt.Pressed && !Controller.Start) {
				if (!RayCastHasControlByPosition(evt.Position))
					Controller["Start"] = true;
				return;
			}

			if (evt.Pressed && Controller.Start && !Controller.Paused) {
				if (!RayCastHasControlByPosition(evt.Position))
					PlayerJump();
			}

		}

		/// <summary>
		/// Detech click screen touch
		/// </summary>
		/// <param name="evt"></param>
		private void OnClickScreenEvent(InputEventMouseButton evt) {

			if (evt.ButtonIndex == (int)ButtonList.Left && evt.Pressed && !Controller.Start) {
				if (!RayCastHasControlByPosition(evt.Position))
					Controller["Start"] = true;
				return;
			}

			if (evt.ButtonIndex == (int)ButtonList.Left && evt.Pressed && Controller.Start && !Controller.Paused) {
				if (!RayCastHasControlByPosition(evt.Position))
					PlayerJump();
			}

		}

		/// <summary>
		/// Check if collision detect other body.
		/// </summary>
		/// <param name="body">Collision body</param>
		private void OnBodyCollisionEntered(Node body) {
			// Check if node is floor
			if (body.IsInGroup("Floor") && Controller.Start) {
				_canJump = true;
				StateMachine.Travel("Walk");
			}

			// Check if node is enemy
			if (body.IsInGroup("Enemy") && Controller.Start) {
				_canJump = false;
				CallDeferred("set_contact_monitor", false);

				StateMachine.Travel("Die");
				SetProcessInput(false);
				SetProcessInput(false);

				// Apply force to die
				ApplyImpulse(Vector2.Zero, ImpulseJump);
				Controller["Start"] = false;
				EmitSignal(nameof(PlayerDestroyed));
			}
		}

		/// <summary>
		/// Handle configuration changed
		/// </summary>
		/// <param name="field"></param>
		private void OnConfigurationChanged(string field) {

			switch (field) {
				case "Paused":
					ConfigurePlayerWithPause();
					break;
				case "Start":
					PlayerJump();
					break;
			}

		}

		/// <summary>
		/// Called when node exit of viewport
		/// </summary>
		/// <param name="viewport"></param>
		private void OnViewportExited(Viewport _) {
			QueueFree();
		}

	}

}
