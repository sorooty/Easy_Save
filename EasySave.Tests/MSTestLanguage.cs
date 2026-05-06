using EasySave.Core.Model.Service;

namespace EasySave.Tests
{
    [TestClass]
    public class LanguageServiceTests
    {
        [TestMethod]
        public void GetText_ShouldReturnFrenchText_WhenLanguageIsFr()
        {
            var languageService = new LanguageService();

            languageService.SetLanguage("fr");

            string result = languageService.GetText("menu.add");

            Assert.AreEqual("2. Ajouter un travail de sauvegarde", result);
        }

        [TestMethod]
        public void GetText_ShouldReturnEnglishText_WhenLanguageIsEn()
        {
            var languageService = new LanguageService();

            languageService.SetLanguage("en");

            string result = languageService.GetText("menu.add");

            Assert.AreEqual("2. Add a backup job", result);
        }
    }
}