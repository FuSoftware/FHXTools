using FHXTools.FHX;

bool Validate(FHXParameter p)
{
	//Recherche les limites de FI-S758C
	 if (p.Module.Name == "FI-S758C")
	{
		if(p.Parent.Name.Contains("LIM")) 
		{
			if(p.Name == "CV")
			{
				return true;
			}
		}
   	 }

    	return false;
}