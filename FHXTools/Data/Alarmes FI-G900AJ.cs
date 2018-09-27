using FHXTools.FHX;

bool Validate(FHXParameter p)
{
    //Recherche les alarmes de FI-G900AJ
    if (p.Module.Name == "FI-G900AJ")
    {
        if(p.Parent.Name.Contains("ALM")) {return true;}
    }

    return false;
}