using TestAPITask.Controllers;
using TestAPITask.Dtos;
using TestAPITask.Services;
using TestAPITask.Helpers;
using Moq;
using Moq.Protected;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestAPITask.Tests
{
    public class ProductUnitTests
    {
        [Fact]
        public async Task GetProductsReturnsData()
        {
            var mockService = new Mock<IProductsService>();
            var filterDto = new Dtos.ProductsFilterDto();
            var expectedResult = new Dtos.ProductsDto
            {
                Products = new[] {
                        new ProductDto(new Models.Product
                        {
                            Title = "A Red Trouser",
                            Price = 10,
                            Sizes = new []
                            {
                                "small",
                                "large",
                                "medium"
                            },
                            Description = "This trouser perfectly pairs with a green shirt."
                        }, new string[0]),
                        new ProductDto(new Models.Product
                        {
                            Title = "A Green Trouser",
                            Price = 15,
                            Sizes = new []
                            {
                                "small"
                            },
                            Description = "This trouser perfectly pairs with a blue shirt."
                        }, new string[0])

                    },
                Info = new ProductsInfoDto
                {
                    Sizes = new[] {
                                "small",
                                "large",
                                "medium"
                            },
                    CommonWords = new string[]
                        {
                            "trouser",
                            "pairs"
                        },
                    MaxPrice = 15,
                    MinPrice = 10
                }
            };

            mockService.Setup(x => x.GetProductsAsync(filterDto, CancellationToken.None))
                .ReturnsAsync(expectedResult);

            var controller = new ProductsController(mockService.Object);

            var result = await controller.Get(filterDto, CancellationToken.None);

            var contentResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedResult, contentResult.Value);
        }

        [Fact]
        public async Task GetProductHighlightWorks()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[\r\n    {\r\n      \"title\": \"A Red Trouser\",\r\n      \"price\": 10,\r\n      \"sizes\": [\r\n        \"small\",\r\n        \"medium\",\r\n        \"large\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a green shirt.\"\r\n    },\r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 11,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a blue shirt.\"\r\n    }]")
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient("Products")).Returns(client);

            var logger = Mock.Of<ILogger<ProductsService>>();
            var service = new ProductsService(httpClientFactoryMock.Object, logger);

            var result = await service.GetProductsAsync(new ProductsFilterDto { Highlight = "this,blue,shirt" }, CancellationToken.None);

            Assert.Equal(result.Products.Count, 2);
            Assert.Equal(result.Products[0].Description, "<em>This</em> trouser perfectly pairs with a green <em>shirt</em>.");
            Assert.Equal(result.Products[1].Description, "<em>This</em> trouser perfectly pairs with a <em>blue</em> <em>shirt</em>.");
        }

        [Fact]
        public async Task GetProductTotalMinMaxWorks()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[\r\n    {\r\n      \"title\": \"A Red Trouser\",\r\n      \"price\": 10,\r\n      \"sizes\": [\r\n        \"small\",\r\n        \"medium\",\r\n        \"large\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a green shirt.\"\r\n    },\r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 16,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a blue shirt.\"\r\n    }]")
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient("Products")).Returns(client);

            var logger = Mock.Of<ILogger<ProductsService>>();
            var service = new ProductsService(httpClientFactoryMock.Object, logger);

            var result = await service.GetProductsAsync(new ProductsFilterDto { }, CancellationToken.None);

            Assert.Equal(result.Info.MaxPrice, 16);
            Assert.Equal(result.Info.MinPrice, 10);
        }

        [Fact]
        public async Task GetProductMinMaxFilterWorks()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[\r\n    {\r\n      \"title\": \"A Red Trouser\",\r\n      \"price\": 10,\r\n      \"sizes\": [\r\n        \"small\",\r\n        \"medium\",\r\n        \"large\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a green shirt.\"\r\n    },\r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 15,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This 15 trouser perfectly pairs with a blue shirt.\"\r\n    }\r\n, \r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 9,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This 9 trouser perfectly pairs with a blue shirt.\"\r\n    }\r\n ,    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 16,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a blue shirt.\"\r\n    }]")
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient("Products")).Returns(client);

            var logger = Mock.Of<ILogger<ProductsService>>();
            var service = new ProductsService(httpClientFactoryMock.Object, logger);

            var result = await service.GetProductsAsync(new ProductsFilterDto { MinPrice=10, MaxPrice=15 }, CancellationToken.None);

            Assert.Equal(result.Info.MaxPrice, 16);
            Assert.Equal(result.Info.MinPrice, 9);
            Assert.Equal(result.Products.Count, 2);
        }


        [Fact]
        public async Task GetProductSizeFilterWorks()
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[\r\n    {\r\n      \"title\": \"A Red Trouser\",\r\n      \"price\": 10,\r\n      \"sizes\": [\r\n        \"small\",\r\n        \"medium\",\r\n        \"large\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a green shirt.\"\r\n    },\r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 15,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This 15 trouser perfectly pairs with a blue shirt.\"\r\n    }\r\n, \r\n    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 9,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This 9 trouser perfectly pairs with a blue shirt.\"\r\n    }\r\n ,    {\r\n      \"title\": \"A Green Trouser\",\r\n      \"price\": 16,\r\n      \"sizes\": [\r\n        \"small\"\r\n      ],\r\n      \"description\": \"This trouser perfectly pairs with a blue shirt.\"\r\n    }]")
                });
            var client = new HttpClient(mockHttpMessageHandler.Object);

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.CreateClient("Products")).Returns(client);

            var logger = Mock.Of<ILogger<ProductsService>>();
            var service = new ProductsService(httpClientFactoryMock.Object, logger);

            var result = await service.GetProductsAsync(new ProductsFilterDto { Size="large" }, CancellationToken.None);

            Assert.Equal(result.Info.Sizes.Length, 3);
            Assert.True(result.Products.All(x => x.Sizes.Contains("large")));
        }

        [Fact]
        public async Task GetProductsIncorrectMinReturnsError()
        {
            var application = new ApiApplication();
            var client = application.CreateClient();

            var result = await client.GetAsync("/api/products?minprice=-1");

            Assert.Equal(400, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetProductsIncorrectMaxReturnsError()
        {
            var application = new ApiApplication();
            var client = application.CreateClient();

            var result = await client.GetAsync("/api/products?maxprice=-1");

            Assert.Equal(400, (int)result.StatusCode);
        }

        [Fact]
        public async Task GetProductsCorrectInputReturnsOk()
        {
            var application = new ApiApplication();
            var client = application.CreateClient();

            var result = await client.GetAsync("/api/products?maxprice=15&minprice=10&size=medium");

            Assert.Equal(200, (int)result.StatusCode);
        }
    }
}