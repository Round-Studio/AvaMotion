using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using AvaMotion.Enum.Text;

namespace AvaMotion.Controls.Text;

public partial class AnimationTextBlock : UserControl
{
    private readonly List<AnimationCharacter> _characterControls = new();
    private CancellationTokenSource? _cts;

    public static readonly StyledProperty<int> CharacterIntervalProperty =
        AvaloniaProperty.Register<AnimationTextBlock, int>(nameof(CharacterInterval), defaultValue: 50);
    
    public TextAnimationType AnimationType { get; set; } = TextAnimationType.SpringBounce;

    public int CharacterInterval
    {
        get => GetValue(CharacterIntervalProperty);
        set => SetValue(CharacterIntervalProperty, value);
    }

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AnimationTextBlock, string?>(nameof(Text));

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    static AnimationTextBlock()
    {
        TextProperty.Changed.AddClassHandler<AnimationTextBlock>((x, e) => x.OnTextChanged(e.NewValue as string));
    }

    public AnimationTextBlock()
    {
        InitializeComponent();
    }

    private void OnTextChanged(string? newText)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        newText ??= string.Empty;

        int newLength = newText.Length;
        int currentLength = _characterControls.Count;
        int interval = CharacterInterval;

        for (int i = 0; i < newLength; i++)
        {
            int delay = i * interval;

            if (i < currentLength)
            {
                _characterControls[i].AnimationType = AnimationType;
                _ = _characterControls[i].ChangeCharacterAsync(newText[i], delay);
            }
            else
            {
                var charControl = new AnimationCharacter(newText[i]);
                charControl.AnimationType = AnimationType;
                _characterControls.Add(charControl);
                Container.Children.Add(charControl);

                _ = charControl.AnimateInAsync(delay + 280);
            }
        }

        if (newLength < currentLength)
        {
            var removeControls = new List<AnimationCharacter>();
            int maxOutDelay = 0;

            for (int i = newLength; i < currentLength; i++)
            {
                var charControlToRemove = _characterControls[i];
                removeControls.Add(charControlToRemove);

                int delay = i * interval;
                maxOutDelay = Math.Max(maxOutDelay, delay);

                _ = charControlToRemove.AnimateOutAsync(delay);
            }

            _characterControls.RemoveRange(newLength, currentLength - newLength);

            int totalWaitTime = maxOutDelay + 280;
            _ = Task.Run(async () =>
            {
                await Task.Delay(totalWaitTime, token);
                if (token.IsCancellationRequested) return;

                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (token.IsCancellationRequested) return;

                    foreach (var ctrl in removeControls)
                    {
                        Container.Children.Remove(ctrl);
                    }
                });
            }, token);
        }
    }
}