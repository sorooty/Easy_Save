using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasySave.ViewModel;

/// <summary>
/// Classe de base pour tous les ViewModels.
/// Implémente INotifyPropertyChanged pour notifier l'interface des changements.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifie l'interface qu'une propriété a changé.
    /// </summary>
    /// <param name="propertyName">Nom de la propriété (automatique avec CallerMemberName)</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Met à jour un champ et notifie l'interface si la valeur a changé.
    /// </summary>
    /// <typeparam name="T">Type de la propriété</typeparam>
    /// <param name="field">Champ à mettre à jour</param>
    /// <param name="value">Nouvelle valeur</param>
    /// <param name="propertyName">Nom de la propriété</param>
    /// <returns>Vrai si la valeur a changé</returns>
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}