using Desafio.Umbler.Controllers;
using Desafio.Umbler.Persistence;
using Desafio.Umbler.Persistence.Models;
using Desafio.Umbler.Features.DomainContext.Dto;
using Desafio.Umbler.Features.DomainContext.Services;
using Desafio.Umbler.Shared.Services.WhoIs;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Test
{
    [TestClass]
    public class ControllersTest
    {       
        [TestMethod]
        public async Task Domain_WithValidCache_ReturnsFromCache()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var domain = new Domain 
            { 
                Id = 1, 
                Ip = "192.168.0.1", 
                Name = "test.com", 
                UpdatedAt = DateTime.UtcNow, 
                HostedAt = "umbler.corp", 
                Ttl = 7200,
                WhoIs = "Ns.umbler.com" 
            };

            using (var db = new DatabaseContext(options))
            {
                db.Domains.Add(domain);
                await db.SaveChangesAsync();
            }

            using (var db = new DatabaseContext(options))
            {
                var mockLookupClient = new Mock<ILookupClient>();
                var mockWhoisClient = new Mock<IWhoIsClient>();
                var mockLogger = new Mock<ILogger<DomainService>>();
                var domainService = new DomainService(db, mockLookupClient.Object, mockLogger.Object, mockWhoisClient.Object);
                var controller = new DomainController(domainService);

                // Act
                var response = await controller.Get("test.com");
                var result = response.Result as ObjectResult;
                var apiResponse = result.Value as ApiResponse<DomainDto>;
                
                // Assert
                Assert.IsNotNull(apiResponse);
                Assert.IsTrue(apiResponse.Success);
                Assert.IsNotNull(apiResponse.Data);
                Assert.AreEqual(domain.Ip, apiResponse.Data.Ip);
                Assert.AreEqual(domain.Name, apiResponse.Data.Name);
                Assert.AreEqual(domain.HostedAt, apiResponse.Data.HostedAt);

                mockLookupClient.Verify(
                    x => x.QueryAsync(
                        It.IsAny<string>(), 
                        It.IsAny<QueryType>(), 
                        It.IsAny<QueryClass>(), 
                        It.IsAny<System.Threading.CancellationToken>()), 
                    Times.Never
                );
            }
        }

        [TestMethod]
        public async Task Domain_Invalid_ReturnsValidationError()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new DatabaseContext(options))
            {
                var mockLookupClient = new Mock<ILookupClient>();
                var mockWhoisClient = new Mock<IWhoIsClient>();
                var mockLogger = new Mock<ILogger<DomainService>>();
                var domainService = new DomainService(db, mockLookupClient.Object, mockLogger.Object, mockWhoisClient.Object);
                var controller = new DomainController(domainService);

                // Act
                var response = await controller.Get("invalid");
                var result = response.Result as ObjectResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(422, result.StatusCode);
                
                var apiResponse = result.Value as ApiResponse<DomainDto>;
                Assert.IsNotNull(apiResponse);
                Assert.IsFalse(apiResponse.Success);
                Assert.IsTrue(apiResponse.Errors.Count > 0);
            }
        }

        [TestMethod]
        public async Task Domain_Moking_WhoisClient()
        {
            // Arrange
            var mockWhoisClient = new Mock<IWhoIsClient>();
            var mockLookupClient = new Mock<ILookupClient>();
            var mockLogger = new Mock<ILogger<DomainService>>();
            
            var whoisResponse = new WhoisResponse
            {
                Raw = "Whois data for test.com",
                OrganizationName = "Test Organization"
            };
            mockWhoisClient.Setup(w => w.QueryAsync(It.IsAny<string>()))
                .ReturnsAsync(whoisResponse);

            var mockDnsResponse = new Mock<IDnsQueryResponse>();
            
            mockLookupClient.Setup(l => l.QueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<QueryType>(),
                    It.IsAny<QueryClass>(),
                    It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(mockDnsResponse.Object);

            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var db = new DatabaseContext(options))
            {
                var domainService = new DomainService(db, mockLookupClient.Object, mockLogger.Object, mockWhoisClient.Object);
                var controller = new DomainController(domainService);

                // Act
                var response = await controller.Get("test.com");
                var result = response.Result as ObjectResult;
                
                // Assert
                Assert.IsNotNull(result);
                var apiResponse = result.Value as ApiResponse<DomainDto>;
                Assert.IsNotNull(apiResponse);
                
                mockWhoisClient.Verify(
                    w => w.QueryAsync(It.IsAny<string>()), 
                    Times.AtLeastOnce
                );
            }
        }

    }
}