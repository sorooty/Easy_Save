using System.Diagnostics;

namespace EasySave.Core.Model.Service
{
    /// <summary>
    /// Détecte si le logiciel métier configuré est en cours d'exécution.
    /// Utilisé pour bloquer le lancement des sauvegardes si nécessaire (v2.0).
    /// </summary>
    public class BusinessSoftwareService
    {
        /// <summary>
        /// Retourne true si un processus portant ce nom est actif sur la machine.
        /// </summary>
        /// <param name="processName">Nom du processus sans extension (.exe).</param>
        public bool IsBusinessSoftwareRunning(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
                return false;

            return Process.GetProcessesByName(processName).Length > 0;
        }
    }
}
