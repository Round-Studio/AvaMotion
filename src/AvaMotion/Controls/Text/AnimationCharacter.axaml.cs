using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AvaMotion.Controls.Text;

public partial class AnimationCharacter : UserControl
{
    private char _character;

    public char Character
    {
        get => _character;
        set => _ = ChangeCharacterAsync(value, 0);
    }

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
            var fontSize = FontSize;

            CharacterBlock.Margin = new Thickness(0, -fontSize / 3, 0, fontSize / 3);
            CharacterBlock.Opacity = 0;
            CharacterBlock.Effect = new BlurEffect() { Radius = fontSize * 4 };

            await Task.Delay(280);

            _isNormal = true;
            CharacterBlock.Text = _character.ToString();
            CharacterBlock.Margin = new Thickness(0);
            CharacterBlock.Opacity = 1;
            CharacterBlock.Effect = new BlurEffect() { Radius = 0 };
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

            CharacterBlock.Margin = new Thickness(0, fontSize / 3, 0, -fontSize / 3);
            CharacterBlock.Opacity = 0;

            _isNormal = false;
            await Task.Delay(280);
        }
    }
}