using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using AvaMotion.Enum;
using AvaMotion.Enum.Text;

namespace AvaMotion.Controls.Text;

public partial class AnimationCharacter : UserControl
{
    private char _character;

    public char Character
    {
        get => _character;
        set => _ = ChangeCharacterAsync(value, 0);
    }

    public TextAnimationType AnimationType { get; set; } = TextAnimationType.SpringBounce;

    private bool _isNormal = false;

    public AnimationCharacter()
    {
        InitializeComponent();
    }

    public AnimationCharacter(char character) : this()
    {
        _character = character;
    }

    public async Task ChangeCharacterAsync(char character, int delayMs = 0)
    {
        _character = character;

        if (_isNormal)
        {
            await AnimateOutAsync(delayMs);
            await AnimateInAsync(0);
        }
        else
        {
            await AnimateInAsync(delayMs);
        }
    }

    public async Task AnimateInAsync(int delayMs = 0)
    {
        if (delayMs > 0)
        {
            await Task.Delay(delayMs);
        }

        if (CharacterBlock != null)
        {
            RunAnimation(AnimationStatus.In);
            await Task.Delay(280);

            _isNormal = true;
            RunAnimation(AnimationStatus.Normal);
        }
    }

    public async Task AnimateOutAsync(int delayMs = 0)
    {
        if (delayMs > 0)
        {
            await Task.Delay(delayMs);
        }

        if (CharacterBlock != null && _isNormal)
        {
            var fontSize = FontSize;

            RunAnimation(AnimationStatus.Out);
            _isNormal = false;
            await Task.Delay(280);
        }
    }

    private void RunAnimation(AnimationStatus status)
    {
        var fontSize = FontSize;
        switch (AnimationType)
        {
            case TextAnimationType.NumberRolling:
                switch (status)
                {
                    case AnimationStatus.In:
                        CharacterBlock.Margin = new Thickness(0, -fontSize / 3, 0, fontSize / 3);
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(0.5)");
                        CharacterBlock.Opacity = 0;
                        CharacterBlock.Effect = new BlurEffect() { Radius = fontSize * 2 };
                        break;
                    case AnimationStatus.Normal:
                        CharacterBlock.Text = _character.ToString();
                        CharacterBlock.Margin = new Thickness(0);
                        CharacterBlock.Opacity = 1;
                        CharacterBlock.Effect = new BlurEffect() { Radius = 0 };
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(1)");
                        break;
                    case AnimationStatus.Out:
                        CharacterBlock.Margin = new Thickness(0, fontSize / 3, 0, -fontSize / 3);
                        CharacterBlock.Opacity = 0;
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(0.5)");
                        break;
                }
                break;
            case TextAnimationType.SpringBounce:
                switch (status)
                {
                    case AnimationStatus.In:
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(0)");
                        CharacterBlock.Opacity = 0;
                        break;
                    case AnimationStatus.Normal:
                        CharacterBlock.Text = _character.ToString();
                        CharacterBlock.Opacity = 1;
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(1)");
                        break;
                    case AnimationStatus.Out:
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(0) rotate(30deg)");
                        CharacterBlock.Opacity = 0;
                        break;
                }
                break;
            case TextAnimationType.Typewriter:
                switch (status)
                {
                    case AnimationStatus.In:
                        CharacterBlock.Opacity = 0;
                        CharacterBlock.RenderTransform = TransformOperations.Parse("translate(0, 5px)");
                        break;
                    case AnimationStatus.Normal:
                        CharacterBlock.Text = _character.ToString();
                        CharacterBlock.Opacity = 1;
                        CharacterBlock.RenderTransform = TransformOperations.Parse("translate(0, 0)");
                        break;
                    case AnimationStatus.Out:
                        CharacterBlock.Opacity = 0;
                        CharacterBlock.RenderTransform = TransformOperations.Parse("translate(0, -5px)");
                        break;
                }
                break;
            case TextAnimationType.NeonGlow:
                switch (status)
                {
                    case AnimationStatus.In:
                        CharacterBlock.Opacity = 0.3;
                        CharacterBlock.Effect = new BlurEffect() { Radius = fontSize * 3 };
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(1.2)");
                        break;
                    case AnimationStatus.Normal:
                        CharacterBlock.Text = _character.ToString();
                        CharacterBlock.Opacity = 1;
                        CharacterBlock.Effect = new BlurEffect() { Radius = 0 };
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(1)");
                        break;
                    case AnimationStatus.Out:
                        CharacterBlock.Opacity = 0.3;
                        CharacterBlock.Effect = new BlurEffect() { Radius = fontSize * 3 };
                        CharacterBlock.RenderTransform = TransformOperations.Parse("scale(1.2)");
                        break;
                }
                break;
        }
    }
}