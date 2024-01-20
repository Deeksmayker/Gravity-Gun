using UnityEngine;

public static class Layers
{
    public static LayerMask PhysicsObjects { get; } = LayerMask.GetMask("PhysicsObject");
    public static LayerMask PhysicsInteractable { get; } = LayerMask.GetMask("PhysicsInteractable");
    public static LayerMask Interactable { get; } = LayerMask.GetMask("Interactable");
    public static LayerMask Environment { get; } = LayerMask.GetMask("Environment");
    public static LayerMask PlayerBase { get; } = LayerMask.GetMask("Player");
	public static LayerMask Standable {get;} = LayerMask.GetMask("Environment", "PhysicsObject", "PhysicsInteractable");
	public static LayerMask GravityObstacle {get;} = LayerMask.GetMask("Environment", "GravityObstacle");
	public static LayerMask GravityTarget {get;} = LayerMask.GetMask("PhysicsObject", "PhysicsInteractable");
}
