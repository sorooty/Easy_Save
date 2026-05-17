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
        public static string Nav_Quit          => Get(nameof(Nav_Quit));
        public static string Nav_About         => Get(nameof(Nav_About));
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
        public static string Settings_OpenLogs     => Get(nameof(Settings_OpenLogs));
        public static string Job_Edit              => Get(nameof(Job_Edit));
        public static string Job_Save              => Get(nameof(Job_Save));
        public static string Job_CancelEdit        => Get(nameof(Job_CancelEdit));
        public static string Job_Pause             => Get(nameof(Job_Pause));
        public static string Job_Resume            => Get(nameof(Job_Resume));
        public static string Job_Stop              => Get(nameof(Job_Stop));
        public static string Type_Full             => Get(nameof(Type_Full));
        public static string Type_Differential     => Get(nameof(Type_Differential));
        public static string Settings_PriorityExtensions => Get(nameof(Settings_PriorityExtensions));
        public static string Settings_LargeFileLimit      => Get(nameof(Settings_LargeFileLimit));
        public static string Job_Size                     => Get(nameof(Job_Size));
        public static string Settings_LogMode             => Get(nameof(Settings_LogMode));
        public static string Settings_CentralEndpoint     => Get(nameof(Settings_CentralEndpoint));
        public static string Nav_Help                     => Get(nameof(Nav_Help));
        public static string Help_Title                   => Get(nameof(Help_Title));
        public static string Help_Section_Jobs            => Get(nameof(Help_Section_Jobs));
        public static string Help_Section_Settings        => Get(nameof(Help_Section_Settings));
        public static string Help_Section_Logs            => Get(nameof(Help_Section_Logs));
        public static string Help_Section_Docker          => Get(nameof(Help_Section_Docker));
        public static string Help_Jobs_Step1              => Get(nameof(Help_Jobs_Step1));
        public static string Help_Jobs_Step2              => Get(nameof(Help_Jobs_Step2));
        public static string Help_Jobs_Step3              => Get(nameof(Help_Jobs_Step3));
        public static string Help_Jobs_Step4              => Get(nameof(Help_Jobs_Step4));
        public static string Help_Jobs_Step5              => Get(nameof(Help_Jobs_Step5));
        public static string Help_Jobs_Step6              => Get(nameof(Help_Jobs_Step6));
        public static string Help_Jobs_Step7              => Get(nameof(Help_Jobs_Step7));
        public static string Help_Settings_LogFormat_Label => Get(nameof(Help_Settings_LogFormat_Label));
        public static string Help_Settings_LogFormat_Desc => Get(nameof(Help_Settings_LogFormat_Desc));
        public static string Help_Settings_Language_Label => Get(nameof(Help_Settings_Language_Label));
        public static string Help_Settings_Language_Desc => Get(nameof(Help_Settings_Language_Desc));
        public static string Help_Settings_Encrypted_Label => Get(nameof(Help_Settings_Encrypted_Label));
        public static string Help_Settings_Encrypted_Desc => Get(nameof(Help_Settings_Encrypted_Desc));
        public static string Help_Settings_Priority_Label => Get(nameof(Help_Settings_Priority_Label));
        public static string Help_Settings_Priority_Desc => Get(nameof(Help_Settings_Priority_Desc));
        public static string Help_Settings_Business_Label => Get(nameof(Help_Settings_Business_Label));
        public static string Help_Settings_Business_Desc => Get(nameof(Help_Settings_Business_Desc));
        public static string Help_Settings_LargeFile_Label => Get(nameof(Help_Settings_LargeFile_Label));
        public static string Help_Settings_LargeFile_Desc => Get(nameof(Help_Settings_LargeFile_Desc));
        public static string Help_Settings_CryptoSoft_Label => Get(nameof(Help_Settings_CryptoSoft_Label));
        public static string Help_Settings_CryptoSoft_Desc => Get(nameof(Help_Settings_CryptoSoft_Desc));
        public static string Help_Logs_Desc1              => Get(nameof(Help_Logs_Desc1));
        public static string Help_Logs_Desc2              => Get(nameof(Help_Logs_Desc2));
        public static string Help_Logs_Desc3              => Get(nameof(Help_Logs_Desc3));
        public static string Help_Logs_Desc4              => Get(nameof(Help_Logs_Desc4));
        public static string Help_Docker_Desc1            => Get(nameof(Help_Docker_Desc1));
        public static string Help_Docker_Desc2            => Get(nameof(Help_Docker_Desc2));
        public static string Help_Docker_Desc3            => Get(nameof(Help_Docker_Desc3));
        public static string Help_Docker_Desc4            => Get(nameof(Help_Docker_Desc4));
        public static string Help_Docker_Desc5            => Get(nameof(Help_Docker_Desc5));
        public static string Help_Docker_Desc6            => Get(nameof(Help_Docker_Desc6));
        public static string About_Title                  => Get(nameof(About_Title));
        public static string About_Version                => Get(nameof(About_Version));
        public static string About_Date                   => Get(nameof(About_Date));
        public static string About_Project_Title          => Get(nameof(About_Project_Title));
        public static string About_Project_Desc           => Get(nameof(About_Project_Desc));
        public static string About_Project_Details        => Get(nameof(About_Project_Details));
        public static string About_Features_Title         => Get(nameof(About_Features_Title));
        public static string About_Features_Backup        => Get(nameof(About_Features_Backup));
        public static string About_Features_Parallel      => Get(nameof(About_Features_Parallel));
        public static string About_Features_Priority      => Get(nameof(About_Features_Priority));
        public static string About_Features_Bandwidth     => Get(nameof(About_Features_Bandwidth));
        public static string About_Features_Docker        => Get(nameof(About_Features_Docker));
        public static string About_Technical_Title        => Get(nameof(About_Technical_Title));
        public static string About_Tech_Language          => Get(nameof(About_Tech_Language));
        public static string About_Tech_Interface         => Get(nameof(About_Tech_Interface));
        public static string About_Tech_Platform          => Get(nameof(About_Tech_Platform));
        public static string About_Tech_Repository        => Get(nameof(About_Tech_Repository));
        public static string About_GetStarted_Title       => Get(nameof(About_GetStarted_Title));
        public static string About_GetStarted_Step1       => Get(nameof(About_GetStarted_Step1));
        public static string About_GetStarted_Step2       => Get(nameof(About_GetStarted_Step2));
        public static string About_GetStarted_Step3       => Get(nameof(About_GetStarted_Step3));
    }
}