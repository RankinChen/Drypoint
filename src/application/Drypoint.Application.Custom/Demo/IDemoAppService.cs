using Drypoint.Application.Custom.Demo.Dto;
using Drypoint.Application.Services;
using Drypoint.Unity.BaseDto.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drypoint.Application.Custom.Demo
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDemoAppService: IApplicationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ListResultDto<DemoOutputDto> GetAll();
    }
}
