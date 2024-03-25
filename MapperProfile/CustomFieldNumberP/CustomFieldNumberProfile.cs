using AutoMapper;
using TaskMate.DTOs.CustomFieldDate;
using TaskMate.DTOs.CustomFieldNumber;
using TaskMate.Entities;

namespace TaskMate.MapperProfile.CustomFieldNumberP;

public class CustomFieldNumberProfile:Profile
{
    public CustomFieldNumberProfile()
    {
        CreateMap<CustomFieldsNumber, UpdateCustomFieldsNumberDto>();
        CreateMap<CustomFieldsNumber, GetCustomFieldNumberDto>().ReverseMap();

    }
}
