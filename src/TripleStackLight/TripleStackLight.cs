using Godot;
using System;

[Tool]
public partial class TripleStackLight : Node3D
{
	PackedScene segmentScene = (PackedScene)ResourceLoader.Load("res://src/TripleStackLight/TripleStackSegment.tscn");
	[Export] TripleStackLightData data = (TripleStackLightData)ResourceLoader.Load("res://src/TripleStackLight/TripleStackLightData.tres");
	
	bool enableComms = false;
	[Export]
	bool EnableComms
	{
		get
		{
			return enableComms;
		}
		set
		{
			enableComms = value;

            NotifyPropertyListChanged();

            if (segmentsContainer != null)
			{
				foreach (TripleStackSegment segment in segmentsContainer.GetChildren())
				{
					segment.EnableComms = enableComms;
                }
			}
		}
	}

    int updateRate = 100;

    [Export]

	int UpdateRate
	{
		get
		{
			return updateRate;
		}
		set
		{
            updateRate = value;
            
            if (segmentsContainer != null)
			{
                foreach (TripleStackSegment segment in segmentsContainer.GetChildren())
				{
                    segment.updateRate = updateRate;
                }
            }
        }
	}


    int segments = 1;
	[Export]
	int Segments
	{
		get
		{
			return segments;
		}
		set
		{
			if (value == segments || running) return;
			
			int new_value = Mathf.Clamp(value, 1, 10);
			if (new_value > segments)
			{
				SpawnSegments(new_value - segments);
			}
			else
			{
				RemoveSegments(segments - new_value);
			}
			
			segments = new_value;
			
			if (segmentsContainer != null)
			{
				data.SetSegments(segments);
				InitSegments();
				
				if (topMesh != null)
					topMesh.Position = new Vector3(0, topMeshInitialYPos + (step * (segments - 1)), 0);
			}
			
			NotifyPropertyListChanged();
		}
	}
	
	float step = 0.048f;
	Node3D segmentsContainer;
	MeshInstance3D topMesh;
	float segmentInitialYPos;
	float topMeshInitialYPos = 0.087f;
	bool running = false;
	
	MeshInstance3D bottomMesh;
	MeshInstance3D stemMesh;
	MeshInstance3D midMesh;

	Root main;
	public Root Main { get; set; }
	
	public override Variant _Get(StringName property)
	{
		if (segmentsContainer == null) return default;
		for (int i = 0; i < segments; i++)
		{
			if (property == "Light " + (i + 1).ToString())
			{
				TripleStackSegment segment = segmentsContainer.GetChild(i) as TripleStackSegment;
				return segment.SegmentData; 
			}
		}
		return default;
	}
	
	public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
	{
		Godot.Collections.Array<Godot.Collections.Dictionary> properties = new Godot.Collections.Array<Godot.Collections.Dictionary>();
		for (int i = segments - 1; i >= 0; i--)
		{
			properties.Add(new Godot.Collections.Dictionary()
			{
				{"name", "Light " + (i + 1).ToString()},
				{"class_name", "TripleStackSegmentData"},
				{"type", (int)Variant.Type.Object},
				{"usage", (int)PropertyUsageFlags.Default}
			});
		}
		return properties;
	}
	
	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		string propertyName = property["name"].AsStringName();

		if (propertyName == PropertyName.data)
		{
			property["usage"] = (int)PropertyUsageFlags.NoEditor;
		}
        else if (propertyName == PropertyName.UpdateRate)
        {
            property["usage"] = (int)(EnableComms ? PropertyUsageFlags.Default : PropertyUsageFlags.NoEditor);
        }
    }

    public override void _Ready()
	{
		Main = GetParent().GetTree().EditedSceneRoot as Root;
		
		if (Main != null)
		{
			Main.SimulationStarted += OnSimulationStarted;
			Main.SimulationEnded += OnSimulationEnded;
		}
		
		data = data.Duplicate(true) as TripleStackLightData;
		data.InitSegments(segments);
		
		segmentsContainer = GetNode<Node3D>("Mid/Segments");
		topMesh = GetNode<MeshInstance3D>("Mid/Top");
		
		bottomMesh = GetNode<MeshInstance3D>("Bottom");
		midMesh = GetNode<MeshInstance3D>("Mid");
		
		segmentInitialYPos = segmentsContainer.GetNode<Node3D>("TripleStackSegment").Position.Y;
		
		if (segmentsContainer.GetChildCount() <= 1)
			SpawnSegments(segments - 1);
			
		topMesh.Position = new Vector3(0, topMeshInitialYPos + (step * (segments - 1)), 0);
		InitSegments();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Scale = new Vector3(Scale.X, Scale.Y, Scale.X);
		bottomMesh.Scale = new Vector3(1, 1 / Scale.Y, 1);
		midMesh.Scale = new Vector3(1 , (1 / Scale.Y) * Scale.X, 1);
	}

	void OnSimulationStarted()
	{
		if (Main == null) return;
		running = true;
	}

	void OnSimulationEnded()
	{
		running = false;
	}

	void InitSegments()
	{
		for (int i = 0; i < segments; i++)
		{
			TripleStackSegment segment = segmentsContainer.GetChild(i) as TripleStackSegment;
			segment.EnableComms = enableComms;
			segment.SegmentData = data.segmentDatas[i];
		}
	}
	
	void SpawnSegments(int count)
	{
		if (segments == 0 || segmentsContainer == null) return;
		
		for (int i = 0; i < count; i++)
		{
			Node3D segment = segmentScene.Instantiate() as Node3D;
			segmentsContainer.AddChild(segment, forceReadableName: true);
			//segment.Owner = this;
			segment.Position = new Vector3(0, segmentInitialYPos + (step * segment.GetIndex()), 0);
		}
	}
	
	void RemoveSegments(int count)
	{
		for (int i = 0; i < count; i++)
		{
			segmentsContainer.GetChild(segmentsContainer.GetChildCount() - 1 - i).QueueFree();
		}
	}
}
