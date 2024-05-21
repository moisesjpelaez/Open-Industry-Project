using Godot;
using System;

[Tool]
public partial class ConveyorAssembly : Node3D
{
	Node3D conveyorContainer;
	Node3D sideGuardsContainer;
	Node3D legStandsContainer;
	
	BeltConveyor conveyor;
	
	public override void _Ready()
	{
		conveyorContainer = GetNode<Node3D>("Conveyor");
		sideGuardsContainer = GetNode<Node3D>("SideGuards");
		legStandsContainer = GetNode<Node3D>("LegStands");
		conveyor = conveyorContainer.GetChild(0) as BeltConveyor;
	}

	public override void _PhysicsProcess(double delta)
	{
		conveyorContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);
		conveyor.Scale = new Vector3 (Scale.X + 3 , 1, Scale.Z);
		
		sideGuardsContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);
		// TODO: use distance based on X scale for each SideGuard
		foreach(SideGuard sideGuard in sideGuardsContainer.GetChildren())
		{
			sideGuard.Scale = new Vector3(Scale.X + 3, 1, 1);
		}
		
		legStandsContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z); 
		// TODO: spawn new legs depending on scale
		// TODO: use distance based on X scale when new legs are spawned
		foreach(ConveyorLeg leg in legStandsContainer.GetChildren())
		{
			leg.Scale = Scale;
		}
	}
}
