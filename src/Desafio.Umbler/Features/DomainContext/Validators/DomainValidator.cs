using System.Linq;
using System.Text.RegularExpressions;

namespace Desafio.Umbler.Features.DomainContext.Validators
{
    public static class DomainValidator
    {
        public static (bool isValid, string error) Validate(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                return (false, "O domínio não pode ser vazio.");

            domainName = domainName.Trim();

            if (domainName.Contains(' '))
                return (false, $"O domínio '{domainName}' não pode conter espaços.");

            if (domainName.Length < 3 || domainName.Length > 253)
                return (false, $"O domínio '{domainName}' deve ter entre 3 e 253 caracteres. Atual: {domainName.Length}.");

            var labels = domainName.Split('.');
            
            if (labels.Length < 2)
                return (false, $"O domínio '{domainName}' deve possuir uma extensão válida (ex: .com, .com.br).");

            var tld = labels[^1];
            if (!TldValidator.IsValidTld(tld))
                return (false, $"O domínio '{domainName}' possui uma extensão inválida '.{tld}'. " +
                    "Use uma extensão válida segundo a IANA (ex: .com, .br, .net, .org, .io).");

            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];

                if (string.IsNullOrEmpty(label))
                    return (false, $"O domínio '{domainName}' é inválido pois possui pontos consecutivos (..) ou começa/termina com ponto.");

                if (label.Length > 63)
                    return (false, $"A parte '{label}' no domínio '{domainName}' excede 63 caracteres. Atual: {label.Length}.");

                var isTld = i == labels.Length - 1;

                if (!isTld)
                {
                    if (label.StartsWith("-") || label.EndsWith("-"))
                        return (false, $"A parte '{label}' no domínio '{domainName}' não pode começar ou terminar com hífen.");

                    if (label.Contains("--"))
                        return (false, $"A parte '{label}' no domínio '{domainName}' não pode ter hífens consecutivos.");

                    if (label.All(char.IsDigit))
                        return (false, $"A parte '{label}' no domínio '{domainName}' não pode conter apenas números.");
                }
            }

            if (!DomainRegex.IsMatch(domainName))
                return (false, $"O domínio '{domainName}' é inválido. " +
                    "Um domínio válido deve conter apenas letras (a-z), números (0-9), hífens, " +
                    "e caracteres acentuados (à, á, â, ã, é, ê, í, ó, ô, õ, ú, ü, ç), " +
                    "com uma extensão válida segundo a IANA (ex: .com, .br, .net, .io).");

            return (true, null);
        }

        private static readonly Regex DomainRegex = new Regex
        (
           @"^(?:[a-zA-Z0-9àáâãéêíóôõúüç](?:[a-zA-Z0-9àáâãéêíóôõúüç-]{0,61}[a-zA-Z0-9àáâãéêíóôõúüç])?\.)+[a-zA-Z]{2,}$",
           RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
    }
}
