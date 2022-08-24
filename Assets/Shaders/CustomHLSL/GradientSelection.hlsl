void GradientSelection_float(bool toon, Gradient BeginColor, Gradient EndColor, out Gradient Out)
{

    if(toon)
        Out = EndColor;
    else
        Out = BeginColor;

}

bool isfinite(Gradient g)
{
    return true;
}