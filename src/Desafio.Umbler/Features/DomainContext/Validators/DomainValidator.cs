using System.Linq;
using System.Text.RegularExpressions;

namespace Desafio.Umbler.Features.DomainContext.Validators
{
    public static class DomainValidator
    {
        public static (bool isValid, string error) Validate(string domainName)
        {
            if (string.IsNullOrWhiteSpace(domainName))
                return (false, "O dom�nio n�o pode ser vazio.");

            domainName = domainName.Trim();

            if (domainName.Contains(' '))
                return (false, $"O dom�nio '{domainName}' n�o pode conter espa�os.");

            if (domainName.Length < 3 || domainName.Length > 253)
                return (false, $"O dom�nio '{domainName}' deve ter entre 3 e 253 caracteres. Atual: {domainName.Length}.");

            var labels = domainName.Split('.');
            
            if (labels.Length < 2)
                return (false, $"O dom�nio '{domainName}' deve possuir uma extens�o v�lida (ex: .com, .com.br).");

            var tld = labels[^1];
            if (!TldValidator.IsValidTld(tld))
                return (false, $"O dom�nio '{domainName}' possui uma extens�o inv�lida '.{tld}'. " +
                    "Use uma extens�o v�lida segundo a IANA (ex: .com, .br, .net, .org, .io).");

            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];

                if (string.IsNullOrEmpty(label))
                    return (false, $"O dom�nio '{domainName}' � inv�lido pois possui pontos consecutivos (..) ou come�a/termina com ponto.");

                if (label.Length > 63)
                    return (false, $"A parte '{label}' no dom�nio '{domainName}' excede 63 caracteres. Atual: {label.Length}.");

                var isTld = i == labels.Length - 1;

                if (!isTld)
                {
                    if (label.StartsWith("-") || label.EndsWith("-"))
                        return (false, $"A parte '{label}' no dom�nio '{domainName}' n�o pode come�ar ou terminar com h�fen.");

                    if (label.Contains("--"))
                        return (false, $"A parte '{label}' no dom�nio '{domainName}' n�o pode ter h�fens consecutivos.");

                    if (label.All(char.IsDigit))
                        return (false, $"A parte '{label}' no dom�nio '{domainName}' n�o pode conter apenas n�meros.");
                }
            }

            if (!DomainRegex.IsMatch(domainName))
                return (false, $"O dom�nio '{domainName}' � inv�lido. " +
                    "Um dom�nio v�lido deve conter apenas letras (a-z), n�meros (0-9), h�fens, " +
                    "e caracteres acentuados (�, �, �, �, �, �, �, �, �, �, �, �, �), " +
                    "com uma extens�o v�lida segundo a IANA (ex: .com, .br, .net, .io).");

            return (true, null);
        }

        private static readonly Regex DomainRegex = new Regex
        (
           @"^(?:[a-zA-Z0-9�������������](?:[a-zA-Z0-9�������������-]{0,61}[a-zA-Z0-9�������������])?\.)+[a-zA-Z]{2,}$",
           RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
    }
}
