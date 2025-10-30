using Desafio.Umbler.Features.DomainContext.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Validators
{
    [TestClass]
    public class TldValidatorTests
    {
        [TestMethod]
        public void IsValidTld_LoadsIanaList_Successfully()
        {
            // Assert
            Assert.IsTrue(TldValidator.Count > 1000, "Deve carregar mais de 1000 TLDs da lista IANA");
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
            var result = TldValidator.IsValidTld(tld);

            // Assert
            Assert.IsTrue(result, $"TLD '{tld}' deveria ser válido");
        }

        [TestMethod]
        [DataRow("COM")] // Case insensitive
        [DataRow("Br")]
        [DataRow("NeT")]
        public void IsValidTld_CaseInsensitive_ReturnsTrue(string tld)
        {
            // Act
            var result = TldValidator.IsValidTld(tld);

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
            var result = TldValidator.IsValidTld(tld);

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
            var result = TldValidator.IsValidTld(tld);

            // Assert
            Assert.IsFalse(result, "TLD vazio ou nulo deveria ser inválido");
        }
    }
}
