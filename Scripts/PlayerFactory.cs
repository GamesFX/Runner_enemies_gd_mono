using Godot;
using RunnerEnemyGD.Scripts.CustomNodes;

namespace RunnerEnemyGD.Scripts {

	/// <summary>
	/// Create a custom factory node
	/// </summary>
	public class PlayerFactory : FactoryNode {

		/// <summary>
		/// This signal is used to create player instances
		/// </summary>
		[Signal]
		public delegate void Spawn();

		/// <summary>
		/// This signal is used to destroy instance node
		/// </summary>
		[Signal]
		public delegate void DestroyInstance();

		/// <summary>
		/// Target instance spawn
		/// </summary>
		[Export]
		public NodePath TargetInstance {
			get; private set;
		}

		/// <summary>
		/// Save current instance
		/// </summary>
		public Node2D CurrentInstance {
			get; private set;
		}

		/// <summary>
		/// Save game controller
		/// </summary>
		public GameController Controller {
			get; private set;
		} = GameController.Controller;

		/// <summary>
		/// Ready method
		/// </summary>
		public override void _Ready() {
			// Connect signals
			Connect(nameof(Spawn), this, nameof(OnSpawnRequest));
			Connect(nameof(InstancedNode), this, nameof(OnInstancedNode));
			Connect(nameof(DestroyInstance), this, nameof(OnDestroyInstance));
			// Create first node
			EmitSignal(nameof(Spawn));
		}

		/// <summary>
		/// Process method
		/// </summary>
		/// <param name="delta"></param>
		public override void _Process(float delta) {
			// Check if instance not exists
			if (CurrentInstance != null)
				return;

			// Instance node
			if (Input.IsActionJustPressed("ui_jump"))
				EmitSignal(nameof(Spawn));
		}

		/// <summary>
		/// Reques spawn object
		/// </summary>
		private void OnSpawnRequest() {
			// Current instance
			if (CurrentInstance == null) {
				Node2D instance = MakeInstance();
				if (TargetInstance == null) {
					GetParent().CallDeferred("add_child", instance);
				} else {
					Node target = GetNode(TargetInstance);
					target.CallDeferred("add_child", instance);
				}
			}
		}

		/// <summary>
		/// Manage instances
		/// </summary>
		/// <param name="node"></param>
		private void OnInstancedNode(Node node) {
			CurrentInstance = node as Node2D;
			CurrentInstance.Connect(nameof(PlayerController.PlayerDestroyed), this, nameof(OnDestroyInstance));
		}

		/// <summary>
		/// Manage destroy instances
		/// </summary>
		private void OnDestroyInstance() {
			// Check if instance exists
			if (CurrentInstance == null)
				return;
			// Destroy instance and restore values
			CurrentInstance = null;
		}


	}

}
