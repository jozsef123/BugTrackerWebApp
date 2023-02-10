using BugTrackerWebApp.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BugTrackerWebAppTests
{
    [TestClass]
    public class UnitTestHomeController
    {
        [TestMethod]
        public void TestPrivacyView()
        {
            var controller = new HomeController();
            var result = controller.Privacy() as ViewResult;
            Assert.AreEqual("Privacy", result.ViewName);
        }
    }
}
