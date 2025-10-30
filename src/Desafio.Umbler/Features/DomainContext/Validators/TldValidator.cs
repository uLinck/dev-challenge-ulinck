using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Desafio.Umbler.Shared.Services.TldRegex;

namespace Desafio.Umbler.Features.DomainContext.Validators
{
    public class TldValidator : ITldValidator
    {
        private readonly ITldRegexService _tldRegexService;

        public TldValidator(ITldRegexService tldRegexService)
        {
            _tldRegexService = tldRegexService;
        }

        public bool IsValidTld(string tld)
        {
            return _tldRegexService.IsValidTld(tld);
        }

        public int Count => _tldRegexService.TldCount;
    }

    public interface ITldValidator
    {
        bool IsValidTld(string tld);
        int Count { get; }
    }
}
