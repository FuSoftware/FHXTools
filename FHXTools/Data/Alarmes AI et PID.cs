bool Validate(FHXParameter p)
{
	//Checks if the parent contains an AI1 or PID1 object
	if(p.Parent.HasChild("AI1") || p.Parent.HasChild("PID1"))
	{
		//Only gets the ALM parameters
		if(p.Path.Contains("HI_HI_ALM") || p.Path.Contains("HI_ALM") || p.Path.Contains("LO_ALM") || p.Path.Contains("LO_LO_ALM"))
		{
			//Only gets the Current Value
			if(p.Name == "CV")
			{
				return true;
			}
		}
	}
	
	return false;
}
