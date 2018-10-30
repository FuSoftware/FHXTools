using FHXTools.FHX;

bool Validate(FHXParameter p)
{
	 if (p.Module.Name.Contains("XV-P") && p.Module.Name.Contains("04"))
	{
		if(p.Path.Contains("CND") || p.Path.Contains("IO_IN") || p.Path.Contains("IO_OUT")) 
		{
			if(p.Name == "REF" || p.Name == "CV" || p.Name == "EXPRESSION")
			{
				return true;
			}
		}
   	 }

    	return false;
}