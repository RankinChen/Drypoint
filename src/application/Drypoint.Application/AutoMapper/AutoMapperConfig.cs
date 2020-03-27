 using AutoMapper;
 using System;
 using System.Collections.Generic;
 using System.Text;

namespace Drypoint.Application.AutoMapper
{
    public class AutoMapperConfig : Profile
     {
         public AutoMapperConfig()
         {
            /*
             * 添加映射规则
             CreateMap<UsersInputDto, Users>().ForMember(d => d.username, u => u.MapFrom(s => s.uname))    //属性名称映射
                                    .ForMember(d => d.password, u => u.MapFrom(s => s.pwd))  //属性名称映射
                                    .ForMember(d => d.age, u => u.Condition(s => s.age >= 0 && s.age <= 120)) //对一些属性做映射判断
                                    .BeforeMap((dto, ent) => ent.fullname = dto.firstname + "_" + dto.lastname)   //对一些属性做映射前处理
            */
                                    ;
         }
     }
}
