namespace GCMS.MathHouse.UI.Common;

public class AnimationManager
{
    private bool _glowActive = false;

    public async Task AnimateStartDoorGlow(Image control)
    {
        _glowActive = true;
        control.Opacity = 0;
        await control.FadeTo(1, 1000);

        while (_glowActive && control.IsVisible)
        {
            await control.ScaleTo(1.1, 700, Easing.SinInOut);
            await control.ScaleTo(1.0, 700, Easing.SinInOut);
        }

        control.Scale = 1.0; // reset
    }

    public void StopGlow()
    {
        _glowActive = false;
    }
}
