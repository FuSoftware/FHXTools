using FHXTools.FHX;

bool Validate(FHXParameter p)
{
    //Recherche les PV des AI et des PID
    if (p.Parent.Name == "AI1/PV" || p.Parent.Name == "PID1/PV" )
    {
        //Récupère le CV
        if (p.Name == "CV")
        {
            return true;
        }
    }

    return false;
}