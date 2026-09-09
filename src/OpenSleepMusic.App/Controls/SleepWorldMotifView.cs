using System.Diagnostics;
using Microsoft.Maui.Graphics;

namespace OpenSleepMusic.App.Controls;

/// <summary>Small, asset-free theme artwork isolated for the visual system planned in #7.</summary>
public sealed class SleepWorldMotifView : GraphicsView
{
    public static readonly BindableProperty WorldIdProperty = BindableProperty.Create(
        nameof(WorldId), typeof(string), typeof(SleepWorldMotifView), string.Empty,
        propertyChanged: static (bindable, _, _) => ((SleepWorldMotifView)bindable).Invalidate());

    private readonly Stopwatch _clock = new();
    private bool _timerRunning;

    public SleepWorldMotifView()
    {
        Drawable = new SleepWorldMotifDrawable(() => WorldId, () => _clock.Elapsed.TotalSeconds);
        Loaded += (_, _) => StartAnimation();
        Unloaded += (_, _) => StopAnimation();
    }

    public string WorldId
    {
        get => (string)GetValue(WorldIdProperty);
        set => SetValue(WorldIdProperty, value);
    }

    private void StartAnimation()
    {
        if (_timerRunning || MotionPreference.ReducedMotion)
        {
            Invalidate();
            return;
        }

        _timerRunning = true;
        _clock.Start();
        Dispatcher.StartTimer(TimeSpan.FromMilliseconds(100), () =>
        {
            if (!_timerRunning || Handler is null) return false;
            Invalidate();
            return true;
        });
    }

    private void StopAnimation()
    {
        _timerRunning = false;
        _clock.Stop();
    }
}

internal static class MotionPreference
{
    public static bool ReducedMotion
    {
        get
        {
#if ANDROID
            return !Android.Animation.ValueAnimator.AreAnimatorsEnabled();
#elif IOS || MACCATALYST
            return UIKit.UIAccessibility.IsReduceMotionEnabled;
#elif WINDOWS
            return !new Windows.UI.ViewManagement.UISettings().AnimationsEnabled;
#else
            return false;
#endif
        }
    }
}

internal sealed class SleepWorldMotifDrawable(Func<string> worldId, Func<double> elapsedSeconds) : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        var seconds = elapsedSeconds();
        var skyPhase = (Math.Sin(seconds / 15d * Math.PI) + 1d) / 2d;
        canvas.FillColor = Mix("#111E46", "#172F68", skyPhase);
        canvas.FillRectangle(dirtyRect);
        DrawStars(canvas, width, height, skyPhase);

        switch (worldId())
        {
            case "quiet-classics": DrawClassics(canvas, width, height); break;
            case "rain": DrawRain(canvas, width, height, seconds); break;
            case "forest": DrawForest(canvas, width, height); break;
            case "waves": DrawWaves(canvas, width, height, seconds); break;
            case "fireplace": DrawFireplace(canvas, width, height, seconds); break;
            default: DrawMoon(canvas, width * .74f, height * .34f, height * .16f); break;
        }
    }

    private static void DrawStars(ICanvas canvas, float width, float height, double phase)
    {
        canvas.FillColor = Mix("#879CD4", "#C7D2F1", phase * .65d);
        foreach (var star in new (float X, float Y, float R)[]
        {
            (.08f, .18f, 1.3f), (.19f, .35f, 1f), (.34f, .16f, 1.2f),
            (.55f, .28f, 1f), (.72f, .12f, 1.2f), (.89f, .31f, 1f)
        }) canvas.FillCircle(width * star.X, height * star.Y, star.R);
    }

    private static void DrawClassics(ICanvas canvas, float width, float height)
    {
        DrawMoon(canvas, width * .78f, height * .28f, height * .15f);
        canvas.FillColor = Color.FromArgb("#503F74");
        canvas.FillRoundedRectangle(width * .17f, height * .43f, width * .58f, height * .37f, 8);
        canvas.FillColor = Color.FromArgb("#E7D8B2");
        canvas.FillRectangle(width * .22f, height * .51f, width * .48f, height * .17f);
        canvas.StrokeColor = Color.FromArgb("#756B67");
        canvas.StrokeSize = 2;
        for (var line = 1; line <= 3; line++)
            canvas.DrawLine(width * .26f, height * (.52f + line * .035f), width * .66f, height * (.52f + line * .035f));
        canvas.FillColor = Color.FromArgb("#252038");
        canvas.FillCircle(width * .5f, height * .74f, 5);
    }

    private static void DrawRain(ICanvas canvas, float width, float height, double seconds)
    {
        DrawMoon(canvas, width * .78f, height * .25f, height * .13f);
        canvas.FillColor = Color.FromArgb("#6F7FA6");
        canvas.FillCircle(width * .37f, height * .42f, height * .16f);
        canvas.FillCircle(width * .51f, height * .36f, height * .2f);
        canvas.FillCircle(width * .63f, height * .43f, height * .15f);
        canvas.FillRoundedRectangle(width * .32f, height * .4f, width * .37f, height * .16f, 10);
        canvas.StrokeColor = Color.FromArgb("#789AD2");
        canvas.StrokeSize = 2;
        var drift = (float)((seconds % 4d) / 4d * height * .08f);
        for (var index = 0; index < 5; index++)
        {
            var x = width * (.34f + index * .075f);
            var y = height * (.62f + (index % 2) * .08f) + drift;
            canvas.DrawLine(x, y, x - 4, y + height * .11f);
        }
    }

    private static void DrawForest(ICanvas canvas, float width, float height)
    {
        DrawMoon(canvas, width * .75f, height * .24f, height * .14f);
        DrawPine(canvas, width * .23f, height * .84f, height * .47f, "#1D493F");
        DrawPine(canvas, width * .47f, height * .87f, height * .62f, "#285A4D");
        DrawPine(canvas, width * .72f, height * .84f, height * .43f, "#1C443C");
    }

    private static void DrawPine(ICanvas canvas, float x, float bottom, float size, string color)
    {
        canvas.FillColor = Color.FromArgb(color);
        var tree = new PathF();
        tree.MoveTo(x, bottom - size);
        tree.LineTo(x - size * .34f, bottom);
        tree.LineTo(x + size * .34f, bottom);
        tree.Close();
        canvas.FillPath(tree);
    }

    private static void DrawWaves(ICanvas canvas, float width, float height, double seconds)
    {
        DrawMoon(canvas, width * .76f, height * .25f, height * .14f);
        var shift = (float)Math.Sin(seconds * Math.PI / 6d) * 5;
        DrawWave(canvas, width, height * .64f, shift, "#315A82");
        DrawWave(canvas, width, height * .76f, -shift, "#39749B");
    }

    private static void DrawWave(ICanvas canvas, float width, float y, float shift, string color)
    {
        canvas.StrokeColor = Color.FromArgb(color);
        canvas.StrokeSize = 8;
        var wave = new PathF();
        wave.MoveTo(-10, y);
        wave.CurveTo(width * .2f + shift, y - 12, width * .3f + shift, y + 12, width * .5f, y);
        wave.CurveTo(width * .7f - shift, y - 12, width * .8f - shift, y + 12, width + 10, y);
        canvas.DrawPath(wave);
    }

    private static void DrawFireplace(ICanvas canvas, float width, float height, double seconds)
    {
        canvas.FillColor = Color.FromArgb("#33251F");
        canvas.FillRoundedRectangle(width * .19f, height * .24f, width * .62f, height * .67f, 10);
        canvas.FillColor = Color.FromArgb("#15151C");
        canvas.FillRoundedRectangle(width * .27f, height * .33f, width * .46f, height * .48f, 7);
        canvas.StrokeColor = Color.FromArgb("#724936");
        canvas.StrokeSize = 8;
        canvas.DrawLine(width * .34f, height * .73f, width * .66f, height * .69f);

        // The flame's shape and color complete one soft crossfade in about three seconds.
        var blend = (Math.Sin(seconds * Math.PI / 3d) + 1d) / 2d;
        canvas.FillColor = Mix("#D76C34", "#E49347", blend);
        var flame = new PathF();
        flame.MoveTo(width * .5f, height * .72f);
        flame.CurveTo(width * (.35f + .04f * (float)blend), height * .63f,
            width * .46f, height * (.49f - .04f * (float)blend), width * .51f, height * .4f);
        flame.CurveTo(width * .55f, height * .52f, width * (.67f - .04f * (float)blend), height * .62f,
            width * .5f, height * .72f);
        flame.Close();
        canvas.FillPath(flame);
        canvas.FillColor = Mix("#EAB85A", "#F1C874", blend);
        canvas.FillEllipse(width * .45f, height * .57f, width * .11f, height * .13f);
    }

    private static void DrawMoon(ICanvas canvas, float x, float y, float radius)
    {
        canvas.FillColor = Color.FromArgb("#F3E8B6");
        canvas.FillCircle(x, y, radius);
        canvas.FillColor = Color.FromArgb("#172957");
        canvas.FillCircle(x + radius * .38f, y - radius * .18f, radius * .82f);
    }

    private static Color Mix(string fromHex, string toHex, double amount)
    {
        var from = Color.FromArgb(fromHex);
        var to = Color.FromArgb(toHex);
        var t = (float)Math.Clamp(amount, 0d, 1d);
        return new Color(from.Red + (to.Red - from.Red) * t, from.Green + (to.Green - from.Green) * t,
            from.Blue + (to.Blue - from.Blue) * t, 1f);
    }
}
