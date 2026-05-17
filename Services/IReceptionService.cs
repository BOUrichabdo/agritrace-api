using TracAgriApi.DTOs;

namespace TracAgriApi.Servises
{
    public interface IReceptionService
    {
        Task<ReceptionResponseDto> CreateReceptionAsync(CreateReceptionDto dto);

    }
}
