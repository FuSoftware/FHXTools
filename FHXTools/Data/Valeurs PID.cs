using FHXTools.FHX;

bool Validate(FHXParameter p)
{
    //Recherche les consignes de PID
    if (p.Path.Contains("PID1/GAIN") || p.Path.Contains("PID1/RESET") || p.Path.Contains("PID1/RATE"))
    {
        //Récupère le CV
        if (p.Name == "CV")
        {
            return true;
        }
    }

    return false;
}