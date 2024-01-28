using Godot;
using System;
using System.Threading.Tasks;

[Tool]
public partial class RollerConveyor : Node3D
{
    [Export]
    bool enableComms;
    [Export]
    string tagName;

    [Export]
    public float Speed = 1.0f;
	
	float skewAngle = 0.0f;
	[Export]
	public float SkewAngle
	{
		get
		{
			return skewAngle;
		}
		set
		{
			skewAngle = value;
			SetRollersRotation();
		}
	}
	
	float nodeScaleX = 1.0f;
	float nodeScaleZ = 1.0f;
	float lastScale = 0.0f;
	
	MeshInstance3D meshInstance;
	Material metalMaterial;
	Rollers rollers;
	Node3D ends;

    readonly Guid id = Guid.NewGuid();
    double scan_interval = 0;
    bool running = false;
    bool readSuccessful = false;

    Node3D rollersLow;
    Node3D rollersMid;
    Node3D rollersHigh;

    Root main;
    public Root Main
    {
        get
        {
            return main;
        }
        set
        {
            main = value;
		}
	}

	public override void _Ready()
	{
		meshInstance = GetNode<MeshInstance3D>("ConvRoller");
		meshInstance.Mesh = meshInstance.Mesh.Duplicate() as Mesh;
		metalMaterial = meshInstance.Mesh.SurfaceGetMaterial(0).Duplicate() as Material;
		meshInstance.Mesh.SurfaceSetMaterial(0, metalMaterial);
		rollers = GetNodeOrNull<Rollers>("Rollers");
		ends = GetNodeOrNull<Node3D>("Ends");

		SetRollersSpeed();
		SetRollersRotation();

        Main = GetParent().GetTree().EditedSceneRoot as Root;

        if (Main != null)
        {
            Main.SimulationStarted += OnSimulationStarted;
            Main.SimulationEnded += OnSimulationEnded;
        }
    }

	public override void _PhysicsProcess(double delta)
	{
		if (Scale.X >= 1.0f)
			nodeScaleX = Scale.X;
		if (Scale.Z >= 1.0f)
			nodeScaleZ = Scale.Z;
		
		Scale = new Vector3(nodeScaleX, 1, nodeScaleZ);
		
		if (metalMaterial != null)
			((ShaderMaterial)metalMaterial).SetShaderParameter("Scale", Scale.X);
		
		if (rollers != null && lastScale != Scale.X)
		{
            rollers.ChangeScale(Scale.X);
			lastScale = Scale.X;
        }

        if (running)
        {
            SetRollersSpeed();

            if (enableComms && running && readSuccessful)
            {
                scan_interval += delta;
                if (scan_interval > 0.1 && readSuccessful)
                {
                    scan_interval = 0;
                    Task.Run(ScanTag);
                }
            }
        }
    }
	
	void SetRollersSpeed()
	{
		if (rollers != null)
		{
			foreach (Roller roller in rollers.GetChildren())
			{
				roller.speed = Speed;
			}
		}
		
		if (ends != null)
		{
			foreach(RollerConveyorEnd end in ends.GetChildren())
			{
				end.SetSpeed(Speed);
			}
		}
	}
	
	void SetRollersRotation()
	{
		if (rollers != null)
		{
			foreach (Roller roller in rollers.GetChildren())
			{
				roller.RotationDegrees = new Vector3(0, SkewAngle, 0);
			}
		}
		
		if (ends != null)
		{
			foreach(RollerConveyorEnd end in ends.GetChildren())
			{
				end.RotateRoller(new Vector3(end.RotationDegrees.X, SkewAngle, end.RotationDegrees.Z));
			}
		}
	}

    void OnSimulationStarted()
    {
		if (enableComms)
		{
            Main.Connect(id, Root.DataType.Float, tagName);
        }

        running = true;
        readSuccessful = true;
    }

    void OnSimulationEnded()
    {
        running = false;
    }

    async Task ScanTag()
    {
        try
        {
            Speed = await Main.ReadFloat(id);
        }
        catch
        {
            GD.PrintErr("Failure to read: " + tagName + " in Node: " + Name);
            readSuccessful = false;
        }
    }
}
