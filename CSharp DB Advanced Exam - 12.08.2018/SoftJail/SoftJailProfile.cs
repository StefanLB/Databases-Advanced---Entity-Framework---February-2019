namespace SoftJail
{
    using AutoMapper;
    using Data.Models;
    using DataProcessor.ImportDto;


    public class SoftJailProfile : Profile
    {
        // Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
        public SoftJailProfile()
        {
            this.CreateMap<DepartmentDto, Department>();

            this.CreateMap<CellDto, Cell>();

            this.CreateMap<MailDto, Mail>();
        }
    }
}
