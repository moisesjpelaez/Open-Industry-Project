using Godot;
using System;

[Tool]
public partial class ConveyorAssembly : Node3D
{
	PackedScene segmentScene = (PackedScene)ResourceLoader.Load("res://parts/ConveyorLegBC.tscn");
	
	int spawnFactor = 4;
	
	bool enableComms = false;
	[Export] bool EnableComms 
	{
		get 
		{ 
			return enableComms; 
		}
		set 
		{
			enableComms = value;
			if (conveyor != null) conveyor.EnableComms = enableComms;
		}
	}
	
	Color beltColor = new Color(1, 1, 1, 1);
	[Export] Color BeltColor 
	{
		get
		{
			return beltColor;
		}
		set
		{
			beltColor = value;
			if (conveyor != null) conveyor.BeltColor = beltColor;
		}
	}
	
	BeltConveyor.ConvTexture beltTexture = BeltConveyor.ConvTexture.Standard;
	[Export] public BeltConveyor.ConvTexture BeltTexture
	{
		get
		{
			return beltTexture;
		}
		set
		{
			beltTexture = value;
			if (conveyor != null) conveyor.BeltTexture = beltTexture;
		}
	}
	
	float speed = -2;
	[Export] float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
			if (conveyor != null) conveyor.Speed = speed;
		}
	}
	
	Node3D conveyorContainer;
	BeltConveyor conveyor;
	Node3D sideGuardsContainer;
	SideGuard sideGuardL;
	SideGuard sideGuardR;
	Vector3 sideGuardsOffset = new Vector3(0, 0.45f, 0.055f);
	Node3D legStandEndsContainer;
	ConveyorLeg legStandEndL;
	ConveyorLeg legStandEndR;
	float legEndsOffset = 1;
	
	public override void _Ready()
	{
		conveyorContainer = GetNode<Node3D>("Conveyor");
		conveyor = conveyorContainer.GetChild(0) as BeltConveyor;
		sideGuardsContainer = GetNode<Node3D>("SideGuards");
		sideGuardL = sideGuardsContainer.GetChild(0) as SideGuard;
		sideGuardR = sideGuardsContainer.GetChild(1) as SideGuard;
		legStandEndsContainer = GetNode<Node3D>("LegStandEnds");
		legStandEndL = legStandEndsContainer.GetChild(0) as ConveyorLeg;
		legStandEndR = legStandEndsContainer.GetChild(1) as ConveyorLeg;
	}

	public override void _PhysicsProcess(double delta)
	{
		conveyorContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);
		conveyor.Scale = new Vector3 (Scale.X + 3 , 1, Scale.Z);
		
		sideGuardsContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);
		sideGuardL.Scale = new Vector3(Scale.X + 3, 1, 1);
		sideGuardL.Position = new Vector3(0, sideGuardsOffset.Y, -(Scale.Z - 1 - sideGuardsOffset.Z));
		sideGuardR.Scale = new Vector3(Scale.X + 3, 1, 1);
		sideGuardR.Position = new Vector3(0, sideGuardsOffset.Y, Scale.Z - 1 - sideGuardsOffset.Z);
		
		legStandEndsContainer.Scale = new Vector3(1 / Scale.X, 1 / Scale.Y, 1 / Scale.Z);
		legStandEndL.Scale = Scale;
		legStandEndL.Position = new Vector3(Scale.X * 0.5f + legEndsOffset, 0, 0);
		legStandEndR.Scale = Scale;
		legStandEndR.Position = new Vector3(-(Scale.X * 0.5f + legEndsOffset), 0, 0);
		
		
		
		foreach(ConveyorLeg leg in legStandEndsContainer.GetChildren())
		{
			leg.GrabsRotation = RotationDegrees.Z;
			leg.RotationDegrees = new Vector3(0, 0, -RotationDegrees.Z);
		}
		
		// TODO: set proper legs height based on Z rotation and their local position 
	}
	
	void SpawnLegs()
	{
		
	}
	
	void RemoveLegs()
	{
		
	}
}
