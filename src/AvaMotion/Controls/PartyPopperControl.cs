using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace AvaMotion.Controls;

public sealed class ConfettiRibbon
{
    public double X;
    public double Y;

    public double VX;
    public double VY;

    public double Width;
    public double Height;

    public double Alpha;
    public double Decay;

    public double Rotation;
    public double RotationSpeed;

    public double CurlPhase;
    public double CurlSpeed;

    public readonly SolidColorBrush Brush = new(Colors.White);

    public void Reset(
        double x,
        double y,
        double vx,
        double vy,
        double width,
        double height,
        Color color,
        double rotation,
        double rotationSpeed,
        double curlPhase,
        double curlSpeed,
        double decay)
    {
        X = x;
        Y = y;

        VX = vx;
        VY = vy;

        Width = width;
        Height = height;

        Alpha = 1;
        Decay = decay;

        Rotation = rotation;
        RotationSpeed = rotationSpeed;

        CurlPhase = curlPhase;
        CurlSpeed = curlSpeed;

        Brush.Color = color;
        Brush.Opacity = 1;
    }
}

public class PartyPopperControl : Avalonia.Controls.Control
{
    private readonly List<ConfettiRibbon> _ribbons = new();
    private readonly Queue<ConfettiRibbon> _pool = new();
    private readonly Queue<Point> _spawnQueue = new();

    private readonly Random _random = new();

    private readonly DispatcherTimer _timer;
    private readonly DispatcherTimer _popperTimer;

    private const int MaxRibbonCount = 300;
    private const int SpawnPerFrame = 6;

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Panel.BackgroundProperty.AddOwner<PartyPopperControl>();

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public PartyPopperControl()
    {
        Background = Brushes.Transparent;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        _timer.Tick += TimerTick;


        _popperTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _popperTimer.Tick += (_, _) =>
        {
            SpawnPopper(
                new Point(
                    Bounds.Width / 2,
                    Bounds.Height / 2));
        };
    }

    protected override void OnAttachedToVisualTree(
        VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        Dispatcher.UIThread.Post(() =>
        {
            SpawnPopper(
                new Point(
                    Bounds.Width / 2,
                    Bounds.Height / 2));

            _popperTimer.Start();
        });
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        SpawnPending();

        if (_ribbons.Count == 0 && _spawnQueue.Count == 0)
        {
            _timer.Stop();
            return;
        }

        UpdateRibbons();

        if (_ribbons.Count > 0)
        {
            InvalidateVisual();
        }
    }

    public void SpawnPopper(Point origin)
    {
        int count = _random.Next(30, 50);

        for (int i = 0; i < count; i++)
        {
            _spawnQueue.Enqueue(origin);
        }

        if (!_timer.IsEnabled)
        {
            _timer.Start();
        }
    }

    private void SpawnPending()
    {
        int count = Math.Min(
            SpawnPerFrame,
            _spawnQueue.Count);

        for (int i = 0; i < count; i++)
        {
            Point origin = _spawnQueue.Dequeue();

            var ribbon = GetRibbon();

            double angle =
                -Math.PI / 2 +
                (_random.NextDouble() - 0.5)
                * Math.PI
                * 0.7;

            double speed =
                _random.NextDouble()
                * 10
                + 8;

            var color = Color.FromRgb(
                (byte)_random.Next(100, 256),
                (byte)_random.Next(100, 256),
                (byte)_random.Next(100, 256));

            ribbon.Reset(
                origin.X,
                origin.Y,
                Math.Cos(angle) * speed,
                Math.Sin(angle) * speed,
                _random.NextDouble() * 3 + 4,
                _random.NextDouble() * 8 + 10,
                color,
                _random.NextDouble() * Math.PI * 2,
                (_random.NextDouble() - 0.5) * 0.25,
                _random.NextDouble() * Math.PI * 2,
                _random.NextDouble() * 0.15 + 0.05,
                _random.NextDouble() * 0.008 + 0.005);

            _ribbons.Add(ribbon);
        }

        while (_ribbons.Count > MaxRibbonCount)
        {
            RemoveRibbonAt(0);
        }
    }

    private ConfettiRibbon GetRibbon()
    {
        if (_pool.Count > 0)
        {
            return _pool.Dequeue();
        }

        return new ConfettiRibbon();
    }

    private void ReturnRibbon(ConfettiRibbon ribbon)
    {
        ribbon.Alpha = 0;
        ribbon.Brush.Opacity = 0;

        _pool.Enqueue(ribbon);
    }

    private void RemoveRibbonAt(int index)
    {
        int last = _ribbons.Count - 1;

        var ribbon = _ribbons[index];

        if (index != last)
        {
            _ribbons[index] = _ribbons[last];
        }

        _ribbons.RemoveAt(last);

        ReturnRibbon(ribbon);
    }

    private void UpdateRibbons()
    {
        const double gravity = 0.25;
        const double drag = 0.96;

        for (int i = _ribbons.Count - 1; i >= 0; i--)
        {
            var ribbon = _ribbons[i];

            ribbon.VX *= drag;

            ribbon.VY =
                ribbon.VY * drag
                + gravity;

            ribbon.X += ribbon.VX;
            ribbon.Y += ribbon.VY;

            ribbon.Rotation += ribbon.RotationSpeed;
            ribbon.CurlPhase += ribbon.CurlSpeed;

            ribbon.Alpha -= ribbon.Decay;

            if (ribbon.Alpha <= 0)
            {
                RemoveRibbonAt(i);
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        foreach (var ribbon in _ribbons)
        {
            double height =
                Math.Abs(
                    ribbon.Height *
                    Math.Sin(ribbon.CurlPhase));

            if (height < 1)
            {
                continue;
            }

            ribbon.Brush.Opacity = ribbon.Alpha;

            var matrix =
                Matrix.CreateTranslation(
                    -ribbon.Width / 2,
                    -height / 2)
                *
                Matrix.CreateRotation(
                    ribbon.Rotation)
                *
                Matrix.CreateTranslation(
                    ribbon.X,
                    ribbon.Y);

            using (context.PushTransform(matrix))
            {
                context.FillRectangle(
                    ribbon.Brush,
                    new Rect(
                        0,
                        0,
                        ribbon.Width,
                        height));
            }
        }
    }

    protected override void OnDetachedFromVisualTree(
        VisualTreeAttachmentEventArgs e)
    {
        _timer.Stop();
        _popperTimer.Stop();

        _spawnQueue.Clear();
        _ribbons.Clear();
        _pool.Clear();

        base.OnDetachedFromVisualTree(e);
    }
}