using AutoMapper;

namespace Auth.Infrastructure.Mappings
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile);
    }
}
