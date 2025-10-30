using Desafio.Umbler.Features.DomainContext.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Desafio.Umbler.Test.Validators
{
    [TestClass]
    public class DomainValidatorTests
    {
        #region Valid Domains

        [TestMethod]
        [DataRow("umbler.com")]
        [DataRow("google.com.br")]
        [DataRow("sub.domain.example.org")]
        [DataRow("my-site.net")]
        [DataRow("test123.app")]
        [DataRow("example.co")]
        [DataRow("caf�.com")]
        [DataRow("a��car.com.br")]
        [DataRow("p�o.net")]
        [DataRow("a1.br")]
        [DataRow("my-super-long-site.com")]
        [DataRow("jos�.com")]
        [DataRow("s�o-paulo.com.br")]
        [DataRow("p�o-de-a��car.net")]
        [DataRow("my-site.com")]
        [DataRow("test-123-abc.com")]
        [DataRow("a-b-c-d.com")]
        public void Validate_ValidDomains_ReturnsTrue(string domain)
        {
            var (isValid, error) = DomainValidator.Validate(domain);
            Assert.IsTrue(isValid, $"Domain '{domain}' should be valid. Error: {error}");
            Assert.IsNull(error, "Valid domain should have no errors");
        }

        #endregion

        #region Invalid Domains

        [TestMethod]
        [DataRow("", "n�o pode ser vazio")]
        [DataRow(" ", "n�o pode ser vazio")]
        [DataRow(null, "n�o pode ser vazio")]
        [DataRow("invalid", "extens�o v�lida")]
        [DataRow("no-extension", "extens�o v�lida")]
        [DataRow(".com", "pontos consecutivos")]
        [DataRow("domain.", "Use uma extens�o v�lida")]
        [DataRow("domain..com", "pontos consecutivos")]
        [DataRow("my domain.com", "n�o pode conter espa�os")]
        [DataRow("domain @site.com", "n�o pode conter espa�os")]
        [DataRow("a.b", "extens�o inv�lida")]
        [DataRow("domain@site.com", "Um dom�nio v�lido deve conter apenas letras")]
        [DataRow("http://domain.com", "Um dom�nio v�lido deve conter apenas letras")]
        [DataRow("domain_name.com", "Um dom�nio v�lido deve conter apenas letras")]
        [DataRow("domain/path.com", "Um dom�nio v�lido deve conter apenas letras")]
        [DataRow("domain#test.com", "Um dom�nio v�lido deve conter apenas letras")]
        public void Validate_InvalidDomains_ReturnsFalseWithError(string domain, string expectedErrorPart)
        {
            var (isValid, error) = DomainValidator.Validate(domain);
            Assert.IsFalse(isValid, $"Domain '{domain}' should be invalid");
            
            if (expectedErrorPart != null)
            {
                Assert.IsNotNull(error, $"Expected error message for domain '{domain}'");
                Assert.IsTrue(error.Contains(expectedErrorPart), 
                    $"Error message '{error}' should contain '{expectedErrorPart}'");
            }
        }

        #endregion

        #region Invalid Domain - Specific Rules

        [TestMethod]
        public void Validate_OnlyNumbers_ReturnsError()
        {
            var (isValid, error) = DomainValidator.Validate("123.com");
            Assert.IsFalse(isValid);
            Assert.IsTrue(error.Contains("apenas n�meros"));
        }

        [TestMethod]
        [DataRow("-invalid.com", "n�o pode come�ar ou terminar com h�fen")]
        [DataRow("invalid-.com", "n�o pode come�ar ou terminar com h�fen")]
        [DataRow("ab--cd.com", "h�fens consecutivos")]
        [DataRow("a--b.com", "h�fens consecutivos")]
        public void Validate_InvalidHyphenUsage_ReturnsError(string domain, string expectedErrorPart)
        {
            var (isValid, error) = DomainValidator.Validate(domain);
            Assert.IsFalse(isValid, $"Domain '{domain}' should be invalid");
            Assert.IsTrue(error.Contains(expectedErrorPart), 
                $"Error '{error}' should contain '{expectedErrorPart}'");
        }

        [TestMethod]
        [DataRow("asdasd.wewe")]
        [DataRow("test.xyz123")]
        [DataRow("domain.fake")]
        [DataRow("site.invalid")]
        [DataRow("domain.c")]
        [DataRow("domain.comxx")]
        [DataRow("domain.123")]
        public void Validate_InvalidTLD_ReturnsError(string domain)
        {
            var (isValid, error) = DomainValidator.Validate(domain);
            Assert.IsFalse(isValid, $"Domain '{domain}' should have invalid TLD");
            Assert.IsTrue(error.Contains("extens�o inv�lida") || error.Contains("inv�lido"));
        }

        [TestMethod]
        public void Validate_LabelTooLong_ReturnsError()
        {
            var longLabel = new string('a', 64) + ".com";
            var (isValid, error) = DomainValidator.Validate(longLabel);
            Assert.IsFalse(isValid);
            Assert.IsTrue(error.Contains("excede 63 caracteres"));
        }

        [TestMethod]
        public void Validate_DomainTooLong_ReturnsError()
        {
            var longDomain = new string('a', 250) + ".com";
            var (isValid, error) = DomainValidator.Validate(longDomain);
            Assert.IsFalse(isValid);
            Assert.IsTrue(error.Contains("entre 3 e 253 caracteres"));
        }

        #endregion
    }
}
