using System.Globalization;
using System.Resources;

namespace EasySave.WPF.Resources
{
    /// <summary>
    /// Wrapper statique pour accéder aux ressources localisées depuis XAML via {x:Static res:Strings.X}.
    /// La culture est définie dans App.xaml.cs au démarrage depuis GeneralSettings.Language.
    /// </summary>
    public static class Strings
    {
        private static readonly ResourceManager _rm =
            new ResourceManager("EasySave.WPF.Resources.Strings", typeof(Strings).Assembly);

        private static string Get(string key) =>
            _rm.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";

        public static string AppTitle          => Get(nameof(AppTitle));
        public static string Nav_Jobs          => Get(nameof(Nav_Jobs));
        public static string Nav_Settings      => Get(nameof(Nav_Settings));
        public static string Jobs_Title        => Get(nameof(Jobs_Title));
        public static string Jobs_AddButton    => Get(nameof(Jobs_AddButton));
        public static string Jobs_ExecuteAll   => Get(nameof(Jobs_ExecuteAll));
        public static string Jobs_EmptyHint    => Get(nameof(Jobs_EmptyHint));
        public static string Jobs_NoJobs       => Get(nameof(Jobs_NoJobs));
        public static string Form_Title        => Get(nameof(Form_Title));
        public static string Form_Name         => Get(nameof(Form_Name));
        public static string Form_Source       => Get(nameof(Form_Source));
        public static string Form_Target       => Get(nameof(Form_Target));
        public static string Form_Type         => Get(nameof(Form_Type));
        public static string Form_Full         => Get(nameof(Form_Full));
        public static string Form_Differential => Get(nameof(Form_Differential));
        public static string Form_Add          => Get(nameof(Form_Add));
        public static string Form_Cancel       => Get(nameof(Form_Cancel));
        public static string Job_Execute       => Get(nameof(Job_Execute));
        public static string Job_Delete        => Get(nameof(Job_Delete));
        public static string Job_Source        => Get(nameof(Job_Source));
        public static string Job_Target        => Get(nameof(Job_Target));
        public static string Job_Type          => Get(nameof(Job_Type));
        public static string Job_Status        => Get(nameof(Job_Status));
        public static string Settings_Title    => Get(nameof(Settings_Title));
        public static string Settings_LogFormat    => Get(nameof(Settings_LogFormat));
        public static string Settings_Language     => Get(nameof(Settings_Language));
        public static string Settings_Extensions   => Get(nameof(Settings_Extensions));
        public static string Settings_BusinessApp  => Get(nameof(Settings_BusinessApp));
        public static string Settings_CryptoSoft   => Get(nameof(Settings_CryptoSoft));
        public static string Settings_Save         => Get(nameof(Settings_Save));
        public static string Settings_Json         => Get(nameof(Settings_Json));
        public static string Settings_Xml          => Get(nameof(Settings_Xml));
        public static string Settings_English      => Get(nameof(Settings_English));
        public static string Settings_French       => Get(nameof(Settings_French));
        public static string Type_Full             => Get(nameof(Type_Full));
        public static string Type_Differential     => Get(nameof(Type_Differential));
    }
}
