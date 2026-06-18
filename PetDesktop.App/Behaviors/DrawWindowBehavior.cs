using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;

namespace PetDesktop.App.Behaviors;

/// <summary>
/// Extensión de <see cref="Behavior"/> para crear un método propio que permita mover la ventana al presionar solo el botón izquierdo
/// </summary>
public class DrawWindowBehavior : Behavior<Control>
{
    /// <summary>
    /// Sobreescribe lo que sucede al crear el componente
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if(AssociatedObject != null) 
            AssociatedObject.PointerPressed += OnLeftPointerPressed;
    }

    /// <summary>
    /// Sobreescribe lo que sucede al destruir el componente
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if(AssociatedObject != null) 
            AssociatedObject.PointerPressed -= OnLeftPointerPressed;
    }

    /// <summary>
    /// Manejador del evento que se dispara cuando el usuario presiona el puntero (ratón, dedo o lápiz) 
    /// sobre el objeto asociado, iniciando el arrastre físico de la ventana si se cumple el filtro.
    /// </summary>
    /// <param name="sender">El objeto que originó el evento (en este caso, el <see cref="Control"/> asociado).</param>
    /// <param name="e">Los argumentos del evento que contienen la información física y espacial de la pulsación.</param>
    private void OnLeftPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        //se escala el árbol visual para encontrar el primer ancestro que sea del tipo Window, el grid al que está asociado no tiene la capacidad de moverse, es la ventana la que puede
        var window = AssociatedObject.FindAncestorOfType<Window>();

        if (window != null)
        {
            var pointerProperties = e.GetCurrentPoint(window).Properties;
            if (pointerProperties.IsLeftButtonPressed) window.BeginMoveDrag(e);
        }
    }
}