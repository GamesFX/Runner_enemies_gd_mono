using Godot;

namespace RunnerEnemyGD.Scripts.CustomNodes {

	/// <summary>
	/// Custom node to create nodes
	/// </summary>
	public class FactoryNode : Position2D {

		/// <summary>
		/// Instanced node.
		/// </summary>
		[Signal]
		public delegate void InstancedNode(Node node);

		/// <summary>
		/// Target scene to load
		/// </summary>
		[Export]
		public PackedScene TargetScene {
			get; protected set;
		}

		/// <summary>
		/// Create node instance
		/// </summary>
		/// <returns></returns>
		public Node2D MakeInstance() {
			Node2D result = TargetScene.Instance() as Node2D;
			ConfigureNode(result);

			EmitSignal(nameof(InstancedNode), result as Node);
			return result;
		}

		/// <summary>
		/// Get node instance typed
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T MakeInstanceTyped<T>() where T : Object {
			Object result = TargetScene.Instance();
			try {
				ConfigureNode(result as Node2D);
				EmitSignal(nameof(InstancedNode), result as Node);
				return result as T;
			} catch {
				return null;
			}
		}

		/// <summary>
		/// Set configuration node
		/// </summary>
		/// <param name="node"></param>
		private void ConfigureNode(Node2D node) {
			node.Position = Position;
			node.RotationDegrees = RotationDegrees;
			node.Scale = Scale;
		}

	}


}
