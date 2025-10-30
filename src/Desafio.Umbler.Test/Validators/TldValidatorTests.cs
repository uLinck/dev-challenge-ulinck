using Desafio.Umbler.Features.DomainContext.Validators;
using Desafio.Umbler.Shared.Services.TldRegex;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Validators
{
    [TestClass]
    public class TldValidatorTests
    {
        private TldValidator _tldValidator;

        [TestInitialize]
        public void Setup()
        {
            var tldRegexService = new TldRegexService();
            _tldValidator = new TldValidator(tldRegexService);
        }

        [TestMethod]
        public void IsValidTld_LoadsIanaList_Successfully()
        {
            // Assert
            Assert.IsTrue(_tldValidator.Count > 1000, "Deve carregar mais de 1000 TLDs da lista IANA");
        }

        [TestMethod]
        [DataRow("com")]
        [DataRow("br")]
        [DataRow("net")]
        [DataRow("org")]
        [DataRow("io")]
        [DataRow("dev")]
        [DataRow("app")]
        [DataRow("tech")]
        [DataRow("ai")]
        [DataRow("uk")]
        [DataRow("ca")]
        [DataRow("de")]
        [DataRow("fr")]
        public void IsValidTld_CommonTlds_ReturnsTrue(string tld)
        {
            // Act
            var result = _tldValidator.IsValidTld(tld);

            // Assert
            Assert.IsTrue(result, $"TLD '{tld}' deveria ser válido");
        }

        [TestMethod]
        [DataRow("COM")]
        [DataRow("Br")]
        [DataRow("NeT")]
        public void IsValidTld_CaseInsensitive_ReturnsTrue(string tld)
        {
            // Act
            var result = _tldValidator.IsValidTld(tld);

            // Assert
            Assert.IsTrue(result, $"TLD '{tld}' deveria ser válido (case insensitive)");
        }

        [TestMethod]
        [DataRow("wewe")]
        [DataRow("fake")]
        [DataRow("invalid")]
        [DataRow("xyz123")]
        [DataRow("test123")]
        [DataRow("notreal")]
        public void IsValidTld_InvalidTlds_ReturnsFalse(string tld)
        {
            // Act
            var result = _tldValidator.IsValidTld(tld);

            // Assert
            Assert.IsFalse(result, $"TLD '{tld}' deveria ser inválido");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public void IsValidTld_EmptyOrNull_ReturnsFalse(string tld)
        {
            // Act
            var result = _tldValidator.IsValidTld(tld);

            // Assert
            Assert.IsFalse(result, "TLD vazio ou nulo deveria ser inválido");
        }
    }
}
