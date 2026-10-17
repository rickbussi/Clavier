using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Clavier.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CombinationToggleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton activeButton)
        {
            foreach (var child in CharacterButtonsPanel.Children)
            {
                if (child is ToggleButton button && !ReferenceEquals(button, activeButton))
                {
                    button.IsChecked = false;
                }
            }
        }

        UpdateMarkers();
    }

    private void KeyboardOverlay_OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateMarkers();
    }

    private void UpdateMarkers()
    {
        var selectedCharacter = GetSelectedCharacter();

        GreenKeyMarker.IsVisible = false;
        ShiftKeyMarker.IsVisible = false;
        BlueShiftKeyMarker.IsVisible = false;
        BlueAltGrMarker.IsVisible = false;
        BlueKeyMarker.IsVisible = selectedCharacter.HasValue;

        if (!selectedCharacter.HasValue)
        {
            return;
        }

        const double imageWidth = 1500;
        const double imageHeight = 693;
        const double markerSide = 96;

        var scaleX = KeyboardOverlay.Bounds.Width / imageWidth;
        var scaleY = KeyboardOverlay.Bounds.Height / imageHeight;
        var markerWidth = markerSide * scaleX;
        var markerHeight = markerSide * scaleY;

        var character = selectedCharacter.Value;
        var isUppercase = char.IsUpper(character);

        if (character is 'ç' or 'Ç')
        {
            BlueKeyMarker.IsVisible = true;
            BlueAltGrMarker.IsVisible = true;
            BlueShiftKeyMarker.IsVisible = isUppercase;
            PositionMarker(BlueKeyMarker, 982, 428, markerWidth, markerHeight, scaleX, scaleY);
            PositionMarker(BlueAltGrMarker, 980, 530, markerWidth, markerHeight, scaleX, scaleY);
            if (isUppercase)
            {
                PositionBlueShiftMarker(markerWidth, markerHeight, scaleX, scaleY, false);
            }

            return;
        }

        if (character is 'œ' or 'Œ')
        {
            BlueKeyMarker.IsVisible = true;
            BlueAltGrMarker.IsVisible = true;
            BlueShiftKeyMarker.IsVisible = isUppercase;
            // Le centre de la touche K sur l'image est légèrement à droite du
            // repère initial. Réduire le carré pour mieux suivre les bords de la touche.
            PositionMarker(BlueKeyMarker, 930, 330, markerWidth * 0.88, markerHeight * 0.88, scaleX, scaleY);
            PositionMarker(BlueAltGrMarker, 980, 530, markerWidth, markerHeight, scaleX, scaleY);
            if (isUppercase)
            {
                PositionBlueShiftMarker(markerWidth, markerHeight, scaleX, scaleY, false);
            }

            return;
        }

        var lowerCharacter = char.ToLowerInvariant(character);
        var targetLetter = GetBaseLetter(lowerCharacter);
        var targetCenter = GetLetterCenter(targetLetter);
        var accent = GetAccentKind(lowerCharacter);
        var accentUsesShift = accent is AccentKind.Circumflex or AccentKind.Diaeresis;
        var bothStepsUseShift = accentUsesShift && isUppercase;

        GreenKeyMarker.IsVisible = true;
        ShiftKeyMarker.IsVisible = isUppercase;
        BlueShiftKeyMarker.IsVisible = accentUsesShift;

        PositionMarker(GreenKeyMarker, targetCenter.X, targetCenter.Y, markerWidth, markerHeight, scaleX, scaleY);
        PositionMarker(BlueKeyMarker, GetAccentKeyCenter(accent).X, GetAccentKeyCenter(accent).Y, markerWidth, markerHeight, scaleX, scaleY);

        if (accentUsesShift)
        {
            PositionBlueShiftMarker(markerWidth, markerHeight, scaleX, scaleY, bothStepsUseShift);
        }

        if (isUppercase)
        {
            PositionGreenShiftMarker(markerWidth, markerHeight, scaleX, scaleY, bothStepsUseShift);
        }
    }

    private char? GetSelectedCharacter()
    {
        foreach (var child in CharacterButtonsPanel.Children)
        {
            if (child is ToggleButton { IsChecked: true, Content: string value } && value.Length == 1)
            {
                return value[0];
            }
        }

        return null;
    }

    private static char GetBaseLetter(char character) => character switch
    {
        'à' or 'â' => 'a',
        'é' or 'è' or 'ê' or 'ë' => 'e',
        'î' or 'ï' => 'i',
        'ô' => 'o',
        'ù' or 'û' => 'u',
        _ => throw new ArgumentOutOfRangeException(nameof(character), character, "Caractère non pris en charge")
    };

    private static Point GetLetterCenter(char letter) => letter switch
    {
        'a' => new Point(239, 329),
        'e' => new Point(407, 229),
        'i' => new Point(907, 229),
        'o' => new Point(1006, 229),
        'u' => new Point(808, 229),
        _ => throw new ArgumentOutOfRangeException(nameof(letter), letter, "Touche non prise en charge")
    };

    private static AccentKind GetAccentKind(char character) => character switch
    {
        'à' or 'è' or 'ù' => AccentKind.Grave,
        'é' => AccentKind.Acute,
        'â' or 'ê' or 'î' or 'ô' or 'û' => AccentKind.Circumflex,
        'ë' or 'ï' => AccentKind.Diaeresis,
        _ => throw new ArgumentOutOfRangeException(nameof(character), character, "Accent non pris en charge")
    };

    private static Point GetAccentKeyCenter(AccentKind accent) => accent switch
    {
        AccentKind.Grave => new Point(65, 128),
        AccentKind.Acute or AccentKind.Diaeresis => new Point(1228, 330),
        AccentKind.Circumflex => new Point(655, 129),
        _ => throw new ArgumentOutOfRangeException(nameof(accent), accent, "Accent non pris en charge")
    };

    private void PositionBlueShiftMarker(double markerWidth, double markerHeight, double scaleX, double scaleY, bool splitShiftMarker)
    {
        if (splitShiftMarker)
        {
            PositionMarker(BlueShiftKeyMarker, 66, 430, markerWidth * 1.125, markerHeight, scaleX, scaleY);
        }
        else
        {
            PositionMarker(BlueShiftKeyMarker, 120, 430, markerWidth * 2.25, markerHeight, scaleX, scaleY);
        }
    }

    private void PositionGreenShiftMarker(double markerWidth, double markerHeight, double scaleX, double scaleY, bool splitShiftMarker)
    {
        var centerX = splitShiftMarker ? 174 : 120;
        PositionMarker(ShiftKeyMarker, centerX, 430, markerWidth * (splitShiftMarker ? 1.125 : 2.25), markerHeight, scaleX, scaleY);
    }

    private enum AccentKind
    {
        Grave,
        Acute,
        Circumflex,
        Diaeresis
    }

    private static void PositionMarker(
        Border marker,
        double centerX,
        double centerY,
        double markerWidth,
        double markerHeight,
        double scaleX,
        double scaleY)
    {
        marker.Width = markerWidth;
        marker.Height = markerHeight;
        Canvas.SetLeft(marker, centerX * scaleX - markerWidth / 2);
        Canvas.SetTop(marker, centerY * scaleY - markerHeight / 2);
    }
}
