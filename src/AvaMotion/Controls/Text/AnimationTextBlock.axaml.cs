using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using AvaMotion.Enum.Text;

namespace AvaMotion.Controls.Text;

public partial class AnimationTextBlock : UserControl
{
    private readonly List<AnimationCharacter> _characterControls = new();
    private readonly List<AnimationCharacter> _animatingOutControls = new();
    
    private long _currentOperationId = 0;

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
        long operationId = ++_currentOperationId;

        if (_animatingOutControls.Count > 0)
        {
            foreach (var ctrl in _animatingOutControls)
            {
                Container.Children.Remove(ctrl);
            }
            _animatingOutControls.Clear();
        }

        newText ??= string.Empty;

        int newLength = newText.Length;
        int maxIndex = Math.Max(newLength, _characterControls.Count);
        int interval = CharacterInterval;

        var newlyCreatedControls = new List<AnimationCharacter>();
        var removeControls = new List<AnimationCharacter>();

        for (int i = 0; i < maxIndex; i++)
        {
            int delay = i * interval;

            if (i < newLength)
            {
                char targetChar = newText[i];

                if (i < _characterControls.Count)
                {
                    var ctrl = _characterControls[i];
                    ctrl.AnimationType = AnimationType;
                    _ = SafeChangeCharacterAsync(ctrl, targetChar, delay, operationId);
                }
                else
                {
                    var charControl = new AnimationCharacter(targetChar)
                    {
                        AnimationType = AnimationType
                    };
                    _characterControls.Add(charControl);
                    newlyCreatedControls.Add(charControl);

                    _ = SafeAnimateInAsync(charControl, delay + 280, operationId);
                }
            }
            else
            {
                var ctrlToRemove = _characterControls[i];
                removeControls.Add(ctrlToRemove);

                _ = AnimateOutAndRemoveAsync(ctrlToRemove, delay, operationId);
            }
        }

        if (removeControls.Count > 0)
        {
            _characterControls.RemoveRange(newLength, removeControls.Count);
            _animatingOutControls.AddRange(removeControls);
        }

        if (newlyCreatedControls.Count > 0)
        {
            Container.Children.AddRange(newlyCreatedControls);
        }
    }

    private async Task SafeChangeCharacterAsync(AnimationCharacter ctrl, char character, int delay, long operationId)
    {
        await ctrl.ChangeCharacterAsync(character, delay);
        if (operationId != _currentOperationId) return;
    }

    private async Task SafeAnimateInAsync(AnimationCharacter ctrl, int delay, long operationId)
    {
        await ctrl.AnimateInAsync(delay);
        if (operationId != _currentOperationId) return;
    }

    private async Task AnimateOutAndRemoveAsync(AnimationCharacter ctrl, int delay, long operationId)
    {
        try
        {
            await ctrl.AnimateOutAsync(delay);
        }
        catch
        {
        }
        finally
        {
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_animatingOutControls.Contains(ctrl))
                {
                    Container.Children.Remove(ctrl);
                    _animatingOutControls.Remove(ctrl);
                }
            });
        }
    }
}