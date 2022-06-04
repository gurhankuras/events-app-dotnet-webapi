using Xunit;
using Moq;
using Auth;
using Auth.Mapper;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Auth.Models;
using Auth.Services;
using System;

namespace Auth2Tests {
    public class EventServiceTests 
    {
        [Fact]
        public async Task Test_ThrowsExceptionWhenPassedNull() 
        {
            var repo = new Mock<IEventRepository>();
            var location = new Coordinates {
                Latitude = 10,
                Longitute = 10
            };

            repo.Setup(r => r.GetNearEvents(location)).ReturnsAsync(new List<NearEventDto>());
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new Auth.Mapper.MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            var sut = new EventService(repo.Object, mapper);

            await sut.GetNearEventsAsync(location);
        }
    }
}


