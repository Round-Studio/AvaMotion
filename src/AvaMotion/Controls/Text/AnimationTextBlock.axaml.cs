using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;

namespace AvaMotion.Controls.Text;

public partial class AnimationTextBlock : UserControl
{
    private readonly List<AnimationCharacter> _characterControls = new();

    public static readonly StyledProperty<int> CharacterIntervalProperty =
        AvaloniaProperty.Register<AnimationTextBlock, int>(nameof(CharacterInterval), defaultValue: 50);

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
        newText ??= string.Empty;

        int newLength = newText.Length;
        int currentLength = _characterControls.Count;
        int interval = CharacterInterval;

        if (newLength < currentLength)
        {
            for (int i = currentLength - 1; i >= newLength; i--)
            {
                var charControlToRemove = _characterControls[i];
                int delay = i * interval;

                _characterControls.RemoveAt(i);

                _ = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    await charControlToRemove.AnimateOutAsync(delay);
                    Container.Children.Remove(charControlToRemove);
                });
            }
        }

        for (int i = 0; i < newLength; i++)
        {
            int delay = i * interval;

            if (i < _characterControls.Count)
            {
                _ = _characterControls[i].ChangeCharacterAsync(newText[i], delay);
            }
            else
            {
                var charControl = new AnimationCharacter(newText[i]);
                _characterControls.Add(charControl);
                Container.Children.Add(charControl);

                _ = charControl.AnimateInAsync(delay + 280);
            }
        }
    }
}