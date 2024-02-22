using TrybeHotel.Models;
using TrybeHotel.Dto;
using TrybeHotel.Errors.ApiExceptions;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;

        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Select(
                city => new CityDto() { CityId = city.CityId, Name = city.Name, State = city.State }
            );
        }

        public CityDto AddCity(City city)
        {
            _context.Cities.Add(city);
            _context.SaveChanges();
            return new CityDto() { CityId = city.CityId, Name = city.Name, State = city.State };
        }

        public CityDto UpdateCity(City city)
        {
            if (!CityExists(city.CityId))
                throw new NotFoundException("City not found");
            _context.Cities.Update(city);
            _context.SaveChanges();
            return new CityDto() { CityId = city.CityId, Name = city.Name, State = city.State };
        }

        public bool CityExists(int cityId)
        {
            var city = _context.Cities.FirstOrDefault(city => city.CityId == cityId);
            if (city is null) return false;
            return true;
        }
    }
}
