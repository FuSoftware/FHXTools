/*
FHXParameter
- Path (string) : The full parameter path
- Parent (FHXObject) : The parent object

FHXObject
- Children : The children objects
*/

 /*
Examples :

bool Validate(FHXParameter p)
{
	if(p.Parent.HasChild("AI1") || p.Parent.HasChild("PID1"))
	{
		if(p.Path.Contains("HI_HI_ALM") || p.Path.Contains("HI_ALM") || p.Path.Contains("LO_ALM") || p.Path.Contains("LO_LO_ALM"))
		{
			return true;
		}
	}
	
	return false;
}
*/



bool Validate(FHXParameter p)
{
	//Your code here
}