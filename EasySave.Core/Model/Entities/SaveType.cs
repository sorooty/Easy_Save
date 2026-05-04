namespace EasySave.Core.Model.Entities
{
    /// <summary>Types de sauvegarde supportés.</summary>
    public enum SaveType
    {
        /// <summary>Copie l'intégralité du dossier source.</summary>
        Full,

        /// <summary>Copie uniquement les fichiers nouveaux ou modifiés depuis la dernière sauvegarde.</summary>
        Differential
    }
}
