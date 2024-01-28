using Godot;
using System;

[Tool]
[GlobalClass]
public partial class TripleStackLightData : Resource
{
	public int segments = 0;
	TripleStackSegmentData segmentData = (TripleStackSegmentData)ResourceLoader.Load("res://src/TripleStackLight/TripleStackSegmentData.tres");
	[Export] public TripleStackSegmentData[] segmentDatas = Array.Empty<TripleStackSegmentData>();
	
	public void InitSegments(int count)
	{
		segments = count;
		
		if (segmentDatas.Length == 0)
		{
			segmentDatas = new TripleStackSegmentData[segments];
			for (int i = 0; i < segments; i++)
			{
				segmentDatas[i] = segmentData.Duplicate() as TripleStackSegmentData;
			}
		}
		else
		{
			TripleStackSegmentData[] cache = new TripleStackSegmentData[count];
			for (int i = 0; i < count; i++)
			{
				cache[i] = segmentDatas[i].Duplicate() as TripleStackSegmentData;
			}
			segmentDatas = cache;
		}
	}
	
	public void SetSegments(int count)
	{
		if (count == segments) return;
		
		TripleStackSegmentData[] cache = new TripleStackSegmentData[count];
		
		if (count < segments)
		{
			for (int i = 0; i < count; i++)
			{
				cache[i] = segmentDatas[i];
			}
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				if (i < segments)
					cache[i] = segmentDatas[i];
				else
					cache[i] = segmentData.Duplicate() as TripleStackSegmentData;
			}
		}
		
		segments = count;
		segmentDatas = cache;
	}
}
