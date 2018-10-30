using FHXTools.FHX;

bool Validate(FHXParameter p)
{
	 if (p.Module.Name.Contains("C-V") && p.Module.Name.Contains("16"))
	{
		if(p.Path.Contains("CONTROL_OPTS") && p.Name == "OPTION10") return true; //PID Direct

		if(p.Path.Contains("IO_OPTS") && p.Name == "OPTION10") return true; //Sortie FO

		if((p.Path.Contains("IO_IN") || p.Path.Contains("IO_OUT")) && p.Name == "REF") return true; // I/O ref

		if((p.Path.Contains("CND") || p.Path.Contains("VANNE_OUVERTE")) && (p.Name == "CV" || p.Name == "EXPRESSION")) return true; //Interlocks
   	 }

    	return false;
}